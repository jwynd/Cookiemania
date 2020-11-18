using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Parsing_Utilities;

namespace Tracking
{

    public static class LocationUtils
    {
        // pretty important to not change scene names randomly btw
        public const string JumperSceneName = "Jumper";
        public const string SpaceSceneName = "Spacemini";

        public class UnityEvent2Locales : UnityEvent<Locale, Locale>
        {

        }

        public static readonly HashSet<Locale> AllLocales = new HashSet<Locale>()
        {   Locale.AnalyticsTab, Locale.Any, Locale.Desktop,
            Locale.EmailTab, Locale.JumpingMinigame, Locale.Minigame,
            Locale.SpaceMinigame, Locale.MinigameTab, Locale.WebsiteTab };

        public static readonly HashSet<Locale> MinigameLocales = new HashSet<Locale>()
        {   Locale.Any, Locale.JumpingMinigame, Locale.Minigame,
            Locale.SpaceMinigame };

        public static readonly HashSet<Locale> DesktopLocales = new HashSet<Locale>()
        {   Locale.AnalyticsTab, Locale.Any, Locale.Desktop,
            Locale.EmailTab, Locale.MinigameTab, Locale.WebsiteTab };

        public static HashSet<Locale> LocaleAlias(this Locale locale)
        {
            if (locale == Locale.Desktop)
            {
                return DesktopLocales;
            }
            if (locale == Locale.Minigame)
            {
                return MinigameLocales;
            }
            if (locale == Locale.Any)
            {
                return AllLocales;
            }
            return new HashSet<Locale>() { locale };
        }

        public static bool IsDesktop(this Locale location)
        {
            return (location == Locale.EmailTab ||
                location == Locale.AnalyticsTab ||
                location == Locale.WebsiteTab ||
                location == Locale.MinigameTab ||
                location == Locale.Desktop ||
                location == Locale.Any);
        }

        public static bool IsGame(this Locale location)
        {
            return (location == Locale.JumpingMinigame ||
                location == Locale.Minigame ||
                location == Locale.SpaceMinigame ||
                location == Locale.Any);
        }

        public static bool IsAny(this Locale location)
        {
            return true;
        }
    }
}
