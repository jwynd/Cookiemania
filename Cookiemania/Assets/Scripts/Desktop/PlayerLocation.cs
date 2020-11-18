using System;
using UnityEngine;
using UnityEngine.Events;

using static Tracking.LocationUtils;
using static Parsing_Utilities;

namespace Tracking
{
    public class PlayerLocation
    {
        public UnityEvent<Locale, Locale> Updated = new UnityEvent2Locales();

        private Locale current = Locale.WebsiteTab;
        public Locale Current
        {
            get
            {
                return current;
            }
            set
            {
                Previous = current;
                current = value;
                Updated?.Invoke(Previous, current);
            }
        }
        public Locale Previous { get; private set; } = Locale.WebsiteTab;
    }
}

