using System;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        [SerializeField] public InternalLogger logger = new InternalLogger();

        [SerializeField] private List<AnalyticsService> services = new List<AnalyticsService>();
        public void LogEvent<T>(Func<AnalyticsService, T> castFunc, Action<T> callbackWithRequiredService, Action<AnalyticsService> callbackWithDefaultService)
        {
            foreach (var service in services)
            {
                //prevent logging this event to this analytics service (avoid duplication of events when these services are linked together on the dashboard)
                //all automatic analytics of tis analytics still works, just not the manual analytics event
                if(!service.EnableAnalyticsLogging)
                    continue;
                var castedService = castFunc(service);
                if (castedService != null)
                    callbackWithRequiredService?.Invoke(castedService);
                else
                    callbackWithDefaultService?.Invoke(service);
            }
        }
    }
}