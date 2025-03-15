using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemToggle), "Start")]
static class ItemTogglePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemToggle __instance)
    {
        if (!SemiFunc.RunIsShop()) return;

        if (DisableItemsInShop.OnlyExplosivesConfig.Value)
        {
            if (__instance.name.Contains("Grenade") || __instance.name.Contains("Mine"))
            {
                __instance.ToggleDisable(true);
                DisableItemsInShop.Logger.LogDebug($"Disable item usage of {__instance.name}");
            }
            return;
        }

        __instance.ToggleDisable(true);
        DisableItemsInShop.Logger.LogDebug($"Disable item usage of {__instance.name}");
    }
}
