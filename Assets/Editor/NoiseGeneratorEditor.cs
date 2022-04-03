using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate terrain")){
            (target as NoiseGenerator).generateTerrain(); 
        };   
    }
}