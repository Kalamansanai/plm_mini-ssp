using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Infrastructure.Database;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Features.Detectors
{
    public class Delete : EndpointBaseAsync
        .WithRequest<Delete.Req>
        .WithActionResult
    {
        private readonly Context _context;

        public Delete(Context context)
        {
            _context = context;
        }

        public class Req
        {
            [FromRoute(Name = "id")] public int Id { get; set; }
        }

        public class Validator : AbstractValidator<Req>
        {
            private readonly Context _context;

            public Validator(Context context)
            {
                _context = context;

                RuleFor(r => r.Id).Must(BeInOffState).WithMessage("Detector must be exist and be in OFF state");
            }

            private bool BeInOffState(int id)
            {
                // TODO(rg): the detector query is duplicated (here and in the action method). Figure out what to do about that
                var detector = _context.Detectors.SingleOrDefault(d => d.Id == id);

                if (detector is null) return false;

                return detector.State is DetectorState.Off;
            }
        }

        [HttpDelete(Routes.Detectors.Delete)]
        [SwaggerOperation(
            Summary = "Delete detector",
            Description = "Delete detector",
            OperationId = "Detectors.Delete",
            Tags = new[] { "Detectors" })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] Req req, CancellationToken ct = new())
        {
            var existingDetector = await _context.Detectors.Where(l => l.Id == req.Id).SingleOrDefaultAsync(ct);
            // NOTE(rg): already checked for null in the validator

            _context.Detectors.Remove(existingDetector);
            await _context.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}