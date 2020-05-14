using SheetCodes;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public class AddSheetWindow : EditorWindow
    {
        private DataSheet dataSheet;
        private Action<SheetPage> callback;
        private SheetPage sheetPage;
        private int index;

        private const int WIDTH = 400;

        private void Awake()
        {
            titleContent.text = "Add Sheet";
            minSize = new Vector2(WIDTH, 135);
        }

        public void Initialize(DataSheet dataSheet, Action<SheetPage> callback)
        {
            this.dataSheet = dataSheet;
            this.callback = callback;

            sheetPage = new SheetPage();

            int highest = 0;
            if (dataSheet.datasheetPages.Length > 0)
                highest = dataSheet.datasheetPages.Max(i => i.index);
            sheetPage.index = highest + 1;
            sheetPage.sheetName = string.Empty;
        }

        private void OnGUI()
        {
            if (focusedWindow != this)
                Focus();

            EditorGUILayout.BeginVertical();
            DrawSheetName();

            CreateHorizontalLine(Localization.SHEET_MODELNAME, sheetPage.sheetModelNameUpperCase);
            CreateHorizontalLine(Localization.SHEET_RECORDNAME, sheetPage.sheetRecordName);
            CreateHorizontalLine(Localization.SHEET_IDENTIFIERNAME, sheetPage.sheetIdentifierName);

            string error;
            bool hasError = TryGetMissingRequirement(out error);
            if (hasError)
                EditorGUILayout.HelpBox(error, MessageType.Error);

            EditorGUI.BeginDisabledGroup(hasError);
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

        private void DrawSheetName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.SHEET_SHEETNAME, GUILayout.Width(120));
            sheetPage.sheetName = GUILayout.TextField(sheetPage.sheetName).RemoveBreakingCharacter();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGenerateButton()
        {
            if (!GUILayout.Button(Localization.GENERATE))
                return;

            callback(sheetPage);
            Close();
        }

        private bool TryGetMissingRequirement(out string error)
        {
            if (string.IsNullOrEmpty(sheetPage.sheetName))
            {
                error = Localization.ERROR_SHEET_SHEETNAME_EMPTY;
                return true;
            }


            if (string.IsNullOrEmpty(sheetPage.sheetEnumCase))
            {
                error = Localization.ERROR_SHEET_ENUMVALUE_EMPTY;
                return true;
            }

            if (sheetPage.sheetEnumCase == "BaseClasses")
            {
                error = Localization.ERROR_SHEET_ENUMVALUE_BASECLASSES;
                return true;
            }

            if (dataSheet.datasheetPages.Any(i => i.sheetName == sheetPage.sheetName))
            {
                error = Localization.ERROR_SHEET_SHEETNAME_MATCH;
                return true;
            }

            if (dataSheet.datasheetPages.Any(i => i.sheetEnumCase == sheetPage.sheetEnumCase))
            {
                error = Localization.ERROR_SHEET_ENUMVALUE_MATCH;
                return true;
            }

            error = string.Empty;
            return false;
        }

        private void CreateHorizontalLine(string leftText, string rightText)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(leftText, GUILayout.Width(120));
            GUILayout.Label(rightText);
            EditorGUILayout.EndHorizontal();
        }
    }
}