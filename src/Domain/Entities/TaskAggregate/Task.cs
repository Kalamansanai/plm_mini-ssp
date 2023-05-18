using System.Collections;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using CsvHelper;
using Domain.Common;
using Domain.Entities.CompanyHierarchy;
using Domain.Interfaces;
using Domain.Specifications;
using FluentResults;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Entities.TaskAggregate;

public class Task : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public TaskType Type { get; set; }

    public int MaxOrderNum { get; private set; }

    public int JobId { get; private set; }
    public Job Job { get; private set; } = default!;
    public int LocationId { get; private set; }

    public Location Location { get; set; } = default!;
    public List<TaskInstance> Instances { get; private set; } = default!;
    public List<Object> Objects { get; private set; } = default!;
    public List<Step> Steps { get; set; } = default!;

    public TaskInstance? OngoingInstance { get; set; }
    public int? OngoingInstanceId { get; set; }
    public List<Coordinate>? MarkerCoordinates { get; set; } = default!;
    
    
   private Task() {}

    private Task(int id, string name, TaskType type, int locationId, int jobId, int? ongoingInstanceId, int maxOrderNum)
    {
        Id = id;
        Name = name;
        Type = type;
        LocationId = locationId;
        JobId = jobId;
        Instances = new List<TaskInstance>();
        Objects = new List<Object>();
        Steps = new List<Step>();
        OngoingInstanceId = ongoingInstanceId;
        MaxOrderNum = maxOrderNum;
    }

    public Task(string name, int locationId, TaskType taskType)
    {
        Name = name;
        Type = taskType;
        Objects = new List<Object>();
        Steps = new List<Step>();
        LocationId = locationId;
        MaxOrderNum = 0;
    }

    private void UpdateMaxOrderNum()
    {
        MaxOrderNum = Steps.Select(s => s.OrderNum).Max();
    }

    public Result CreateInstance()
    {
        if (OngoingInstance is not null)
            return Result.Fail("Cannot create a new Task Instance while there's already an ongoing Instance");

        var instance = new TaskInstance(this);

        OngoingInstance = instance;
        if (Instances.IsNullOrEmpty())
        {
            Instances = new List<TaskInstance>();
        }
        Instances.Add(instance);

        return Result.Ok();
    }

    public void AddObjects(IEnumerable<Object> objects)
    {
        if (Objects.IsNullOrEmpty())
        {
            Objects = new List<Object>();
        }
        Objects.AddRange(objects);
    }

    public void AddSteps(IEnumerable<Step> steps)
    {
        if (Steps.IsNullOrEmpty())
        {
            Steps = new List<Step>();
        }
        Steps.AddRange(steps);
        UpdateMaxOrderNum();
    }

    public Result ModifyObject(int objectId, string name, ObjectCoordinates coords)
    {
        var obj = Objects.FirstOrDefault(o => o.Id == objectId);
        if (obj is null)
            return Result.Fail("Object to modify does not exist within the Task");

        var renameResult = obj.Rename(name, this);
        if (renameResult.IsFailed) return renameResult;

        var coordsResult = obj.UpdateCoordinates(coords);
        if (coordsResult.IsFailed) return coordsResult;

        return Result.Ok();
    }

    public Result ModifyStep(int stepId, int orderNum, TemplateState init, TemplateState subs, Object obj)
    {
        var step = Steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Fail("Step to modify does not exist within the Task");

        var updateResult = step.Update(orderNum, init, subs, obj);
        if (updateResult.IsFailed) return updateResult;

        UpdateMaxOrderNum();

        return Result.Ok();
    }

    public void RemoveObjects(IEnumerable<int> objectIds)
    {
        Objects.RemoveAll(o => objectIds.Contains(o.Id));
    }

    public void RemoveSteps(IEnumerable<int> stepIds)
    {
        Steps.RemoveAll(s => stepIds.Contains(s.Id));
        UpdateMaxOrderNum();
    }


    public Result Rename(string newName, Job parentJob)
    {
        if (parentJob.Tasks.Where(t => t.Id != Id).Select(t => t.Name).Contains(newName))
            return Result.Fail("Tasks' names within the same Job must be unique");

        Name = newName;

        return Result.Ok();
    }

    public Result<bool> AddEventToCurrentInstance(int stepId, EventResult eventResult, Detector detector)
    {
        if (OngoingInstance is null)
            return Result.Fail("Task does not have an ongoing instance");

        var step = Steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Fail("Task does not contain the referenced Step");

        var addEventResult = OngoingInstance.AddEvent(Steps, step, eventResult);
        if (addEventResult.IsFailed) return addEventResult;

        if (OngoingInstance.State is TaskInstanceState.Completed)
        {
            detector.RemoveFromState(DetectorState.Monitoring);
            detector.AddToState(DetectorState.Standby);
            
            OngoingInstance = null;
            Location.OngoingTask = null;
            
            return Result.Ok(true);
        }

        return Result.Ok(false);
    }

    public Result StopCurrentInstance()
    {
        if (OngoingInstance is null)
            return Result.Fail("Task does not have an ongoing instance");

        var abandonResult = OngoingInstance.Abandon();
        if (abandonResult.IsFailed) return abandonResult;

        OngoingInstance = null;

        return Result.Ok();
    }

    public Result PauseCurrentInstance()
    {
        if (OngoingInstance is null)
            return Result.Fail("Task does not have an ongoing instance");

        var pauseResult = OngoingInstance.Pause();
        return pauseResult.IsFailed ? pauseResult : Result.Ok();
    }

    public Result ResumeCurrentInstance()
    {
        if (OngoingInstance is null)
            return Result.Fail("Task does not have an ongoing instance");

        var resumeResult = OngoingInstance.Resume();
        return resumeResult.IsFailed ? resumeResult : Result.Ok();
    }

    private class EventDTO
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventResult { get; set; } = default!;
        public string? FailureReason { get; set; } = default!;
        public int StepId { get; set; }
        public int TaskInstanceId { get; set; }
    }
    public async Task<Result> SaveEventLog(IRepository<TaskInstance> instanceRepo)
    {
        try
        {
            if (OngoingInstanceId is null)
            {
                return Result.Fail("task has no ongoing instance");
            }
            
            var instance = await instanceRepo.FirstOrDefaultAsync(new TaskInstanceWithEventsByIdSpec(OngoingInstanceId.Value));
            if (instance is null)
            {
                return Result.Fail("instance is not found");
            }

            var output = instance.Events.Select(element => new EventDTO
                {
                    Id = element.Id,
                    Timestamp = element.Timestamp,
                    EventResult = element.Result.Success.ToString(),
                    FailureReason = element.Result.FailureReason,
                    StepId = element.StepId,
                    TaskInstanceId = element.TaskInstanceId
                })
                .ToList();


            await using var writer = new StreamWriter($"../../../plm-ssp/logs/events/{Name}-{DateTime.Now}.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            await csv.WriteRecordsAsync(output);
        }
        catch(Exception e)
        {
            return Result.Fail(e.Message);
        }

        return Result.Ok();
    }
}