using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace DisableItemsInShop;

class DisableItemsInShopConfig
{
    public class Section
    {
        public readonly ConfigEntry<bool> Explosives;
        public readonly ConfigEntry<bool> CartWeapons;
        public readonly ConfigEntry<bool> Guns;
        public readonly ConfigEntry<bool> Melees;
        public readonly ConfigEntry<bool> Drones;
        public readonly ConfigEntry<bool> Orbs;
        public readonly ConfigEntry<bool> RubberDuck;
        private readonly ConfigEntry<string> CustomItems;
        private List<string> _customItemsList = new List<string>();
        public IReadOnlyList<string> CustomItemsList => _customItemsList;

        internal Section(ConfigFile cfg, string sectionName)
        {
            Explosives = cfg.Bind(sectionName, "DisableExplosives", true);
            CartWeapons = cfg.Bind(sectionName, "DisableCartWeapons", true);
            Guns = cfg.Bind(sectionName, "DisableGuns", false);
            Melees = cfg.Bind(sectionName, "DisableMelees", false);
            Drones = cfg.Bind(sectionName, "DisableDrones", false);
            Orbs = cfg.Bind(sectionName, "DisableOrbs", false);
            RubberDuck = cfg.Bind(sectionName, "DisableRubberDuck", false);
            CustomItems = cfg.Bind(
                sectionName,
                "DisableCustomItems",
                "",
                new ConfigDescription("Comma-separated list of item name patterns to disable. Only works for toggleable items like guns, mines, etc. (ex. Rifle, Drone, ...)")
            );

            CacheCustomItemsList();
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

    public readonly Section Shop;
    public readonly Section Lobby;

    public DisableItemsInShopConfig(ConfigFile cfg)
    {
        cfg.SaveOnConfigSet = false;

        Shop = new Section(cfg, "Shop");
        Lobby = new Section(cfg, "Lobby");

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
