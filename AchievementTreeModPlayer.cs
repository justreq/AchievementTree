using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AchievementTree;
public class AchievementTreeModPlayer : ModPlayer
{
    public List<LocalAchievement> LocalAchievements = [];

    public LocalAchievement FindAchievement(string name) => LocalAchievements.FirstOrDefault(e => e.name == name);


    public override void Initialize()
    {
        LocalAchievements = [];
        Main.Achievements.CreateAchievementsList().Select(e => e.Name).ToList().ForEach(e => LocalAchievements.Add(new(e)));
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(LocalAchievements), LocalAchievements);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet(nameof(LocalAchievements), out List<LocalAchievement> localAchievements)) LocalAchievements = localAchievements;
    }
}

/*public class ItemConditions : GlobalItem
{
    readonly List<int> Woods = [9, 619, 2504, 620, 2503, 2260, 621, 911, 1729, 5215];
    readonly List<int> Hammers = [2775, 2746, 5283, 3505, 654, 3517, 7, 3493, 2780, 1513, 2516, 660, 3481, 657, 922, 3511, 2785, 3499, 3487, 196, 367, 104, 797, 2320, 787, 1234, 1262, 3465, 204, 217, 1507, 3524, 3522, 3525, 3523, 4317, 1305];

    public override bool OnPickup(Item item, Player player)
    {
        AchievementTreeModPlayer modPlayer = player.GetModPlayer<AchievementTreeModPlayer>();

        if (Woods.Contains(item.type)) modPlayer.ToggleAchievement(AchievementName.TIMBER, true);
        if (Hammers.Contains(item.type)) modPlayer.ToggleAchievement(AchievementName.OBTAIN_HAMMER, true);
        if (new List<int> { 35, 716 }.Contains(item.type)) modPlayer.ToggleAchievement(AchievementName.HEAVY_METAL, true);

        return base.OnPickup(item, player);
    }

    public override void OnCreated(Item item, ItemCreationContext context)
    {
        AchievementTreeModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>();

        if (context is RecipeItemCreationContext)
        {
            if (ItemID.Sets.Workbenches.Contains((short)item.type)) modPlayer.ToggleAchievement(AchievementName.BENCHED, true);
        }

        base.OnCreated(item, context);
    }

    public override bool? UseItem(Item item, Player player)
    {
        AchievementTreeModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>();

        if (item.type == ItemID.ManaCrystal) modPlayer.ToggleAchievement(AchievementName.STAR_POWER, true);

        return base.UseItem(item, player);
    }
}

public class TileConditions : GlobalTile
{
    readonly List<int> Ores = [7, 6, 9, 8, 166, 167, 168, 169, 22, 204, 58, 107, 108, 111, 221, 222, 223, 211];

    public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (!fail)
        {
            // if (Ores.Contains(type))
            // if (type == TileID.Heart)
        }

        base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
    }
}*/