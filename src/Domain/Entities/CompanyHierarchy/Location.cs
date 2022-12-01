using Domain.Common;
using Domain.Interfaces;
using FluentResults;
using static FluentResults.Result;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Domain.Entities.CompanyHierarchy;

public class Location : ICHNodeWithParent<Station>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Station Parent { get; set; } = default!;
    public int ParentId { get; set; }
    public Detector? Detector { get; set; }
    public List<Task> Tasks { get; set; } = default!;
    public CalibrationCoordinates? Coordinates { get; set; } = default!;

    public byte[]? Snapshot { get; set; }

    private Location() {}

    private Location(int id, string name, int parentId, bool snapshot)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Snapshot = snapshot ? new[] { (byte) 2, (byte) 3 } : null ;
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

    public async Task<Result> SendRecalibrate(IDetectorConnection DetectorConnection, int[]? newTrayCoordinates=null)
    {
        if (Detector is null || Detector.State == DetectorState.Off)
        {
            return Fail("This location has no active detector!");
        }

        var result = await Detector.SendRecalibrate(Coordinates, DetectorConnection, newTrayCoordinates);

        Coordinates = result.Value;

        return Ok();
    }
    
}