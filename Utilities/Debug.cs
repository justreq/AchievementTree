using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;
using AchievementTree;
using System.Reflection.Metadata;
using SDL2;

[assembly: MetadataUpdateHandler(typeof(AchievementTree.Utilities.QuickRestart))]

namespace AchievementTree.Utilities;

#if DEBUG

class QuickRestart
{
    public static void UpdateApplication(Type[] updatedTypes)
    {
        IngameFancyUI.Close();
        IngameFancyUI.OpenUIState(new AchievementTreeUI());
    }
}

class DebugPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        IngameFancyUI.OpenUIState(new AchievementTreeUI());
    }
}

class DebugSystem : ModSystem
{
    public override void Load()
    {
        MonoModHooks.Add(typeof(ModContent).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer")!.GetMethod("SaveLastLaunchedMods", BindingFlags.NonPublic | BindingFlags.Static)!, (Action orig) =>
        {
            orig();
            enterCharacterSelectMenu = true;
        });

        On_Main.DrawMenu += Main_DrawMenu;
    }

    static bool enterCharacterSelectMenu;
    private void Main_DrawMenu(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (enterCharacterSelectMenu && Main.netMode == NetmodeID.SinglePlayer)
        {
            enterCharacterSelectMenu = false;
            Main.OpenCharacterSelectUI();

            var player = Main.PlayerList.FirstOrDefault(d => d.Name == "Mod Testing");
            if (player == null) return;
            Main.SelectPlayer(player);

            Main.OpenWorldSelectUI();
            UIWorldSelect worldSelect = (UIWorldSelect)typeof(Main).GetField("_worldSelectMenu", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)!.GetValue(null!)!;
            UIList uiList = (UIList)typeof(UIWorldSelect).GetField("_worldList", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)!.GetValue(worldSelect)!;
            var item = uiList._items.OfType<UIWorldListItem>().FirstOrDefault(d =>
            {
                return ((WorldFileData)typeof(UIWorldListItem).GetField("_data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(d)!).Name == "Mod Testing";
            });
            if (item != null)
                typeof(UIWorldListItem).GetMethod("PlayGame", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)!.Invoke(item, new object[]
                {
                    new UIMouseEvent(item, item.GetOuterDimensions().Position()), item
                });
        }
    }
}

#endif