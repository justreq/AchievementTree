using Microsoft.Xna.Framework;
using Terraria.Achievements;
using Terraria.ModLoader.IO;

namespace AchievementTree.Utilities;
public class LocalAchievementSerializer : TagSerializer<LocalAchievement, TagCompound>
{
    public override TagCompound Serialize(LocalAchievement value) => new()
    {
        [nameof(value.name)] = value.name,
        [nameof(value.friendlyName)] = value.friendlyName,
        [nameof(value.description)] = value.description,
        [nameof(value.category)] = (int)value.category,
        [nameof(value.icons)] = value.icons,
    };

    public override LocalAchievement Deserialize(TagCompound tag) => new(tag.GetString("name"), tag.GetString("friendlyName"), tag.GetString("description"), (AchievementCategory)tag.GetInt("category"), tag.Get<LocalAchievementTexture>("icons"));
}

public class LocalAchievementTextureSerializer : TagSerializer<LocalAchievementTexture, TagCompound>
{
    public override TagCompound Serialize(LocalAchievementTexture value) => new()
    {
        [nameof(value.TexturePath)] = value.TexturePath,
        [nameof(value.CompleteFrame)] = value.CompleteFrame,
        [nameof(value.IncompleteFrame)] = value.IncompleteFrame,
    };

    public override LocalAchievementTexture Deserialize(TagCompound tag) => new(tag.GetString("TexturePath"), tag.Get<Rectangle>("CompleteFrame"), tag.Get<Rectangle>("IncompleteFrame"));
}