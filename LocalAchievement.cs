using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Achievements;

namespace AchievementTree;

public record struct LocalAchievementTexture(string TexturePath, Rectangle CompleteFrame, Rectangle IncompleteFrame);

public class LocalAchievement
{
    public bool isCompleted = false;

    internal string name;
    internal string friendlyName;
    internal string description;
    internal AchievementCategory category;
    internal LocalAchievementTexture icons;

    internal bool modded;

    /// <summary>
    /// Local modded achievement
    /// </summary>
    /// <param name="name">Internal achievement name</param>
    /// <param name="friendlyName">Ingame achievement name</param>
    /// <param name="description">Ingame achievement description</param>
    /// <param name="category">Achievement category</param>
    /// <param name="icon">Achievement icon (thumbnail image)</param>
    public LocalAchievement(string name, string friendlyName, string description, AchievementCategory category, LocalAchievementTexture icons)
    {
        this.name = name;
        this.friendlyName = friendlyName;
        this.description = description;
        this.category = category;
        this.icons = icons;

        modded = true;
    }

    /// <summary>
    /// Local vanilla achievement
    /// </summary>
    /// <param name="name">Internal achievement name</param>
    public LocalAchievement(string name)
    {
        Achievement sourceAchievement = Main.Achievements.GetAchievement(name);
        int iconIndex = Main.Achievements.GetIconIndex(name);
        Rectangle completeFrame = new(iconIndex % 8 * 66, iconIndex / 8 * 66, 64, 64);
        Rectangle incompleteFrame = new(completeFrame.X + 66 * 8, completeFrame.Y, 64, 64);

        this.name = name;
        friendlyName = sourceAchievement.FriendlyName.ToString();
        description = sourceAchievement.Description.ToString();
        category = sourceAchievement.Category;
        icons = new("Images/UI/Achievements", completeFrame, incompleteFrame);

        modded = false;
    }

    public void Complete()
    {
        if (!isCompleted)
        {
            isCompleted = true;
        }
    }
}