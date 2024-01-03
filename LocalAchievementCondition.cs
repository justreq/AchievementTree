namespace AchievementTree;
public class LocalCondition
{
    bool isMet;

    public LocalCondition() { }

    public void Meet()
    {
        if (!isMet)
        {
            isMet = true;
        }
    }
}

public class LocalTypeCondition(params int[] types) : LocalCondition()
{
    internal int[] types = types;
}

public class LocalItemCraftCondition(params int[] types) : LocalTypeCondition(types) { }

public class LocalItemPickupCondition(params int[] types) : LocalTypeCondition(types) { }

public class LocalNPCKilledCondition(params int[] types) : LocalTypeCondition(types) { }

public class LocalTileDestroyedCondition(params int[] types) : LocalTypeCondition(types) { }