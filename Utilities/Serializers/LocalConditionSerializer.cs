using Terraria.ModLoader.IO;

namespace AchievementTree.Utilities.Serializers;

public class LocalItemCraftConditionSerializer : TagSerializer<LocalItemCraftCondition, TagCompound>
{
    public override TagCompound Serialize(LocalItemCraftCondition value) => new()
    {
        [nameof(value.achievement)] = value.achievement,
        [nameof(value.types)] = value.types,
        [nameof(value.isMet)] = value.isMet,
    };

    public override LocalItemCraftCondition Deserialize(TagCompound tag) => new(tag.Get<LocalAchievement>("achievement"), tag.GetIntArray("types"));
}

public class LocalItemPickupConditionSerializer : TagSerializer<LocalItemPickupCondition, TagCompound>
{
    public override TagCompound Serialize(LocalItemPickupCondition value) => new()
    {
        [nameof(value.achievement)] = value.achievement,
        [nameof(value.types)] = value.types,
        [nameof(value.isMet)] = value.isMet,
    };

    public override LocalItemPickupCondition Deserialize(TagCompound tag) => new(tag.Get<LocalAchievement>("achievement"), tag.GetIntArray("types"));
}

public class LocalNPCKilledConditionSerializer : TagSerializer<LocalNPCKilledCondition, TagCompound>
{
    public override TagCompound Serialize(LocalNPCKilledCondition value) => new()
    {
        [nameof(value.achievement)] = value.achievement,
        [nameof(value.types)] = value.types,
        [nameof(value.isMet)] = value.isMet,
    };

    public override LocalNPCKilledCondition Deserialize(TagCompound tag) => new(tag.Get<LocalAchievement>("achievement"), tag.GetIntArray("types"));
}

public class LocalTileDestroyedConditionSerializer : TagSerializer<LocalTileDestroyedCondition, TagCompound>
{
    public override TagCompound Serialize(LocalTileDestroyedCondition value) => new()
    {
        [nameof(value.achievement)] = value.achievement,
        [nameof(value.types)] = value.types,
        [nameof(value.isMet)] = value.isMet,
    };

    public override LocalTileDestroyedCondition Deserialize(TagCompound tag) => new(tag.Get<LocalAchievement>("achievement"), tag.GetIntArray("types"));
}