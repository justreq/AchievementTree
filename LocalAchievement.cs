using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Achievements;
using Terraria.ID;

namespace AchievementTree;

public record struct LocalAchievementTexture(string TexturePath, Rectangle CompleteFrame, Rectangle IncompleteFrame);

public abstract class LocalAchievement
{
    public List<LocalCondition> conditions = [];
    public bool isCompleted = false;

    internal string name;
    internal string friendlyName;
    internal string description;
    internal AchievementCategory category;
    internal LocalAchievementTexture icons;

    public LocalAchievement() { }

    public void AddCondition(LocalCondition condition)
    {
        conditions.Add(condition);
    }

    public void Complete()
    {
        if (!isCompleted)
        {
            isCompleted = true;
        }
    }
}

/// <summary>
/// Local modded achievement
/// </summary>
/// <param name="name">Internal achievement name</param>
/// <param name="friendlyName">Ingame achievement name</param>
/// <param name="description">Ingame achievement description</param>
/// <param name="category">Achievement category</param>
/// <param name="icon">Achievement icon (thumbnail image)</param>
public class LocalModdedAchievement : LocalAchievement
{
    public LocalModdedAchievement(string name, string friendlyName, string description, AchievementCategory category, LocalAchievementTexture icons) : base()
    {
        base.name = name;
        base.friendlyName = friendlyName;
        base.description = description;
        base.category = category;
        base.icons = icons;
    }
}

