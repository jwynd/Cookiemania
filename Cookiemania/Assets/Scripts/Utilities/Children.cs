using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General_Utilities
{
    public static class Children
    {
        public static void SetActiveChildren(this Transform trans, bool active)
        {
            foreach (Transform child in trans)
            {
                child.gameObject.SetActive(active);
            }
        }
    }
}
