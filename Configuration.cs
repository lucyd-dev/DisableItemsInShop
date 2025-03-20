using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx.Configuration;

namespace DisableItemsInShop;

class DisableItemsInShopConfig
{
    // We define our config variables in a public scope

    public readonly ConfigEntry<string> Level;
    public readonly ConfigEntry<bool> Explosives;
    public readonly ConfigEntry<bool> Guns;
    public readonly ConfigEntry<bool> Melee;
    public readonly ConfigEntry<bool> RubberDuck;

    public DisableItemsInShopConfig(ConfigFile cfg)
    {
        cfg.SaveOnConfigSet = false;

        Level = cfg.Bind(
            "General",
            "DisableLevel",
            "Shop",
            new ConfigDescription("Where should items be disabled", new AcceptableValueList<string>("Shop", "Lobby", "Both"))
        );
        Explosives = cfg.Bind("General", "DisableExplosives", true);
        Guns = cfg.Bind("General", "DisableGuns", false);
        Melee = cfg.Bind("General", "DisableMelees", false);
        RubberDuck = cfg.Bind("General", "DisableRubberDuck", false);

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
}
