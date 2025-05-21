// bossStatTrackerPanel.cs
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
        private PanelHeader panelHeader;
        private StatisticPanel statPanel;

        public BossStatTrackerPanel()
        {
            // Set up the panel itself
            SetPadding(0);
            Width.Set(550, 0); // Set the width of the panel
            Height.Set(450, 0); // Set the height of the panel
            BackgroundColor = new Color(252, 249, 238); // Set the background color of the panel : Elderflower
            HAlign = VAlign = 0.5f; // 1, set the panel to the middle of the screen

            // Add UI elements to the panel below---------------------------------------------

            // Create and append the header section
            panelHeader = new PanelHeader();
            panelHeader.Top.Set(0f, 0f);
            Append(panelHeader);

            //create and append the statistics panel
            statPanel =new StatisticPanel();
            statPanel.Top.Set(40f, 0f);
            Append(statPanel);

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



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var player = Main.LocalPlayer.GetModPlayer<bossStatPlayer>();

            panelHeader.UpdateBossName(player.CurrentBossDisplayName);
        }

        //make the statpanel visible to the UIState so it can be updated
        public void UpdateStats(bossStatPlayer player)
        {
            statPanel.UpdateText(player);
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
