using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemRubberDuck))]
static class ItemRubberDuckPatch
{
    private static bool IsRubberDuckDisabled
    {
        get
        {
            return IsInDisabledLevel && BoundConfig.RubberDuck.Value;
        }
    }

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemRubberDuck __instance)
    {
        if (!RubberDuckDisabled) return;
        DisableItemsInShop.Logger.LogDebug($"Disabled item usage: {__instance.name}");
    }

    [HarmonyPatch("Quack")]
    [HarmonyPrefix]
    private static bool Quack_Prefix(ItemRubberDuck __instance)
    {
        if (!RubberDuckDisabled) return true;

        return false;
    }
}
