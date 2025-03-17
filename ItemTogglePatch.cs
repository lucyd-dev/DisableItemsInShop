using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemToggle), "Start")]
static class ItemTogglePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemToggle __instance)
    {
        if (!SemiFunc.RunIsShop() && (!DisableItemsInShop.DisableInLobbyConfig.Value || !SemiFunc.RunIsLobby()))
            return;

        bool isExplosive = __instance.name.StartsWith("Item Grenade") || __instance.name.StartsWith("Item Mine");

        if (!DisableItemsInShop.OnlyExplosivesConfig.Value || isExplosive)
        {
            __instance.ToggleDisable(true);
            DisableItemsInShop.Logger.LogDebug($"Disabled item usage: {__instance.name}");
        }
    }
}
