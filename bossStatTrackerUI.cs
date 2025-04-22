using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using bossStatTracker;
using bossStatTracker.UI; // Import the panel

namespace bossStatTracker
{
    class bossStatTrackerUISystem : ModSystem
    {
        internal UserInterface TrackerInterface;
        internal bossStatTrackerUI TrackerUI; //TrackerUI is type bossStatTrackerUI
        private GameTime _lastUpdateUiGameTime;
        private bool wasInventoryOpenLastFrame = false;


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
                }
                else
                {
                    HideTrackerUI();//if hotkey is pressed hide the ui
                }
            }

            // Automatically close when inventory closes
            if (!Main.playerInventory && wasInventoryOpenLastFrame)
            {
                HideTrackerUI(); // Or ToggleUI(false);
            }

            wasInventoryOpenLastFrame = Main.playerInventory;

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

        internal static void ToggleUI(bool visible)
        {
            if (!Main.dedServ && ModContent.GetInstance<bossStatTrackerUISystem>() is bossStatTrackerUISystem system)
            {
                if (visible)
                    system.ShowTrackerUI();
                else
                    system.HideTrackerUI();
            }
        }

    }

    // This is the main UIState that gets activated/deactivated by the ModSystem
    class bossStatTrackerUI : UIState
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
            panel.UpdateText(player);
        }
    }
}

