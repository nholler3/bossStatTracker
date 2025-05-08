using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace bossStatTracker.UI
{
    // This is the main UIState that gets activated/deactivated by the ModSystem
    public class bossStatTrackerUI : UIState
    {
        private BossStatTrackerPanel panel;

        public override void OnInitialize()
        {
            // Create and add our custom panel (moved to its own file)
            panel = new BossStatTrackerPanel();
            Append(panel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the ModPlayer instance to fetch boss stats
            bossStatPlayer player = Main.LocalPlayer.GetModPlayer<bossStatPlayer>();

            // Pass player data to the panel to update the text fields
            panel.UpdateStats(player);
        }
    }
}