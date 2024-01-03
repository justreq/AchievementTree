using Microsoft.Xna.Framework;
using Terraria.Achievements;
using Terraria.ModLoader.IO;

namespace AchievementTree.Utilities;

public class LocalModdedAchievementSerializer : TagSerializer<LocalModdedAchievement, TagCompound>
{
    public override TagCompound Serialize(LocalModdedAchievement value) => new()
    {
        [nameof(value.name)] = value.name,
        [nameof(value.friendlyName)] = value.friendlyName,
        [nameof(value.description)] = value.description,
        [nameof(value.category)] = (int)value.category,
        [nameof(value.icons)] = value.icons,
    };

    public override LocalModdedAchievement Deserialize(TagCompound tag) => new(tag.GetString("name"), tag.GetString("friendlyName"), tag.GetString("description"), (AchievementCategory)tag.GetInt("category"), tag.Get<LocalAchievementTexture>("icons"));
}

public class LocalVanillaAchievementSerializer : TagSerializer<LocalVanillaAchievement, TagCompound>
{
    public override TagCompound Serialize(LocalVanillaAchievement value) => new()
    {
        [nameof(value.name)] = value.name,
    };

    public override LocalVanillaAchievement Deserialize(TagCompound tag) => new(tag.GetString("name"));
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