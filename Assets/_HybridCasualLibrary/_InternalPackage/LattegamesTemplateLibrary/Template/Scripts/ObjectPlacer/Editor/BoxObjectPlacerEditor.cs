using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace LatteGames
{
    [CustomEditor(typeof(BoxObjectPlacer))]
    [CanEditMultipleObjects]
    public class BoxObjectPlacerEditor : ObjectPlacerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var boxPlacer = target as BoxObjectPlacer;
            EditorGUILayout.HelpBox($"Object count {boxPlacer.GetObjectCount()}", MessageType.Info);
        }


        private BoxBoundsHandle boxBound = new BoxBoundsHandle();
        private void OnSceneGUI() {
            var boxPlacer = target as BoxObjectPlacer;
            boxBound.center = boxPlacer.transform.TransformPoint(boxPlacer.center);
            boxBound.size =  boxPlacer.transform.TransformVector(boxPlacer.size);
            EditorGUI.BeginChangeCheck();
                boxBound.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(boxPlacer, "Change Bounds");
                boxPlacer.center = boxPlacer.transform.InverseTransformPoint(boxBound.center);
                boxPlacer.size = boxPlacer.transform.InverseTransformVector(boxBound.size);
                EditorUtility.SetDirty(boxPlacer);
            }
        }
    }
}