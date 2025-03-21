using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DisableItemsInShop;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class DisableItemsInShop : BaseUnityPlugin
{
    internal static DisableItemsInShop Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
    internal static DisableItemsInShopConfig BoundConfig { get; private set; } = null!;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        BoundConfig = new DisableItemsInShopConfig(Config);

        harmony.PatchAll(typeof(ItemTogglePatch));
        harmony.PatchAll(typeof(ItemMeleePatch));
        harmony.PatchAll(typeof(ItemRubberDuckPatch));

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} by {MyPluginInfo.PLUGIN_AUTHOR} has loaded!");
    }

    public static bool ShouldDisable()
    {
        bool inShop = SemiFunc.RunIsShop();
        bool inLobby = SemiFunc.RunIsLobby();

        return BoundConfig.Level.Value switch
        {
            "Shop" => inShop,
            "Lobby" => inLobby,
            "Both" => inShop || inLobby,
            _ => false
        };
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }
}
