using Domain.Interfaces;
using FluentResults;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Domain.Entities;

public class Job : IBaseEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public List<Task> Tasks { get; set; } = default!;

    private Job()
    {
    }

    private Job(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public Job(string name)
    {
        Name = name;
    }

    public static Result<Job> Create(string name, IJobNameUniquenessChecker nameUniquenessChecker)
    {
        if (nameUniquenessChecker.IsDuplicate(name, null).GetAwaiter().GetResult())
            return Result.Fail("Duplicate name");

        return Result.Ok(new Job { Name = name });
    }

    public Result Rename(string newName, IJobNameUniquenessChecker nameUniquenessChecker)
    {
        if (nameUniquenessChecker.IsDuplicate(newName, this).GetAwaiter().GetResult())
            return Result.Fail("Duplicate name");

        Name = newName;

        return Result.Ok();
    }

    public void DeleteTask(Task task)
    {
        Tasks.Remove(task);
    }
}