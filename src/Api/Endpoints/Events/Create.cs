using Domain.Common;
using Domain.Entities.CompanyHierarchy;
using Domain.Entities.TaskAggregate;
using Domain.Interfaces;
using Domain.Specifications;
using FastEndpoints;
using Infrastructure;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Api.Endpoints.Events;

public class Create: Endpoint<Create.Req, EmptyResponse>
{
    public IRepository<Task> TaskRepo { get; set; } = default!;
    public IRepository<Location> LocationRepo { get; set; } = default!;
    public INotifyChannel NotifyChannel { get; set; } = default!;
    public IRepository<TaskInstance> InstanceRepo { get; set; }
    public class Req
    {
        public int TaskId { get; set; }
        public int StepId { get; set; }
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
    }

    public override void Configure()
    {
        Post(Api.Routes.Events.Create);
        AllowAnonymous();
        Options(x => x.WithTags("Events"));
    }

    public override async System.Threading.Tasks.Task HandleAsync(Req req, CancellationToken ct)
    {
        var task = await TaskRepo.FirstOrDefaultAsync(new TaskWithChildrenSpec(req.TaskId), ct);
        if (task is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var location = await LocationRepo.FirstOrDefaultAsync(new LocationWithDetectorSpec(task.LocationId), ct);
        if (location is null)
        {
            ThrowError("Location does not exist");
            return;
        }

        if (location.Detector is null)
        {
            ThrowError("Location does not have a Detector attached to it");
            return;
        }

        if (!Equals(HttpContext.Connection.RemoteIpAddress, location.Detector.IpAddress))
        {
            ThrowError("Remote IP address is not equal with the detector's IP");
            return;
        }

        var eventResult = EventResult.Create(req.Success, req.FailureReason).Unwrap();

        var isEnded = task.AddEventToCurrentInstance(req.StepId, eventResult, location.Detector).Unwrap();
        // if (isEnded)
        // {
        //     var res = await task.SaveEventLog(InstanceRepo);
        //     res.Unwrap();
        //     Console.WriteLine("SaveEventLog:" + res);
        // }
        
        await TaskRepo.SaveChangesAsync(ct);
        
        //SSE
        NotifyChannel.AddNotify(location.Id);
        await SendNoContentAsync(ct);
    }
}
