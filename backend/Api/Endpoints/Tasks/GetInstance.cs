using Domain.Common;
using Domain.Entities.TaskAggregate;
using Domain.Interfaces;
using Domain.Specifications;
using FastEndpoints;

namespace Api.Endpoints.Tasks;

public class GetInstance : Endpoint<GetInstance.Req, GetInstance.Res>
{
    public IRepository<TaskInstance> InstanceRepo { get; set; } = default!;
    public class Req
    {
        public int TaskId { get; set; }
    }

    public class Res
    {
        public ResTaskInstance Instance { get; set; } = default!;

        public record ResTaskInstance(int Id, TaskInstanceFinalState? FinalState, IEnumerable<ResEvent> Events, int TaskId);

        public record ResEvent(int Id, DateTime TimeStamp, string? FailureReason, int StepId, int TaskInstanceId);

    }

    public override void Configure()
    {
        Get(Api.Routes.Tasks.GetInstance);
        AllowAnonymous();
        Options(x => x.WithTags("Tasks"));
    }

    public override async System.Threading.Tasks.Task HandleAsync(Req req, CancellationToken ct)
    {
        var instance = await InstanceRepo.FirstOrDefaultAsync(new TaskInstanceWithEvents(req.TaskId), ct);

        if (instance is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var resEvents = instance.Events.Select(e =>
            new Res.ResEvent(e.Id, e.Timestamp, e.FailureReason, e.StepId, e.TaskInstanceId));
        
        var res = new Res();
        res.Instance = new Res.ResTaskInstance(instance.Id, instance.FinalState, resEvents, instance.TaskId);
        await SendOkAsync(res, ct);
    }
}