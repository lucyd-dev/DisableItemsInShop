using HarmonyLib;
using static DisableItemsInShop.Plugin;

namespace DisableItemsInShop.Patches;

[HarmonyPatch(typeof(ItemGun))]
static class ItemGunPatch
{
    private static bool IsGunDisabled
    {
        get
        {
            return IsInDisabledLevel && BoundConfig.Guns.Value;
        }
    }

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(ItemGun __instance)
    {
        if (!IsGunDisabled) return;
        Logger.LogDebug($"Disabled gun misfire: {__instance.name}");
    }

    [HarmonyPatch("Misfire")]
    [HarmonyPrefix]
    private static bool Misfire_Prefix(ItemGun __instance)
    {
        if (!IsGunDisabled) return true;

        return false;
    }
}
