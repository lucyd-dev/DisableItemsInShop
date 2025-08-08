#nullable enable
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using DisableItemsInShop.Patches;

namespace DisableItemsInShop;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
    internal static DisableItemsInShopConfig BoundConfig { get; private set; } = null!;
    internal static DisableItemsInShopConfig.Section? ActiveConfig
        => SemiFunc.RunIsShop() ? BoundConfig.Shop : SemiFunc.RunIsLobby() ? BoundConfig.Lobby : null;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        BoundConfig = new DisableItemsInShopConfig(Config);

        harmony.PatchAll(typeof(ItemTogglePatch));
        harmony.PatchAll(typeof(ItemGunPatch));
        harmony.PatchAll(typeof(ItemMeleePatch));
        harmony.PatchAll(typeof(ItemRubberDuckPatch));

        Logger.LogInfo($"🚫 {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} by {MyPluginInfo.PLUGIN_AUTHOR} initialized!");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }
}
