using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Common;
using Api.Domain.Entities;
using Api.Infrastructure.Database;
using Api.Infrastructure.Validation;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Task = Api.Domain.Entities.Task;
using TaskStatus = Api.Domain.Common.TaskStatus;

namespace Api.Features.Tasks
{
    public class Update : EndpointBaseAsync
        .WithRequest<Update.Req>
        .WithActionResult
    {
        private readonly Context _context;
        private readonly IValidator<Req> _validator;

        public Update(Context context, IValidator<Req> validator)
        {
            _context = context;
            _validator = validator;
        }

        public class Req
        {
            [FromRoute(Name = "id")] public int Id { get; set; }

            [FromBody] public ReqBody Body { get; set; } = default!;

            public record ReqBody(string Name, bool? Ordered, List<Template>? Templates);

            public record Template(
                string Name,
                int? X, int? Y, int? Width, int? Height,
                int? OrderNum,
                TemplateState? ExpectedInitialState,
                TemplateState? ExpectedSubsequentState);
        }

        public class Validator : AbstractValidator<Req>
        {
            public Validator(Context context)
            {
                RuleFor(t => t.Body.Name).NotEmpty().MaximumLength(64);
                RuleFor(t => t.Body.Templates).Must(HaveUniqueNamesWithinTask)
                    .WithMessage("All Template must have unique names within the parent Task");
                RuleFor(t => t.Body.Templates).Must(HaveTemplatesInOrder).When(t => t.Body.Ordered.HasValue && t.Body.Ordered.Value)
                    .WithMessage("Templates' order numbers must be in ascending order, with starting index 1 and step 1, if the Task is Ordered.");
                RuleFor(t => t.Body.Templates).Must(BeUnordered).When(t => t.Body.Ordered.HasValue && !t.Body.Ordered.Value)
                    .WithMessage("Templates' order numbers must all be null, if the Task is not Ordered.");
                RuleForEach(t => t.Body.Templates).SetValidator(new TemplateValidator());
            }

            private bool BeUnordered(List<Req.Template>? ts) =>
                ts is null || ts.All(t => t.OrderNum is null);

            private bool HaveTemplatesInOrder(List<Req.Template>? ts)
            {
                if (ts is null) return true;

                // There's probably a nicer way of doing this
                var orderNums = ts.Select(t => t.OrderNum).OrderBy(n => n).ToList();
                return orderNums.Distinct().Count() == orderNums.Count
                    && orderNums[0] == 1
                    && orderNums[^1] == orderNums.Count;
            }

            private bool HaveUniqueNamesWithinTask(List<Req.Template>? ts)
            {
                if (ts is null) return true;

                var names = ts.Select(t => t.Name).ToList();
                return names.Distinct().Count() == names.Count;
            }
        }

        public class TemplateValidator : AbstractValidator<Req.Template>
        {
            public TemplateValidator()
            {
                RuleFor(t => t).Must(BeWithinBounds)
                    .WithMessage("All Templates must fit within the 640x480 snapshot area");
                // TODO(rg): NotNull for x,y,w,h?
                RuleFor(t => t.ExpectedInitialState).NotNull();
                RuleFor(t => t.ExpectedSubsequentState).NotNull();
            }

            private bool BeWithinBounds(Req.Template t)
            {
                if (t.X < 0 || t.Y < 0 || t.Width <= 0 | t.Height <= 0)
                    return false;

                if (t.X + t.Width > 640 || t.Y + t.Height > 640)
                    return false;

                return true;
            }

        }

        [HttpPut(Routes.Tasks.Update)]
        [SwaggerOperation(
            Summary = "Update an existing task",
            Description = "Update an existing task",
            OperationId = "Tasks.Update",
            Tags = new[] { "Tasks" })
        ]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] Req req,
            CancellationToken ct = new())
        {
            var validation = await _validator.ValidateToModelStateAsync(req, ModelState, ct);
            if (!validation.IsValid)
                return ValidationProblem();

            var task = await _context.Tasks
                .Where(t => t.Id == req.Id)
                .Include(t => t.Templates)
                .Include(t => t.Job)
                .ThenInclude(j => j.Tasks)
                .SingleOrDefaultAsync(ct);

            if (task is null) return NotFound();
            if (task.Status is not TaskStatus.Inactive)
                return BadRequest("The Task must be in the Inactive state");

            if (!ValidateTaskNamesUniqueness(req, task))
                return BadRequest("'Name' must be unique within the parent Job");

            var qa = task.Job.Type == JobType.QA;

            task.Name = req.Body.Name;

            if (req.Body.Ordered.HasValue)
            {
                if (qa)
                    return BadRequest("QA tasks cannot set the 'Ordered' field");

                task.Ordered = req.Body.Ordered!.Value;
            }

            if (req.Body.Templates is not null)
            {
                if (qa)
                    return BadRequest("QA tasks cannot have templates");

                _context.RemoveRange(task.Templates!);

                var newTemplates = MapTemplates(req);
                task.Templates = newTemplates;
            }

            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        private static List<Template> MapTemplates(Req req) =>
            req.Body.Templates!.Select(t => new Template
            {
                Name = t.Name,
                X = t.X!.Value,
                Y = t.Y!.Value,
                Width = t.Width!.Value,
                Height = t.Height!.Value,
                OrderNum = t.OrderNum,
                ExpectedInitialState = t.ExpectedInitialState!.Value,
                ExpectedSubsequentState = t.ExpectedSubsequentState!.Value
            }).ToList();

        private static bool ValidateTaskNamesUniqueness(Req req, Task task) =>
            task.Job.Tasks
                .Where(t => t.Id != req.Id)
                .All(t => t.Name != req.Body.Name);
    }
}