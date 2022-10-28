using Domain.Entities;
using Domain.Specifications;
using FastEndpoints;
using Infrastructure.Database;

namespace Api.Endpoints.Tasks;

public class Delete: Endpoint<Delete.Req, EmptyResponse>
{
    public IRepository<Job> JobRepo { get; set; } = default!;
    public IRepository<Domain.Entities.TaskAggregate.Task> TaskRepo { get; set; } = default!;

    public class Req
    {
        public int Id { get; set; }
        public int ParentJobId { get; set; }
    }

    public override void Configure()
    {
        Delete(Api.Routes.Tasks.Delete);
        AllowAnonymous();
        Options(x => x.WithTags("Tasks"));
    }
    
    public override async Task HandleAsync(Req req, CancellationToken ct)
    {
        var job = await JobRepo.GetByIdAsync(req.ParentJobId, ct);
        
        if (job is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        // TODO: check detectors below 
        var task = await TaskRepo.FirstOrDefaultAsync(new TaskWithChildrenSpec(req.Id), ct);
        if (task is null)
        {
            await SendNotFoundAsync(ct);
            return;   
        }
        
        job.DeleteTask(task);
        await TaskRepo.DeleteAsync(task);
        await JobRepo.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}