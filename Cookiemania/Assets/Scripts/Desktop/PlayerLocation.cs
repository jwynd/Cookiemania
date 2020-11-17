﻿using System;
using UnityEngine;
using UnityEngine.Events;

using static Tracking.LocationUtils;

namespace Tracking
{
    public class PlayerLocation : MonoBehaviour
    {
        public UnityEvent<Locale, Locale> Updated = new UnityEvent2Locales();

        private Locale current = Locale.Website;
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
        public Locale Previous { get; private set; } = Locale.Website;
    }
}

