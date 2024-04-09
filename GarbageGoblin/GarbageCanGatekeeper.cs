using StardewModdingAPI;
using StardewValley;

namespace GarbageGoblin;

internal sealed class AccessLogInfo
{
    internal int? LastCheck { get; set; }
    internal int? LastFind { get; set; }
}

internal static class GarbageCanGatekeeper
{
    private static readonly Dictionary<string, AccessLogInfo> AccessLogs = new();

    internal static bool CanCheck(string garbageCanId)
    {
        // Imported from StardewValley/GameLocation.cs; updated 03/27/24
        switch (garbageCanId)
        {
            case "0":
                garbageCanId = "JodiAndKent";
                break;
            case "1":
                garbageCanId = "EmilyAndHaley";
                break;
            case "2":
                garbageCanId = "Mayor";
                break;
            case "3":
                garbageCanId = "Museum";
                break;
            case "4":
                garbageCanId = "Blacksmith";
                break;
            case "5":
                garbageCanId = "Saloon";
                break;
            case "6":
                garbageCanId = "Evelyn";
                break;
            case "7":
                garbageCanId = "JojaMart";
                break;
        }

        if (AccessLogs.ContainsKey(garbageCanId))
        {
            int? lastCheck = AccessLogs[garbageCanId].LastCheck;
            int? lastFind = AccessLogs[garbageCanId].LastFind;

            int minutesSinceLastCheck = (int)(lastCheck != null ? GetTimeOfDayInMinutes() - lastCheck : int.MaxValue);
            int minutesSinceLastFind = (int)(lastFind != null ? GetTimeOfDayInMinutes() - lastFind : int.MaxValue);

            ModEntry.ModMonitor.Log(
                $"{nameof(CanCheck)}: {garbageCanId}\nlastCheck={lastCheck} minutesSinceLastCheck={minutesSinceLastCheck}/60\nlastFind={lastFind} minutesSinceLastFind={minutesSinceLastFind}/240",
                LogLevel.Trace
            );
            if (minutesSinceLastCheck >= ModEntry.Config.MinutesSinceLastCheckDelay && minutesSinceLastFind >= ModEntry.Config.MinutesSinceLastFindDelay)
            {
                return true;
            }
        }
        else
        {
            return true;
        }

        return false;
    }

    internal static void LogCheck(string garbageCanId)
    {
        if (!AccessLogs.ContainsKey(garbageCanId))
        {
            AccessLogs[garbageCanId] = new AccessLogInfo();
        }
        AccessLogs[garbageCanId].LastCheck = GetTimeOfDayInMinutes();
    }

    internal static void LogFind(string garbageCanId)
    {
        if (!AccessLogs.ContainsKey(garbageCanId))
        {
            AccessLogs[garbageCanId] = new AccessLogInfo();
        }
        AccessLogs[garbageCanId].LastFind = GetTimeOfDayInMinutes();
    }

    internal static void Reset()
    {
        AccessLogs.Clear();
    }

    private static int GetTimeOfDayInMinutes()
    {
        string timeOfDay = Game1.timeOfDay.ToString().PadLeft(4, '0');
        int timeOfDayHours = int.Parse(timeOfDay.Substring(0, 2));
        int timeOfDayMinutes = int.Parse(timeOfDay.Substring(2, 2));
        int timeOfDayInMinutes = timeOfDayHours * 60 + timeOfDayMinutes;
        return timeOfDayInMinutes;
    }
}
