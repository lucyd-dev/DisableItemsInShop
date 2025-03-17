using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemToggle), "Start")]
static class ItemTogglePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemToggle __instance)
    {
        bool inShop = SemiFunc.RunIsShop();
        bool inLobby = SemiFunc.RunIsLobby();

        bool shouldDisable = DisableItemsInShop.DisableLevelConfig.Value switch
        {
            "Shop" => inShop,
            "Lobby" => inLobby,
            "Both" => inShop || inLobby,
            _ => false
        };
        if (!shouldDisable) return;

        bool isExplosive = __instance.name.StartsWith("Item Grenade") || __instance.name.StartsWith("Item Mine");

        if (!DisableItemsInShop.OnlyExplosivesConfig.Value || isExplosive)
        {
            __instance.ToggleDisable(true);
            DisableItemsInShop.Logger.LogDebug($"Disabled item usage: {__instance.name}");
        }
    }
}
