using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LatteGames
{
    [CustomEditor(typeof(MixerVolumeControl))]
    public class MixerVolumeControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mixerControl = target as MixerVolumeControl;
            mixerControl.IsBGMEnabled = EditorGUILayout.Toggle("BGM on: ", mixerControl.IsBGMEnabled);
            mixerControl.BGMVol = EditorGUILayout.Slider("BGM Vol", mixerControl.BGMVol, 0, 1);

            mixerControl.IsFXEnabled = EditorGUILayout.Toggle("FX on: ", mixerControl.IsFXEnabled);
            mixerControl.FXVol = EditorGUILayout.Slider("FX Vol", mixerControl.FXVol, 0, 1);
        }
    }
}