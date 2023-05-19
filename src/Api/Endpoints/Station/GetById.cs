using Domain.Common;
using Domain.Entities;
using Domain.Entities.CompanyHierarchy;
using Domain.Interfaces;
using Domain.Specifications;
using FastEndpoints;

namespace Api.Endpoints.Stations;

public class GetById : Endpoint<GetById.Req, GetById.Res>
{
    public IRepository<Station> StationRepo { get; set; } = default!;
    public IRepository<Detector> DetectorRepo { get; set; } = default!;

    public class Req
    {
        public int Id { get; set; }
    }

    public class Res
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public IEnumerable<LocationRes> Locations { get; set; } = default!;

        public record LocationRes(int Id, string Name, bool HasSnapshot, DetectorRes? Detector);

        public record DetectorRes(int Id, string Name, string MacAddress, DetectorState State);
    }

    private static Res MapOut(Station s) =>
        new()
        {
            Id = s.Id,
            Name = s.Name,
            Locations = s.Children.Select(l =>
                new Res.LocationRes(l.Id, l.Name, l.Snapshot != null,
                    l.Detector is null
                        ? null
                        : new Res.DetectorRes(l.Detector.Id, l.Detector.Name, l.Detector.MacAddress.ToString(),
                            l.Detector.State)))
        };

    public override void Configure()
    {
        Get(Api.Routes.Stations.GetById);
        AllowAnonymous();
        Options(x => x.WithTags("Stations"));
    }

    public override async Task HandleAsync(Req req, CancellationToken ct)
    {
        var station = await StationRepo.FirstOrDefaultAsync(new StationWithLocationsAndDetectorsSpec(req.Id), ct);

        if (station is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        foreach (var location in station.Children)
        {
            if (location.Detector != null)
            {
                var detector = await DetectorRepo.FirstOrDefaultAsync(new HeartBeatByDetectorIdSpec(location.Detector.Id), ct);

                detector?.CheckState();
            }
        }
        await DetectorRepo.SaveChangesAsync(ct);


        var res = MapOut(station);
        await SendOkAsync(res, ct);
    }
}