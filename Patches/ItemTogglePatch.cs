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
        if (ShouldDisableItem(__instance.name))
        {
            __instance.ToggleDisable(true);
            Logger.LogDebug($"Disabled item usage: {__instance.name}");
        }
    }

    private static bool ShouldDisableItem(string itemName)
    {
        if (ActiveConfig == null) return false;

        bool explosives = ActiveConfig.Explosives.Value;
        bool cartWeapons = ActiveConfig.CartWeapons.Value;
        bool guns = ActiveConfig.Guns.Value;
        bool drones = ActiveConfig.Drones.Value;
        bool orbs = ActiveConfig.Orbs.Value;

        bool isExplosive = explosives && (itemName.StartsWith("Item Grenade") || itemName.StartsWith("Item Mine"));
        bool isCartWeapon = cartWeapons && (itemName.StartsWith("Item Cart Cannon") || itemName.StartsWith("Item Cart Laser"));
        bool isGun = guns && itemName.StartsWith("Item Gun");
        bool isDrone = drones && itemName.StartsWith("Item Drone");
        bool isOrb = orbs && itemName.StartsWith("Item Orb");

        bool isCustomItem = ActiveConfig.CustomItemsList.Any(item => itemName.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0);

        return isExplosive || isCartWeapon || isGun || isDrone || isOrb || isCustomItem;
    }
}
