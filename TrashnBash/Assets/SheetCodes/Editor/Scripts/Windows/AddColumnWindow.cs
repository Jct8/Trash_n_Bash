using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SheetCodes;

namespace SheetCodesEditor
{
    public class AddColumnWindow : EditorWindow
    {
        private DataSheet dataSheet;
        private SheetPage sheetPage;
        private Action<SheetColumn, int> callback;
        private SheetDataType dataType;
        private string serializationName;
        private string propertyName;
        private bool isCollection;
        private string referenceSheet;
        private bool propertyManuallyChanged;
        private int insertIndex;

        private const int WIDTH = 400;

        private void Awake()
        {
            titleContent.text = "Add Column";
            minSize = new Vector2(WIDTH, 200);

            dataType = SheetDataType.Int;
            isCollection = false;
            referenceSheet = default;
            propertyManuallyChanged = false;

            serializationName = string.Empty;
            propertyName = string.Empty;
            
        }
        
        public void Initialize(DataSheet dataSheet, SheetPage sheetPage, Action<SheetColumn, int> callback, int insertIndex)
        {
            this.dataSheet = dataSheet;
            this.sheetPage = sheetPage;
            this.callback = callback;
            this.insertIndex = insertIndex;
            string[] options = dataSheet.datasheetPages.Select(i => i.sheetName).ToArray();
            referenceSheet = options[0];
        }

        private void OnGUI()
        {
            if (focusedWindow != this)
                Focus();

            EditorGUILayout.BeginVertical();

            DrawSerializationName();
            DrawPropertyName();
            DrawIsCollection();
            DrawDataType();

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

        private void DrawSerializationName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_COLUMN_NAME, GUILayout.Width(120));
            string newSerializationName = GUILayout.TextField(serializationName).RemoveBreakingCharacter();
            EditorGUILayout.EndHorizontal();

            if (newSerializationName == serializationName)
                return;

            serializationName = newSerializationName;
            if (propertyManuallyChanged)
                return;

            propertyName = serializationName.ConvertStringToEnumString();
        }

        private void DrawPropertyName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_PROPERTY_NAME, GUILayout.Width(120));
            string newPropertyName = GUILayout.TextField(propertyName).RemoveBreakingCharacter().CreatePropertyName();
            EditorGUILayout.EndHorizontal();

            if (newPropertyName == propertyName)
                return;

            propertyName = newPropertyName;
            propertyManuallyChanged = true;
        }

        private void DrawIsCollection()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_IS_ARRAY, GUILayout.Width(120));
            isCollection = GUILayout.Toggle(isCollection, "");
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDataType()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_DATA_TYPE, GUILayout.Width(120));
            dataType = (SheetDataType)EditorGUILayout.EnumPopup(dataType);
            EditorGUILayout.EndHorizontal();

            if (dataType == SheetDataType.Reference)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_REFERENCE_SHEET, GUILayout.Width(120));
                string[] options = dataSheet.datasheetPages.Select(i => i.sheetName).ToArray();
                int currentOption = Array.IndexOf(options, referenceSheet);
                int selectedOption = EditorGUILayout.Popup(currentOption, options);
                referenceSheet = options[selectedOption];
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawGenerateButton()
        {
            if (!GUILayout.Button(Localization.GENERATE))
                return;

            string reference = dataType == SheetDataType.Reference ? referenceSheet : "";

            SheetColumn sheetColumn = new SheetColumn(sheetPage, serializationName, propertyName, dataType, reference, isCollection, insertIndex);
            callback(sheetColumn, insertIndex);
            Close();
        }

        private bool ContainsExceptions(out string error)
        {
            if (string.IsNullOrEmpty(serializationName))
            {
                error = Localization.ERROR_COLUMN_SERIALIZATIONNAME_EMPTY;
                return true;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                error = Localization.ERROR_COLUMN_PROPERTYNAME_EMPTY;
                return true;
            }

            if (propertyName == "Identifier")
            {
                error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCHES_IDENTIFIER;
                return true;
            }

            if (sheetPage.columns.Any(i => i.serializationName == serializationName))
            {
                error = Localization.ERROR_COLUMN_SERIALIZATIONNAME_MATCH;
                return true;
            }

            foreach (SheetColumn otherColumn in sheetPage.columns)
            {
                if (otherColumn.propertyName == propertyName)
                {
                    error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCH;
                    return true;
                }

                if (otherColumn.dataType == SheetDataType.Reference)
                {
                    if (otherColumn.isCollection && otherColumn.propertyName + "Records" == propertyName)
                    {
                        error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCH_EXTRA;
                        return true;
                    }
                    else if (!otherColumn.isCollection && otherColumn.propertyName + "Record" == propertyName)
                    {
                        error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCH_EXTRA;
                        return true;
                    }
                }
            }

            error = string.Empty;
            return false;
        }
    }
}