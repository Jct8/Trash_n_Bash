using SheetCodes;
using System;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    [CustomEditor(typeof(BaseModel<,>), true)]
    public class BaseModelEditor : Editor
    {
        SerializedProperty records;

        private void OnEnable()
        {
            records = serializedObject.FindProperty("records");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Localization.ONLY_ADJUSTABLE_WITH_TOOL);
            if (GUILayout.Button(Localization.OPEN_WITH_TOOL))
            {
                string modelName = serializedObject.targetObject.GetType().Name;
                string identifier = modelName.Remove(modelName.Length - "Model".Length, "Model".Length);
                DatasheetType datasheetType = (DatasheetType)Enum.Parse(typeof(DatasheetType), identifier);
                string sheetName = datasheetType.GetIdentifier();
                SheetCodesWindow.ShowWindow(sheetName);
            }
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(true);
            serializedObject.Update();
            for (int i = 0; i < records.arraySize; i++)
            {
                SerializedProperty record = records.GetArrayElementAtIndex(i);
                SerializedProperty property = record.FindPropertyRelative("identifier");
                string enumName = property.enumValueIndex >= 0 ? property.enumDisplayNames[property.enumValueIndex] : "";
                EditorGUILayout.PropertyField(records.GetArrayElementAtIndex(i), new GUIContent(enumName), true);
            }
            EditorGUI.BeginDisabledGroup(false);
        }
    }
}
