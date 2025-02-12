using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;

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

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (TrackerInterface?.CurrentState != null)
            {
                TrackerInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "MyMod: MyInterface",
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

        public override void OnInitialize()
        {

            // Create a new panel
            UIPanel panel = new UIPanel();
            panel.SetPadding(0);
            panel.Width.Set(400, 0); // Set the width of the panel
            panel.Height.Set(300, 0); // Set the height of the panel
            panel.BackgroundColor = new Color(73, 94, 171); // Set the background color of the panel
            Append(panel); // Append the panel to the UIState

            // Add other UI elements to the panel here
            UIText text = new UIText("Boss Stats will be here soon :)");
            panel.Append(text);

            // Set the UI state for the UserInterface
            bossStatTrackerUISystem uiSystemInstance = ModContent.GetInstance<bossStatTrackerUISystem>();
            uiSystemInstance.TrackerInterface.SetState(uiSystemInstance.TrackerUI);
        }


    }
}