using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;

namespace HierarchyDecorator
{
    public static class HierarchyGUI
    {
        private static GUIStyle TextStyle = new GUIStyle(EditorStyles.label);
        private static readonly Color DarkModeText = new Color(0.48f, 0.67f, 0.95f, 1f);
        private static readonly Color WhiteModeText = new Color(0.1f, 0.3f, 0.7f, 1f);

        public static void DrawHierarchyStyle(HierarchyStyle style, Rect styleRect, Rect labelRect, string label, bool removePrefix = true)
        {
            if (removePrefix)
            {
                label = label.Substring (style.prefix.Length).Trim ();
            }

            ModeOptions styleSetting = style.GetCurrentMode (EditorGUIUtility.isProSkin);

            EditorGUI.DrawRect (styleRect, styleSetting.backgroundColour);
            EditorGUI.LabelField (labelRect, style.FormatString(label), style.style);
        }

        public static void DrawStandardContent(Rect rect, GameObject instance)
        {
            Profiler.BeginSample("HierarchyGUI.DrawStandardContent");
            // Get prefab info

            var prefabType = PrefabUtility.GetPrefabAssetType(instance);
            bool isPrefab = prefabType != PrefabAssetType.NotAPrefab;
            
            GameObject prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(instance);
            bool isPrefabParent = prefabRoot == instance;

            // Get the content needed for the icon

            GUIContent content = GetStandardContent (rect, instance, prefabType, isPrefabParent);

            // Handle colours

            Color textColour = EditorStyles.label.normal.textColor;
            if (isPrefab)
            {
                if (prefabType == PrefabAssetType.MissingAsset)
                    textColour = new Color(255, 0.55f, 0.55f);
                else textColour = (EditorGUIUtility.isProSkin) ? DarkModeText : WhiteModeText;
            }

            if (Selection.Contains(instance) && prefabType != PrefabAssetType.MissingAsset)
            {
                textColour = Color.white;
            }

            TextStyle.normal.textColor = textColour;

            // Draw prefab context icon

            if (isPrefabParent)
            {
                DrawPrefabArrow(rect);
            }

            // Draw label

            DrawStandardLabel (rect, content, instance.name, TextStyle);

            // Add the small prefab indicator if required

            if (!isPrefab && PrefabUtility.IsAddedGameObjectOverride(instance))
            {
                EditorGUI.LabelField(rect, EditorGUIUtility.IconContent("PrefabOverlayAdded Icon"));
            }

            Profiler.EndSample();
        }

        private static void DrawStandardLabel(Rect rect, GUIContent icon, string label, GUIStyle style)
        {
            // Draw Label + Icon
            Vector2 originalIconSize = EditorGUIUtility.GetIconSize ();
            EditorGUIUtility.SetIconSize (Vector2.one * rect.height);
            {
                EditorGUI.LabelField (rect, icon, style);

                rect.x += 18f;
                rect.y--;

                EditorGUI.LabelField (rect, label, style);
            }
            EditorGUIUtility.SetIconSize (originalIconSize);
        }

        private static void DrawPrefabArrow(Rect rect)
        {
            Rect iconRect = rect;
            iconRect.x = rect.width + rect.x;
            iconRect.width = rect.height;

            GUI.DrawTexture (iconRect, EditorGUIUtility.IconContent ("tab_next").image, ScaleMode.ScaleToFit);
        }

        // Content Helpers

        public static GUIContent GetStandardContent(Rect rect, GameObject instance, PrefabAssetType prefabType, bool isPrefabParent)
        { 

            if (!isPrefabParent || prefabType == PrefabAssetType.NotAPrefab)
                return EditorGUIUtility.IconContent("GameObject Icon");

            if (prefabType == PrefabAssetType.Model)
                return EditorGUIUtility.IconContent("PrefabModel Icon");

            if (prefabType == PrefabAssetType.Variant)
                return EditorGUIUtility.IconContent("PrefabVariant Icon");


            return EditorGUIUtility.IconContent("Prefab Icon"); 
        }

        public static Color GetTwoToneColour(Rect selectionRect)
        {
            bool isEvenRow = selectionRect.y % 32 != 0;

            if (EditorGUIUtility.isProSkin)
            {
                return isEvenRow ? Constants.DarkModeEvenColor : Constants.DarkModeOddColor;
            }
            else
            {
                return isEvenRow ? Constants.LightModeEvenColor : Constants.LightModeOddColor;
            }
        }

        // Version GUI Helpers

        public static void Space(float width = 9f)
        {
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.Space (width);
#else
            GUILayout.Space (width);
#endif
        }
    }
}
