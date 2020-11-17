using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tracking
{
    
    public static class LocationUtils
    {
        public class UnityEvent2Locales : UnityEvent<Locale, Locale>
        {

        }

        public enum Locale
        {
            Analytics,
            Spacegame,
            Jumpergame,
            Email,
            Website,
            MinigameHub,
        }

        public static bool IsDesktop(this Locale location)
        {
            return (location == Locale.Analytics ||
                location == Locale.Website ||
                location == Locale.MinigameHub ||
                location == Locale.Email);
        }

        public static bool IsGame(this Locale location)
        {
            return (location == Locale.Jumpergame ||
                location == Locale.Spacegame);
        }
    }
}
