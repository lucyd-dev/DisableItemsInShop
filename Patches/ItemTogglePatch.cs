using System;
using System.Linq;
using HarmonyLib;
using static DisableItemsInShop.Plugin;

namespace DisableItemsInShop.Patches;

[HarmonyPatch(typeof(ItemToggle))]
static class ItemTogglePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemToggle __instance)
    {
        if (!IsInDisabledLevel) return;

        DisableItemsInShopConfig cfg = BoundConfig;

        if (ShouldDisableItem(__instance.name, cfg))
        {
            __instance.ToggleDisable(true);
            Logger.LogDebug($"Disabled item usage: {__instance.name}");
        }
    }

    private static bool ShouldDisableItem(string itemName, DisableItemsInShopConfig cfg)
    {
        bool isExplosive = cfg.Explosives.Value && (itemName.StartsWith("Item Grenade") || itemName.StartsWith("Item Mine"));
        bool isCartWeapon = cfg.CartWeapons.Value && (itemName.StartsWith("Item Cart Cannon") || itemName.StartsWith("Item Cart Laser"));
        bool isGun = cfg.Guns.Value && itemName.StartsWith("Item Gun");
        bool isDrone = cfg.Drones.Value && itemName.StartsWith("Item Drone");
        bool isOrb = cfg.Orbs.Value && itemName.StartsWith("Item Orb");
        bool isCustomItem = cfg.CustomItemsList
            .Any(item => itemName.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0);

        return isExplosive || isCartWeapon || isGun || isDrone || isOrb || isCustomItem;
    }
}
