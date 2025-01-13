using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace bossStatTracker
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class bossStatTracker : Mod
	{
		//mod wide configurations
	}

	public class BossStatPlayer : ModPlayer{
		//individual player based configs
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

		

	}
}