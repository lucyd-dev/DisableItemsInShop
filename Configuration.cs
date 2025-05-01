using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace DisableItemsInShop;

class DisableItemsInShopConfig
{
    // We define our config variables in a public scope

    public readonly ConfigEntry<string> Level;
    public readonly ConfigEntry<bool> Explosives;
    public readonly ConfigEntry<bool> Guns;
    public readonly ConfigEntry<bool> Melees;
    public readonly ConfigEntry<bool> Drones;
    public readonly ConfigEntry<bool> Orbs;
    public readonly ConfigEntry<bool> RubberDuck;
    private readonly ConfigEntry<string> CustomItems;
    private List<string> _customItemsList;
    public IReadOnlyList<string> CustomItemsList => _customItemsList;

    public DisableItemsInShopConfig(ConfigFile cfg)
    {
        cfg.SaveOnConfigSet = false;

        Level = cfg.Bind(
            "General",
            "DisableLevel",
            "Shop",
            new ConfigDescription(
                "Where should items be disabled",
                new AcceptableValueList<string>("Shop", "Lobby", "Both")
            )
        );
        Explosives = cfg.Bind("General", "DisableExplosives", true);
        Guns = cfg.Bind("General", "DisableGuns", false);
        Melees = cfg.Bind("General", "DisableMelees", false);
        Drones = cfg.Bind("General", "DisableDrones", false);
        Orbs = cfg.Bind("General", "DisableOrbs", false);
        RubberDuck = cfg.Bind("General", "DisableRubberDuck", false);
        CustomItems = cfg.Bind(
            "Custom",
            "DisableCustomItems",
            "",
            new ConfigDescription("Comma-separated list of item name patterns to disable. Only works for toggleable items like guns, mines, etc. (ex. Rifle, Drone, ...)")
        );
        CacheCustomItemsList();

        ClearOrphanedEntries(cfg);
        cfg.Save();
        cfg.SaveOnConfigSet = true;
    }

    static void ClearOrphanedEntries(ConfigFile cfg)
    {
        PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
        orphanedEntries.Clear();
    }

    public void CacheCustomItemsList()
    {
        _customItemsList = CustomItems.Value
            .Split(',')
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrEmpty(item))
            .ToList();
    }
}
