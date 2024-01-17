using AchievementTree.Elements;
using AchievementTree.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
    AchievementTreeModPlayer ModPlayer => Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>();

    UIElement ContentContainer { get; set; }
    UIElement AchievementTreeContainer { get; set; }

    readonly List<UIToggleImage> CategoryToggles = [];
    readonly List<UIAchievementTreePanel> AchievementTreePanels = [new(), new(), new(), new(), new()];
    readonly List<UIAchievementItem> AchievementItems = [];

    private UIAchievementItem FindAchievementItem(string name) => AchievementItems.FirstOrDefault(e => e.localAchievement.name == name);

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
                e.SetState(i == 0);
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
                float width = e.Children.Max(e => e.GetDimensions().X) - e.GetDimensions().X + 72f;
                float height = e.Children.Max(e => e.GetDimensions().Y) - e.GetDimensions().Y + 72f;

                e.MaxWidth = e.Width = StyleDimension.FromPixels(width);
                e.MaxHeight = e.Height = StyleDimension.FromPixels(height);
            }
        });

        ShowTreePanel(AchievementTreePanels[0]);
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

            hoverText = $"{element.localAchievement.friendlyName}\n{element.localAchievement.description}";

            float x = FontAssets.MouseText.Value.MeasureString(hoverText).X;

            if (mouseTextPosition.X > Main.screenWidth - x) mouseTextPosition.X = Main.screenWidth - x;

            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, hoverText, mouseTextPosition.X, mouseTextPosition.Y, element.complete ? new Color(255, 221, 67) : Color.White, Color.Black, Vector2.Zero);

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
        List<UIAchievementItem> items = [];
        int offsetX = 0;

        switch (index)
        {
            case 0:
                /*
                AppendAchievementItem(items, VanillaAchievementName.VEHICULAR_MANSLAUGHTER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.EYE_ON_YOU, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.WORM_FODDER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.STING_OPERATION, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PRETTY_IN_PINK, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DECEIVER_OF_FOOLS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SLIPPERY_SHINOBI, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.MASTERMIND, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_DEERCLOPS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THERE_ARE_SOME_WHO_CALL_HIM, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ARCHAEOLOGIST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TIL_DEATH, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.WALK_THE_PLANK, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.STILL_HUNGRY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_DREADNAUTILUS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GOBLIN_PUNTER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FISH_OUT_OF_WATER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BUCKETS_OF_BOLTS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_QUEEN_SLIME, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DO_YOU_WANT_TO_SLAY_A_SNOWMAN, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BALEFUL_HARVEST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THE_GREAT_SOUTHERN_PLANTKILL, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ICE_SCREAM, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_OLD_ONES_ARMY_TIER3, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.LIHZAHRDIAN_IDOL, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_EMPRESS_OF_LIGHT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GAIN_TORCH_GODS_FAVOR, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BONED, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TIN_FOIL_HATTER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.OBSESSIVE_DEVOTION, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.STAR_DESTROYER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.CHAMPION_OF_TERRARIA, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FASHION_STATEMENT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THE_CAVALRY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TIMBER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GET_ANKH_SHIELD, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GET_CELL_PHONE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DYE_HARD, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.HEAD_IN_THE_CLOUDS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.STAR_POWER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BENCHED, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.MATCHING_ATTIRE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GET_TERRASPARK_BOOTS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.OBTAIN_HAMMER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.LIKE_A_BOSS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TEMPLE_RAIDER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.HOLD_ON_TIGHT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.HEAVY_METAL, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DRAX_ATTAX, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.COMPLETELY_AWESOME, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GLORIOUS_GOLDEN_POLE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.MINER_FOR_FIRE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SWORD_OF_THE_HERO, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PRISMANCER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GET_GOLDEN_DELIGHT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SICK_THROW, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GET_ZENITH, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.YOU_CAN_DO_IT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BLOODBATH, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ITS_GETTING_HOT_IN_HERE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.WATCH_YOUR_STEP, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.OOO_SHINY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.STICKY_SITUATION, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ROCK_BOTTOM, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.HEART_BREAKER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.I_AM_LOOT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ROBBING_THE_GRAVE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BIG_BOOTY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FUNKYTOWN, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.JEEPERS_CREEPERS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PLAY_ON_A_SPECIAL_SEED, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.INTO_ORBIT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.KILL_THE_SUN, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ITS_HARD, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.IT_CAN_TALK, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TRANSMUTE_ITEM, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.NO_HOBO, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FIND_A_FAIRY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BEGONE_EVIL, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PHOTOSYNTHESIS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GET_A_LIFE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SMASHING_POPPET, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FOUND_GRAVEYARD, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.EXTRA_SHINY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.WHERES_MY_HONEY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DUNGEON_HEIST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TALK_TO_NPC_AT_MAX_HAPPINESS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PET_THE_PET, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GELATIN_WORLD_TOUR, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.NOT_THE_BEES, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THROW_A_PARTY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TOPPED_OFF, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.RAINBOWS_AND_UNICORNS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.YOU_AND_WHAT_ARMY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BULLDOZER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SERVANT_IN_TRAINING, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SLAYER_OF_WORLDS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DIE_TO_DEAD_MANS_CHEST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.LUCKY_BREAK, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.MARATHON_MEDALIST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GOOD_LITTLE_SLAVE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ALL_TOWN_SLIMES, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TURN_GNOME_TO_STATUE, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FLY_A_KITE_ON_A_WINDY_DAY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FREQUENT_FLYER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TROUT_MONKEY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.REAL_ESTATE_AGENT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TO_INFINITY_AND_BEYOND, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THROWING_LINES, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FAST_AND_FISHIOUS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GO_LAVA_FISHING, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PURIFY_ENTIRE_WORLD, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.MECHA_MAYHEM, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SUPREME_HELPER_MINION, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DRINK_BOTTLED_WATER_WHILE_DROWNING, new(offsetX, 0));
                */
                break;
            case 1:
                AppendAchievementItem(items, VanillaAchievementName.VEHICULAR_MANSLAUGHTER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.EYE_ON_YOU, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.WORM_FODDER, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.STING_OPERATION, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.PRETTY_IN_PINK, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.DECEIVER_OF_FOOLS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SLIPPERY_SHINOBI, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.MASTERMIND, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_DEERCLOPS, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.THERE_ARE_SOME_WHO_CALL_HIM, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.ARCHAEOLOGIST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TIL_DEATH, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.WALK_THE_PLANK, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.STILL_HUNGRY, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_DREADNAUTILUS, new(offsetX, 8));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.GOBLIN_PUNTER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FISH_OUT_OF_WATER, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.BUCKETS_OF_BOLTS, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_QUEEN_SLIME, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.DO_YOU_WANT_TO_SLAY_A_SNOWMAN, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.BALEFUL_HARVEST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THE_GREAT_SOUTHERN_PLANTKILL, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.ICE_SCREAM, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_OLD_ONES_ARMY_TIER3, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.LIHZAHRDIAN_IDOL, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.DEFEAT_EMPRESS_OF_LIGHT, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.GAIN_TORCH_GODS_FAVOR, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.BONED, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.TIN_FOIL_HATTER, new(offsetX, 8));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.OBSESSIVE_DEVOTION, new(offsetX, 6));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.STAR_DESTROYER, new(offsetX, 6));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.CHAMPION_OF_TERRARIA, new(offsetX, 6));

                ConnectAchievementItems(VanillaAchievementName.STILL_HUNGRY, [VanillaAchievementName.BUCKETS_OF_BOLTS, VanillaAchievementName.DEFEAT_QUEEN_SLIME, VanillaAchievementName.FISH_OUT_OF_WATER, VanillaAchievementName.WALK_THE_PLANK, VanillaAchievementName.DEFEAT_DREADNAUTILUS]);
                ConnectAchievementItems(VanillaAchievementName.THE_GREAT_SOUTHERN_PLANTKILL, [VanillaAchievementName.BUCKETS_OF_BOLTS, VanillaAchievementName.LIHZAHRDIAN_IDOL, VanillaAchievementName.DEFEAT_EMPRESS_OF_LIGHT, VanillaAchievementName.BALEFUL_HARVEST, VanillaAchievementName.ICE_SCREAM]);
                ConnectAchievementItems(VanillaAchievementName.LIHZAHRDIAN_IDOL, [VanillaAchievementName.TIN_FOIL_HATTER, VanillaAchievementName.DEFEAT_OLD_ONES_ARMY_TIER3]);
                ConnectAchievementItems(VanillaAchievementName.OBSESSIVE_DEVOTION, [VanillaAchievementName.BONED, VanillaAchievementName.LIHZAHRDIAN_IDOL, VanillaAchievementName.STAR_DESTROYER]);
                ConnectAchievementItems(VanillaAchievementName.CHAMPION_OF_TERRARIA, [VanillaAchievementName.STAR_DESTROYER]);
                break;
            case 2:
                AppendAchievementItem(items, VanillaAchievementName.FASHION_STATEMENT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.THE_CAVALRY, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.TIMBER, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.GET_ANKH_SHIELD, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.GET_CELL_PHONE, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.DYE_HARD, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.HEAD_IN_THE_CLOUDS, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.STAR_POWER, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.BENCHED, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.MATCHING_ATTIRE, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.GET_TERRASPARK_BOOTS, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.OBTAIN_HAMMER, new(offsetX, 4));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.LIKE_A_BOSS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TEMPLE_RAIDER, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.HOLD_ON_TIGHT, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.HEAVY_METAL, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.DRAX_ATTAX, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.COMPLETELY_AWESOME, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.GLORIOUS_GOLDEN_POLE, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.MINER_FOR_FIRE, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.SWORD_OF_THE_HERO, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.PRISMANCER, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.GET_GOLDEN_DELIGHT, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.SICK_THROW, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.GET_ZENITH, new(offsetX, 6));

                ConnectAchievementItems(VanillaAchievementName.BENCHED, [VanillaAchievementName.TIMBER, VanillaAchievementName.OBTAIN_HAMMER, VanillaAchievementName.HEAVY_METAL, VanillaAchievementName.MATCHING_ATTIRE, VanillaAchievementName.STAR_POWER]);
                ConnectAchievementItems(VanillaAchievementName.HEAVY_METAL, [VanillaAchievementName.HOLD_ON_TIGHT, VanillaAchievementName.MINER_FOR_FIRE, VanillaAchievementName.PRISMANCER, VanillaAchievementName.SWORD_OF_THE_HERO, VanillaAchievementName.DRAX_ATTAX]);
                ConnectAchievementItems(VanillaAchievementName.SWORD_OF_THE_HERO, [VanillaAchievementName.GET_ZENITH]);
                break;
            case 3:
                AppendAchievementItem(items, VanillaAchievementName.YOU_CAN_DO_IT, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.BLOODBATH, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.ITS_GETTING_HOT_IN_HERE, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.WATCH_YOUR_STEP, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.OOO_SHINY, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.STICKY_SITUATION, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.ROCK_BOTTOM, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.HEART_BREAKER, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.I_AM_LOOT, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.ROBBING_THE_GRAVE, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.BIG_BOOTY, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.FUNKYTOWN, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.JEEPERS_CREEPERS, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.PLAY_ON_A_SPECIAL_SEED, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.INTO_ORBIT, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.KILL_THE_SUN, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.ITS_HARD, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.IT_CAN_TALK, new(offsetX, 8));
                AppendAchievementItem(items, VanillaAchievementName.TRANSMUTE_ITEM, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.NO_HOBO, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FIND_A_FAIRY, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.BEGONE_EVIL, new(offsetX, 4));
                AppendAchievementItem(items, VanillaAchievementName.PHOTOSYNTHESIS, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.GET_A_LIFE, new(offsetX, 8));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.SMASHING_POPPET, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.FOUND_GRAVEYARD, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.EXTRA_SHINY, new(offsetX, 4));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.WHERES_MY_HONEY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.DUNGEON_HEIST, new(offsetX, 2));

                ConnectAchievementItems(VanillaAchievementName.ITS_GETTING_HOT_IN_HERE, [VanillaAchievementName.ROCK_BOTTOM]);
                ConnectAchievementItems(VanillaAchievementName.ITS_HARD, [VanillaAchievementName.ITS_GETTING_HOT_IN_HERE, VanillaAchievementName.GET_A_LIFE, VanillaAchievementName.BEGONE_EVIL, VanillaAchievementName.PHOTOSYNTHESIS, VanillaAchievementName.IT_CAN_TALK, VanillaAchievementName.ROBBING_THE_GRAVE, VanillaAchievementName.KILL_THE_SUN, VanillaAchievementName.BIG_BOOTY]);
                ConnectAchievementItems(VanillaAchievementName.BEGONE_EVIL, [VanillaAchievementName.EXTRA_SHINY]);
                ConnectAchievementItems(VanillaAchievementName.FUNKYTOWN, [VanillaAchievementName.IT_CAN_TALK]);
                break;
            case 4:
                AppendAchievementItem(items, VanillaAchievementName.TALK_TO_NPC_AT_MAX_HAPPINESS, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.PET_THE_PET, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.GELATIN_WORLD_TOUR, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.NOT_THE_BEES, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.DRINK_BOTTLED_WATER_WHILE_DROWNING, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.THROW_A_PARTY, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TOPPED_OFF, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.RAINBOWS_AND_UNICORNS, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.YOU_AND_WHAT_ARMY, new(offsetX, 6));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.BULLDOZER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.SERVANT_IN_TRAINING, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.SLAYER_OF_WORLDS, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.DIE_TO_DEAD_MANS_CHEST, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.LUCKY_BREAK, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.MARATHON_MEDALIST, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.GOOD_LITTLE_SLAVE, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.ALL_TOWN_SLIMES, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.TURN_GNOME_TO_STATUE, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.FLY_A_KITE_ON_A_WINDY_DAY, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.FREQUENT_FLYER, new(offsetX, 0));
                AppendAchievementItem(items, VanillaAchievementName.TROUT_MONKEY, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.REAL_ESTATE_AGENT, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.TO_INFINITY_AND_BEYOND, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.THROWING_LINES, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.FAST_AND_FISHIOUS, new(offsetX, 2));
                AppendAchievementItem(items, VanillaAchievementName.GO_LAVA_FISHING, new(offsetX, 10));
                AppendAchievementItem(items, VanillaAchievementName.PURIFY_ENTIRE_WORLD, new(offsetX, 6));
                AppendAchievementItem(items, VanillaAchievementName.MECHA_MAYHEM, new(offsetX, 12));
                offsetX += 2;
                AppendAchievementItem(items, VanillaAchievementName.SUPREME_HELPER_MINION, new(offsetX, 2));

                ConnectAchievementItems(VanillaAchievementName.GOOD_LITTLE_SLAVE, [VanillaAchievementName.SERVANT_IN_TRAINING, VanillaAchievementName.TROUT_MONKEY]);
                ConnectAchievementItems(VanillaAchievementName.FAST_AND_FISHIOUS, [VanillaAchievementName.TROUT_MONKEY, VanillaAchievementName.SUPREME_HELPER_MINION]);
                break;
        }

        items.ForEach(panel.Append);

        void AppendAchievementItem(List<UIAchievementItem> list, string achievement, Vector2 displacementFactor)
        {
            new UIAchievementItem(ModPlayer.FindAchievement(achievement)).With(e =>
            {
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
