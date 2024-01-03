using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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

        Main.Achievements.CreateAchievementsList().Select(e => e.Name).ToList().ForEach(e => LocalAchievements.Add(new LocalVanillaAchievement(e)));
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(LocalAchievements), LocalAchievements);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet(nameof(LocalAchievements), out List<LocalAchievement> localAchievements)) LocalAchievements = localAchievements;
    }

    public override bool OnPickup(Item item)
    {
        LocalAchievements.ForEach(e =>
        {
            Main.NewText(e.conditions.Count);
            if (e.conditions.Count != 0) Main.NewText(e.conditions.First() is LocalItemPickupCondition);
        });

        Main.NewText(LocalAchievements.Where(x => x.conditions.Any(y => y is LocalItemPickupCondition)).ToList().Count);
        return base.OnPickup(item);
    }
}

public class ItemConditions : GlobalItem
{
    public override void OnCreated(Item item, ItemCreationContext context)
    {
        AchievementTreeModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>();

        base.OnCreated(item, context);
    }
}