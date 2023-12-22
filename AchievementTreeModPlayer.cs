using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AchievementTree;
public class AchievementTreeModPlayer : ModPlayer
{
    public List<bool> IsAchievementUnlocked = new(new bool[Main.Achievements.CreateAchievementsList().Count]);

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(IsAchievementUnlocked), IsAchievementUnlocked);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet(nameof(IsAchievementUnlocked), out List<bool> isAchievementUnlocked)) IsAchievementUnlocked = isAchievementUnlocked;
    }
}