using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutofillKeystorePassword : MonoBehaviour
{
    private const string k_KeystorePasswordPath = "Assets/_Sandbox/KeystorePassword/KeystorePassword.txt";

    [InitializeOnLoadMethod]
    private static void OnProjectLoadedInEditor()
    {
        if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass) || string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
        {
            // Load keystore password
            var passTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(k_KeystorePasswordPath);
            if (passTextAsset == null)
                return;
            PlayerSettings.Android.keystorePass = passTextAsset.text;
            PlayerSettings.Android.keyaliasPass = passTextAsset.text;
        }
    }
}