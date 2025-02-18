using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LatteGames
{
    [CustomEditor(typeof(ObjectPlacer))]
    [CanEditMultipleObjects]
    public class ObjectPlacerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Placing!"))
            {
                (target as ObjectPlacer).UpdatePlacing();
            }
             if(GUILayout.Button("Clear!"))
            {
                (target as ObjectPlacer).Clear();
            }
        }
    }       
}