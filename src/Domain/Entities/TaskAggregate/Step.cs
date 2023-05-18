using System.Text;
using Domain.Common;
using FluentResults;

namespace Domain.Entities.TaskAggregate;

public class Step : IBaseEntity
{
    public int Id { get; set; }
    public int OrderNum { get; private set; }
    public TemplateState ExpectedInitialState { get; private set; }
    public TemplateState ExpectedSubsequentState { get; private set; }

    public Object Object { get; private set; } = default!;
    public int ObjectId { get; private set; }

    public int TaskId { get; private set; }

    private Step()
    {
    }

    public Result Update(int orderNum, TemplateState init, TemplateState subs, Object obj)
    {
        if (!init.IsValid()) return Result.Fail("Invalid expected initial state");
        if (!subs.IsValid()) return Result.Fail("Invalid subsequent initial state");

        OrderNum = orderNum;
        ExpectedInitialState = init;
        ExpectedSubsequentState = subs;
        Object = obj;

        return Result.Ok();
    }

    private Step(int id, int orderNum, TemplateState init, TemplateState subs, int objectId, int taskId)
    {
        Id = id;
        OrderNum = orderNum;
        ExpectedInitialState = init;
        ExpectedSubsequentState = subs;
        ObjectId = objectId;
        TaskId = taskId;
    }

    public static Step Create(int orderNum, TemplateState exInitState, TemplateState exSubsState, Object obj)
    {
        return new Step
        {
            OrderNum = orderNum,
            ExpectedInitialState = exInitState,
            ExpectedSubsequentState = exSubsState,
            Object = obj
        };
    }
    public Step(int id)
    {
        Id = id;
    }
}