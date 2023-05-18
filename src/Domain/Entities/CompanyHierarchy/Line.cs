using Domain.Interfaces;
using FluentResults;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Entities.CompanyHierarchy;

public class Line : ICHNode<OPU, Station>
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public List<Station> Children { get; private set; } = default!;

    public OPU Parent { get; private set; } = default!;
    public int ParentId { get; private set; } = 1;

    private Line() {}
    public Line(int id, string name, int parentID)
    {
        Id = id;
        Name = name;
        ParentId = parentID;
    }
    public Line(string name)
    {
        Name = name;
    }

    public Result<Station> AddChildNode(string stationName,
        ICHNameUniquenessChecker<Line, Station> nameUniquenessChecker)
    {
        if (nameUniquenessChecker.IsDuplicate(this, stationName, null).GetAwaiter().GetResult())
        {
            return Result.Fail("Duplicate name");
        }

        var station = new Station(stationName);
        if (Children.IsNullOrEmpty())
        {
            Children = new List<Station>();
        }
        Children.Add(station);

        return Result.Ok(station);
    }
}