public class LocalVanillaAchievement : LocalAchievement
{
    /// <summary>
    /// Local vanilla achievement
    /// </summary>
    /// <param name="name">Internal achievement name</param>
    public LocalVanillaAchievement(string name) : base()
    {
        Achievement sourceAchievement = Main.Achievements.GetAchievement(name);
        int iconIndex = Main.Achievements.GetIconIndex(name);
        Rectangle completeFrame = new(iconIndex % 8 * 66, iconIndex / 8 * 66, 64, 64);
        Rectangle incompleteFrame = new(completeFrame.X + 66 * 8, completeFrame.Y, 64, 64);

        friendlyName = sourceAchievement.FriendlyName.ToString();
        description = sourceAchievement.Description.ToString();
        category = sourceAchievement.Category;
        icons = new("Images/UI/Achievements", completeFrame, incompleteFrame);

        switch (name)
        {
            case VanillaAchievementName.TIMBER:
                AddCondition(new LocalItemPickupCondition(9, 619, 2504, 620, 2503, 2260, 621, 911, 1729, 5215));
                break;
            case VanillaAchievementName.BENCHED:
                AddCondition(new LocalItemCraftCondition([.. ItemID.Sets.Workbenches]));
                break;
            case VanillaAchievementName.NO_HOBO:
                break;
            case VanillaAchievementName.OBTAIN_HAMMER:
                AddCondition(new LocalItemPickupCondition(2775, 2746, 5283, 3505, 654, 3517, 7, 3493, 2780, 1513, 2516, 660, 3481, 657, 922, 3511, 2785, 3499, 3487, 196, 367, 104, 797, 2320, 787, 1234, 1262, 3465, 204, 217, 1507, 3524, 3522, 3525, 3523, 4317, 1305));
                break;
            case VanillaAchievementName.OOO_SHINY:
                AddCondition(new LocalTileDestroyedCondition(7, 6, 9, 8, 166, 167, 168, 169, 22, 204, 58, 107, 108, 111, 221, 222, 223, 211));
                break;
            case VanillaAchievementName.HEART_BREAKER:
                AddCondition(new LocalTileDestroyedCondition(12));
                break;
            case VanillaAchievementName.HEAVY_METAL:
                AddCondition(new LocalItemPickupCondition(35, 716));
                break;
            case VanillaAchievementName.I_AM_LOOT:
                break;
            case VanillaAchievementName.STAR_POWER:
                break;
            case VanillaAchievementName.HOLD_ON_TIGHT:
                break;
            case VanillaAchievementName.EYE_ON_YOU:
                AddCondition(new LocalNPCKilledCondition(4));
                break;
            case VanillaAchievementName.SMASHING_POPPET:
                break;
            case VanillaAchievementName.WORM_FODDER:
                AddCondition(new LocalNPCKilledCondition(13, 14, 15));
                break;
            case VanillaAchievementName.MASTERMIND:
                AddCondition(new LocalNPCKilledCondition(266));
                break;
            case VanillaAchievementName.WHERES_MY_HONEY:
                break;
            case VanillaAchievementName.STING_OPERATION:
                AddCondition(new LocalNPCKilledCondition(222));
                break;
            case VanillaAchievementName.BONED:
                AddCondition(new LocalNPCKilledCondition(35));
                break;
            case VanillaAchievementName.DUNGEON_HEIST:
                AddCondition(new LocalItemPickupCondition(327));
                // progression bullshit as well
                break;
            case VanillaAchievementName.ITS_GETTING_HOT_IN_HERE:
                break;
            case VanillaAchievementName.MINER_FOR_FIRE:
                AddCondition(new LocalItemCraftCondition(122));
                break;
            case VanillaAchievementName.STILL_HUNGRY:
                AddCondition(new LocalNPCKilledCondition(113, 114));
                break;
            case VanillaAchievementName.ITS_HARD:
                break;
            case VanillaAchievementName.BEGONE_EVIL:
                break;
            case VanillaAchievementName.EXTRA_SHINY:
                AddCondition(new LocalTileDestroyedCondition(107, 108, 111, 221, 222, 223));
                break;
            case VanillaAchievementName.HEAD_IN_THE_CLOUDS:
                AddCondition(new LocalItemPickupCondition(1133, 1331, 1307, 267, 1293, 5334, 557, 544, 556, 560, 43, 70, 3601, 5120, 4961, 4988, 2673));
                break;
            case VanillaAchievementName.LIKE_A_BOSS:
                AddCondition(new LocalItemPickupCondition(1133, 1331, 1307, 267, 1293, 5334, 557, 544, 556, 560, 43, 70, 3601, 5120, 4961, 4988, 2673));
                break;
            case VanillaAchievementName.BUCKETS_OF_BOLTS:
                AddCondition(new LocalNPCKilledCondition(125, 126));
                AddCondition(new LocalNPCKilledCondition(127));
                AddCondition(new LocalNPCKilledCondition(134));
                break;
            case VanillaAchievementName.DRAX_ATTAX:
                AddCondition(new LocalItemCraftCondition(579, 990));
                break;
            case VanillaAchievementName.PHOTOSYNTHESIS:
                AddCondition(new LocalTileDestroyedCondition(211));
                break;
            case VanillaAchievementName.GET_A_LIFE:
                break;
            case VanillaAchievementName.THE_GREAT_SOUTHERN_PLANTKILL:
                AddCondition(new LocalNPCKilledCondition(262));
                break;
            case VanillaAchievementName.TEMPLE_RAIDER:
                break;
            case VanillaAchievementName.LIHZAHRDIAN_IDOL:
                AddCondition(new LocalNPCKilledCondition(245));
                break;
            case VanillaAchievementName.ROBBING_THE_GRAVE:
                AddCondition(new LocalItemPickupCondition(1513, 938, 963, 977, 1300, 1254, 1514, 679, 759, 1446, 1445, 1444, 1183, 1266, 671, 3291, 4679));
                break;
            case VanillaAchievementName.BIG_BOOTY:
                break;
            case VanillaAchievementName.FISH_OUT_OF_WATER:
                AddCondition(new LocalNPCKilledCondition(370));
                break;
            case VanillaAchievementName.OBSESSIVE_DEVOTION:
                AddCondition(new LocalNPCKilledCondition(439));
                break;
            case VanillaAchievementName.STAR_DESTROYER:
                AddCondition(new LocalNPCKilledCondition(517));
                AddCondition(new LocalNPCKilledCondition(422));
                AddCondition(new LocalNPCKilledCondition(507));
                AddCondition(new LocalNPCKilledCondition(493));
                break;
            case VanillaAchievementName.CHAMPION_OF_TERRARIA:
                AddCondition(new LocalNPCKilledCondition(398));
                break;
            case VanillaAchievementName.BLOODBATH:
                break;
            case VanillaAchievementName.SLIPPERY_SHINOBI:
                AddCondition(new LocalNPCKilledCondition(50));
                break;
            case VanillaAchievementName.GOBLIN_PUNTER:
                break;
            case VanillaAchievementName.WALK_THE_PLANK:
                break;
            case VanillaAchievementName.KILL_THE_SUN:
                break;
            case VanillaAchievementName.DO_YOU_WANT_TO_SLAY_A_SNOWMAN:
                break;
            case VanillaAchievementName.TIN_FOIL_HATTER:
                break;
            case VanillaAchievementName.BALEFUL_HARVEST:
                break;
            case VanillaAchievementName.ICE_SCREAM:
                break;
            case VanillaAchievementName.STICKY_SITUATION:
                break;
            case VanillaAchievementName.REAL_ESTATE_AGENT:
                break;
            case VanillaAchievementName.NOT_THE_BEES:
                break;
            case VanillaAchievementName.JEEPERS_CREEPERS:
                break;
            case VanillaAchievementName.FUNKYTOWN:
                break;
            case VanillaAchievementName.INTO_ORBIT:
                break;
            case VanillaAchievementName.ROCK_BOTTOM:
                break;
            case VanillaAchievementName.MECHA_MAYHEM:
                break;
            case VanillaAchievementName.GELATIN_WORLD_TOUR:
                AddCondition(new LocalNPCKilledCondition(-5));
                AddCondition(new LocalNPCKilledCondition(-6));
                AddCondition(new LocalNPCKilledCondition(1));
                AddCondition(new LocalNPCKilledCondition(81));
                AddCondition(new LocalNPCKilledCondition(71));
                AddCondition(new LocalNPCKilledCondition(-3));
                AddCondition(new LocalNPCKilledCondition(147));
                AddCondition(new LocalNPCKilledCondition(138));
                AddCondition(new LocalNPCKilledCondition(-10));
                AddCondition(new LocalNPCKilledCondition(50));
                AddCondition(new LocalNPCKilledCondition(59));
                AddCondition(new LocalNPCKilledCondition(16));
                AddCondition(new LocalNPCKilledCondition(-7));
                AddCondition(new LocalNPCKilledCondition(244));
                AddCondition(new LocalNPCKilledCondition(-8));
                AddCondition(new LocalNPCKilledCondition(-1));
                AddCondition(new LocalNPCKilledCondition(-2));
                AddCondition(new LocalNPCKilledCondition(184));
                AddCondition(new LocalNPCKilledCondition(204));
                AddCondition(new LocalNPCKilledCondition(225));
                AddCondition(new LocalNPCKilledCondition(-9));
                AddCondition(new LocalNPCKilledCondition(141));
                AddCondition(new LocalNPCKilledCondition(183));
                AddCondition(new LocalNPCKilledCondition(-4));
                break;
            case VanillaAchievementName.FASHION_STATEMENT:
                break;
            case VanillaAchievementName.VEHICULAR_MANSLAUGHTER:
                break;
            case VanillaAchievementName.BULLDOZER:
                break;
            case VanillaAchievementName.THERE_ARE_SOME_WHO_CALL_HIM:
                AddCondition(new LocalNPCKilledCondition(45));
                break;
            case VanillaAchievementName.DECEIVER_OF_FOOLS:
                AddCondition(new LocalNPCKilledCondition(196));
                break;
            case VanillaAchievementName.SWORD_OF_THE_HERO:
                AddCondition(new LocalItemPickupCondition(757));
                break;
            case VanillaAchievementName.LUCKY_BREAK:
                break;
            case VanillaAchievementName.THROWING_LINES:
                break;
            case VanillaAchievementName.DYE_HARD:
                break;
            case VanillaAchievementName.SICK_THROW:
                AddCondition(new LocalItemPickupCondition(3389));
                break;
            case VanillaAchievementName.FREQUENT_FLYER:
                break;
            case VanillaAchievementName.THE_CAVALRY:
                break;
            case VanillaAchievementName.COMPLETELY_AWESOME:
                AddCondition(new LocalItemPickupCondition(98));
                break;
            case VanillaAchievementName.TIL_DEATH:
                AddCondition(new LocalNPCKilledCondition(53));
                break;
            case VanillaAchievementName.ARCHAEOLOGIST:
                AddCondition(new LocalNPCKilledCondition(52));
                break;
            case VanillaAchievementName.PRETTY_IN_PINK:
                AddCondition(new LocalNPCKilledCondition(-4));
                break;
            case VanillaAchievementName.RAINBOWS_AND_UNICORNS:
                break;
            case VanillaAchievementName.YOU_AND_WHAT_ARMY:
                break;
            case VanillaAchievementName.PRISMANCER:
                AddCondition(new LocalItemPickupCondition(495));
                break;
            case VanillaAchievementName.IT_CAN_TALK:
                break;
            case VanillaAchievementName.WATCH_YOUR_STEP:
                break;
            case VanillaAchievementName.MARATHON_MEDALIST:
                break;
            case VanillaAchievementName.GLORIOUS_GOLDEN_POLE:
                AddCondition(new LocalItemPickupCondition(2294));
                break;
            case VanillaAchievementName.SERVANT_IN_TRAINING:
                break;
            case VanillaAchievementName.GOOD_LITTLE_SLAVE:
                break;
            case VanillaAchievementName.TROUT_MONKEY:
                break;
            case VanillaAchievementName.FAST_AND_FISHIOUS:
                break;
            case VanillaAchievementName.SUPREME_HELPER_MINION:
                break;
            case VanillaAchievementName.TOPPED_OFF:
                break;
            case VanillaAchievementName.SLAYER_OF_WORLDS:
                AddCondition(new LocalNPCKilledCondition(13, 14, 15));
                AddCondition(new LocalNPCKilledCondition(113, 114));
                AddCondition(new LocalNPCKilledCondition(125, 126));
                AddCondition(new LocalNPCKilledCondition(4));
                AddCondition(new LocalNPCKilledCondition(266));
                AddCondition(new LocalNPCKilledCondition(35));
                AddCondition(new LocalNPCKilledCondition(50));
                AddCondition(new LocalNPCKilledCondition(222));
                AddCondition(new LocalNPCKilledCondition(134));
                AddCondition(new LocalNPCKilledCondition(127));
                AddCondition(new LocalNPCKilledCondition(262));
                AddCondition(new LocalNPCKilledCondition(245));
                AddCondition(new LocalNPCKilledCondition(439));
                AddCondition(new LocalNPCKilledCondition(398));
                AddCondition(new LocalNPCKilledCondition(370));
                break;
            case VanillaAchievementName.YOU_CAN_DO_IT:
                break;
            case VanillaAchievementName.MATCHING_ATTIRE:
                break;
            case VanillaAchievementName.DEFEAT_EMPRESS_OF_LIGHT:
                AddCondition(new LocalNPCKilledCondition(636));
                break;
            case VanillaAchievementName.DEFEAT_QUEEN_SLIME:
                AddCondition(new LocalNPCKilledCondition(657));
                break;
            case VanillaAchievementName.DEFEAT_DREADNAUTILUS:
                AddCondition(new LocalNPCKilledCondition(618));
                break;
            case VanillaAchievementName.DEFEAT_OLD_ONES_ARMY_TIER3:
                break;
            case VanillaAchievementName.GET_ZENITH:
                AddCondition(new LocalItemPickupCondition(4956));
                break;
            case VanillaAchievementName.GET_TERRASPARK_BOOTS:
                AddCondition(new LocalItemPickupCondition(5000));
                break;
            case VanillaAchievementName.FLY_A_KITE_ON_A_WINDY_DAY:
                break;
            case VanillaAchievementName.FOUND_GRAVEYARD:
                break;
            case VanillaAchievementName.GO_LAVA_FISHING:
                break;
            case VanillaAchievementName.TURN_GNOME_TO_STATUE:
                break;
            case VanillaAchievementName.TALK_TO_NPC_AT_MAX_HAPPINESS:
                break;
            case VanillaAchievementName.PET_THE_PET:
                break;
            case VanillaAchievementName.FIND_A_FAIRY:
                break;
            case VanillaAchievementName.THROW_A_PARTY:
                break;
            case VanillaAchievementName.DIE_TO_DEAD_MANS_CHEST:
                break;
            case VanillaAchievementName.DEFEAT_DEERCLOPS:
                AddCondition(new LocalNPCKilledCondition(668));
                break;
            case VanillaAchievementName.GET_GOLDEN_DELIGHT:
                AddCondition(new LocalNPCKilledCondition(4022));
                break;
            case VanillaAchievementName.DRINK_BOTTLED_WATER_WHILE_DROWNING:
                break;
            case VanillaAchievementName.GET_CELL_PHONE:
                AddCondition(new LocalItemPickupCondition(3124));
                break;
            case VanillaAchievementName.GET_ANKH_SHIELD:
                AddCondition(new LocalItemPickupCondition(1613));
                break;
            case VanillaAchievementName.GAIN_TORCH_GODS_FAVOR:
                break;
            case VanillaAchievementName.PLAY_ON_A_SPECIAL_SEED:
                break;
            case VanillaAchievementName.ALL_TOWN_SLIMES:
                break;
            case VanillaAchievementName.TRANSMUTE_ITEM:
                break;
            case VanillaAchievementName.PURIFY_ENTIRE_WORLD:
                break;
            case VanillaAchievementName.TO_INFINITY_AND_BEYOND:
                break;
        }
    }
}