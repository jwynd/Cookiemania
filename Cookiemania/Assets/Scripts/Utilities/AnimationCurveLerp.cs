using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General_Utilities
{
    public static class AnimationCurveLerp
    {
        public static float Interpolate(AnimationCurve curve, float from, float to, float t)
        {
            return from + curve.Evaluate(t) * (to - from);
        }

        public static Vector3 Interpolate(AnimationCurve curve, Vector3 from, Vector3 to, float t)
        {
            return from + curve.Evaluate(t) * (to - from);
        }
    }
}

