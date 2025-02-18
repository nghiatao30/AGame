using UnityEngine;
using UnityEditor;
#if LatteGames_Adjust
using com.adjust.sdk;
#endif

namespace LatteGames.Analytics
{
     public static class AdjustSetup
    {
        public static void GUI(string appToken)
        {
#if LatteGames_Adjust
            var adjustGameObject = GameObject.FindObjectOfType<Adjust>();
            if(adjustGameObject == null)
                return;
            if (string.IsNullOrEmpty(adjustGameObject.appToken))
            {
                EditorGUILayout.HelpBox("Adjust Token is missing!", MessageType.Warning);
            }
            if (adjustGameObject.environment != AdjustEnvironment.Production)
            {
                EditorGUILayout.HelpBox($"Adjust environment is set to be {adjustGameObject.environment} while it should be Production. Make sure this is not a mistake", MessageType.Warning);
            }
            if (adjustGameObject.startManually)
            {
                EditorGUILayout.HelpBox($"Adjust is set to launch manually. Make sure you have manual initialization or turn this off. (LatteGames normal implementation of Adjust Service will no longer init the sdk)", MessageType.Warning);
            }
            if (GUILayout.Button("Setup Adjust"))
            {


                if(adjustGameObject != null)
                {
                    adjustGameObject.appToken = appToken;
                    adjustGameObject.startManually = false;
                    adjustGameObject.environment = AdjustEnvironment.Production;
                    EditorUtility.SetDirty(adjustGameObject);
                }
            }
#endif
        }
    }
}