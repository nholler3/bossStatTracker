using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace bossStatTracker
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class bossStatTracker : Mod
	{
		//mod wide configurations
		public override void Load(){
            // Check for Boss Checklist dependency
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null){
                // You can perform any setup related to Boss Checklist here if needed
            }
        }
	}

	public class bossStatPlayer : ModPlayer{
		//individual player based configs

		private int bossDmgDealtThisFrame = 0; // Damage dealt in the current second
        private int bossDps = 0; // DPS value
		private bool isBossActive = false;
        private int bossFightTimer = 0;
		private int frameCounter=0;
		private int maxDmgPerSec =0;
		private int totalDamage=0;
		
		private Terraria.NPC bossNPC= null;



        public override void PostUpdate(){
			
            bool bossFound = false;
			frameCounter++; //tick the frame counter up, this runs at the end of every frame

			//reset DPS calculations every second (60fps)
			if (frameCounter>=60){
				bossDps=bossDmgDealtThisFrame; //store the value of the 60 frames worth of damage and reset 
				bossDmgDealtThisFrame=0;
				frameCounter=0;

				//send the dps over to maxDPS for calculations
				maxDPS(bossDps);
				totalDMG(bossDps);
			}

            // Check if any boss is active
            foreach (var npc in Main.npc){
                if (npc.active && npc.boss){ // NPC is active (Alive) and is a boss
                    bossFound = true;
					bossNPC=npc;
                    break;
                }
			}

			if (bossFound){ //if the boss is alive
				if(!isBossActive){ //tells it this is the beginning since we reset the variable below
					//fight has started
					isBossActive=true;
					resetVariables();
				}
				bossFightTimer++;
			}else{
				if (isBossActive){ //if the boss is no longer alive
					//fight has ended
					isBossActive=false;
					UpdateBossChecklist(bossNPC);
				}
			}
		}


 		// Regular OnHitNPC (accounts for all weapon types)
		public override void OnHitNPC( NPC target, NPC.HitInfo hitInfo, int damage){
            // Only track damage if the target is a boss
            if (target.active && target.boss && damage > 0){
                bossDmgDealtThisFrame += damage;
            }
        }


		//keeps track of the highest dps during the boss fight
		public void maxDPS(int bossDps){
			int tempDps= bossDps;
			if(tempDps > maxDmgPerSec){maxDmgPerSec = tempDps;}
		}


		//adding up each dps for the entirety of the battle
		public void totalDMG(int bossDps){
			totalDamage+=bossDps;
		}


		public void UpdateBossChecklist(NPC boss){
            // Check if the Boss Checklist mod is loaded
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null){
                // Assuming you have a method to get the boss name or ID
                string bossName = boss.FullName; // Replace with the actual boss name

                // Update the Boss Checklist with the total damage and max DPS
                bossChecklist.Call("AddCustomText", bossName, $"Total Damage Done: {totalDamage}");
                bossChecklist.Call("AddCustomText", bossName, $"Max DPS: {maxDmgPerSec}");

                // Optionally, reset the total damage and max DPS for the next fight
                totalDamage = 0;
                maxDmgPerSec = 0;
            }
        }


		public void resetVariables(){
			//set all global variables to 0
			bossFightTimer=0;
			frameCounter=0;
			bossDps=0;
			bossDmgDealtThisFrame=0;
			maxDmgPerSec=0;
			totalDamage=0;
			bossNPC=null;
		}



	}
}