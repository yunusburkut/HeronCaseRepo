using System.Collections.Generic;
using HeronCaseRepo.Scripts.Data;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private bool[] _tubeFoldouts = new bool[0];

    private static readonly Dictionary<WaterColor, Color> ColorMap = new Dictionary<WaterColor, Color>
    {
        { WaterColor.Red,    new Color(0.9f, 0.2f, 0.2f) },
        { WaterColor.Blue,   new Color(0.2f, 0.4f, 0.9f) },
        { WaterColor.Green,  new Color(0.2f, 0.75f, 0.3f) },
        { WaterColor.Yellow, new Color(0.95f, 0.85f, 0.1f) },
        { WaterColor.Orange, new Color(0.95f, 0.55f, 0.1f) },
        { WaterColor.Purple, new Color(0.6f, 0.2f, 0.9f) },
        { WaterColor.Cyan,   new Color(0.1f, 0.85f, 0.9f) },
        { WaterColor.Pink,   new Color(0.95f, 0.4f, 0.7f) },
        { WaterColor.Brown,  new Color(0.55f, 0.3f, 0.1f) },
        { WaterColor.White,  new Color(0.9f, 0.9f, 0.9f) },
    };

    private static readonly Color HiddenColor = new Color(0.25f, 0.25f, 0.25f);

    public override void OnInspectorGUI()
    {
        var levelData = (LevelData)target;

        DrawPropertiesExcluding(serializedObject, "tubes");
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Random", GUILayout.Height(28)))
        {
            Undo.RecordObject(levelData, "Generate Random Level");
            levelData.tubes = LevelGenerator.BuildTubeData(levelData);
            _tubeFoldouts = new bool[levelData.tubes.Count];
            for (var i = 0; i < _tubeFoldouts.Length; i++) _tubeFoldouts[i] = true;
            EditorUtility.SetDirty(levelData);
        }
        if (levelData.tubes.Count > 0 && GUILayout.Button("Clear", GUILayout.Height(28), GUILayout.Width(60)))
        {
            Undo.RecordObject(levelData, "Clear Generated Level");
            levelData.tubes.Clear();
            EditorUtility.SetDirty(levelData);
        }
        EditorGUILayout.EndHorizontal();

        if (levelData.tubes.Count == 0) return;

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Generated Tubes", EditorStyles.boldLabel);

        if (_tubeFoldouts.Length != levelData.tubes.Count)
        {
            var prev = _tubeFoldouts;
            _tubeFoldouts = new bool[levelData.tubes.Count];
            for (var i = 0; i < Mathf.Min(prev.Length, _tubeFoldouts.Length); i++)
                _tubeFoldouts[i] = prev[i];
        }

        for (var i = 0; i < levelData.tubes.Count; i++)
        {
            var tube = levelData.tubes[i];
            var label = tube.waters.Count == 0
                ? $"Tube {i}  (empty)"
                : $"Tube {i}  ({tube.waters.Count} waters)";

            _tubeFoldouts[i] = EditorGUILayout.Foldout(_tubeFoldouts[i], label, true);
            if (!_tubeFoldouts[i]) continue;

            EditorGUI.indentLevel++;

            // top → bottom sırası (son index = top)
            for (var j = tube.waters.Count - 1; j >= 0; j--)
            {
                var entry = tube.waters[j];
                var isTop = j == tube.waters.Count - 1;

                EditorGUILayout.BeginHorizontal();

                var swatchColor = entry.modifier != WaterModifier.None
                    ? HiddenColor
                    : (ColorMap.TryGetValue(entry.color, out var c) ? c : Color.grey);

                var prevColor = GUI.color;
                GUI.color = swatchColor;
                GUILayout.Label("■", GUILayout.Width(16));
                GUI.color = prevColor;

                EditorGUILayout.LabelField(entry.color.ToString(), GUILayout.Width(70));

                if (isTop)
                {
                    var s = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.5f, 0.8f, 0.5f) } };
                    GUILayout.Label("top (always visible)", s);
                }
                else
                {
                    var newModifier = (WaterModifier)EditorGUILayout.EnumPopup(entry.modifier, GUILayout.Width(80));
                    if (newModifier != entry.modifier)
                    {
                        Undo.RecordObject(levelData, "Change Water Modifier");
                        entry.modifier = newModifier;
                        EditorUtility.SetDirty(levelData);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }
    }
}
