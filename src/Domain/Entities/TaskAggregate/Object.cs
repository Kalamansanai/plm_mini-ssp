using Domain.Common;
using FluentResults;

namespace Domain.Entities.TaskAggregate;

public partial class Object : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public ObjectCoordinates Coordinates { get; set; } = default!;
    public int TaskId { get; set; }

    private Object()
    {
    }

    private Object(int id, string name, ObjectCoordinates coordinates, int taskId)
    {
        Id = id;
        Name = name;
        Coordinates = coordinates;
        TaskId = taskId;
    }
    public Object(string name)
    {
        Name = name;
    }

    public static Result<Object> Create(string newName, ObjectCoordinates newCoordinates, Task parent)
    {
        if (parent.Objects.Select(o => o.Name).Contains(newName))
            return Result.Fail("Objects' names within the same Task must be unique");

        if (!newCoordinates.IsValid())
            return Result.Fail("New object coordinates are invalid");

        return new Object
        {
            Name = newName,
            Coordinates = newCoordinates,
            TaskId = parent.Id
        };
    }

    public Result Rename(string newName, Task parent)
    {
        if (parent.Objects.Where(o => o.Id != Id).Select(o => o.Name).Contains(newName))
            return Result.Fail("Objects' names within the same Task must be unique");

        Name = newName;

        return Result.Ok();
    }

    public Result UpdateCoordinates(ObjectCoordinates coords)
    {
        if (!coords.IsValid())
            return Result.Fail("New object coordinates are invalid");

        Coordinates = coords;

        return Result.Ok();
    }
}