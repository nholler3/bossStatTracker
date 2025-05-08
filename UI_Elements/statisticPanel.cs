using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

namespace bossStatTracker.UI{

    public class StatisticPanel : UIPanel{

        private UIText totalDmgText;
        private UIText maxDPSText;

        public StatisticPanel(){
            Width.Set(0f, 1f); //width of parent, 1f denotes from the parent
            Height.Set(0f, 0.33f); // 30% of the height of the parent

            //shows the panel
            BackgroundColor = new Color(255, 0, 0, 80); // light red background

            //total damage done text and call to the method in the backend
            totalDmgText = new UIText($"Total Damage Done: ");
            totalDmgText.HAlign = 0.03f;
            totalDmgText.Top.Set(5f, 0);
            //totalDmgText.Left.Set(12f, 0);
            Append(totalDmgText);

            // max dps
            maxDPSText = new UIText("Highest DPS:  ");
            maxDPSText.HAlign = 0.03f;
            maxDPSText.Top.Set(35f, 0);
            //maxDPSText.Left.Set(12f, 0);
            Append(maxDPSText);
        }

        // Dynamically update the values based on the player stats
        public void UpdateText(bossStatPlayer player)
        {
            // Update the total DPS text
            totalDmgText?.SetText($"Total DMG: {player.TotalDamage}/{player.MaxBossHealth} ({player.DamagePercentage:F1}%)");
            maxDPSText?.SetText($"Maximum DPS: {player.MaxDps}");
        }

    }

}