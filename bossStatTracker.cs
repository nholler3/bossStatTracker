//bossStatTracker.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Steamworks;
using bossStatTracker.UI;

namespace bossStatTracker
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class bossStatTracker : Mod
	{
		//mod wide configurations
		public static ModKeybind ToggleTrackerUI;
		private static bool bossChecklistDataLoaded = false; // Moved to the class level

		public override void Load()//perform intilization tasks, when the mod is loaded into the game
		{
			ToggleTrackerUI = KeybindLoader.RegisterKeybind(this, "Toggle Tracker UI", Microsoft.Xna.Framework.Input.Keys.P); // Change "P" to your desired key
		}

		public override void Unload()//when the mod is closed, clear it's data and resources
		{
			ToggleTrackerUI = null;
		}

	}

	public class bossStatPlayer : ModPlayer
	{
		//individual player based configs


		private int bossDmgDealtThisFrame = 0; // Damage dealt in the current second
		private int bossDps = 0; // DPS value
		private bool isBossActive = false;
		private int bossFightTimer = 0;
		private int frameCounter = 0;
		private int maxDmgPerSec = 0;
		private int totalDamage = 0;
		public string CurrentBossKey { get; private set; } = "";
		public string CurrentBossDisplayName { get; private set; } = "";
		private static bool bossChecklistDataLoaded = false;
		private int bossMaxHealth = 0;
		private float damagePercentage = 0;



		public override void PostUpdate()
		{
			bool bossFound = false;
			frameCounter++; //tick the frame counter up, this runs at the end of every frame

			//reset DPS calculations every second (60fps)
			if (frameCounter >= 60)
			{
				bossDps = bossDmgDealtThisFrame; //store the value of the 60 frames worth of damage and reset 
				bossDmgDealtThisFrame = 0;
				frameCounter = 0;

				//send the dps over to maxDPS for calculations
				maxDPS(bossDps);
				totalDPS(bossDps);
			}

			// Check if any boss is active
			foreach (var npc in Main.npc)
			{
				if (npc.active && npc.boss)
				{ // NPC is active (Alive) and is a boss
					bossFound = true;
					//Main.NewText("npc type:" + npc.type);

					//for the bossname, checking from the bossChecklist api
					string bossKey = BossChecklistData.GetBossKeyFromNpc(npc.type);
					if (bossKey != null)
					{
						CurrentBossKey = bossKey;
						CurrentBossDisplayName = BossChecklistData.GetDisplayNameFromKey(bossKey) ?? npc.TypeName;
						//Main.NewText("Detected Boss: " + CurrentBossDisplayName); // Debug ddd
					}
					//get the bosses maxhealth
					if (bossFightTimer <= 2) bossMaxHealth = npc.lifeMax;
					break;
				}
			}

			if (bossFound)
			{
				if (!isBossActive)
				{
					isBossActive = true;
					bossFightTimer = 0; // Start timer fresh
				}

				bossFightTimer++; // Add time every frame boss is active
			}
			else if (isBossActive)
			{
				isBossActive = false;
				// Do NOT reset bossFightTimer — keep it for display
			}

		}

		// Regular OnHitNPC (accounts for all weapon types)
		public override void OnHitNPC(NPC target, NPC.HitInfo hitInfo, int damage)
		{
			// Only track damage if the target is a boss
			if (target.active && target.boss && damage > 0)
			{
				bossDmgDealtThisFrame += damage;
			}
		}

		//keeps track of the highest dps during the boss fight
		public void maxDPS(int bossDps)
		{
			int tempDps = bossDps;
			if (tempDps > maxDmgPerSec) { maxDmgPerSec = tempDps; }
		}

		public int MaxDps => maxDmgPerSec;

		//adding up each dps for the entirety of the battle
		public void totalDPS(int bossDps)
		{
			totalDamage += bossDps;
			//get the percentage
			if (bossMaxHealth > 0)
			{
				damagePercentage = (float)totalDamage / bossMaxHealth * 100f;
			}
		}

		// Property to get the total damage
		public int TotalDamage => totalDamage;

		public int MaxBossHealth => bossMaxHealth;
		public float DamagePercentage => damagePercentage;
		

		public string GetFormattedTime()
		{
			int seconds = bossFightTimer / 60;
			int minutes = seconds / 60;
			seconds %= 60;
			return $"{minutes}:{seconds:D2}";
		}

	}
}