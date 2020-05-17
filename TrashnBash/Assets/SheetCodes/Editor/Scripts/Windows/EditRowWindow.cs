using SheetCodes;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public class EditRowWindow : EditorWindow
    {
        private DataSheet dataSheet;
        private SheetPage sheetPage;
        private SheetRow sheetRow;
        private string identifier;
        private string enumValue;
        private int index;
        private const int WIDTH = 400;

        public void Initialize(DataSheet dataSheet, SheetPage sheetPage, SheetRow sheetRow)
        {
            this.dataSheet = dataSheet;
            this.sheetPage = sheetPage;
            this.sheetRow = sheetRow;
            identifier = sheetRow.identifier;
            enumValue = sheetRow.enumValue;
            index = sheetRow.index;
        }

        private void Awake()
        {
            titleContent.text = "Edit Row";
            minSize = new Vector2(WIDTH, 150);
            maxSize = minSize;
        }

        private void OnGUI()
        {
            if (focusedWindow != this)
                Focus();

            EditorGUILayout.BeginVertical();

            DrawIdentifier();
            DrawEnumValue();
            DrawIndex();

            string exception;
            bool containsExceptions = ContainsExceptions(out exception);
            if (containsExceptions)
                EditorGUILayout.HelpBox(exception, MessageType.Error);

            EditorGUI.BeginDisabledGroup(containsExceptions);
            DrawSaveButton();
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
            identifier = GUILayout.TextField(identifier).RemoveBreakingCharacter();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEnumValue()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.ROW_ENUMVALUE, GUILayout.Width(120));
            enumValue = GUILayout.TextField(enumValue, GUI.skin.textField).RemoveBreakingCharacter().CreatePropertyName();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawIndex()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.ROW_INDEX, GUILayout.Width(120));
            index = EditorGUILayout.IntField(index, GUI.skin.textField);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSaveButton()
        {
            if (!GUILayout.Button(Localization.SAVE))
                return;

            sheetRow.ChangeIdentifier(dataSheet, sheetPage, identifier);
            sheetRow.identifier = identifier;
            sheetRow.enumValue = enumValue;
            sheetRow.index = index;

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

            if (sheetPage.rows.Any(i => i != sheetRow && i.identifier == identifier))
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

            if (sheetPage.rows.Any(i => i != sheetRow && i.enumValue == enumValue))
            {
                error = Localization.ERROR_ROW_ENUMVALUE_MATCH;
                return true;
            }

            if (index < 1)
            {
                error = Localization.ERROR_ROW_INDEX_LOWER_THAN_ONE;
                return true;
            }

            if (sheetPage.rows.Any(i => i != sheetRow && i.index == index))
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