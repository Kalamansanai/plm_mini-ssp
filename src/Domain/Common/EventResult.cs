using System.Text.Json.Serialization;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Domain.Common;

[Owned]
public class EventResult
{
    public bool Success { get; set; }
    public string? FailureReason { get; set; }

    private EventResult() {}
    public EventResult(bool success)
    {
        Success = success;
    }

    public static Result<EventResult> Create(bool success, string? failureReason)
    {
        if (success && failureReason is not null)
            return Result.Fail("Successful events cannot have a failure reason");

        if (!success && failureReason is null)
            return Result.Fail("Failed events must have a failure reason");

        return new EventResult
        {
            Success = success,
            FailureReason = failureReason
        };
    }
}