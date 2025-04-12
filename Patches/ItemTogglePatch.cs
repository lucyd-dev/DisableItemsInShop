using HarmonyLib;
using System.Linq;
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

        string Name = __instance.name;
        DisableItemsInShopConfig cfg = BoundConfig;

        if (ShouldDisableItem(Name, cfg))
        {
            __instance.ToggleDisable(true);
            Logger.LogDebug($"Disabled item usage: {Name}");
        }
    }

    private static bool ShouldDisableItem(string itemName, DisableItemsInShopConfig cfg)
    {
        bool isExplosive = cfg.Explosives.Value && (itemName.StartsWith("Item Grenade") || itemName.StartsWith("Item Mine"));
        bool isGun = cfg.Guns.Value && itemName.StartsWith("Item Gun");

        bool isCustomItem = cfg.CustomItems.Value.Split(',')
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => item.Trim().ToLower())
            .Any(item => itemName.ToLower().Contains(item));

        return isExplosive || isGun || isCustomItem;
    }
}
