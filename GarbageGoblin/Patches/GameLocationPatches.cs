using System.Diagnostics;
using StardewModdingAPI;
using StardewValley;

namespace GarbageGoblin.Patches;

internal sealed class GameLocationPatches
{
    internal static bool CheckGarbage_Prefix(string __0, ref bool __result)
    {
        try
        {
            if (!GarbageCanGatekeeper.CanCheck(__0))
            {
                __result = false;
                return false;
            }

            Game1.netWorldState.Value.CheckedGarbage.Clear();
            return true;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in {nameof(CheckGarbage_Prefix)}:\n{ex}", LogLevel.Error);
            return true;
        }
    }

    internal static bool CreateDaySaveRandom_Prefix(ref Random __result)
    {
        try
        {
            if (new StackTrace().GetFrame(2)?.GetMethod()?.Name.StartsWith("StardewValley.GameLocation.TryGetGarbageItem") ?? false)
            {
                __result = Utility.CreateRandom(
                    Game1.hash.GetDeterministicHashCode(Game1.stats.Get("trashCansChecked").ToString()),
                    Game1.hash.GetDeterministicHashCode(Game1.stats.StepsTaken.ToString()),
                    Game1.hash.GetDeterministicHashCode(Game1.ticks.ToString())
                );
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in {nameof(CreateDaySaveRandom_Prefix)}:\n{ex}", LogLevel.Error);
            return true;
        }
    }

    internal static void TryGetGarbageItem_Postfix(string __0, ref bool __result)
    {
        try
        {
            GarbageCanGatekeeper.LogCheck(__0);
            if (__result)
            {
                GarbageCanGatekeeper.LogFind(__0);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in {nameof(TryGetGarbageItem_Postfix)}:\n{ex}", LogLevel.Error);
        }
    }
}
