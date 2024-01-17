using AchievementTree.Utilities.Extensions;
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
    public List<LocalVanillaAchievement> LocalVanillaAchievements = [];
    public List<LocalModdedAchievement> LocalModdedAchievements = [];

    public LocalAchievement FindAchievement(string name) => LocalAchievements.FirstOrDefault(e => e.name == name);

    public override void Initialize()
    {
        LocalAchievements = [];
        LocalVanillaAchievements = [];
        LocalModdedAchievements = [];

        Main.Achievements.CreateAchievementsList().Select(e => e.Name).ForEach(e => LocalVanillaAchievements.Add(new LocalVanillaAchievement(e)));
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(LocalVanillaAchievements), LocalVanillaAchievements);
        tag.Add(nameof(LocalModdedAchievements), LocalModdedAchievements);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet(nameof(LocalVanillaAchievements), out List<LocalVanillaAchievement> localVanillaAchievements)) LocalVanillaAchievements = localVanillaAchievements;
        if (tag.TryGet(nameof(LocalModdedAchievements), out List<LocalModdedAchievement> localModdedAchievements)) LocalModdedAchievements = localModdedAchievements;

        LocalAchievements = [.. LocalVanillaAchievements, .. LocalModdedAchievements];
    }

    public override void PostUpdate()
    {
        LocalAchievements.Where(x => x.conditions.Any(y => y is LocalItemPickupCondition)).ForEach(e =>
        {
            e.conditions.ForEach(c =>
            {
                if (((LocalItemPickupCondition)c).types.Any(i => Player.HasItem(i))) c.Meet();
            });
        });
    }
}

public class ItemConditions : GlobalItem
{
    AchievementTreeModPlayer ModPlayer => Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>();

    public override void OnCreated(Item item, ItemCreationContext context)
    {
        ModPlayer.LocalAchievements.Where(x => x.conditions.Any(y => y is LocalItemCraftCondition)).ForEach(e =>
        {
            e.conditions.ForEach(c =>
            {
                if (((LocalItemCraftCondition)c).types.Contains(item.type)) c.Meet();
            });
        });

        base.OnCreated(item, context);
    }
}

public class NPCConditions : GlobalNPC
{
    AchievementTreeModPlayer ModPlayer => Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>();

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        if (npc.life <= 0)
        {
            ModPlayer.LocalAchievements.Where(x => x.conditions.Any(y => y is LocalNPCKilledCondition)).ForEach(e =>
            {
                e.conditions.ForEach(c =>
                {
                    if (((LocalNPCKilledCondition)c).types.Contains(npc.type)) c.Meet();
                });
            });
        }

        base.HitEffect(npc, hit);
    }
}