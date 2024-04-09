using GarbageGoblin.Integrations;
using GarbageGoblin.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GarbageGoblin;

internal sealed class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; } = null!;
    internal static IMonitor ModMonitor { get; private set; } = null!;

    public override void Entry(IModHelper helper)
    {
        Config = Helper.ReadConfig<ModConfig>();
        ModMonitor = Monitor;

        RegisterHarmonyPatches();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
    }

    private void RegisterHarmonyPatches()
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.CheckGarbage)),
            prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.CheckGarbage_Prefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.CreateDaySaveRandom)),
            prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.CreateDaySaveRandom_Prefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.TryGetGarbageItem)),
            postfix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.TryGetGarbageItem_Postfix))
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        configMenu.Register(mod: ModManifest, reset: () => Config = new ModConfig(), save: () => Helper.WriteConfig(Config));

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "MinutesSinceLastCheckDelay",
            tooltip: () => "MinutesSinceLastCheckDelay tooltip",
            min: 0,
            max: 1200,
            interval: 10,
            getValue: () => Config.MinutesSinceLastCheckDelay,
            setValue: value => Config.MinutesSinceLastCheckDelay = value
        );
        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "MinutesSinceLastFindDelay",
            tooltip: () => "MinutesSinceLastFindDelay tooltip",
            min: 0,
            max: 1200,
            interval: 10,
            getValue: () => Config.MinutesSinceLastFindDelay,
            setValue: value => Config.MinutesSinceLastFindDelay = value
        );
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        GarbageCanGatekeeper.Reset();
    }
}
