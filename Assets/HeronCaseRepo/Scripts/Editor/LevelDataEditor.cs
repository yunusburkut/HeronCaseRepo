using HeronCaseRepo.Scripts.Data;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private bool[] _tubeFoldouts = new bool[0];

    private static readonly Color HiddenColor = new Color(0.25f, 0.25f, 0.25f);

    private static WaterColorPalette _palette;

    private static WaterColorPalette Palette
    {
        get
        {
            if (_palette != null) return _palette;
            var guids = AssetDatabase.FindAssets("t:WaterColorPalette");
            if (guids.Length > 0)
                _palette = AssetDatabase.LoadAssetAtPath<WaterColorPalette>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return _palette;
        }
    }

    private static Color GetSwatchColor(WaterEntry entry)
    {
        if (entry.modifier != WaterModifier.None) return HiddenColor;
        var palette = Palette;
        if (palette != null) return palette.Get(entry.color);
        return Color.grey;
    }

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
            levelData.tubes = LevelDataBuilder.Build(levelData);
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
