using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Audio;
using Terraria.ID;
using System.Collections.Generic;
using bossStatTracker.UI; // Import the panel/State
using System;

namespace bossStatTracker.System
{
    public class bossStatTrackerUISystem : ModSystem
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

            //the little button for the bottom right of the inventory screen, modelled off of boss checklist
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
                    "bossStatTracker: InventoryIcon",
                    delegate
                    {
                        if (Main.playerInventory)
                        {
                            // Set position — bottom right of the inventory screen
                            Vector2 position = new Vector2(Main.screenWidth - 325f, Main.screenHeight - 35f);

                            // Load your icon texture
                            //Texture2D icon = ModContent.Request<Texture2D>("bossStatTracker/Assets/TrackerIcon", AssetRequestMode.ImmediateLoad).Value;

                            // Define clickable area
                            Rectangle hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 32); // placeholder icon size

                            // Mouse interaction
                            if (hitbox.Contains(Main.mouseX, Main.mouseY))
                            {
                                Main.LocalPlayer.mouseInterface = true;

                                if (Main.mouseLeft && Main.mouseLeftRelease)
                                {
                                    ToggleUI(ModContent.GetInstance<bossStatTrackerUISystem>().TrackerInterface.CurrentState == null);
                                    SoundEngine.PlaySound(SoundID.MenuOpen);
                                }
                            }

                            // Draw the button
                            //Main.spriteBatch.Draw(icon, position, Color.White); //we need to get an icon
                            Utils.DrawBorderString(Main.spriteBatch, "BST", position, Color.White);

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

        //setting up the boss checklist api
        private static Dictionary<string, Dictionary<string, object>> _bossDict;
        private static bool bossChecklistDataLoaded = false;
        public static Dictionary<string, Dictionary<string, object>> BossData => _bossDict;
        public override void PostSetupContent()
        {
            if (bossChecklistDataLoaded)
                return;

            // Log the attempt to get the BossChecklist mod
            Mod.Logger.Info("Attempting to resolve BossChecklist mod...");

            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                Mod.Logger.Info("Successfully resolved BossChecklist mod.");

                // Attempt to call GetBossInfoDictionary
                var dict = bossChecklist.Call("GetBossInfoDictionary", Mod, new Version(1, 6).ToString()) as Dictionary<string, Dictionary<string, object>>;

                if (dict != null)
                {
                    _bossDict = dict;
                    bossChecklistDataLoaded = true;
                    Mod.Logger.Info($"BossChecklist data loaded successfully. Entries count: {_bossDict.Count}");
                }
                else
                {
                    Mod.Logger.Warn("⚠️ BossChecklist returned null for GetBossInfoDictionary.");
                }
            }
            else
            {
                Mod.Logger.Warn("⚠️ Failed to resolve BossChecklist mod.");
            }
        }

    }
}