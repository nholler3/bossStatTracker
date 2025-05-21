//panelHeader.cs
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace bossStatTracker.UI
{
    public class PanelHeader : UIPanel
    {
        private UIText bossNameHeader;
        private string lastBossName = "";

        public PanelHeader()
        {
            Width.Set(0f, 1f);
            Height.Set(40f, 0f);

            //adds the bg color
            BackgroundColor = new Color(0, 100, 255, 80); // translucent blue

            bossNameHeader = new UIText("Boss: none");
            bossNameHeader.HAlign = 0.5f;
            bossNameHeader.Top.Set(3f, 0f);

            Append(bossNameHeader);
        }

        public bool UpdateBossName(string newName)
        {
            if (newName != lastBossName)
            {
                bossNameHeader.SetText(string.IsNullOrEmpty(newName) ? "No boss active" : newName);
                lastBossName = newName;
                return true;
            }
            return false;
        }

    }
}