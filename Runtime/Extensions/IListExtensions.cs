using System;
using System.Collections.Generic;

namespace JoyKirito.ObjectPool
{
    public static class IListExtensions
    {
        public static int LastIndex<T>(this IReadOnlyList<T> list) => list.Count - 1;
        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> list) => list == null || list.Count < 1;
        public static bool ContainsIndex<T>(this IReadOnlyList<T> list, int index) => list.Count > index;

        public static bool ExistItem<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < list.Count; i++)
                if (predicate(list[i]))
                    return true;

            return false;
        }
    }
}