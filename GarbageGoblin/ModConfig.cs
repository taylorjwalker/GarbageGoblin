namespace GarbageGoblin;

public sealed class ModConfig
{
    public int MinutesSinceLastCheckDelay { get; set; } = 60;
    public int MinutesSinceLastFindDelay { get; set; } = 240;
}
