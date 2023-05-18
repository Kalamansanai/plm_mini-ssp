namespace Api;

public static class Routes
{
    public static class Locations
    {
        public const string GetAllFree = "locations/free";
        public const string GetById = "locations/{id}";
        public const string GetTasks = "locations/{id}/tasks";
        public const string Create = "locations";
        public const string Update = "locations/{id}";
        public const string Delete = "locations/{id}";
        public const string GetSnapshot = "locations/{id}/snapshot";
        public const string notify = "locations/{id}/sse-notify";
        public const string GetOngoingTask = "locations{id}/get_ongoing_task";
        public const string GetPrevInstances = "locations/{locationId}/prev-instances";
        public const string RequestSnapshot = "locations/{id}/request-snapshot/{taskId}";
    }

    public static class Detectors
    {
        public const string List = "detectors";

        public const string Command = "detectors/{id}/command";
        public const string Snapshot = "detectors/{id}/snapshot";
        public const string Stream = "detectors/{id}/stream";

        public const string GetAllAttachable = "detectors/attachable";
        public const string GetById = "detectors/{id}";
        public const string Update = "detectors/{id}";
        public const string Delete = "detectors/{id}";

        public const string Attach = "detectors/{LocationId}/attach/{DetectorId}";
        public const string Detach = "detectors/{LocationId}/detach";

        public const string Identify = "detectors/{LocationId}/identify";
        public const string HeartBeat = "detectors/{MacAddress}/heartbeat";

        public const string ReCalibrate = "detectors/{locationId}/recalibrate";
        public const string RequestCalibrationPreview = "detectors/{id}/requestcalibrationpreview";
        public const string GetInitialCalibrationData = "detectors/{id}/getinitialcalibrationdata";
        
        public const string ResetTaskInstance = "detectors/{locationId}/reset";
        public const string CollectData = "detectors/{id}/collect";
        public const string GetHeartBeat = "detectors/{id}/getheartbeat";
    }

    public static class Jobs
    {
        public const string GetById = "jobs/{id}";
        public const string Create = "jobs";
        public const string Update = "jobs/{id}";
        public const string Delete = "jobs/{id}";
        public const string List = "jobs";
    }

    public static class Tasks
    {
        public const string GetById = "tasks/{id}";
        public const string Create = "tasks";
        public const string Update = "tasks/{id}";
        public const string Delete = "tasks/{id}";
        public const string GetObjectsAndEvents = "tasks/{TaskId}/objects_and_steps";
        public const string GetInstance = "tasks/instance/{id}";
    }

    public static class Events
    {
        public const string Create = "events";
        public const string GetByInstanceId = "{instanceId}/events";
    }
    
    public static class Users
    {
        public const string Create = "users/register";
        public const string Delete = "users/{id}";
        public const string Update = "users/{id}";
        public const string SignIn = "user";
        public const string SetRole = "users/{id}/setrole";
        public const string GetUser = "user";
        public const string SignOut = "user/signout";
        public const string List = "user/list";
    }

    public static class PLM_Files
    {
        public const string Upload = "files/upload";
        public const string Download = "files/{name}/download";
        public const string GetNewestVersion = "files/{name}/version";
    }

    public static class CAA
    {
        public const string Example = "caa/example";
    }
}