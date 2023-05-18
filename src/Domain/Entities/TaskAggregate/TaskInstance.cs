using Domain.Common;
using FluentResults;

namespace Domain.Entities.TaskAggregate;

public class TaskInstance : IBaseEntity
{
    public int Id { get; private set; }
    public TaskInstanceState State { get; set; }
    public List<Event> Events { get; private set; } = default!;

    public int CurrentOrderNum { get; private set; } = 1;

    public Task Task { get; private set; } = default!;
    public int TaskId { get; private set; }
    public int[] RemainingStepIds { get; set; } = default!;

    private TaskInstance()
    {
    }

    private TaskInstance(int id, TaskInstanceState state, int taskId, int[] remainingStepIds, int currentOrderNum)
    {
        Id = id;
        State = state;
        TaskId = taskId;
        RemainingStepIds = remainingStepIds;
        CurrentOrderNum = currentOrderNum;
    }

    public TaskInstance(Task task)
    {
        Events = new List<Event>();
        RemainingStepIds = task.Steps.Select(s => s.Id).ToArray();
        State = TaskInstanceState.InProgress;
    }

    private void RemoveRemainingStep(int stepId)
    {
        var list = RemainingStepIds.ToList();
        list.Remove(stepId);
        RemainingStepIds = list.ToArray();
    }

    public Result AddEvent(List<Step> steps, Step thisStep, EventResult result)
    {
        if (!RemainingStepIds.Contains(thisStep.Id))
            return Result.Fail("Current Task instance does not expect this Step");

        Events.Add(new Event(DateTime.Now, result, thisStep.Id, Id));

        if (result.Success)
        {
            RemoveRemainingStep(thisStep.Id);

            CurrentOrderNum = !RemainingStepIds.Any()
                ? 0
                : steps.FindAll(s => RemainingStepIds.Contains(s.Id)).Select(s => s.OrderNum).Min();

            if (!RemainingStepIds.Any())
                State = TaskInstanceState.Completed; 
        }
        
        return Result.Ok();
    }

    public Result Abandon()
    {
        if (State is TaskInstanceState.Completed or TaskInstanceState.Abandoned)
            return Result.Fail("The Instance has been finished, therefore it cannot be stopped");

        State = TaskInstanceState.Abandoned;
        return Result.Ok();
    }

    public Result Pause()
    {
        if (State is not TaskInstanceState.InProgress)
            return Result.Fail("The Instance is not in progress, therefore it cannot be paused");

        State = TaskInstanceState.Paused;
        return Result.Ok();
    }

    public Result Resume()
    {
        if (State is not TaskInstanceState.Paused)
            return Result.Fail("The Instance is not paused, therefore it cannot be resumed");

        State = TaskInstanceState.InProgress;
        return Result.Ok();
    }
}
