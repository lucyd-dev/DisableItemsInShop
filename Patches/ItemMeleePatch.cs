using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemMelee))]
static class ItemMeleePatch
{
    private static bool IsMeleeDisabled
    {
        get
        {
            return IsInDisabledLevel && BoundConfig.Melee.Value;
        }
    }

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
