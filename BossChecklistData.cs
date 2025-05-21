//BossChecklistData.cs
using Terraria.ModLoader;
using System.Collections.Generic;
using bossStatTracker.System;

namespace bossStatTracker {
    public static class BossChecklistData {

        // Reference to the public dictionary in your mod system class
        public static Dictionary<string, Dictionary<string, object>> _bossDict => bossStatTrackerUISystem.BossData;

        // Get the Boss Checklist key (e.g., "Terraria/EyeOfCthulhu") from the npc.type (4 is Eye of Cthulhu)
        public static string GetBossKeyFromNpc(int npcType) {
            // Check if the BossData is loaded
            if (_bossDict == null) {
                ModContent.GetInstance<bossStatTracker>().Logger.Info("BossChecklist dictionary not loaded.");
                return null;
            }

            foreach (var kvp in _bossDict) {
                if (kvp.Value.TryGetValue("npcIDs", out var npcIDsObj) &&
                    npcIDsObj is List<int> npcIDs &&
                    npcIDs.Contains(npcType)) {

                    // Log the matched key
                    ModContent.GetInstance<bossStatTracker>().Logger.Info($"Matched npcType {npcType} to key {kvp.Key}");
                    return kvp.Key;
                }
            }

            ModContent.GetInstance<bossStatTracker>().Logger.Info($"Could not find key for npcType {npcType}");
            return null;
        }

        // Get the display name associated with the key from the dictionary
        public static string GetDisplayNameFromKey(string key) {
            if (_bossDict != null &&
                _bossDict.TryGetValue(key, out var entry) &&
                entry.TryGetValue("displayName", out var displayNameObj)) {
                return displayNameObj as string;
            }
            return null;
        }
    }
}
