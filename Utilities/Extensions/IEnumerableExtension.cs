using System;
using System.Collections;
using System.Collections.Generic;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace AchievementTree.Utilities.Extensions;
public static class IEnumerableExtension
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T item in collection) action(item);
    }
}