using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS && LatteGames_UnityAds_IOSSupport
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif
using Unity.Advertisement.IosSupport;
#endif

namespace LatteGames.Monetization
{
    [DefaultExecutionOrder(-150)]
    [OptionalDependency("Unity.Advertisement.IosSupport.ATTrackingStatusBinding", "LatteGames_UnityAds_IOSSupport")]
    public class ATTPermissionRequest : MonoBehaviour
    {
#if UNITY_IOS && LatteGames_UnityAds_IOSSupport

        private const string PrivacyUserTrackingDescriptionKey  = "NSUserTrackingUsageDescription";
        private const string PrivacyUserTrackingDescription     = "Your data will be used to provide you a better and personalized ad experience.";

        private void Awake() 
        {
            // Check the user's consent status.
            // If the status is undetermined, display the request request:
            if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) 
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
        }

#if UNITY_EDITOR
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            var rootDict = plist.root;
            rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://ttpsdk.info");

            if(!rootDict.values.ContainsKey(PrivacyUserTrackingDescriptionKey))
            {
                rootDict.SetString(PrivacyUserTrackingDescriptionKey, PrivacyUserTrackingDescription);
            }
            File.WriteAllText(plistPath, plist.WriteToString());
        }
#endif
#endif
    }
}