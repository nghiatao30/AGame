using System.Collections.Generic;
using UnityEngine;

#if LatteGames_Adjust
using com.adjust.sdk;
#endif

namespace LatteGames.Analytics
{
    [OptionalDependency("com.adjust.sdk.Adjust", "LatteGames_Adjust")]
    public class AdjustAnalyticsService : AnalyticsService
    {        
#if LatteGames_Adjust
        private void Awake() {
            Adjust.requestTrackingAuthorizationWithCompletionHandler((status) =>
            {
                switch (status)
                {
                    case 0:
                        // ATTrackingManagerAuthorizationStatusNotDetermined case
                        break;
                    case 1:
                        // ATTrackingManagerAuthorizationStatusRestricted case
                        break;
                    case 2:
                        // ATTrackingManagerAuthorizationStatusDenied case
                        break;
                    case 3:
                        // ATTrackingManagerAuthorizationStatusAuthorized case
                        break;
                }
            });
        }

//         [SerializeField]
//         private string _IsoToken, _AndroidToken;
//         void Start()
//         {
//             // import this package into the project:
//             // https://github.com/adjust/unity_sdk/releases

//     #if UNITY_IOS
//             /* Mandatory - set your iOS app token here */
//             InitAdjust(_IsoToken);
//     #elif UNITY_ANDROID
//             /* Mandatory - set your Android app token here */
//             InitAdjust(_AndroidToken);
//     #endif
//         }


//         private void InitAdjust(string adjustAppToken)
//         {
//             var adjustConfig = new AdjustConfig(
//                 adjustAppToken,
//                 AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
//                 true
//             );
//             adjustConfig.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
//             adjustConfig.setSendInBackground(true);
//             new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename

//             // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters

//             //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
//             //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
//             //});

//             Adjust.start(adjustConfig);

//         }
#endif

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
            logger.Log("Not actually logging since log event required setup on dashboard");
        }
    }   
}