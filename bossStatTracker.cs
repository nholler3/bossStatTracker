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
	}

	public class bossStatPlayer : ModPlayer{
		//individual player based configs

		private int bossDamageDealtThisSecond = 0; // Damage dealt in the current second
        private int bossDps = 0; // DPS value
		private bool isBossActive = false;
        private int bossFightTimer = 0;

        public override void PreUpdate(){
			
            bool bossFound = false;

            // Check if any boss is active
            foreach (var npc in Main.npc){
                if (npc.active && npc.boss){ // NPC is active and is a boss
                    bossFound = true;
                    break;
                }
            }
			if (bossFound){
				if(!isBossActive){
					//fight has started
					isBossActive=true;
					bossFightTimer=0; //reset timer
				}
				bossFightTimer++;
			}else{
				if (isBossActive){
					//fight has ended
					isBossActive=false;
				}
			}
		}

 		// Regular OnHitNPC (accounts for all weapon types)
		public override void OnHitNPC( NPC target, NPC.HitInfo hitInfo, int damage){
            // Only track damage if the target is a boss
            if (target.active && target.boss && damage > 0){
                bossDamageDealtThisSecond += damage;
            }
        }

	}
}