using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private bool[] _tubeFoldouts = new bool[0];

    private static readonly Color HiddenColor = new Color(0.25f, 0.25f, 0.25f);
    private static GUIStyle _topLabelStyle;

    private static WaterColorPalette _palette;
    private static bool _paletteSearched;

    private static WaterColorPalette Palette
    {
        get
        {
            if (_palette != null) return _palette;
            if (_paletteSearched) return null;
            _paletteSearched = true;
            var guids = AssetDatabase.FindAssets("t:WaterColorPalette");
            if (guids.Length > 0)
            {
                _palette = AssetDatabase.LoadAssetAtPath<WaterColorPalette>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            else
            {
                Debug.LogWarning("[LevelDataEditor] No WaterColorPalette asset found in project. Water swatches will not show colors.");
            }
            return _palette;
        }
    }

    private static Color GetSwatchColor(WaterEntry entry)
    {
        if (entry.modifier != WaterModifier.None) return HiddenColor;
        var palette = Palette;
        return palette != null ? palette.Get(entry.color) : Color.grey;
    }

    public override void OnInspectorGUI()
    {
        var levelData = (LevelData)target;

        DrawPropertiesExcluding(serializedObject, "_tubes");
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Random", GUILayout.Height(28)))
        {
            Undo.RecordObject(levelData, "Generate Random Level");
            levelData.SetTubes(LevelDataBuilder.Build(levelData));
            _tubeFoldouts = new bool[levelData.Tubes.Count];
            for (var i = 0; i < _tubeFoldouts.Length; i++)
                _tubeFoldouts[i] = true;
            EditorUtility.SetDirty(levelData);
        }
        if (levelData.Tubes.Count > 0 && GUILayout.Button("Clear", GUILayout.Height(28), GUILayout.Width(60)))
        {
            Undo.RecordObject(levelData, "Clear Generated Level");
            levelData.Tubes.Clear();
            EditorUtility.SetDirty(levelData);
        }
        EditorGUILayout.EndHorizontal();

        if (levelData.Tubes.Count == 0) return;

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Generated Tubes", EditorStyles.boldLabel);

        if (_tubeFoldouts.Length != levelData.Tubes.Count)
        {
            var prev = _tubeFoldouts;
            _tubeFoldouts = new bool[levelData.Tubes.Count];
            for (var i = 0; i < Mathf.Min(prev.Length, _tubeFoldouts.Length); i++)
                _tubeFoldouts[i] = prev[i];
        }

        for (var i = 0; i < levelData.Tubes.Count; i++)
        {
            var tube = levelData.Tubes[i];
            var label = tube.waters.Count == 0
                ? $"Tube {i}  (empty)"
                : $"Tube {i}  ({tube.waters.Count} waters)";

            _tubeFoldouts[i] = EditorGUILayout.Foldout(_tubeFoldouts[i], label, true);
            if (!_tubeFoldouts[i]) continue;

            EditorGUI.indentLevel++;

            var newModifierTube = (TubeModifier)EditorGUILayout.EnumPopup("Tube Modifier", tube.modifier);
            if (newModifierTube != tube.modifier)
            {
                Undo.RecordObject(levelData, "Change Tube Modifier");
                tube.modifier = newModifierTube;
                EditorUtility.SetDirty(levelData);
            }

            if (tube.modifier == TubeModifier.Cloak)
            {
                var newTrigger = (WaterColor)EditorGUILayout.EnumPopup("Cloak Trigger Color", tube.cloakTriggerColor);
                if (newTrigger != tube.cloakTriggerColor)
                {
                    Undo.RecordObject(levelData, "Change Cloak Trigger Color");
                    tube.cloakTriggerColor = newTrigger;
                    EditorUtility.SetDirty(levelData);
                }
            }

            for (var j = tube.waters.Count - 1; j >= 0; j--)
            {
                var entry = tube.waters[j];
                var isTop = j == tube.waters.Count - 1;

                EditorGUILayout.BeginHorizontal();

                var prevColor = GUI.color;
                GUI.color = GetSwatchColor(entry);
                GUILayout.Label("■", GUILayout.Width(16));
                GUI.color = prevColor;

                EditorGUILayout.LabelField(entry.color.ToString(), GUILayout.Width(70));

                if (isTop)
                {
                    if (_topLabelStyle == null)
                        _topLabelStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.5f, 0.8f, 0.5f) } };
                    GUILayout.Label("top (always visible)", _topLabelStyle);
                }
                else
                {
                    var newModifier = (WaterModifier)EditorGUILayout.EnumPopup(entry.modifier, GUILayout.Width(80));
                    if (newModifier != entry.modifier)
                    {
                        Undo.RecordObject(levelData, "Change Water Modifier");
                        entry.modifier = newModifier;
                        tube.waters[j] = entry;
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
