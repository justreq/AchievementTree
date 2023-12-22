using AchievementTree.Elements;
using AchievementTree.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AchievementTree;
public class AchievementTreeUI : UIState
{
    UIElement ContentContainer { get; set; }
    UIElement AchievementTreeContainer { get; set; }

    readonly List<UIToggleImage> CategoryToggles = new();
    readonly List<UIAchievementTreePanel> AchievementTreePanels = new() { new(), new(), new(), new(), new() };
    readonly List<UIAchievementItem> AchievementItems = new();

    private static Achievement FindAchievement(string name) => Main.Achievements.GetAchievement(name);
    private UIAchievementItem FindAchievementItem(string name) => AchievementItems.FirstOrDefault(e => e.achievement.Name == name);

    public override void OnInitialize()
    {
        ContentContainer = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPercent(0.8f);
            e.MaxWidth = StyleDimension.FromPixels(1000f);
            e.MinWidth = StyleDimension.FromPixels(600f);
            e.Height = StyleDimension.FromPixelsAndPercent(-178f, 1f);
            e.HAlign = e.VAlign = 0.5f;
        }));

        UIPanel MainPanel = ContentContainer.AddElement(new UIPanel().With(e =>
        {
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixelsAndPercent(0f, 1f);
            e.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            e.PaddingTop = 0f;
        }));

        var CategoryContainer = MainPanel.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(32f);
            e.Top = StyleDimension.FromPixels(10f);
        }));

        for (int i = 0; i < 5; i++)
        {
            var categoryToggle = CategoryContainer.AddElement(new UIToggleImage(ModContent.Request<Texture2D>("AchievementTree/Assets/Achievement_Categories"), 32, 32, new(34 * i, 0), new(34 * i, 34)).With(e =>
            {
                e.Left = StyleDimension.FromPixels(i * 36f + 8f);
                e.SetState(i == 3);
                e.OnLeftClick += OnCategoryToggle;
                CategoryToggles.Add(e);
            }));
        }

        AchievementTreeContainer = MainPanel.AddElement(new UIPanel().With(e =>
        {
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixelsAndPercent(-50f, 1f);
            e.Top = StyleDimension.FromPixels(50f);
            e.BackgroundColor = new Color(13, 20, 44);
            e.BorderColor = new Color(6, 10, 22);
            e.OverflowHidden = true;
        }));

        AchievementTreePanels.ForEach(e =>
        {
            e.MinWidth = StyleDimension.FromPercent(1f);
            e.MinHeight = StyleDimension.FromPercent(1f);

            PopulateTreePanel(e);

            if (e.Children.Any())
            {
                float width = e.Children.ToList().Max(e => e.GetDimensions().X) - e.GetDimensions().X + 72f;
                float height = e.Children.ToList().Max(e => e.GetDimensions().Y) - e.GetDimensions().Y + 72f;

                e.MaxWidth = e.Width = StyleDimension.FromPixels(width);
                e.MaxHeight = e.Height = StyleDimension.FromPixels(height);
            }
        });

        ShowTreePanel(AchievementTreePanels[3]);
    }

    private void OnCategoryToggle(UIMouseEvent evt, UIElement listeningElement)
    {
        int index = CategoryToggles.IndexOf((UIToggleImage)listeningElement);

        for (int i = 0; i < CategoryToggles.Count; i++)
        {
            CategoryToggles[i].SetState(i == index);
        }

        ShowTreePanel(AchievementTreePanels[index]);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Vector2 mouseTextPosition = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);

        string hoverText;

        for (int i = 0; i < CategoryToggles.Count; i++)
        {
            if (!CategoryToggles[i].IsMouseHovering) continue;

            hoverText = i switch
            {
                0 => Language.GetTextValue("GameTitle.0").Split(":").First(),
                1 => Language.GetTextValue("Achievements.SlayerCategory"),
                2 => Language.GetTextValue("Achievements.CollectorCategory"),
                3 => Language.GetTextValue("Achievements.ExplorerCategory"),
                4 => Language.GetTextValue("Achievements.ChallengerCategory"),
            };

            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, hoverText, mouseTextPosition.X, mouseTextPosition.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);

            break;
        }

        for (int i = 0; i < AchievementItems.Count; i++)
        {
            UIAchievementItem element = AchievementItems[i];
            if (!element.IsMouseHovering) continue;

            hoverText = $"{element.achievement.FriendlyName}\n{element.achievement.Description}";

            float x = FontAssets.MouseText.Value.MeasureString(hoverText).X;

            if (mouseTextPosition.X > Main.screenWidth - x) mouseTextPosition.X = Main.screenWidth - x;

            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, hoverText, mouseTextPosition.X, mouseTextPosition.Y, element.locked ? Color.White : new Color(255, 221, 67), Color.Black, Vector2.Zero);

            break;
        }
    }

    private void ShowTreePanel(UIAchievementTreePanel panel)
    {
        AchievementTreeContainer.RemoveAllChildren();
        AchievementTreeContainer.Append(panel);
    }

    private void PopulateTreePanel(UIAchievementTreePanel panel)
    {
        int index = AchievementTreePanels.IndexOf(panel);
        List<UIAchievementItem> items = new();

        switch (index)
        {
            case 0:
                break;
            case 1:
                AppendAchievementItem(items, "VEHICULAR_MANSLAUGHTER", new(0, -6));
                AppendAchievementItem(items, "EYE_ON_YOU", new(0, -2));
                AppendAchievementItem(items, "WORM_FODDER", new(0, 0));
                AppendAchievementItem(items, "STING_OPERATION", new(0, 2));
                AppendAchievementItem(items, "PRETTY_IN_PINK", new(0, 6));

                AppendAchievementItem(items, "DECEIVER_OF_FOOLS", new(2, -6));
                AppendAchievementItem(items, "SLIPPERY_SHINOBI", new(2, -2));
                AppendAchievementItem(items, "MASTERMIND", new(2, 0));
                AppendAchievementItem(items, "DEFEAT_DEERCLOPS", new(2, 2));
                AppendAchievementItem(items, "THERE_ARE_SOME_WHO_CALL_HIM", new(2, 6));

                AppendAchievementItem(items, "ARCHAEOLOGIST", new(4, -6));
                AppendAchievementItem(items, "TIL_DEATH", new(4, 6));

                AppendAchievementItem(items, "WALK_THE_PLANK", new(6, -2));
                AppendAchievementItem(items, "STILL_HUNGRY", new(6, 0));
                AppendAchievementItem(items, "DEFEAT_DREADNAUTILUS", new(6, 2));

                AppendAchievementItem(items, "GOBLIN_PUNTER", new(8, -6));
                AppendAchievementItem(items, "FISH_OUT_OF_WATER", new(8, -2));
                AppendAchievementItem(items, "BUCKETS_OF_BOLTS", new(8, 0));
                AppendAchievementItem(items, "DEFEAT_QUEEN_SLIME", new(8, 2));
                AppendAchievementItem(items, "DO_YOU_WANT_TO_SLAY_A_SNOWMAN", new(8, 6));

                AppendAchievementItem(items, "BALEFUL_HARVEST", new(10, -6));
                AppendAchievementItem(items, "THE_GREAT_SOUTHERN_PLANTKILL", new(10, 0));
                AppendAchievementItem(items, "ICE_SCREAM", new(10, 6));

                AppendAchievementItem(items, "DEFEAT_OLD_ONES_ARMY_TIER3", new(12, -6));
                AppendAchievementItem(items, "LIHZAHRDIAN_IDOL", new(12, 0));
                AppendAchievementItem(items, "DEFEAT_EMPRESS_OF_LIGHT", new(12, 2));
                AppendAchievementItem(items, "GAIN_TORCH_GODS_FAVOR", new(12, 6));

                AppendAchievementItem(items, "BONED", new(14, -2));
                AppendAchievementItem(items, "TIN_FOIL_HATTER", new(14, 2));

                AppendAchievementItem(items, "OBSESSIVE_DEVOTION", new(16, 0));

                AppendAchievementItem(items, "STAR_DESTROYER", new(18, 0));

                AppendAchievementItem(items, "CHAMPION_OF_TERRARIA", new(20, 0));

                ConnectAchievementItems("STILL_HUNGRY", new() { "BUCKETS_OF_BOLTS", "DEFEAT_QUEEN_SLIME", "FISH_OUT_OF_WATER", "WALK_THE_PLANK", "DEFEAT_DREADNAUTILUS" });
                ConnectAchievementItems("THE_GREAT_SOUTHERN_PLANTKILL", new() { "BUCKETS_OF_BOLTS", "LIHZAHRDIAN_IDOL", "DEFEAT_EMPRESS_OF_LIGHT", "BALEFUL_HARVEST", "ICE_SCREAM" });
                ConnectAchievementItems("LIHZAHRDIAN_IDOL", new() { "TIN_FOIL_HATTER", "DEFEAT_OLD_ONES_ARMY_TIER3" });
                ConnectAchievementItems("OBSESSIVE_DEVOTION", new() { "BONED", "LIHZAHRDIAN_IDOL", "STAR_DESTROYER" });
                ConnectAchievementItems("CHAMPION_OF_TERRARIA", new() { "STAR_DESTROYER" });
                break;
            case 2:
                AppendAchievementItem(items, "FASHION_STATEMENT", new(0, -6));
                AppendAchievementItem(items, "TIMBER", new(0, 0));
                AppendAchievementItem(items, "GLORIOUS_GOLDEN_POLE", new(0, 6));

                AppendAchievementItem(items, "DYE_HARD", new(2, -6));
                AppendAchievementItem(items, "STAR_POWER", new(2, -2));
                AppendAchievementItem(items, "BENCHED", new(2, 0));
                AppendAchievementItem(items, "MATCHING_ATTIRE", new(2, 2));
                AppendAchievementItem(items, "GET_CELL_PHONE", new(2, 6));

                AppendAchievementItem(items, "THE_CAVALRY", new(4, -6));
                AppendAchievementItem(items, "OBTAIN_HAMMER", new(4, -2));
                AppendAchievementItem(items, "COMPLETELY_AWESOME", new(4, 2));
                AppendAchievementItem(items, "GET_TERRASPARK_BOOTS", new(4, 6));

                AppendAchievementItem(items, "LIKE_A_BOSS", new(6, -6));
                AppendAchievementItem(items, "HOLD_ON_TIGHT", new(6, -2));
                AppendAchievementItem(items, "HEAVY_METAL", new(6, 0));
                AppendAchievementItem(items, "DRAX_ATTAX", new(6, 2));
                AppendAchievementItem(items, "GET_GOLDEN_DELIGHT", new(6, 6));

                AppendAchievementItem(items, "HEAD_IN_THE_CLOUDS", new(8, -6));
                AppendAchievementItem(items, "MINER_FOR_FIRE", new(8, -2));
                AppendAchievementItem(items, "SWORD_OF_THE_HERO", new(8, 0));
                AppendAchievementItem(items, "PRISMANCER", new(8, 2));
                AppendAchievementItem(items, "GET_ANKH_SHIELD", new(8, 6));

                AppendAchievementItem(items, "TEMPLE_RAIDER", new(10, -6));
                AppendAchievementItem(items, "GET_ZENITH", new(10, 0));
                AppendAchievementItem(items, "SICK_THROW", new(10, 6));


                ConnectAchievementItems("BENCHED", new() { "TIMBER", "OBTAIN_HAMMER", "HEAVY_METAL", "MATCHING_ATTIRE", "STAR_POWER" });
                ConnectAchievementItems("HEAVY_METAL", new() { "HOLD_ON_TIGHT", "MINER_FOR_FIRE", "COMPLETELY_AWESOME", "PRISMANCER", "SWORD_OF_THE_HERO", "DRAX_ATTAX" });
                ConnectAchievementItems("SWORD_OF_THE_HERO", new() { "GET_ZENITH" });
                break;
            case 3:
                AppendAchievementItem(items, "NO_HOBO", new(0, -6));
                AppendAchievementItem(items, "OOO_SHINY", new(1, -6));
                AppendAchievementItem(items, "HEART_BREAKER", new(2, -6));
                AppendAchievementItem(items, "I_AM_LOOT", new(3, -6));
                AppendAchievementItem(items, "SMASHING_POPPET", new(4, -6));
                AppendAchievementItem(items, "WHERES_MY_HONEY", new(5, -6));
                AppendAchievementItem(items, "DUNGEON_HEIST", new(6, -6));
                AppendAchievementItem(items, "ITS_GETTING_HOT_IN_HERE", new(7, -6));
                AppendAchievementItem(items, "ITS_HARD", new(8, -6));
                AppendAchievementItem(items, "BEGONE_EVIL", new(9, -6));
                AppendAchievementItem(items, "EXTRA_SHINY", new(10, -6));
                AppendAchievementItem(items, "PHOTOSYNTHESIS", new(11, -6));
                AppendAchievementItem(items, "GET_A_LIFE", new(0, -5));
                AppendAchievementItem(items, "ROBBING_THE_GRAVE", new(1, -5));
                AppendAchievementItem(items, "BIG_BOOTY", new(2, -5));
                AppendAchievementItem(items, "BLOODBATH", new(3, -5));
                AppendAchievementItem(items, "KILL_THE_SUN", new(4, -5));
                AppendAchievementItem(items, "STICKY_SITUATION", new(5, -5));
                AppendAchievementItem(items, "JEEPERS_CREEPERS", new(6, -5));
                AppendAchievementItem(items, "FUNKYTOWN", new(7, -5));
                AppendAchievementItem(items, "INTO_ORBIT", new(8, -5));
                AppendAchievementItem(items, "ROCK_BOTTOM", new(9, -5));
                AppendAchievementItem(items, "IT_CAN_TALK", new(10, -5));
                AppendAchievementItem(items, "WATCH_YOUR_STEP", new(11, -5));
                AppendAchievementItem(items, "YOU_CAN_DO_IT", new(0, -4));
                AppendAchievementItem(items, "FOUND_GRAVEYARD", new(1, -4));
                AppendAchievementItem(items, "FIND_A_FAIRY", new(2, -4));
                AppendAchievementItem(items, "PLAY_ON_A_SPECIAL_SEED", new(3, -4));
                AppendAchievementItem(items, "TRANSMUTE_ITEM", new(4, -4));
                break;
            case 4:
                AppendAchievementItem(items, "REAL_ESTATE_AGENT", new(0, 0));
                AppendAchievementItem(items, "NOT_THE_BEES", new(0, 0));
                AppendAchievementItem(items, "MECHA_MAYHEM", new(0, 0));
                AppendAchievementItem(items, "GELATIN_WORLD_TOUR", new(0, 0));
                AppendAchievementItem(items, "BULLDOZER", new(0, 0));
                AppendAchievementItem(items, "LUCKY_BREAK", new(0, 0));
                AppendAchievementItem(items, "THROWING_LINES", new(0, 0));
                AppendAchievementItem(items, "FREQUENT_FLYER", new(0, 0));
                AppendAchievementItem(items, "RAINBOWS_AND_UNICORNS", new(0, 0));
                AppendAchievementItem(items, "YOU_AND_WHAT_ARMY", new(0, 0));
                AppendAchievementItem(items, "MARATHON_MEDALIST", new(0, 0));
                AppendAchievementItem(items, "SERVANT_IN_TRAINING", new(0, 0));
                AppendAchievementItem(items, "GOOD_LITTLE_SLAVE", new(0, 0));
                AppendAchievementItem(items, "TROUT_MONKEY", new(0, 0));
                AppendAchievementItem(items, "FAST_AND_FISHIOUS", new(0, 0));
                AppendAchievementItem(items, "SUPREME_HELPER_MINION", new(0, 0));
                AppendAchievementItem(items, "TOPPED_OFF", new(0, 0));
                AppendAchievementItem(items, "SLAYER_OF_WORLDS", new(0, 0));
                AppendAchievementItem(items, "FLY_A_KITE_ON_A_WINDY_DAY", new(0, 0));
                AppendAchievementItem(items, "GO_LAVA_FISHING", new(0, 0));
                AppendAchievementItem(items, "TURN_GNOME_TO_STATUE", new(0, 0));
                AppendAchievementItem(items, "TALK_TO_NPC_AT_MAX_HAPPINESS", new(0, 0));
                AppendAchievementItem(items, "PET_THE_PET", new(0, 0));
                AppendAchievementItem(items, "THROW_A_PARTY", new(0, 0));
                AppendAchievementItem(items, "DIE_TO_DEAD_MANS_CHEST", new(0, 0));
                AppendAchievementItem(items, "DRINK_BOTTLED_WATER_WHILE_DROWNING", new(0, 0));
                AppendAchievementItem(items, "ALL_TOWN_SLIMES", new(0, 0));
                AppendAchievementItem(items, "PURIFY_ENTIRE_WORLD", new(0, 0));
                AppendAchievementItem(items, "TO_INFINITY_AND_BEYOND", new(0, 0));
                break;
        }

        items.ForEach(panel.Append);

        void AppendAchievementItem(List<UIAchievementItem> list, string achievement, Vector2 displacementFactor, float vAlign = 0.5f)
        {
            new UIAchievementItem(FindAchievement(achievement)).With(e =>
            {
                e.VAlign = vAlign;
                e.Left = StyleDimension.FromPixels(60f * displacementFactor.X);
                e.Top = StyleDimension.FromPixels(60f * displacementFactor.Y);

                list.Add(e);
                AchievementItems.Add(e);
            });
        }

        void ConnectAchievementItems(string connectFrom, List<string> connectTo)
        {
            connectTo.ForEach(e =>
            {
                panel.AddElement(new UIAchievementConnector(FindAchievementItem(connectFrom), FindAchievementItem(e)));
            });
        }
    }
}
