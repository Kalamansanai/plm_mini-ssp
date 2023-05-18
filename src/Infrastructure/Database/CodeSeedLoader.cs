using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.CompanyHierarchy;
using Domain.Entities.TaskAggregate;
using BindingFlags = System.Reflection.BindingFlags;
using File = System.IO.File;
using Object = Domain.Entities.TaskAggregate.Object;
using Task = Domain.Entities.TaskAggregate.Task;

namespace Infrastructure.Database;

public class CodeSeedLoader
{
    private readonly Context _context;
    private readonly byte[] _snapshot;

    private static readonly ConstructorInfo SiteConstructor =
        typeof(Site).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string) })!;

    private static readonly ConstructorInfo OpuConstructor =
        typeof(OPU).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string), typeof(int) })!;

    private static readonly ConstructorInfo LineConstructor =
        typeof(Line).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string), typeof(int) })!;

    private static readonly ConstructorInfo StationConstructor =
        typeof(Station).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string), typeof(int) })!;

    private static readonly ConstructorInfo LocationConstructor =
        typeof(Location).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string), typeof(int), typeof(byte[]), typeof(int?) })!;

    private static readonly ConstructorInfo ObjectConstructor =
        typeof(Object).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string), typeof(ObjectCoordinates), typeof(int) })!;

    private static readonly ConstructorInfo StepConstructor =
        typeof(Step).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[]
            {
                typeof(int), typeof(int), typeof(TemplateState), typeof(TemplateState), typeof(int), typeof(int)
            })!;

    private static readonly ConstructorInfo TaskConstructor =
        typeof(Task).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[]
            {
                typeof(int), typeof(string), typeof(TaskType), typeof(int), typeof(int), typeof(int?), typeof(int)
            })!;

    private static readonly ConstructorInfo JobConstructor =
        typeof(Job).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string) })!;

    private static readonly ConstructorInfo DetectorConstructor =
        typeof(Detector).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(string), typeof(string), typeof(string), typeof(DetectorState), typeof(int) })!;

    private static readonly ConstructorInfo TaskInstanceConstructor =
        typeof(TaskInstance).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(TaskInstanceState), typeof(int), typeof(int[]), typeof(int) })!;

    private static readonly ConstructorInfo EventConstructor =
        typeof(Event).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(int), typeof(DateTime), typeof(bool), typeof(string), typeof(int), typeof(int) })!;

    public CodeSeedLoader(Context context, string seedFolderRelativePath)
    {
        _context = context;

        var cd = Directory.GetCurrentDirectory();
        var path = Path.Combine(cd, seedFolderRelativePath, "Files", "snapshot.jpg");

        _snapshot = File.ReadAllBytes(path);
    }

    public void Load()
    {
        var sites = new List<Site>
        {
            (Site)SiteConstructor.Invoke(new object?[] { 1, "Site 1"})
        };

        var opus = new List<OPU>
        {
            (OPU)OpuConstructor.Invoke(new object?[] { 1, "OPU 1-1", 1 }),
        };

        var lines = new List<Line>
        {
            (Line)LineConstructor.Invoke(new object?[] { 1, "Line 1-1", 1 }),
        };

        var stations = new List<Station>
        {
            (Station)StationConstructor.Invoke(new object?[] { 1 ,  "Station 1-1", 1 }),
        };

        var locations = new List<Location>
        {
            (Location)LocationConstructor.Invoke(new object?[] { 1, "Location 1", _snapshot, null, 1 }),
            (Location)LocationConstructor.Invoke(new object?[] { 2, "Location 2", _snapshot, null, 1 }),
            (Location)LocationConstructor.Invoke(new object?[] { 3, "Location 3", _snapshot, null, 1 }),
            (Location)LocationConstructor.Invoke(new object?[] { 4, "Location 4", null, null, 1 })
        };

        var objects = new List<Object>
        {
            (Object)ObjectConstructor.Invoke(new object?[]
                { 1, "Object 1", new ObjectCoordinates { X = 20, Y = 20, Width = 100, Height = 100 }, 1 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 2, "Object 2", new ObjectCoordinates { X = 200, Y = 300, Width = 100, Height = 80 }, 1 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 3, "Object 3", new ObjectCoordinates { X = 50, Y = 100, Width = 100, Height = 200 }, 1 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 4, "Object 4", new ObjectCoordinates { X = 300, Y = 300, Width = 100, Height = 100 }, 2 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 5, "Object 5", new ObjectCoordinates { X = 40, Y = 20, Width = 20, Height = 20 }, 2 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 6, "Object 6", new ObjectCoordinates { X = 80, Y = 20, Width = 20, Height = 20 }, 2 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 7, "Object 7", new ObjectCoordinates { X = 300, Y = 300, Width = 100, Height = 100 }, 3 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 8, "Object 8", new ObjectCoordinates { X = 40, Y = 20, Width = 20, Height = 20 }, 3 }),
            (Object)ObjectConstructor.Invoke(new object?[]
                { 9, "Object 9", new ObjectCoordinates { X = 80, Y = 20, Width = 20, Height = 20 }, 3 })
        };

        var steps = new List<Step>
        {
            (Step)StepConstructor.Invoke(new object?[]
                { 1, 1, TemplateState.Present, TemplateState.Missing, 1, 1 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 2, 2, TemplateState.Present, TemplateState.Missing, 2, 1 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 3, 3, TemplateState.Present, TemplateState.Missing, 3, 1 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 4, 1, TemplateState.Present, TemplateState.Missing, 4, 2 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 5, 2, TemplateState.Present, TemplateState.Missing, 5, 2 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 6, 3, TemplateState.Present, TemplateState.Missing, 6, 2 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 7, 1, TemplateState.Present, TemplateState.Missing, 7, 3 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 8, 2, TemplateState.Present, TemplateState.Missing, 8, 3 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 9, 3, TemplateState.Present, TemplateState.Missing, 9, 3 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 10, 1, TemplateState.Missing, TemplateState.Present, 7, 3 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 11, 2, TemplateState.Missing, TemplateState.Present, 8, 3 }),
            (Step)StepConstructor.Invoke(new object?[]
                { 12, 3, TemplateState.Missing, TemplateState.Present, 9, 3 })
        };

        var tasks = new List<Task>
        {
            (Task)TaskConstructor.Invoke(new object?[]
                { 1, "Task 1", TaskType.ItemKit, 1, 1, null, 3 }),
            (Task)TaskConstructor.Invoke(new object?[]
                { 2, "Task 2", TaskType.ToolKit, 2, 1, null, 3 }),
            (Task)TaskConstructor.Invoke(new object?[]
                { 3, "Task 3", TaskType.ToolKit, 1, 1, null, 3 })
        };

        var jobs = new List<Job>
        {
            (Job)JobConstructor.Invoke(new object?[] { 1, "Job 1" }),
            (Job)JobConstructor.Invoke(new object?[] { 2, "Job 2" }),
            (Job)JobConstructor.Invoke(new object?[] { 3, "Job 3" }),
            (Job)JobConstructor.Invoke(new object?[] { 4, "Job 4" }),
            (Job)JobConstructor.Invoke(new object?[] { 5, "Job 5" }),
            (Job)JobConstructor.Invoke(new object?[] { 6, "Job 6" }),
            (Job)JobConstructor.Invoke(new object?[] { 7, "Job 7" }),
            (Job)JobConstructor.Invoke(new object?[] { 8, "Job 8" }),
            (Job)JobConstructor.Invoke(new object?[] { 9, "Job 9" }),
            (Job)JobConstructor.Invoke(new object?[] { 10, "Job 10" })
        };

        var detectors = new List<Detector>
        {
            (Detector)DetectorConstructor.Invoke(new object?[]
                { 1, "Detector 1", "11:22:33:44:55:66", "127.0.0.1", DetectorState.Standby, 1 }),
            (Detector)DetectorConstructor.Invoke(new object?[]
                { 2, "Detector 2", "12:34:56:78:90:AB", "127.0.0.1", DetectorState.Off, 2 })
        };

        var taskInstances = new List<TaskInstance>
        {
            (TaskInstance)TaskInstanceConstructor.Invoke(new object?[]
            {
                1, TaskInstanceState.Completed, 3, Array.Empty<int>(), 0
            }),
            (TaskInstance)TaskInstanceConstructor.Invoke(new object?[]
            {
                2, TaskInstanceState.Abandoned, 3, new[] { 8, 9, 10, 11, 12 }, 1
            }),
            (TaskInstance)TaskInstanceConstructor.Invoke(new object?[]
            {
                3, TaskInstanceState.InProgress, 3, new[] { 11, 12 }, 2
            })
        };

        var events = new List<Event>
        {
            (Event)EventConstructor.Invoke(new object?[]
                { 1, DateTime.Now - TimeSpan.FromSeconds(120), true, null, 7, 1 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 2, DateTime.Now - TimeSpan.FromSeconds(100), true, null, 8, 1 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 3, DateTime.Now - TimeSpan.FromSeconds(87), true, null, 9, 1 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 4, DateTime.Now - TimeSpan.FromSeconds(45), true, null, 10, 1 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 5, DateTime.Now - TimeSpan.FromSeconds(23), true, null, 11, 1 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 6, DateTime.Now - TimeSpan.FromSeconds(2), true, null, 12, 1 }),

            (Event)EventConstructor.Invoke(new object?[]
                { 7, DateTime.Now - TimeSpan.FromSeconds(50), false, "Wrong initial state", 7, 2 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 8, DateTime.Now - TimeSpan.FromSeconds(25), true, null, 8, 2 }),

            (Event)EventConstructor.Invoke(new object?[]
                { 9, DateTime.Now - TimeSpan.FromSeconds(120), true, null, 7, 3 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 10, DateTime.Now - TimeSpan.FromSeconds(100), true, null, 8, 3 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 11, DateTime.Now - TimeSpan.FromSeconds(87), false, "Wrong initial state", 9, 3 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 12, DateTime.Now - TimeSpan.FromSeconds(45), true, null, 9, 3 }),
            (Event)EventConstructor.Invoke(new object?[]
                { 13, DateTime.Now - TimeSpan.FromSeconds(23), true, null, 10, 3 }),
        };

        _context.Sites.AddRange(sites);
        _context.OPUs.AddRange(opus);
        _context.Lines.AddRange(lines);
        _context.Stations.AddRange(stations);
        _context.Locations.AddRange(locations);

        _context.Jobs.AddRange(jobs);
        _context.Tasks.AddRange(tasks);
        _context.Objects.AddRange(objects);
        _context.Steps.AddRange(steps);
        _context.Detectors.AddRange(detectors);
        _context.TaskInstances.AddRange(taskInstances);
        _context.Events.AddRange(events);

        _context.SaveChanges();

        _context.Locations.First(l => l.Id == 1).OngoingTaskId = 3;
        _context.Tasks.First(t => t.Id == 3).OngoingInstanceId = 3;

        _context.SaveChanges();
    }
}