using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LatteGames.GameManagement
{
    public class GenerateSceneNameEnum
    {
        private static void GenerateSceneName()
        {
            // Get the list of scenes in the Build Settings.
            EditorBuildSettingsScene[] buildSettingScenes = EditorBuildSettings.scenes;

            // Create list enum values
            List<string> sceneNames = new List<string>();
            for (int i = 0; i < buildSettingScenes.Length; i++)
            {
                var pathSplit = buildSettingScenes[i].path.Split("/");
                var sceneName = pathSplit[^1].Split(".")[0];
                sceneNames.Add(sceneName);
            }

            // Create a new enum file.
            string fileName = "SceneNameEnum";
            string absolutePath = EditorUtils.FindScriptByName(nameof(SceneManager)).Replace($"{nameof(SceneManager)}.cs", $"/GameConfigs/{fileName}.cs");
            string relativeFilePath = absolutePath.ToRelativePath();
            string summary = "Define all scenes in Build Settings.";
            string enumName = "SceneName";
            EditorUtils.GenerateEnum(relativeFilePath, sceneNames, summary, enumName);
        }

        [MenuItem("LatteGames/SceneManager/GenerateSceneNameEnum")]
        private static void GenerateEnum()
        {
            GenerateSceneName();
        }
    }
}