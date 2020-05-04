using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General_Utilities
{
    public static class ListExtensions
    {
        public static T Pop<T>(this IList<T> list)
        {
            if (list.Count < 1)
            {
                return default(T);
            }
            T t = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return t;
        }

        public static T PopFront<T>(this IList<T> list)
        {
            if (list.Count < 1)
            {
                return default(T);
            }
            T t = list[0];
            list.RemoveAt(0);
            return t;
        }

        public static T PopRandom<T>(this IList<T> list)
        {
            int ran = Random.Range(0, list.Count - 1);
            T t = list[ran];
            list.RemoveAt(ran);
            return t;
        }
    }
}
