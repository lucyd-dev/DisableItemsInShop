using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

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

        if (ShouldDisableItem(Name, cfg))
        {
            __instance.ToggleDisable(true);
            DisableItemsInShop.Logger.LogDebug($"Disabled item usage: {Name}");
        }
    }

    private static bool ShouldDisableItem(string itemName, DisableItemsInShopConfig cfg)
    {
        bool isExplosive = cfg.Explosives.Value && (itemName.StartsWith("Item Grenade") || itemName.StartsWith("Item Mine"));
        bool isGun = cfg.Guns.Value && itemName.StartsWith("Item Gun");
        DisableItemsInShop.Logger.LogDebug($"custom items: {cfg.CustomItems.Value}");

        bool isCustomItem = cfg.CustomItems.Value.Split(',')
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => item.Trim().ToLower())
            .Any(item => itemName.ToLower().Contains(item));

        return isExplosive || isGun || isCustomItem;
    }
}
