using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;       // for SoundEngine
using Terraria.ID;          // for SoundID
using bossStatTracker.System;


namespace bossStatTracker.UI
{
    public class BossStatTrackerPanel : UIPanel
    {
        private UIText totalDmgText;
        private UIText maxDPSText;
        private UIText bossNameHeader;
        private string lastBossName = "";


        public BossStatTrackerPanel()
        {
            // Set up the panel itself
            SetPadding(0);
            Width.Set(550, 0); // Set the width of the panel
            Height.Set(450, 0); // Set the height of the panel
            BackgroundColor = new Color(252, 249, 238); // Set the background color of the panel : Elderflower
            HAlign = VAlign = 0.5f; // 1, set the panel to the middle of the screen

            // Add UI elements to the panel below---------------------------------------------

            //boss name, title part of the ui
            bossNameHeader = new UIText("Boss: none"); // we need to get a variable with the boss name 
            bossNameHeader.HAlign = 0.5f; // horizontal alignment set to 50%
            bossNameHeader.Top.Set(15, 0); // Top position is 15 pixels from the top of the UI Panel
            Append(bossNameHeader);

            //total damage done text and call to the method in the backend
            totalDmgText = new UIText($"Total Damage Done: ");
            totalDmgText.HAlign = 0.1f;
            totalDmgText.Top.Set(45, 0);
            totalDmgText.Left.Set(12, 0);
            Append(totalDmgText);

            // max dps
            maxDPSText = new UIText("Highest DPS:  ");
            maxDPSText.HAlign = 0.1f;
            maxDPSText.Top.Set(75, 0);
            maxDPSText.Left.Set(12, 0);
            Append(maxDPSText);

            //"X" close button
            UITextPanel<string> closeButton = new UITextPanel<string>("X", 0.5f, true);
            closeButton.Width.Set(5f, 0f);
            closeButton.Height.Set(5f, 0f);
            closeButton.Left.Set(this.Width.Pixels - 42f, 0f); // Position from right edge
            closeButton.Top.Set(5f, 0f);
            closeButton.OnLeftMouseDown += OnCloseButtonClick;
            Append(closeButton);

            //forget this for now
            // // Right-side arrow button to toggle boss list
            // UITextPanel<string> bossListSidePanel = new UITextPanel<string>("▶", 0.8f, true);
            // bossListSidePanel.Width.Set(20f, 0f);
            // bossListSidePanel.Height.Set(20f, 0f);
            // bossListSidePanel.Left.Set(this.Width.Pixels - 25f, 0f);  // Near right edge
            // bossListSidePanel.Top.Set(35f, 0f);
            // bossListSidePanel.OnClick += ToggleBossList;
            // Append(bossListSidePanel);
        }

        // Dynamically update the values based on the player stats
        public void UpdateText(bossStatPlayer player)
        {
            // Update the total DPS text
            totalDmgText?.SetText($"Total DPS: {player.TotalDamage}");
            maxDPSText?.SetText($"Maximum DPS: {player.MaxDps}");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            string currentName = Main.LocalPlayer.GetModPlayer<bossStatPlayer>().CurrentBossDisplayName;

            if (currentName != lastBossName) {
                bossNameHeader.SetText(string.IsNullOrEmpty(currentName) ? "No boss active" : "Boss: " + currentName);
                lastBossName = currentName;
            }
        }

        // from the advanced ui git wiki page
        // this is to create a button
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // We can do stuff in here!
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            // this blocks keeps the player from using whatever is in their hand while clicking in the panel
            // this was taken from the advanced ui guide

            // If this code is in the panel or container element, check it directly
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        private void OnCloseButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Close the UI by calling the system-level toggle
            bossStatTrackerUISystem.ToggleUI(false);
            SoundEngine.PlaySound(SoundID.MenuClose); // Play a UI close sound
        }
    }
}
