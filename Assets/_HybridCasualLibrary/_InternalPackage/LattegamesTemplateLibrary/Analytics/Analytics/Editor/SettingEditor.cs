using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LatteGames.EditorUtil;
namespace LatteGames.Analytics
{
    [CustomEditor(typeof(Setting))]
    public class SettingEditor : Editor
    {
        private Editor _editor;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            var preset = serializedObject.FindProperty("preset");
            CreateCachedEditor(preset.objectReferenceValue, null, ref _editor);
            if(_editor != null)
            {
                _editor.OnInspectorGUI();
                var presetSetting = (preset.objectReferenceValue as System.Object as PresetSetting);
                presetSetting.DrawCustomInspector();
            }
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("LatteGames/AnalyticsSetting")]
        private static void SelectAnalyticsSetting(){
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(PackageRootFolderDetection.GetPath()+ "Analytics/Analytics/Editor/Setting.asset", typeof(Setting));
        }
    }
}
