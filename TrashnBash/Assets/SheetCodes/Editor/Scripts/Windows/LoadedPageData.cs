using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public class LoadedPageData
    {
        public string dataSheetIdentifier;
        public SheetPage sheetPage;
        public List<int> widthDimensions;
        public List<int> heightDimensions;
        public int totalHeight;
        public int totalWidth;

        public int GetVisibleHeight(float screenHeight)
        {
            float height = screenHeight - heightDimensions[0] - WindowSettings.BORDER_SIZE;
            height -= WindowSettings.SCROLLBAR_REALSIZE;
            height -= WindowSettings.TASKBAR_HEIGHT;
            return (int)height;
        }

        public int GetVisibleWidth(float screenWidth, bool showingSelectionEdit, int collectionEditWidth)
        {
            float width = screenWidth - widthDimensions[0] - WindowSettings.BORDER_SIZE;
            width -= WindowSettings.SCROLLBAR_REALSIZE;

            if (showingSelectionEdit)
                width -= collectionEditWidth;

            return (int)width;
        }

        public void RecalculateSizes()
        {
            RecalculateTotalHeight();
            RecalculateTotalWidth();
        }

        private void RecalculateTotalHeight()
        {
            totalHeight = 0;
            for (int i = 1; i < heightDimensions.Count; i++)
                totalHeight += heightDimensions[i];
            totalHeight += (heightDimensions.Count - 1) * WindowSettings.BORDER_SIZE;
        }

        private void RecalculateTotalWidth()
        {
            totalWidth = 0;
            for (int i = 1; i < widthDimensions.Count; i++)
                totalWidth += widthDimensions[i];
            totalWidth += (widthDimensions.Count - 1) * WindowSettings.BORDER_SIZE;
        }

        private bool TryLoadStoredDimensions()
        {
            EditorPrefs.GetString(string.Format(WindowSettings.EDITORPREFS_KEY_WIDTHDIMENSIONS, dataSheetIdentifier));

            string storedWidthDimensions = EditorPrefs.GetString(string.Format(WindowSettings.EDITORPREFS_KEY_WIDTHDIMENSIONS, dataSheetIdentifier), null);
            string storedHeightDimensions = EditorPrefs.GetString(string.Format(WindowSettings.EDITORPREFS_KEY_HEIGHTDIMENSIONS, dataSheetIdentifier), null);

            if (string.IsNullOrEmpty(storedHeightDimensions))
                return false;

            if (string.IsNullOrEmpty(storedWidthDimensions))
                return false;

            int[] storedWidth = SheetDataType.Int.GetDataValue(storedWidthDimensions, true) as int[];
            int[] storedHeights = SheetDataType.Int.GetDataValue(storedHeightDimensions, true) as int[];

            if (storedWidth.Length != sheetPage.columns.Count + 1)
                return false;

            if (storedHeights.Length != sheetPage.rows.Count + 1)
                return false;

            widthDimensions = new List<int>(storedWidth);
            heightDimensions = new List<int>(storedHeights);
            return true;
        }

        private void CalculateDimensions()
        {
            int[,] widthSizes = new int[sheetPage.columns.Count + 1, sheetPage.rows.Count + 1];
            int[,] heightSizes = new int[sheetPage.columns.Count + 1, sheetPage.rows.Count + 1];

            int identifierWidth = CalculateWidth("Identifier");
            widthSizes[0, 0] = identifierWidth;

            widthDimensions = new List<int>();
            heightDimensions = new List<int>();

            for (int i = 0; i < sheetPage.rows.Count; i++)
            {
                SheetRow row = sheetPage.rows[i];
                identifierWidth = CalculateWidth(row.identifier);
                widthSizes[0, i + 1] = identifierWidth;
            }

            for (int i = 0; i < sheetPage.columns.Count; i++)
            {
                SheetColumn column = sheetPage.columns[i];

                int columnWidth = CalculateWidth(column.serializationName);

                widthSizes[i + 1, 0] = columnWidth;
                heightSizes[i + 1, 0] = WindowSettings.ONE_LINE_HEIGHT * 2;

                for (int j = 0; j < sheetPage.rows.Count; j++)
                {
                    SheetRow row = sheetPage.rows[j];
                    SheetCell cell = row.cells[i];

                    if (column.isCollection)
                    {
                        Array cellData = (Array)cell.data;
                        int highestWidth = 0;
                        for (int h = 0; h < cellData.Length; h++)
                        {
                            object cellObject = cellData.GetValue(h);
                            string content = cellObject != null ? cellObject.ToString() : Localization.NULLOBJECT_TO_STRING;
                            int width = CalculateWidth(content);
                            highestWidth = Mathf.Max(highestWidth, width);
                        }

                        int widthSet = Mathf.Clamp(highestWidth, WindowSettings.MINWIDTH, WindowSettings.MAXWIDTH_INITALIZATION);
                        int heightSet = Mathf.Min(cellData.Length * WindowSettings.ONE_LINE_HEIGHT, WindowSettings.MAXHEIGHT_INITALIZATION);

                        widthSizes[i + 1, j + 1] = widthSet;
                        heightSizes[i + 1, j + 1] = heightSet;
                    }
                    else
                    {
                        string content = cell.data != null ? cell.data.ToString() : Localization.NULLOBJECT_TO_STRING;
                        int width = CalculateWidth(content);
                        width = Mathf.Clamp(width, WindowSettings.MINWIDTH, WindowSettings.MAXWIDTH_INITALIZATION);

                        widthSizes[i + 1, j + 1] = width;
                        heightSizes[i + 1, j + 1] = WindowSettings.ONE_LINE_HEIGHT;
                    }
                }
            }

            for (int i = 0; i < widthSizes.GetLength(0); i++)
            {
                int heighestWidth = WindowSettings.MINWIDTH;

                for (int j = 0; j < widthSizes.GetLength(1); j++)
                {
                    int width = widthSizes[i, j];
                    heighestWidth = Mathf.Max(heighestWidth, width);
                }

                widthDimensions.Add(heighestWidth + WindowSettings.WIDTH_PADDING);
            }

            for (int i = 0; i < heightSizes.GetLength(1); i++)
            {
                int heighestHeight = WindowSettings.MINHEIGHT;

                for (int j = 0; j < heightSizes.GetLength(0); j++)
                {
                    int height = heightSizes[j, i];
                    heighestHeight = Mathf.Max(heighestHeight, height);
                }

                heightDimensions.Add(heighestHeight + WindowSettings.HEIGHT_PADDING);
            }
        }

        public bool ContainsChanges()
        {
            foreach (SheetRow sheetRow in sheetPage.rows)
            {
                foreach (SheetCell sheetCell in sheetRow.cells)
                {
                    if (!sheetCell.dirty)
                        continue;

                    return true;
                }
            }

            DatasheetType dataSheetType;
            if (!dataSheetIdentifier.TryGetIdentifierEnum(out dataSheetType))
                return true;

            SheetPage oldPage = ModelGeneration.DeconstructDatasheetCode(dataSheetType);
            if (!oldPage.CheckIfSameCodebase(sheetPage))
                return true;

            return false;
        }

        private int CalculateWidth(string content)
        {
            return (int)WindowSettings.GUI_STYLE.CalcSize(new GUIContent(content)).x;
        }

        public static LoadedPageData Initialize(DataSheet dataSheet, DatasheetType dataSheetType)
        {
            string dataString = dataSheetType.GetIdentifier();
            SheetPage sheetPage = Array.Find(dataSheet.datasheetPages, i => i.sheetName == dataString);
            LoadedPageData pageData = new LoadedPageData();
            pageData.dataSheetIdentifier = dataString;
            pageData.sheetPage = sheetPage;

            if (!pageData.TryLoadStoredDimensions())
                pageData.CalculateDimensions();

            pageData.RecalculateTotalHeight();
            pageData.RecalculateTotalWidth();
            return pageData;
        }

        public void SaveDimensions()
        {
            string widthDimensionsString = SheetDataType.Int.GetStringValue(widthDimensions.ToArray(), true);
            string heightDimensionsString = SheetDataType.Int.GetStringValue(heightDimensions.ToArray(), true);
            EditorPrefs.SetString(string.Format(WindowSettings.EDITORPREFS_KEY_WIDTHDIMENSIONS, dataSheetIdentifier), widthDimensionsString);
            EditorPrefs.SetString(string.Format(WindowSettings.EDITORPREFS_KEY_HEIGHTDIMENSIONS, dataSheetIdentifier), heightDimensionsString);
        }
    }
}