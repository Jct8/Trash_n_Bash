using SheetCodes;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public class AddRowWindow : EditorWindow
    {
        private SheetPage sheetPage;
        private Action<SheetRow, int> callback;
        private string identifier;
        private string enumValue;
        private int index;
        private int insertIndex;

        private bool enumCaseManuallyChanged;

        private const int WIDTH = 400;

        public void Initialize(SheetPage sheetPage, Action<SheetRow, int> callback, int insertIndex)
        {
            this.sheetPage = sheetPage;
            this.callback = callback;
            this.insertIndex = insertIndex;
            index = sheetPage.GetAvailableRowIndex();
        }

        private void Awake()
        {
            titleContent.text = "Add Row";
            minSize = new Vector2(WIDTH, 115);
            enumCaseManuallyChanged = false;
            identifier = string.Empty;
            enumValue = string.Empty;
        }

        private void OnGUI()
        {
            if (focusedWindow != this)
                Focus();

            EditorGUILayout.BeginVertical();

            DrawIdentifier();
            DrawEnumCase();
            DrawIndex();

            string exception;
            bool containsExceptions = ContainsExceptions(out exception);
            if (containsExceptions)
                EditorGUILayout.HelpBox(exception, MessageType.Error);

            EditorGUI.BeginDisabledGroup(containsExceptions);
            DrawGenerateButton();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                minSize = new Vector2(WIDTH, lastRect.yMax - 4);
                maxSize = minSize;
            }
        }

        private void DrawIdentifier()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.ROW_NAME, GUILayout.Width(120));
            string newIdentifier = GUILayout.TextField(identifier).RemoveBreakingCharacter();
            EditorGUILayout.EndHorizontal();

            if (newIdentifier == identifier)
                return;

            identifier = newIdentifier;

            if (enumCaseManuallyChanged)
                return;

            enumValue = identifier.ConvertStringToEnumString();
        }

        private void DrawEnumCase()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.ROW_ENUMVALUE, GUILayout.Width(120));
            string newEnumCaseName = GUILayout.TextField(enumValue, GUI.skin.textField).RemoveBreakingCharacter().CreatePropertyName();
            EditorGUILayout.EndHorizontal();

            if (newEnumCaseName == enumValue)
                return;

            enumValue = newEnumCaseName;
            enumCaseManuallyChanged = true;
        }

        private void DrawIndex()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.ROW_INDEX, GUILayout.Width(120));
            index = EditorGUILayout.IntField(index, GUI.skin.textField);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGenerateButton()
        {
            if (!GUILayout.Button(Localization.GENERATE))
                return;

            SheetRow sheetRow = new SheetRow(sheetPage, index, identifier, enumValue, insertIndex);
            callback(sheetRow, insertIndex);
            Close();
        }

        private bool ContainsExceptions(out string error)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                error = Localization.ERROR_ROW_IDENTIFIER_EMPTY;
                return true;
            }

            if (identifier.ToLower() == "none")
            {
                error = Localization.ERROR_ROW_IDENTIFIER_MATCHES_NONE;
                return true;
            }

            if (sheetPage.rows.Any(i => i.identifier == identifier))
            {
                error = Localization.ERROR_ROW_IDENTIFIER_MATCH;
                return true;
            }

            if (string.IsNullOrEmpty(enumValue))
            {
                error = Localization.ERROR_ROW_ENUMVALUE_EMPTY;
                return true;
            }

            if (enumValue.ToLower() == "none")
            {
                error = Localization.ERROR_ROW_ENUMVALUE_MATCHES_NONE;
                return true;
            }

            if (sheetPage.rows.Any(i => i.enumValue == enumValue))
            {
                error = Localization.ERROR_ROW_ENUMVALUE_MATCH;
                return true;
            }

            if (index < 1)
            {
                error = Localization.ERROR_ROW_INDEX_LOWER_THAN_ONE;
                return true;
            }

            if (sheetPage.rows.Any(i => i.index == index))
            {
                error = Localization.ERROR_ROW_INDEX_MATCH;
                return true;
            }

            error = string.Empty;
            return false;
        }

        private void Update()
        {
            Repaint();
        }
    }
}