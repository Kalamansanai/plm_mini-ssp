using Domain.Interfaces;
using FluentResults;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Entities.CompanyHierarchy;

public class Site : ICHNodeWithChildren<OPU>
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public List<OPU> Children { get; private set; } = default!;

    private Site() {}

    public Site(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public Site(string name)
    {
        Name = name;
    }
    public static Result<Site> New(string name,
        ICHNameUniquenessChecker<Site> nameUniquenessChecker)
    {
        if (nameUniquenessChecker.IsDuplicate(name, null).GetAwaiter().GetResult())
        {
            return Result.Fail("Duplicate name");
        }

        var site = new Site(name);
        return site;
    }

    public Result<OPU> AddChildNode(string opuName, ICHNameUniquenessChecker<Site, OPU> nameUniquenessChecker)
    {
        if (nameUniquenessChecker.IsDuplicate(this, opuName, null).GetAwaiter().GetResult())
        {
            return Result.Fail("Duplicate name");
        }

        var opu = new OPU(opuName);
        if (Children.IsNullOrEmpty())
        {
            Children = new List<OPU>();
        }
        Children.Add(opu);

        return opu;
    }
}