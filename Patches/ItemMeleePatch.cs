using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemMelee))]
static class ItemMeleePatch
{
    private static bool MeleeDisabled = DisableItemsInShop.ShouldDisable() && DisableItemsInShop.BoundConfig.Melee.Value;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemMelee __instance)
    {
        if (!MeleeDisabled) return;
        DisableItemsInShop.Logger.LogDebug($"Disabled item usage: {__instance.name}");
    }

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    private static bool Update_Prefix(ItemMelee __instance)
    {
        if (!MeleeDisabled) return true;

        return false;
    }
}
