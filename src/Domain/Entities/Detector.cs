using System.Net;
using System.Net.NetworkInformation;
using Domain.Common;
using Domain.Entities.CompanyHierarchy;
using Domain.Interfaces;
using Domain.Specifications;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public class Detector : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; private set; } = default!;
    public PhysicalAddress MacAddress { get; private set; } = default!;
    public IPAddress IpAddress { get; private set; } = default!;
    public DetectorState State { get; set; }
    public List<HeartBeatLog> HeartBeatLogs { get; private set; } = default!;

    public Location? Location { get; private set; }
    public int? LocationId { get; private set; }

    [Owned]
    public class HeartBeatLog
    {
        public int Id { get; set; }
        public int Temperature { get; set; }
        public int StoragePercentage { get; set; }
        public long Uptime { get; set; }
        public float Cpu { get; set; }
        public float Ram { get; set; } 
        public DateTime TimeStamp { get; set; }
    }

    private Detector() { }

    private Detector(int id, string name, string macAddressString, string ipAddressString, DetectorState state, int locationId)
    {
        Id = id;
        Name = name;
        MacAddress = PhysicalAddress.Parse(macAddressString);
        IpAddress = IPAddress.Parse(ipAddressString);
        State = state;
        HeartBeatLogs = new List<HeartBeatLog>();
        LocationId = locationId;
    }
    public Detector(string name)
    {
        Name = name;
    }

    public static Result<Detector> Create(string newName, PhysicalAddress newMacAddress, IPAddress newAddress, Location? location)
    {
        var detector = new Detector
        {
            Name = newName,
            MacAddress = newMacAddress,
            IpAddress = newAddress,
            State = DetectorState.Standby,
            HeartBeatLogs = new List<HeartBeatLog>()
        };

        if (location is null)
        {
            return Result.Ok(detector);
        }

        return location.AttachDetector(detector).ToResult(detector);
    }

    // public async Task<Result<List<CalibrationCoordinates.Koordinates>>> SendRecalibrate(CalibrationCoordinates? coords, IDetectorConnection detectorConnection, List<CalibrationCoordinates.Koordinates>? newTrayPoints=null)
    // {
    //     if (coords is null)
    //     {
    //         return Result.Fail("This detector has no original coordinates!");
    //     }
    //
    //     var result = await detectorConnection.SendCalibrationData(this, coords, newTrayPoints);
    //     if (result.IsFailed)
    //     {
    //         return result;
    //     }
    //     // we working with a location's detector so the location will always be something
    //     var newCords = new CalibrationCoordinates(result.Value, Location!.Coordinates.Tray);
    //     Location!.Coordinates = newCords;
    //
    //     return Result.Ok(result.Value);
    // }

    public void SetState(DetectorState state)
    {
        State = state;
    }

    public void AddToState(DetectorState state)
    {
        State |= state;
    }

    public void RemoveFromState(DetectorState state)
    {
        State &= ~state;
    }

    public void setIpAddress(IPAddress newIp)
    {
        IpAddress = newIp;
    }

    public void CheckState()
    {
        var lastLog = HeartBeatLogs.LastOrDefault();
        if (lastLog != null)
        {
            Console.WriteLine(lastLog.TimeStamp);
            if (lastLog.TimeStamp.AddSeconds(10) < DateTime.Now)
            {
                State = DetectorState.Off;
                // Location.OngoingTask = null;
            }
        }
        
        
    }
}