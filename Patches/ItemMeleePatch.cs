using HarmonyLib;
using static DisableItemsInShop.Plugin;

namespace DisableItemsInShop.Patches;

[HarmonyPatch(typeof(ItemMelee))]
static class ItemMeleePatch
{
    private static bool IsMeleeDisabled => ActiveConfig != null && ActiveConfig.Melees.Value;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemMelee __instance)
    {
        if (!IsMeleeDisabled) return;
        Logger.LogDebug($"Disabled item usage: {__instance.name}");
    }

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    private static bool Update_Prefix(ItemMelee __instance)
    {
        if (!IsMeleeDisabled) return true;

        return false;
    }
}
