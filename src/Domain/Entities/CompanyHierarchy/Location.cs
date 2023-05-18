using Domain.Common;
using Domain.Interfaces;
using FluentResults;
using static FluentResults.Result;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Domain.Entities.CompanyHierarchy;

public class Location : ICHNodeWithParent<Station>
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public Station Parent { get; private set; } = default!;
    public int ParentId { get; private set; }
    public Detector? Detector { get; set; }
    public List<Task> Tasks { get; private set; } = default!;

    public Task? OngoingTask { get; set; }
    public int? OngoingTaskId { get; set; }
    // public CalibrationCoordinates? Coordinates { get; set; }

    public byte[]? Snapshot { get; set; }

    private Location() {}

    private Location(int id, string name, byte[]? snapshot, int? ongoingTaskId, int parentID)
    {
        Id = id;
        Name = name;
        Snapshot = snapshot;
        OngoingTaskId = ongoingTaskId;
        ParentId = parentID;
    }

    public Location(string name)
    {
        Name = name;
    }

    public Result Rename(string newName, ICHNameUniquenessChecker<Station, Location> nameUniquenessChecker)
    {
        if (nameUniquenessChecker.IsDuplicate(Parent, newName, this).GetAwaiter().GetResult())
        {
            return Fail("Duplicate name!");
        }

        Name = newName;

        return Ok();
    }

    public Result AttachDetector(Detector newDetector)
    {
        if (Detector is null)
        {
            Detector = newDetector;
            return Ok();
        }

        if (Detector.Id == newDetector.Id)
            return Result.Ok();

        if (Detector.State == DetectorState.Off)
        {
            Detector = newDetector;
            return Ok();
        }

        return Fail("Location already have a detector attached to it!");
    }

    public Result DetachDetector()
    {
        if (Detector is null)
        {
            return Fail("Location already does not have a detector attached to it!");
        }

        if (Detector.State == DetectorState.Streaming)
        {
            return Fail("An other detector currently running on this location!");
        }

        Detector = null;
        return Ok();
    }

    // public async Task<Result<List<CalibrationCoordinates.Koordinates>>> SendRecalibrate(IDetectorConnection detectorConnection, List<CalibrationCoordinates.Koordinates>? newTrayCoordinates=null)
    // {
    //     if (Detector is null || Detector.State == DetectorState.Off)
    //     {
    //         return Fail("This location has no active detector!");
    //     }
    //
    //     var result = await Detector.SendRecalibrate(Coordinates, detectorConnection, newTrayCoordinates);
    //     if (result.IsFailed)
    //     {
    //         return result.ToResult();
    //     }
    //     
    //     return Ok(result.Value);
    // }

}