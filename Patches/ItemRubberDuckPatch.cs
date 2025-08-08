using HarmonyLib;
using static DisableItemsInShop.Plugin;

namespace DisableItemsInShop.Patches;

[HarmonyPatch(typeof(ItemRubberDuck))]
static class ItemRubberDuckPatch
{
    private static bool IsRubberDuckDisabled => ActiveConfig != null && ActiveConfig.RubberDuck.Value;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemRubberDuck __instance)
    {
        if (!IsRubberDuckDisabled) return;
        Logger.LogDebug($"Disabled item usage: {__instance.name}");
    }

    [HarmonyPatch("Quack")]
    [HarmonyPrefix]
    private static bool Quack_Prefix(ItemRubberDuck __instance)
    {
        if (!IsRubberDuckDisabled) return true;

        return false;
    }
}
