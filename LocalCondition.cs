using System.Linq;

namespace AchievementTree;
public abstract class LocalCondition(LocalAchievement achievement)
{
    public bool isMet;

    internal LocalAchievement achievement = achievement;

    public void Meet()
    {
        if (!isMet)
        {
            isMet = true;

            if (achievement.conditions.All(e => e.isMet)) achievement.Complete();
        }
    }
}

public abstract class LocalTypeCondition(LocalAchievement achievement, params int[] types) : LocalCondition(achievement)
{
    internal int[] types = types;
}

public class LocalItemCraftCondition(LocalAchievement achievement, params int[] types) : LocalTypeCondition(achievement, types) { }

public class LocalItemPickupCondition(LocalAchievement achievement, params int[] types) : LocalTypeCondition(achievement, types) { }

public class LocalNPCKilledCondition(LocalAchievement achievement, params int[] types) : LocalTypeCondition(achievement, types) { }

public class LocalTileDestroyedCondition(LocalAchievement achievement, params int[] types) : LocalTypeCondition(achievement, types) { }