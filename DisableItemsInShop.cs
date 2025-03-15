using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace DisableItemsInShop;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class DisableItemsInShop : BaseUnityPlugin
{
    internal static DisableItemsInShop Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    public static ConfigEntry<bool> OnlyExplosivesConfig { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        OnlyExplosivesConfig = Config.Bind(
            "General",
            "OnlyExplosives",
            false,
            new ConfigDescription("Should only explosives be disabled in the shop?")
        );

        harmony.PatchAll(typeof(ItemTogglePatch));

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} by {MyPluginInfo.PLUGIN_AUTHOR} has loaded!");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }
}
