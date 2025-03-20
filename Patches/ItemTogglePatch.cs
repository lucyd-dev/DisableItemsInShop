using HarmonyLib;

namespace DisableItemsInShop;

[HarmonyPatch(typeof(ItemToggle))]
static class ItemTogglePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemToggle __instance)
    {
        if (!DisableItemsInShop.ShouldDisable()) return;

        string Name = __instance.name;
        DisableItemsInShopConfig cfg = DisableItemsInShop.BoundConfig;
        bool ExplosivesSet = cfg.Explosives.Value;
        bool GunsSet = cfg.Guns.Value;

        if (
            (ExplosivesSet && (Name.StartsWith("Item Grenade") || Name.StartsWith("Item Mine"))) ||
            (GunsSet && Name.StartsWith("Item Gun"))
        )
        {
            __instance.ToggleDisable(true);
            DisableItemsInShop.Logger.LogDebug($"Disabled item usage: {Name}");
        }
    }
}
