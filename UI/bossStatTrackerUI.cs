using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using bossStatTracker;

namespace bossStatTracker
{
    class bossStatTrackerUISystem : ModSystem
    {
        internal UserInterface TrackerInterface;
        internal bossStatTrackerUI TrackerUI; //TrackerUI is type bossStatTrackerUI
        private GameTime _lastUpdateUiGameTime;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                TrackerInterface = new UserInterface();

                TrackerUI = new bossStatTrackerUI();
                TrackerUI.Activate(); // Activate calls Initialize() on the UIState if not initialized and calls OnActivate, then calls Activate on every child element.
            }
        }

        public override void Unload()
        {
            //TrackerUI?.SomeKindOfUnload(); // If you hold data that needs to be unloaded, call it in OO-fashion
            //TrackerUI = null;
        }

        public override void UpdateUI(GameTime gameTime) //call .Update on your interface and propagate it to its state and underlying elements.
        {
            _lastUpdateUiGameTime = gameTime;
            if (TrackerInterface?.CurrentState != null)
            {
                TrackerInterface.Update(gameTime);
            }

            // Check if the hotkey is pressed
            if (bossStatTracker.ToggleTrackerUI.JustPressed)
            {
                if (TrackerInterface.CurrentState == null)
                {
                    ShowTrackerUI(); //if hotkey is pressed, show the ui
                }else{
                    HideTrackerUI();//if hotkey is pressed hide the ui
                }
            }
        }

        internal void ShowTrackerUI()//helper method for displaying the the ui
        {
            TrackerInterface?.SetState(TrackerUI);
        }

        internal void HideTrackerUI()//helper method for hiding the ui
        {
            TrackerInterface?.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) //adds a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state. makes UI actually draw and show up on screen. Set the InterfaceScaleType to UI for appropriate UI scaling.
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "bossStatTracker: TrackerInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && TrackerInterface?.CurrentState != null)
                        {
                            TrackerInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }

    class bossStatTrackerUI : UIState
    {
        private UIText totalDmgText;
        private UIText maxDPSText;
        public override void OnInitialize()//override when our mod loads
        {

            // Create a new panel
            UIPanel panel = new UIPanel();
            panel.SetPadding(0);
            panel.Width.Set(550, 0); // Set the width of the panel
            panel.Height.Set(450, 0); // Set the height of the panel
            panel.BackgroundColor = new Color(252,249,238); // Set the background color of the panel : Elderflower
            panel.HAlign = panel.VAlign = 0.5f; // 1, set the panel to the middle of the screen
            Append(panel); // Append the panel to the UIState

            // Add other UI elements to the panel here
            UIText text = new UIText("Boss Stats will be here soon :)");

            panel.Append(text);


            UIText bossNameHeader = new UIText("Boss Name");//we need to get a variable with the boss name 
            bossNameHeader.HAlign = 0.5f; //horizontal alignment set to 50%
            bossNameHeader.Top.Set(15, 0); //Top position is 15 pixels from the top of the UI Panel
            panel.Append(bossNameHeader);

            totalDmgText = new UIText($"Total Damage Done: ");
            totalDmgText.HAlign = 0.1f;
            totalDmgText.Top.Set(45,0);
            totalDmgText.Left.Set(12, 0);
            panel.Append(totalDmgText);

            maxDPSText = new UIText("Highest DPS:  ");
            maxDPSText.HAlign = 0.1f;
            maxDPSText.Top.Set(75,0);
            maxDPSText.Left.Set(12, 0);
            panel.Append(maxDPSText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the player instance
            bossStatPlayer player = Main.LocalPlayer.GetModPlayer<bossStatPlayer>();

            // Update the total DPS text
            totalDmgText.SetText($"Total DPS: {player.TotalDamage}");
            maxDPSText.SetText($"Maximum DPS: {player.MaxDps}");
        }


    }

}