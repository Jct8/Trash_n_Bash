using SheetCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SheetCodesEditor
{
    public class SheetCodesWindow : EditorWindow
    {
        [MenuItem("Tools/Sheet Codes Free")]
        public static void ShowWindow()
        {
            if (CompilerExceptionDetector.containsCompilerErrors)
            {
                EditorUtility.DisplayDialog(Localization.COMPILER_ERRORS_TITLE, Localization.COMPILER_ERRORS_MESSAGE, Localization.OK);
                return;
            }

            SheetCodesWindow sheetCodesWindow = GetWindow<SheetCodesWindow>();
            sheetCodesWindow.Show();
            sheetCodesWindow.sheetNameToLoad = EditorPrefs.GetString(WindowSettings.EDITORPREFS_KEY_LAST_OPENED_SHEET, string.Empty);
        }

        public static void ShowWindow(string startingSheet)
        {
            if (CompilerExceptionDetector.containsCompilerErrors)
            {
                EditorUtility.DisplayDialog(Localization.COMPILER_ERRORS_TITLE, Localization.COMPILER_ERRORS_MESSAGE, Localization.OK);
                return;
            }

            SheetCodesWindow sheetCodesWindow = GetWindow<SheetCodesWindow>();
            sheetCodesWindow.Show();
            sheetCodesWindow.sheetNameToLoad = startingSheet;
        }

        [DidReloadScripts]
        private static void CloseWindowOnReload()
        {
            SheetCodesWindow[] windows = Resources.FindObjectsOfTypeAll<SheetCodesWindow>();
            if (windows.Length == 0)
                return;

            SheetCodesWindow sheetCodesWindow = GetWindow<SheetCodesWindow>();
            sheetCodesWindow.forceClosed = true;
            sheetCodesWindow.Close();
        }

        #region Variables
        private DataSheet dataSheet;
        private Dictionary<string, LoadedPageData> loadedPages;
        private LoadedPageData selectedPage;

        private EditorWindow subWindow;
        private bool firstFrame;
        private string sheetNameToLoad;

        private bool forceClosed;
        private int startingSize;
        private int draggingIndex;
        private bool isDraggingColumn;
        private bool isDraggingRow;
        private bool isDraggingHorizontally;
        private bool isDraggingVertically;
        private bool isDraggingCollectionEdit;
        private bool isDraggingCollectionEditItem;
        private bool isShowingEnumIndex;

        private Vector2 clickDragOffset;
        private Vector2 mousePressedLocation;
        private Vector2 scrollPosition;
        private float verticalScrollValue;
        private float horizontalScrollValue;
        private Rect previousWindowSize;
        private string previousFocussedGUI;
        private bool refocusPreviousFocuccesGUI;
        private bool focusedGUIExists;

        private float selectedCellScrollValue;
        private bool cellSelected;
        private SheetCell selectedCell;
        private SheetColumn selectedColumn;
        private int selectedCellTotalHeight;

        private Texture2D dots;
        private Texture2D collectionEditIcon;

        private int totalWidth { get { return selectedPage.totalWidth; } }
        private int totalHeight { get { return selectedPage.totalHeight; } }
        private int visibleWidth { get { return selectedPage.GetVisibleWidth(position.width, cellSelected, collectionEditWidth); } }
        private int visibleHeight { get { return selectedPage.GetVisibleHeight(position.height); } }

        private int collectionEditWidth
        {
            get { return EditorPrefs.GetInt(WindowSettings.EDITORPREFS_KEY_COLLECTIONEDIT_WIDTH, WindowSettings.WIDTH_SELECTIONEDIT); }
            set { EditorPrefs.SetInt(WindowSettings.EDITORPREFS_KEY_COLLECTIONEDIT_WIDTH, Mathf.Clamp(value, WindowSettings.MINWIDTH_COLLECTIONEDIT, maxWidthCollectionEdit)); }
        }

        private int maxWidthCollectionEdit
        {
            get { return (int)position.width - WindowSettings.MINWIDTH_DATADISPLAY; }
        }

        #endregion

        #region Unity Editor Functions
        private void Awake()
        {
            minSize = new Vector2(WindowSettings.MINWIDTH_DATADISPLAY, WindowSettings.MINHEIGTH_WINDOW);
            firstFrame = true;
            forceClosed = false;
            titleContent.text = "Sheet Codes Free";
            CompilerExceptionDetector.onCompilerError += OnCompilerError;
        }

        private void OnCompilerError()
        {
            forceClosed = true;
            Close();
        }

        private void OnGUI()
        {
            if (firstFrame)
            {
                Initialize();
                previousFocussedGUI = string.Empty;
                firstFrame = false;
            }

            if (selectedPage == null)
            {
                DrawBorderColors_NoSheet();
                DrawButtonRow_NoSheet();
                DrawRowColors_NoSheet();
            }
            else
            {
                previousFocussedGUI = GUI.GetNameOfFocusedControl();
                DrawRange drawRange = GetDrawRange();
                refocusPreviousFocuccesGUI = false;
                focusedGUIExists = false;

                selectedPage.RecalculateSizes();
                HandleScrollWheelScrolling(drawRange);
                DrawScrollbars();

                CellDrawData[,] cellDrawDatas = CellDrawData.GetCellDrawData(selectedPage, drawRange, visibleWidth, visibleHeight);
                DrawBorderColors(drawRange, cellDrawDatas);
                DrawMainContent(drawRange, cellDrawDatas);
                DrawLockedContent(drawRange, cellDrawDatas);
                DrawResizingAreas(drawRange, cellDrawDatas);
                DrawDragableColumns(drawRange, cellDrawDatas);
                DrawDragableRows(drawRange, cellDrawDatas);
                DrawRightClickMenu(drawRange, cellDrawDatas);
                DrawButtonRow();
                DrawCollectionEdit();
                DrawRowColors(drawRange, cellDrawDatas);

                if (Event.current.type == EventType.Layout)
                    refocusPreviousFocuccesGUI = true;

                if (refocusPreviousFocuccesGUI)
                {
                    if (focusedGUIExists)
                        GUI.FocusControl(previousFocussedGUI);
                    else
                        GUI.FocusControl(null);
                }
            }

            previousWindowSize = position;
        }

        private void OnDestroy()
        {
            if (subWindow != null)
                subWindow.Close();

            SaveWindowSettings();

            if (!forceClosed && ContainsChanges() && EditorUtility.DisplayDialog(Localization.UNSAVED_CHANGES_TITLE, Localization.UNSAVED_CHANGES_MESSAGE, Localization.YES, Localization.NO))
                SaveChanges();
        }

        private void Initialize()
        {
            dataSheet = new DataSheet();

            loadedPages = new Dictionary<string, LoadedPageData>();
            DatasheetType[] dataSheetTypes = Enum.GetValues(typeof(DatasheetType)) as DatasheetType[];
            foreach (DatasheetType dataSheetType in dataSheetTypes)
            {
                string dataString = dataSheetType.GetIdentifier();
                loadedPages.Add(dataString, LoadedPageData.Initialize(dataSheet, dataSheetType));
            }
            
            dots = AssetDatabase.LoadAssetAtPath<Texture2D>(SheetStringDefinitions.TEXTURE_DIRECTORY + SheetStringDefinitions.DOTS_TEXTURE_FILE_NAME);
            collectionEditIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(SheetStringDefinitions.TEXTURE_DIRECTORY + SheetStringDefinitions.COLLECTIONEDIT_TEXTURE_ICON_FILE_NAME);

            previousWindowSize = position;
            
            if (!loadedPages.TryGetValue(sheetNameToLoad, out selectedPage))
                selectedPage = loadedPages.Values.FirstOrDefault();
        }
        #endregion

        #region Utility Functions

        private void SetDrawRangeXMin(int newValue)
        {
            List<int> widthDimensions = selectedPage.widthDimensions;
            if (widthDimensions.Count == 1)
                return;

            if (totalWidth <= visibleWidth)
                return;

            int currentWidth = 0;
            int widthOverFlow = 0;
            for (int i = widthDimensions.Count - 1; i > 0; i--)
            {
                int previousWidth = currentWidth;
                currentWidth += widthDimensions[i] + WindowSettings.BORDER_SIZE;
                if (currentWidth < visibleWidth)
                    continue;

                widthOverFlow = visibleWidth - previousWidth;
                break;
            }

            int maxScrollValue = totalWidth - visibleWidth;
            int maxAdjustedScrollValue = maxScrollValue + widthOverFlow;
            float horizontalFactor = (float)maxScrollValue / maxAdjustedScrollValue;

            float newScrollValue = 0;
            if (newValue < widthDimensions.Count - 2)
            {
                for (int i = 1; i <= newValue; i++)
                    newScrollValue += widthDimensions[i] + WindowSettings.BORDER_SIZE;
                horizontalScrollValue = newScrollValue * horizontalFactor;
            }
            else
                horizontalScrollValue = maxScrollValue;
        }

        private void SetDrawRangeYMin(int newValue)
        {
            List<int> heightDimensions = selectedPage.heightDimensions;
            if (heightDimensions.Count == 1)
                return;

            if (totalHeight <= visibleHeight)
                return;

            int currentHeight = 0;
            int heightOverflow = 0;
            for (int i = heightDimensions.Count - 1; i > 0; i--)
            {
                int previousHeight = currentHeight;
                currentHeight += heightDimensions[i] + WindowSettings.BORDER_SIZE;
                if (currentHeight < visibleHeight)
                    continue;

                heightOverflow = visibleHeight - previousHeight;
                break;
            }

            int maxScrollValue = totalHeight - visibleHeight;
            int maxAdjustedScrollValue = maxScrollValue + heightOverflow;
            float verticalFactor = (float)maxScrollValue / maxAdjustedScrollValue;

            float newScrollValue = 0;
            if (newValue < heightDimensions.Count - 2)
            {
                for (int i = 1; i <= newValue; i++)
                    newScrollValue += heightDimensions[i] + WindowSettings.BORDER_SIZE;
                verticalScrollValue = newScrollValue * verticalFactor;
            }
            else
                verticalScrollValue = maxScrollValue;
        }

        private void HandleScrollWheelScrolling(DrawRange drawRange)
        {
            if (Event.current.type != EventType.ScrollWheel)
                return;

            if (cellSelected)
            {
                Vector2 mouseLocation = Event.current.mousePosition;
                Rect collectionEditRect = new Rect(position.width - collectionEditWidth, 0, collectionEditWidth, position.height);
                if (collectionEditRect.Contains(mouseLocation))
                    return;
            }

            if (Event.current.control)
            {
                int targetColumn = Event.current.delta.y > 0 ? drawRange.xMin + 1 : drawRange.xMin - 1;
                targetColumn = Mathf.Clamp(targetColumn, 0, selectedPage.sheetPage.columns.Count - 1);
                if (drawRange.xMin == targetColumn)
                    return;

                SetDrawRangeXMin(targetColumn);
            }
            else
            {
                int targetRow = Event.current.delta.y > 0 ? drawRange.yMin + 1 : drawRange.yMin - 1;
                targetRow = Mathf.Clamp(targetRow, 0, selectedPage.sheetPage.rows.Count - 1);
                if (drawRange.yMin == targetRow)
                    return;

                SetDrawRangeYMin(targetRow);
            }
            refocusPreviousFocuccesGUI = true;
            Repaint();
        }
        #endregion

        #region Save Related Functions
        private bool ContainsChanges()
        {
            if (dataSheet.dirty)
                return true;

            foreach (LoadedPageData loadedPageData in loadedPages.Values)
            {
                if (!loadedPageData.ContainsChanges())
                    continue;

                return true;
            }

            return false;
        }

        private void SaveChanges()
        {
            Dictionary<string, SheetPage> updatedSheetPages = new Dictionary<string, SheetPage>();
            foreach (KeyValuePair<string, LoadedPageData> keyValuePair in loadedPages)
                updatedSheetPages.Add(keyValuePair.Key, keyValuePair.Value.sheetPage);
            ModelGeneration.UpdateModelCode(loadedPages.Values.Select(i => i.sheetPage).ToArray());
        }

        private void SaveWindowSettings()
        {
            //This can happen when recompiling
            if (loadedPages == null)
                return;

            if (selectedPage != null)
                EditorPrefs.SetString(WindowSettings.EDITORPREFS_KEY_LAST_OPENED_SHEET, selectedPage.dataSheetIdentifier);

            foreach (LoadedPageData loadedPage in loadedPages.Values)
                loadedPage.SaveDimensions();
        }
        #endregion

        #region Rightclick Context Menus
        private void DrawRightClickMenu(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            bool mousePressed = Event.current.type == EventType.ContextClick;
            if (!mousePressed)
                return;

            Vector2 mouseLocation = Event.current.mousePosition;
            SheetPage sheetPage = selectedPage.sheetPage;
            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            for (int i = 1; i < cellDrawDatas.GetLength(0); i++)
            {
                CellDrawData cellDrawData = cellDrawDatas[i, 0];
                if (cellDrawData.cellRect.Contains(mouseLocation))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent(Localization.EDIT), false, OnCallback_EditColumn, i + drawRange.xMin - 1);
                    menu.AddItem(new GUIContent(Localization.INSERT_LEFT), false, OnCallback_InsertColumn, i + drawRange.xMin - 1);
                    menu.AddItem(new GUIContent(Localization.INSERT_RIGHT), false, OnCallback_InsertColumn, i + drawRange.xMin);
                    menu.AddItem(new GUIContent(Localization.DELETE), false, OnCallback_DeleteColumn, i + drawRange.xMin - 1);
                    menu.ShowAsContext();
                    return;
                }
            }

            for (int i = 1; i < cellDrawDatas.GetLength(1); i++)
            {
                CellDrawData cellDrawData = cellDrawDatas[0, i];
                if (cellDrawData.cellRect.Contains(mouseLocation))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent(Localization.EDIT), false, OnCallback_EditRow, i + drawRange.yMin - 1);
                    menu.AddItem(new GUIContent(Localization.INSERT_ABOVE), false, OnCallback_InsertRow, i + drawRange.yMin - 1);
                    menu.AddItem(new GUIContent(Localization.INSERT_BELOW), false, OnCallback_InsertRow, i + drawRange.yMin);
                    menu.AddItem(new GUIContent(Localization.DELETE), false, OnCallback_DeleteRow, i + drawRange.yMin - 1);
                    menu.ShowAsContext();
                    return;
                }
            }
        }

        private void OnCallback_InsertRow(object userData)
        {
            CloseSubWindow();

            int insertIndex = (int)userData;
            AddRowWindow rowWindow = GetWindow<AddRowWindow>();
            rowWindow.Initialize(selectedPage.sheetPage, OnCallback_AddRow, insertIndex);
            subWindow = rowWindow;
        }

        private void OnCallback_DeleteRow(object userData)
        {
            DeselectCell();
            CloseSubWindow();

            int rowIndex = (int)userData;

            SheetRow sheetRow = selectedPage.sheetPage.rows[rowIndex];
            sheetRow.ChangeIdentifier(dataSheet, selectedPage.sheetPage, SheetStringDefinitions.IDENTIFIER_DEFAULT_VALUE);

            selectedPage.heightDimensions.RemoveAt(rowIndex + 1);
            selectedPage.sheetPage.rows.RemoveAt(rowIndex);
        }

        private void OnCallback_InsertColumn(object userData)
        {
            int insertIndex = (int)userData;

            CloseSubWindow();

            AddColumnWindow columnWindow = GetWindow<AddColumnWindow>();
            columnWindow.Initialize(dataSheet, selectedPage.sheetPage, OnCallback_AddColumn, insertIndex);
            subWindow = columnWindow;
        }

        private void OnCallback_DeleteColumn(object userData)
        {
            DeselectCell();
            CloseSubWindow();

            int columnIndex = (int)userData;

            selectedPage.widthDimensions.RemoveAt(columnIndex + 1);
            selectedPage.sheetPage.columns.RemoveAt(columnIndex);

            foreach (SheetRow sheetRow in selectedPage.sheetPage.rows)
                sheetRow.cells.RemoveAt(columnIndex);
        }

        private void OnCallback_EditRow(object userData)
        {
            CloseSubWindow();
            int rowIndex = (int)userData;
            EditRowWindow editWindow = GetWindow<EditRowWindow>();
            editWindow.Initialize(dataSheet, selectedPage.sheetPage, selectedPage.sheetPage.rows[rowIndex]);
            subWindow = editWindow;
        }

        private void OnCallback_EditColumn(object userData)
        {
            CloseSubWindow();
            int columnIndex = (int)userData;
            EditColumnWindow editWindow = GetWindow<EditColumnWindow>();
            editWindow.Initialize(dataSheet, selectedPage.sheetPage, selectedPage.sheetPage.columns[columnIndex], OnCallback_ColumnEdited);
            subWindow = editWindow;
        }

        private void OnCallback_ColumnEdited()
        {
            Repaint();
        }

        #endregion

        #region Collection Edit
        private void DrawCollectionEdit()
        {
            //Can happen when editing a column
            if (cellSelected && !selectedColumn.isCollection)
                DeselectCell();

            if (!cellSelected)
                return;

            DrawCollectionEdit_ResizingArea();
            DrawCollectionEdit_Buttons();
            DrawCollectionEdit_HandleScrollWheelScrolling();
            DrawCollectionEdit_Scrollbar();
            DrawCollectionEdit_HandleWindowResizing();
            DrawCollectionEdit_MainContent();
            DrawCollectionEdit_DraggingArea();
            DrawCollectionEdit_RightClickContextMenu();
        }

        private void DrawCollectionEdit_RightClickContextMenu()
        {
            CollectionEditDrawRange drawRange = DrawCollectionEdit_GetDrawRange();
            bool mousePressed = Event.current.type == EventType.ContextClick;

            Vector2 mouseLocation = Event.current.mousePosition;
            Array cellData = selectedCell.data as Array;

            Rect imageRect = new Rect();
            imageRect.x = position.width - collectionEditWidth + WindowSettings.BORDER_SIZE * 2 + WindowSettings.LABELWIDTH_COLLECTIONEDIT;
            imageRect.y = WindowSettings.TASKBAR_HEIGHT;
            imageRect.width = WindowSettings.MINHEIGHT;
            imageRect.height = WindowSettings.MINHEIGHT;

            for (int i = drawRange.yMin; i < drawRange.yMax; i++)
            {
                GUI.DrawTexture(imageRect, collectionEditIcon);

                if (mousePressed && imageRect.Contains(mouseLocation))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent(Localization.INSERT_ABOVE), false, OnCallback_CollectionEdit_Insert, i);
                    menu.AddItem(new GUIContent(Localization.INSERT_BELOW), false, OnCallback_CollectionEdit_Insert, i + 1);
                    menu.AddItem(new GUIContent(Localization.DELETE), false, OnCallback_CollectionEdit_Delete, i);
                    menu.ShowAsContext();
                }

                imageRect.y += WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE;
            }
        }

        private void DrawCollectionEdit_DraggingArea()
        {
            CollectionEditDrawRange drawRange = DrawCollectionEdit_GetDrawRange();
            bool mouseReleased = Event.current.rawType == EventType.MouseUp && Event.current.button == 0;

            Vector2 mouseLocation = Event.current.mousePosition;
            Array cellData = selectedCell.data as Array;

            if (isDraggingCollectionEditItem)
            {
                int offset = (int)((mouseLocation.y - WindowSettings.TASKBAR_HEIGHT) / (WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE));
                if (offset < 0)
                    offset = 0;
                int originalIndex = drawRange.yMin + offset;
                int hoveringIndex = originalIndex;

                if (hoveringIndex > draggingIndex)
                    hoveringIndex++;

                if (hoveringIndex >= drawRange.yMax)
                    hoveringIndex = drawRange.yMax;

                if (mouseReleased)
                {
                    isDraggingCollectionEditItem = false;

                    if (originalIndex == draggingIndex)
                        return;

                    object temp = cellData.GetValue(draggingIndex);
                    if (hoveringIndex < draggingIndex)
                    {
                        for (int i = draggingIndex; i > hoveringIndex; i--)
                            cellData.SetValue(cellData.GetValue(i - 1), i);

                        cellData.SetValue(temp, hoveringIndex);
                    }
                    else
                    {
                        for (int i = draggingIndex; i < hoveringIndex - 1; i++)
                            cellData.SetValue(cellData.GetValue(i + 1), i);

                        cellData.SetValue(temp, hoveringIndex - 1);
                    }
                    GUI.FocusControl(null);
                }
                else
                {
                    Rect mouseRect = new Rect(mouseLocation.x - 20, mouseLocation.y - 20, 40, 40);
                    EditorGUIUtility.AddCursorRect(mouseRect, MouseCursor.Pan);

                    object data = cellData.GetValue(draggingIndex);

                    Rect imageRect = new Rect();
                    imageRect.x = mouseLocation.x - clickDragOffset.x;
                    imageRect.y = mouseLocation.y - clickDragOffset.y;
                    imageRect.width = WindowSettings.MINHEIGHT;
                    imageRect.height = WindowSettings.MINHEIGHT;

                    GUI.DrawTexture(imageRect, collectionEditIcon);

                    Rect indexRect = new Rect();
                    indexRect.x = imageRect.x - WindowSettings.LABELWIDTH_COLLECTIONEDIT;
                    indexRect.y = imageRect.y;
                    indexRect.width = WindowSettings.LABELWIDTH_COLLECTIONEDIT;
                    indexRect.height = imageRect.height;

                    EditorGUI.LabelField(indexRect, draggingIndex + ":");

                    Rect dataRect = new Rect();
                    dataRect.x = imageRect.x + WindowSettings.BORDER_SIZE + WindowSettings.MINHEIGHT;
                    dataRect.y = imageRect.y;
                    dataRect.height = imageRect.height;
                    dataRect.width = collectionEditWidth - WindowSettings.BORDER_SIZE * 4 - WindowSettings.SCROLLBAR_SIZE - WindowSettings.SCROLLBAR_HALFSIZE - WindowSettings.MINHEIGHT - WindowSettings.LABELWIDTH_COLLECTIONEDIT;

                    ShowGUILayout_Single(dataRect, selectedColumn, data);

                    if (originalIndex == draggingIndex)
                        return;

                    Rect lineRect = new Rect();
                    lineRect.x = position.width - collectionEditWidth + WindowSettings.BORDER_SIZE * 3;
                    lineRect.y = (hoveringIndex - drawRange.yMin) * (WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE) + WindowSettings.TASKBAR_HEIGHT - WindowSettings.BORDER_SIZE;
                    lineRect.width = collectionEditWidth - WindowSettings.BORDER_SIZE * 3;
                    lineRect.height = WindowSettings.BORDER_SIZE;

                    EditorGUI.DrawRect(lineRect, new Color(0.25f, 0.25f, 0.25f));

                    Repaint();
                }
            }
            else
            {
                bool mousePressed = Event.current.rawType == EventType.MouseDown && Event.current.button == 0;

                Rect imageRect = new Rect();
                imageRect.x = position.width - collectionEditWidth + WindowSettings.BORDER_SIZE * 2 + WindowSettings.LABELWIDTH_COLLECTIONEDIT;
                imageRect.y = WindowSettings.BORDER_SIZE + WindowSettings.TASKBAR_HEIGHT;
                imageRect.width = WindowSettings.MINHEIGHT;
                imageRect.height = WindowSettings.MINHEIGHT;

                for (int i = drawRange.yMin; i < drawRange.yMax; i++)
                {
                    EditorGUIUtility.AddCursorRect(imageRect, MouseCursor.Pan);

                    if (mousePressed && imageRect.Contains(mouseLocation))
                    {
                        isDraggingCollectionEditItem = true;
                        draggingIndex = i;

                        float x = mouseLocation.x - imageRect.xMin;
                        float y = mouseLocation.y - imageRect.yMin;
                        clickDragOffset = new Vector2(x, y);
                    }

                    imageRect.y += WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE;
                }
            }
        }

        private void OnCallback_CollectionEdit_Insert(object data)
        {
            int index = (int)data;
            Array cellData = selectedCell.data as Array;
            Array newCellData = Array.CreateInstance(cellData.GetType().GetElementType(), cellData.Length + 1);
            newCellData.SetValue(selectedColumn.dataType.GetDefaultValue(false), index);
            for (int i = 0; i < index; i++)
                newCellData.SetValue(cellData.GetValue(i), i);

            for (int i = index; i < cellData.Length; i++)
                newCellData.SetValue(cellData.GetValue(i), i + 1);
            selectedCell.data = newCellData;
        }

        private void OnCallback_CollectionEdit_Delete(object data)
        {
            int index = (int)data;
            Array cellData = selectedCell.data as Array;
            Array newCellData = Array.CreateInstance(cellData.GetType().GetElementType(), cellData.Length - 1);
            for (int i = 0; i < index; i++)
                newCellData.SetValue(cellData.GetValue(i), i);

            for (int i = index + 1; i < cellData.Length; i++)
                newCellData.SetValue(cellData.GetValue(i), i - 1);
            selectedCell.data = newCellData;
        }

        private void DrawCollectionEdit_MainContent()
        {
            CollectionEditDrawRange drawRange = DrawCollectionEdit_GetDrawRange();
            Array cellData = selectedCell.data as Array;
            Rect itemRect = new Rect();
            itemRect.x = position.width - collectionEditWidth + WindowSettings.BORDER_SIZE * 3 + WindowSettings.MINHEIGHT + WindowSettings.LABELWIDTH_COLLECTIONEDIT;
            itemRect.y = WindowSettings.TASKBAR_HEIGHT;
            itemRect.width = collectionEditWidth - WindowSettings.BORDER_SIZE * 4 - WindowSettings.SCROLLBAR_SIZE - WindowSettings.SCROLLBAR_HALFSIZE - WindowSettings.MINHEIGHT - WindowSettings.LABELWIDTH_COLLECTIONEDIT;
            itemRect.height = WindowSettings.MINHEIGHT;

            Rect labelRect = new Rect();
            labelRect.x = position.width - collectionEditWidth + WindowSettings.BORDER_SIZE * 2;
            labelRect.y = WindowSettings.TASKBAR_HEIGHT;
            labelRect.width = WindowSettings.LABELWIDTH_COLLECTIONEDIT;
            labelRect.height = WindowSettings.MINHEIGHT;

            for (int i = drawRange.yMin; i < drawRange.yMax; i++)
            {
                object cellValue = cellData.GetValue(i);
                string controlName = "CollectionEdit " + i;
                if (previousFocussedGUI == controlName)
                    focusedGUIExists = true;
                GUI.SetNextControlName(controlName);
                object newCellValue = ShowGUILayout_Single(itemRect, selectedColumn, cellValue);
                cellData.SetValue(newCellValue, i);

                EditorGUI.LabelField(labelRect, i + ":");

                itemRect.y += WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE;
                labelRect.y = itemRect.y;
            }
        }

        private CollectionEditDrawRange DrawCollectionEdit_GetDrawRange()
        {
            Array cellData = selectedCell.data as Array;
            if (cellData.Length == 0)
                return new CollectionEditDrawRange(0, -1);
            int totalVisibleHeight = (int)position.height - WindowSettings.MINHEIGHT - WindowSettings.BORDER_SIZE;
            float maxScrollFactor = selectedCellTotalHeight - totalVisibleHeight;
            if (maxScrollFactor <= 0)
                return new CollectionEditDrawRange(0, cellData.Length);

            float scrollFactor = selectedCellScrollValue / maxScrollFactor;
            float startIndexFloat = selectedCellScrollValue / (WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE);
            int startIndex = (int)startIndexFloat;
            if (startIndexFloat % 1 > 1 - scrollFactor)
                startIndex++;

            int visibleIndexes = totalVisibleHeight / (WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE);
            int endIndex = startIndex + visibleIndexes;
            return new CollectionEditDrawRange(startIndex, endIndex);
        }

        private void DrawCollectionEdit_HandleScrollWheelScrolling()
        {
            if (Event.current.type != EventType.ScrollWheel)
                return;

            selectedCellTotalHeight = (selectedCell.data as Array).Length * (WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE);
            int totalVisibleHeight = (int)position.height - WindowSettings.MINHEIGHT - WindowSettings.BORDER_SIZE;
            int maxScrollValue = selectedCellTotalHeight - totalVisibleHeight;
            if (maxScrollValue < 0)
                return;

            Vector2 mouseLocation = Event.current.mousePosition;
            Rect collectionEditRect = new Rect(position.width - collectionEditWidth, 0, collectionEditWidth, position.height);
            if (!collectionEditRect.Contains(mouseLocation))
                return;

            if (Event.current.delta.y > 0)
                selectedCellScrollValue += WindowSettings.SCROLLSPEED_COLLECTIONEDIT;
            else
                selectedCellScrollValue -= WindowSettings.SCROLLSPEED_COLLECTIONEDIT;

            selectedCellScrollValue = Mathf.Clamp(selectedCellScrollValue, 0, maxScrollValue);
            refocusPreviousFocuccesGUI = true;
        }

        private void DrawCollectionEdit_Scrollbar()
        {
            selectedCellTotalHeight = (selectedCell.data as Array).Length * (WindowSettings.MINHEIGHT + WindowSettings.BORDER_SIZE);
            int totalVisibleHeight = (int)position.height - WindowSettings.MINHEIGHT;
            if (selectedCellTotalHeight <= totalVisibleHeight)
            {
                selectedCellScrollValue = 0;
                return;
            }

            Rect scrollbarRect = new Rect();
            scrollbarRect.y = WindowSettings.TASKBAR_HEIGHT;
            scrollbarRect.height = position.height - WindowSettings.TASKBAR_HEIGHT;
            scrollbarRect.x = position.width - WindowSettings.SCROLLBAR_SIZE - WindowSettings.SCROLLBAR_HALFSIZE;
            scrollbarRect.width = WindowSettings.SCROLLBAR_SIZE;

            selectedCellScrollValue = GUI.VerticalScrollbar(scrollbarRect, selectedCellScrollValue, totalVisibleHeight, 0, selectedCellTotalHeight);
        }

        private void DrawCollectionEdit_HandleWindowResizing()
        {
            if (previousWindowSize.width == position.width)
                return;

            EditorPrefs.SetInt(WindowSettings.EDITORPREFS_KEY_COLLECTIONEDIT_WIDTH, Mathf.Clamp(collectionEditWidth, WindowSettings.MINWIDTH_COLLECTIONEDIT, maxWidthCollectionEdit));
        }

        private void DrawCollectionEdit_Buttons()
        {
            int currentY = 2;
            int currentX = (int)position.width - collectionEditWidth + WindowSettings.BORDER_SIZE * 2 + 2;

            Rect buttonRect = new Rect(currentX, currentY, 78, WindowSettings.MINHEIGHT);
            DrawCollectionEdit_Buttons_AddButton(buttonRect);
            buttonRect.x += buttonRect.width + 2;
            DrawCollectionEdit_Buttons_RemoveButton(buttonRect);
            buttonRect.x += buttonRect.width + 2;
            DrawCollectionEdit_Buttons_ClearButton(buttonRect);
        }

        private void DrawCollectionEdit_Buttons_AddButton(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.ADD))
                return;

            Array cellData = selectedCell.data as Array;
            Array newCellData = Array.CreateInstance(cellData.GetType().GetElementType(), cellData.Length + 1);
            for (int i = 0; i < cellData.Length; i++)
                newCellData.SetValue(cellData.GetValue(i), i);
            newCellData.SetValue(selectedColumn.dataType.GetDefaultValue(false), cellData.Length);
            selectedCell.data = newCellData;
        }

        private void DrawCollectionEdit_Buttons_RemoveButton(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.REMOVE))
                return;

            Array cellData = selectedCell.data as Array;

            if (cellData.Length == 0)
                return;

            Array newCellData = Array.CreateInstance(cellData.GetType().GetElementType(), cellData.Length - 1);
            for (int i = 0; i < cellData.Length - 1; i++)
                newCellData.SetValue(cellData.GetValue(i), i);
            selectedCell.data = newCellData;
        }

        private void DrawCollectionEdit_Buttons_ClearButton(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.CLEAR))
                return;

            selectedCell.data = selectedColumn.GetDefaultCellValue();
        }

        private void DrawCollectionEdit_ResizingArea()
        {
            Rect borderRect = new Rect(position.width - collectionEditWidth + WindowSettings.BORDER_SIZE, 0, WindowSettings.BORDER_SIZE, position.height);
            EditorGUI.DrawRect(borderRect, Color.gray);
            Vector2 mouseLocation = Event.current.mousePosition;
            bool mousePressed = Event.current.rawType == EventType.MouseDown && Event.current.button == 0;
            bool mouseReleased = Event.current.rawType == EventType.MouseUp && Event.current.button == 0;

            if (mousePressed)
                mousePressedLocation = mouseLocation;

            EditorGUIUtility.AddCursorRect(borderRect, MouseCursor.SplitResizeLeftRight);

            if (mousePressed && borderRect.Contains(mouseLocation))
            {
                isDraggingCollectionEdit = true;
                startingSize = collectionEditWidth;
            }

            if (isDraggingCollectionEdit)
            {
                if (mouseReleased)
                    isDraggingCollectionEdit = false;
                else
                    collectionEditWidth = startingSize + (int)(mousePressedLocation.x - mouseLocation.x);

                Repaint();
            }
        }
        #endregion

        #region Drawing Functions

        private void DrawResizingAreas(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            Vector2 mouseLocation = Event.current.mousePosition;
            bool mousePressed = Event.current.rawType == EventType.MouseDown && Event.current.button == 0;
            bool mouseReleased = Event.current.rawType == EventType.MouseUp && Event.current.button == 0;

            if (mousePressed)
                mousePressedLocation = mouseLocation;

            if (mouseReleased)
            {
                if (isDraggingHorizontally)
                {
                    isDraggingHorizontally = false;
                    draggingIndex = 0;
                }
                else if (isDraggingVertically)
                {
                    isDraggingVertically = false;
                    draggingIndex = 0;
                }
            }
            else if (isDraggingVertically)
            {
                int newSize = startingSize + (int)(mouseLocation.y - mousePressedLocation.y);
                if (draggingIndex == 0)
                    heightDimensions[draggingIndex] = Mathf.Clamp(newSize, WindowSettings.MINHEIGHT, WindowSettings.MAXHEIGHT_LOCKEDCONTENT);
                else
                    heightDimensions[draggingIndex] = Mathf.Clamp(newSize, WindowSettings.MINHEIGHT, WindowSettings.MAXHEIGHT_MAINCONTENT);
                selectedPage.RecalculateSizes();
                refocusPreviousFocuccesGUI = true;
                Repaint();
            }
            else if (isDraggingHorizontally)
            {
                int newSize = startingSize + (int)(mouseLocation.x - mousePressedLocation.x);
                if (draggingIndex == 0)
                    widthDimensions[draggingIndex] = Mathf.Clamp(newSize, WindowSettings.MINWIDTH, WindowSettings.MAXWIDTH_LOCKEDCONTENT);
                else
                    widthDimensions[draggingIndex] = Mathf.Clamp(newSize, WindowSettings.MINWIDTH, WindowSettings.MAXWIDTH_MAINCONTENT);
                selectedPage.RecalculateSizes();
                refocusPreviousFocuccesGUI = true;
                Repaint();
            }

            for (int i = 0; i < cellDrawDatas.GetLength(0); i++)
            {
                for (int j = 0; j < cellDrawDatas.GetLength(1); j++)
                {
                    CellDrawData cellDrawData = cellDrawDatas[i, j];
                    if (cellDrawData.hasRightBorder)
                    {
                        EditorGUIUtility.AddCursorRect(cellDrawData.rightBorderRect, MouseCursor.SplitResizeLeftRight);
                        if (mousePressed && cellDrawData.rightBorderRect.Contains(mouseLocation))
                        {
                            isDraggingHorizontally = true;
                            if (i == 0)
                            {
                                draggingIndex = i;
                                startingSize = widthDimensions[i];
                            }
                            else
                            {
                                draggingIndex = i + drawRange.xMin;
                                startingSize = widthDimensions[i + drawRange.xMin];
                            }
                        }
                    }

                    if (cellDrawData.hasBottomBorder)
                    {
                        EditorGUIUtility.AddCursorRect(cellDrawData.bottomBorderRect, MouseCursor.SplitResizeUpDown);
                        if (mousePressed && cellDrawData.bottomBorderRect.Contains(mouseLocation))
                        {
                            isDraggingVertically = true;
                            if (j == 0)
                            {
                                draggingIndex = j;
                                startingSize = heightDimensions[j];
                            }
                            else
                            {
                                draggingIndex = j + drawRange.yMin;
                                startingSize = heightDimensions[j + drawRange.yMin];
                            }
                        }
                    }
                }
            }
        }

        private void DrawBorderColors_NoSheet()
        {
            Rect leftBorderRect = new Rect();
            leftBorderRect.x = (int)WindowSettings.GUI_STYLE.CalculateTotalSize("Row Name").x;
            leftBorderRect.y = WindowSettings.TASKBAR_HEIGHT;
            leftBorderRect.height = position.height - WindowSettings.SCROLLBAR_REALSIZE - WindowSettings.TASKBAR_HEIGHT;
            leftBorderRect.width = WindowSettings.BORDER_SIZE;
            EditorGUI.DrawRect(leftBorderRect, new Color(0.55f, 0.575f, 0.6f));

            Rect topBorderRect = new Rect();
            topBorderRect.x = 0;
            topBorderRect.y = WindowSettings.TASKBAR_HEIGHT + WindowSettings.ONE_LINE_HEIGHT * 2 + WindowSettings.HEIGHT_PADDING;
            topBorderRect.height = WindowSettings.BORDER_SIZE;
            topBorderRect.width = position.width - WindowSettings.SCROLLBAR_REALSIZE;

            EditorGUI.DrawRect(topBorderRect, new Color(0.55f, 0.575f, 0.6f));
        }

        private void DrawBorderColors(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            List<int> widthDimensions = selectedPage.widthDimensions;
            List<int> heightDimensions = selectedPage.heightDimensions;

            int height = visibleHeight + heightDimensions[0] + WindowSettings.BORDER_SIZE;
            int width = visibleWidth + widthDimensions[0] + WindowSettings.BORDER_SIZE;

            Rect leftBorderRect = new Rect(widthDimensions[0], WindowSettings.TASKBAR_HEIGHT, WindowSettings.BORDER_SIZE, height);
            EditorGUI.DrawRect(leftBorderRect, new Color(0.55f, 0.575f, 0.6f));

            Rect topBorderRect = new Rect(0, heightDimensions[0] + WindowSettings.TASKBAR_HEIGHT, width, WindowSettings.BORDER_SIZE);
            EditorGUI.DrawRect(topBorderRect, new Color(0.55f, 0.575f, 0.6f));
        }

        private void DrawRowColors_NoSheet()
        {
            int width = (int) position.width - WindowSettings.SCROLLBAR_REALSIZE;
            int currentHeight = WindowSettings.ONE_LINE_HEIGHT * 2 + WindowSettings.HEIGHT_PADDING + WindowSettings.TASKBAR_HEIGHT + WindowSettings.BORDER_SIZE / 2;
            
            int maxHeight = (int)position.height - WindowSettings.SCROLLBAR_REALSIZE;
            while (currentHeight < maxHeight)
            {
                int height = Mathf.Min(WindowSettings.EMPTY_ROW_HEIGHT, maxHeight - currentHeight);
                Rect colorRect = new Rect(0, currentHeight, width, height);
                EditorGUI.DrawRect(colorRect, new Color(0f, 0f, 0f, 0.1f));
                currentHeight += WindowSettings.EMPTY_ROW_HEIGHT * 2;
            }
        }

        private void DrawRowColors(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            if (selectedPage == null)
                return;

            List<int> widthDimensions = selectedPage.widthDimensions;
            List<int> heightDimensions = selectedPage.heightDimensions;

            int width = visibleWidth + widthDimensions[0] + WindowSettings.BORDER_SIZE;
            int currentHeight = WindowSettings.TASKBAR_HEIGHT - WindowSettings.BORDER_SIZE / 2;
            for (int i = 1; i < cellDrawDatas.GetLength(1); i += 2)
            {
                CellDrawData cellDrawData = cellDrawDatas[0, i];
                Rect colorRect = new Rect(0, cellDrawData.cellRect.y - WindowSettings.BORDER_SIZE / 2, width, cellDrawData.cellRect.height + cellDrawData.bottomBorderRect.height);
                currentHeight = (int) colorRect.yMax;
                EditorGUI.DrawRect(colorRect, new Color(0f, 0f, 0f, 0.1f));
            }

            if (cellDrawDatas.GetLength(1) % 2 == 1)
            {
                CellDrawData cellDrawData = cellDrawDatas[0, cellDrawDatas.GetLength(1) - 1];
                currentHeight += (int) cellDrawData.cellRect.height + (int) cellDrawData.bottomBorderRect.height;
            }
            else
                currentHeight += WindowSettings.EMPTY_ROW_HEIGHT;

            int maxHeight = (int) position.height - WindowSettings.SCROLLBAR_REALSIZE;
            while (currentHeight < maxHeight)
            {
                int height = Mathf.Min(WindowSettings.EMPTY_ROW_HEIGHT, maxHeight - currentHeight);
                Rect colorRect = new Rect(0, currentHeight, width, height);
                EditorGUI.DrawRect(colorRect, new Color(0f, 0f, 0f, 0.1f));
                currentHeight += WindowSettings.EMPTY_ROW_HEIGHT *  2;
            }
        }

        private void DrawMainContent(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            Vector2 mouseLocation = Event.current.mousePosition;
            bool mousePressed = Event.current.rawType == EventType.MouseDown && Event.current.button == 0;

            SheetPage sheetPage = selectedPage.sheetPage;

            for (int i = 1; i < cellDrawDatas.GetLength(0); i++)
            {
                int columnIndex = i + drawRange.xMin - 1;
                SheetColumn sheetColumn = sheetPage.columns[columnIndex];

                for (int j = 1; j < cellDrawDatas.GetLength(1); j++)
                {
                    int rowIndex = j + drawRange.yMin - 1;
                    SheetRow sheetRow = sheetPage.rows[rowIndex];
                    SheetCell sheetCell = sheetRow.cells[columnIndex];
                    CellDrawData cellDrawData = cellDrawDatas[i, j];

                    string controlName = "MainContent " + rowIndex + "," + columnIndex;
                    if (previousFocussedGUI == controlName)
                        focusedGUIExists = true;

                    GUI.SetNextControlName(controlName);
                    object newData = ShowGUILayout(cellDrawData.cellRect, sheetColumn, sheetCell.data, selectedCell == sheetCell);
                    if (IsDataChanged(sheetCell.data, newData))
                    {
                        sheetCell.data = newData;
                        if (!sheetColumn.isCollection)
                        {
                            Type dataType = sheetCell.data.GetType();
                            Type newType = newData.GetType();
                            Vector2 newSize = GetMinSize(sheetColumn, newData);
                            int width = selectedPage.widthDimensions[columnIndex + 1];
                            width = Mathf.Max(width, (int)newSize.x);
                            width = Mathf.Min(width, WindowSettings.MAXWIDTH_MAINCONTENT, cellDrawData.maxAutoWidth);
                            selectedPage.widthDimensions[columnIndex + 1] = width;
                            refocusPreviousFocuccesGUI = true;
                        }
                    }

                    if(IsOverflowing(sheetColumn, sheetCell.data, cellDrawData.cellRect))
                        GUI.DrawTexture(cellDrawData.iconRect, dots);

                    if (mousePressed && cellDrawData.cellRect.Contains(mouseLocation))
                    {
                        if (sheetColumn.isCollection)
                            SelectCell(sheetColumn, sheetCell);
                        else
                            DeselectCell();
                    }
                }
            }
        }

        private bool IsDataChanged(object oldData, object newData)
        {
            if (oldData == null && newData != null)
                return true;

            if (oldData != null && newData == null)
                return true;

            if (oldData == null && newData == null)
                return false;

            return !oldData.Equals(newData);
        }

        private void SelectCell(SheetColumn sheetColumn, SheetCell sheetCell)
        {
            minSize = new Vector2(WindowSettings.MINWIDTH_DATADISPLAY + WindowSettings.MINWIDTH_COLLECTIONEDIT, WindowSettings.MINHEIGTH_WINDOW);
            GUI.FocusControl("");
            selectedCell = sheetCell;
            selectedColumn = sheetColumn;
            cellSelected = true;
            EditorPrefs.SetBool(WindowSettings.EDITORPREFS_KEY_SELECTIONEDIT, true);
            Repaint();
        }

        private void DeselectCell()
        {
            if (!cellSelected)
                return;

            minSize = new Vector2(WindowSettings.MINWIDTH_DATADISPLAY, WindowSettings.MINHEIGTH_WINDOW);
            selectedCell = null;
            selectedColumn = null;
            cellSelected = false;
            EditorPrefs.SetBool(WindowSettings.EDITORPREFS_KEY_SELECTIONEDIT, false);
            Repaint();
        }

        private void DrawLockedContent(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            SheetPage sheetPage = selectedPage.sheetPage;

            CellDrawData identifierData = cellDrawDatas[0, 0];
            string content = isShowingEnumIndex ? Localization.IDENTIFIER_ENUM_INDEX : Localization.IDENTIFIER;
            EditorGUI.LabelField(identifierData.cellRect, content, GUI.skin.textField);
            if(IsOverflowing(content, identifierData.cellRect))
                GUI.DrawTexture(identifierData.iconRect, dots);

            for (int i = 1; i < cellDrawDatas.GetLength(0); i++)
            {
                SheetColumn sheetColumn = sheetPage.columns[i + drawRange.xMin - 1];
                CellDrawData columnCellDrawData = cellDrawDatas[i, 0];
                string text = sheetColumn.serializationName + "\n";

                if (sheetColumn.dataType == SheetDataType.Reference)
                {
                    if(sheetColumn.isCollection)
                        text += string.Format(Localization.ADDITION_REFERENCE_ARRAY, sheetColumn.referenceSheet);
                    else
                        text += string.Format(Localization.ADDITION_REFERENCE, sheetColumn.referenceSheet);
                }
                else
                {
                    text += sheetColumn.dataType;

                    if (sheetColumn.isCollection)
                        text += string.Format(Localization.ADDITION_ARRAY, sheetColumn.referenceSheet);
                }

                EditorGUI.LabelField(columnCellDrawData.cellRect, text, GUI.skin.textField);
                if (IsOverflowing(text, columnCellDrawData.cellRect))
                    GUI.DrawTexture(columnCellDrawData.iconRect, dots);
            }

            for (int i = 1; i < cellDrawDatas.GetLength(1); i++)
            {
                SheetRow sheetRow = sheetPage.rows[i + drawRange.yMin - 1];
                CellDrawData rowCellDrawData = cellDrawDatas[0, i];

                string text = sheetRow.identifier;
                if (isShowingEnumIndex)
                {
                    text += "\n" + sheetRow.enumValue;
                    text += "\n" + sheetRow.index;
                }

                EditorGUI.LabelField(rowCellDrawData.cellRect, text, GUI.skin.textField);
                if (IsOverflowing(text, rowCellDrawData.cellRect))
                    GUI.DrawTexture(rowCellDrawData.iconRect, dots);
            }
        }

        private void DrawDragableColumns(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            SheetPage sheetPage = selectedPage.sheetPage;
            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            Vector2 mouseLocation = Event.current.mousePosition;
            bool mousePressed = Event.current.rawType == EventType.MouseDown && Event.current.button == 0;
            bool mouseReleased = Event.current.rawType == EventType.MouseUp && Event.current.button == 0;

            if (mouseReleased)
            {
                if (isDraggingColumn)
                {
                    isDraggingColumn = false;

                    int currentX = widthDimensions[0] + WindowSettings.BORDER_SIZE;

                    for (int i = drawRange.xMin; i < drawRange.xMax; i++)
                    {
                        currentX += widthDimensions[i + 1] + WindowSettings.BORDER_SIZE;
                        if (mouseLocation.x > currentX && i < drawRange.xMax - 1)
                            continue;

                        if (i == draggingIndex)
                            break;

                        int tempWidth = widthDimensions[draggingIndex + 1];
                        widthDimensions.RemoveAt(draggingIndex + 1);
                        widthDimensions.Insert(i + 1, tempWidth);

                        SheetColumn tempColumn = sheetPage.columns[draggingIndex];
                        sheetPage.columns.RemoveAt(draggingIndex);
                        sheetPage.columns.Insert(i, tempColumn);

                        foreach (SheetRow sheetRow in sheetPage.rows)
                        {
                            SheetCell tempCell = sheetRow.cells[draggingIndex];
                            sheetRow.cells.RemoveAt(draggingIndex);
                            sheetRow.cells.Insert(i, tempCell);
                        }
                        break;
                    }
                }
            }
            else if (isDraggingColumn)
            {
                SheetColumn column = sheetPage.columns[draggingIndex];
                float height = heightDimensions[0];
                float width = widthDimensions[draggingIndex + 1];
                float x = mouseLocation.x - clickDragOffset.x;
                float y = mouseLocation.y - clickDragOffset.y;
                Rect draggingRect = new Rect(x, y, width, height);
                EditorGUI.LabelField(draggingRect, column.serializationName, GUI.skin.textField);

                int currentWidth = widthDimensions[0];
                int currentX = currentWidth + WindowSettings.BORDER_SIZE;
                height += visibleHeight + WindowSettings.BORDER_SIZE;
                for (int i = drawRange.xMin; i < drawRange.xMax; i++)
                {
                    currentX += widthDimensions[i + 1] + WindowSettings.BORDER_SIZE;
                    if (mouseLocation.x > currentX && i < drawRange.xMax - 1)
                        continue;

                    if (i == draggingIndex)
                        break;

                    if (i < draggingIndex)
                        currentX -= widthDimensions[i + 1] + WindowSettings.BORDER_SIZE;

                    int maxX = Mathf.Min(currentX - WindowSettings.BORDER_SIZE, widthDimensions[0] + visibleWidth);
                    EditorGUI.DrawRect(new Rect(maxX, WindowSettings.TASKBAR_HEIGHT, WindowSettings.BORDER_SIZE, height), new Color(0.25f, 0.25f, 0.25f));
                    break;
                }

                Rect mouseRect = new Rect(mouseLocation.x - 20, mouseLocation.y - 20, 40, 40);
                EditorGUIUtility.AddCursorRect(mouseRect, MouseCursor.Pan);
                Repaint();
            }
            else
            {
                for (int i = 1; i < cellDrawDatas.GetLength(0); i++)
                {
                    SheetColumn sheetColumn = sheetPage.columns[i + drawRange.xMin - 1];
                    CellDrawData cellDrawData = cellDrawDatas[i, 0];
                    EditorGUIUtility.AddCursorRect(cellDrawData.cellRect, MouseCursor.Pan);

                    if (mousePressed && cellDrawData.cellRect.Contains(mouseLocation))
                    {
                        mousePressedLocation = mouseLocation;
                        isDraggingColumn = true;
                        draggingIndex = i + drawRange.xMin - 1;

                        float x = mouseLocation.x - cellDrawData.cellRect.xMin;
                        float y = mouseLocation.y - cellDrawData.cellRect.yMin;
                        clickDragOffset = new Vector2(x, y);
                    }
                }
            }
        }

        private void DrawDragableRows(DrawRange drawRange, CellDrawData[,] cellDrawDatas)
        {
            SheetPage sheetPage = selectedPage.sheetPage;
            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            Vector2 mouseLocation = Event.current.mousePosition;
            bool mousePressed = Event.current.rawType == EventType.MouseDown && Event.current.button == 0;
            bool mouseReleased = Event.current.rawType == EventType.MouseUp && Event.current.button == 0;

            if (mouseReleased)
            {
                if (isDraggingRow)
                {
                    isDraggingRow = false;

                    int currentY = heightDimensions[0] + WindowSettings.BORDER_SIZE + WindowSettings.TASKBAR_HEIGHT;

                    for (int i = drawRange.yMin; i < drawRange.yMax; i++)
                    {
                        currentY += heightDimensions[i + 1] + WindowSettings.BORDER_SIZE;
                        if (mouseLocation.y > currentY && i < drawRange.yMax - 1)
                            continue;

                        if (i == draggingIndex)
                            break;

                        int tempHeight = heightDimensions[draggingIndex + 1];
                        heightDimensions.RemoveAt(draggingIndex + 1);
                        heightDimensions.Insert(i + 1, tempHeight);

                        SheetRow tempRow = sheetPage.rows[draggingIndex];
                        sheetPage.rows.RemoveAt(draggingIndex);
                        sheetPage.rows.Insert(i, tempRow);
                        break;
                    }
                }
            }
            else if (isDraggingRow)
            {
                SheetRow row = sheetPage.rows[draggingIndex];
                float height = heightDimensions[draggingIndex + 1];
                float width = widthDimensions[0];
                float x = mouseLocation.x - clickDragOffset.x;
                float y = mouseLocation.y - clickDragOffset.y;
                Rect draggingRect = new Rect(x, y, width, height);
                EditorGUI.LabelField(draggingRect, row.identifier, GUI.skin.textField);

                width += visibleWidth + WindowSettings.BORDER_SIZE;
                int currentHeight = heightDimensions[0];
                int currentY = currentHeight + WindowSettings.BORDER_SIZE + WindowSettings.TASKBAR_HEIGHT;

                for (int i = drawRange.yMin; i < drawRange.yMax; i++)
                {
                    currentY += heightDimensions[i + 1] + WindowSettings.BORDER_SIZE;
                    if (mouseLocation.y > currentY && i < drawRange.yMax - 1)
                        continue;

                    if (i == draggingIndex)
                        break;

                    if (i < draggingIndex)
                        currentY -= heightDimensions[i + 1] + WindowSettings.BORDER_SIZE;

                    int maxY = Mathf.Min(currentY - WindowSettings.BORDER_SIZE, heightDimensions[0] + visibleHeight - WindowSettings.BORDER_SIZE);
                    EditorGUI.DrawRect(new Rect(0, maxY, width, WindowSettings.BORDER_SIZE), new Color(0.25f, 0.25f, 0.25f));
                    break;
                }

                Rect mouseRect = new Rect(mouseLocation.x - 20, mouseLocation.y - 20, 40, 40);
                EditorGUIUtility.AddCursorRect(mouseRect, MouseCursor.Pan);
                Repaint();
            }
            else
            {
                for (int i = 1; i < cellDrawDatas.GetLength(1); i++)
                {
                    SheetRow sheetRow = sheetPage.rows[i + drawRange.yMin - 1];

                    CellDrawData cellDrawData = cellDrawDatas[0, i];
                    EditorGUIUtility.AddCursorRect(cellDrawData.cellRect, MouseCursor.Pan);

                    if (mousePressed && cellDrawData.cellRect.Contains(mouseLocation))
                    {
                        mousePressedLocation = mouseLocation;
                        isDraggingRow = true;
                        draggingIndex = i + drawRange.yMin - 1;

                        float x = mouseLocation.x - cellDrawData.cellRect.xMin;
                        float y = mouseLocation.y - cellDrawData.cellRect.yMin;
                        clickDragOffset = new Vector2(x, y);
                    }
                }
            }
        }

        private void DrawScrollbars()
        {
            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            if (totalHeight > visibleHeight)
            {
                Rect scrollbarRect = new Rect();
                scrollbarRect.y = heightDimensions[0] + WindowSettings.BORDER_SIZE + WindowSettings.TASKBAR_HEIGHT;
                scrollbarRect.height = position.height - scrollbarRect.y - WindowSettings.SCROLLBAR_REALSIZE;
                scrollbarRect.x = widthDimensions[0] + WindowSettings.BORDER_SIZE + visibleWidth;
                scrollbarRect.width = WindowSettings.SCROLLBAR_SIZE;

                verticalScrollValue = GUI.VerticalScrollbar(scrollbarRect, verticalScrollValue, visibleHeight, 0, totalHeight);
            }
            else
                verticalScrollValue = 0;

            if (totalWidth > visibleWidth)
            {
                Rect scrollbarRect = new Rect();
                scrollbarRect.x = widthDimensions[0] + WindowSettings.BORDER_SIZE;
                scrollbarRect.width = visibleWidth;
                scrollbarRect.y = position.height - WindowSettings.SCROLLBAR_REALSIZE;
                scrollbarRect.height = WindowSettings.SCROLLBAR_SIZE;

                horizontalScrollValue = GUI.HorizontalScrollbar(scrollbarRect, horizontalScrollValue, visibleWidth, 0, totalWidth);
            }
            else
                horizontalScrollValue = 0;
        }

        #region Button Row No Sheet
        private void DrawButtonRow_NoSheet()
        {
            Rect taskbarRect = new Rect(0, 0, position.width, WindowSettings.BUTTONROW_HEIGHT + WindowSettings.TASKBAR_SEPARATOR_HEIGHT - 2);
            EditorGUI.DrawRect(taskbarRect, new Color(0.9f, 0.9f, 0.9f));
            Rect buttonRect = new Rect(2, 2, 88, WindowSettings.BUTTONROW_HEIGHT);

            DrawButtonRow_SaveButton(buttonRect);

            buttonRect.x += buttonRect.width + 2;
            DrawButtonRow_AddSheet(buttonRect);

            buttonRect.x += buttonRect.width + 2;
            buttonRect.y += 2;
            buttonRect.width = 120;
            isShowingEnumIndex = GUI.Toggle(buttonRect, isShowingEnumIndex, Localization.SHOW_ENUM_VALUE);

            taskbarRect = new Rect(0, taskbarRect.height - 1, position.width, 1);
            EditorGUI.DrawRect(taskbarRect, new Color(0.65f, 0.65f, 0.65f));
        }
        #endregion

        #region Button Row Selected Sheet
        private void DrawButtonRow()
        {
            Rect taskbarRect = new Rect(0, 0, position.width, WindowSettings.BUTTONROW_HEIGHT + WindowSettings.TASKBAR_SEPARATOR_HEIGHT - 2);
            EditorGUI.DrawRect(taskbarRect, new Color(0.9f, 0.9f, 0.9f));
            Rect buttonRect = new Rect(2, 2, 90, WindowSettings.BUTTONROW_HEIGHT);

            DrawButtonRow_SaveButton(buttonRect);
            buttonRect.x += buttonRect.width + 2;
            DrawButtonRow_AddRow(buttonRect);

            buttonRect.x += buttonRect.width + 2;
            DrawButtonRow_AddColumn(buttonRect);

            buttonRect.x += buttonRect.width + 2;
            DrawButtonRow_AddSheet(buttonRect);

            buttonRect.x += buttonRect.width + 2;
            DrawButtonRow_DeleteSheet(buttonRect);

            if (dataSheet.datasheetPages.Length == 0)
                return;
            
            buttonRect.x += buttonRect.width + 2;
            buttonRect.y += 2;
            buttonRect.width = 165;

            DrawButtonRow_SelectedPage(buttonRect);

            buttonRect.x += buttonRect.width + 2;
            buttonRect.width = 120;
            isShowingEnumIndex = GUI.Toggle(buttonRect, isShowingEnumIndex, Localization.SHOW_ENUM_VALUE);

            taskbarRect = new Rect(0, taskbarRect.height - 1, position.width, 1);
            EditorGUI.DrawRect(taskbarRect, new Color(0.65f, 0.65f, 0.65f));
        }

        private void DrawButtonRow_SaveButton(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.SAVE))
                return;

            CloseSubWindow();

            if (ContainsChanges())
                SaveChanges();

            SaveWindowSettings();

            forceClosed = true;
            Close();
        }

        private void DrawButtonRow_AddRow(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.ADD_ROW))
                return;

            CloseSubWindow();

            AddRowWindow rowWindow = GetWindow<AddRowWindow>();
            rowWindow.Initialize(selectedPage.sheetPage, OnCallback_AddRow, selectedPage.sheetPage.rows.Count);
            subWindow = rowWindow;
        }

        private void DrawButtonRow_AddColumn(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.ADD_COLUMN))
                return;

            CloseSubWindow();

            AddColumnWindow columnWindow = GetWindow<AddColumnWindow>();
            columnWindow.Initialize(dataSheet, selectedPage.sheetPage, OnCallback_AddColumn, selectedPage.sheetPage.columns.Count);
            subWindow = columnWindow;
        }

        private void DrawButtonRow_AddSheet(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.ADD_SHEET))
                return;

            CloseSubWindow();

            AddSheetWindow sheetWindow = GetWindow<AddSheetWindow>();
            sheetWindow.Initialize(dataSheet, OnCallback_AddSheetPage);
            subWindow = sheetWindow;
        }

        private void DrawButtonRow_DeleteSheet(Rect buttonRect)
        {
            if (!GUI.Button(buttonRect, Localization.DELETE_SHEET))
                return;

            CloseSubWindow();

            if (!EditorUtility.DisplayDialog(Localization.DELETE_SHEET_MESSAGE_TITLE, Localization.DELETE_SHEET_MESSAGE_MESSAGE, Localization.YES, Localization.NO))
                return;

            DeselectCell();

            dataSheet.dirty = true;

            int index = Array.IndexOf(dataSheet.datasheetPages, selectedPage.sheetPage);
            loadedPages.Remove(selectedPage.dataSheetIdentifier);
            dataSheet.datasheetPages = dataSheet.datasheetPages.RemoveAt(index);

            foreach (SheetPage sheetPage in dataSheet.datasheetPages)
            {
                for (int i = sheetPage.columns.Count - 1; i >= 0; i--)
                {
                    SheetColumn sheetColumn = sheetPage.columns[i];
                    if (sheetColumn.dataType != SheetDataType.Reference)
                        continue;

                    if (sheetColumn.referenceSheet != selectedPage.dataSheetIdentifier)
                        continue;

                    sheetPage.columns.RemoveAt(i);
                    loadedPages[sheetPage.sheetName].widthDimensions.RemoveAt(i + 1);
                    foreach (SheetRow sheetRow in sheetPage.rows)
                        sheetRow.cells.RemoveAt(i);
                }
            }
            selectedPage = loadedPages.Values.FirstOrDefault();
        }

        private void DrawButtonRow_SelectedPage(Rect buttonRect)
        {
            string[] originalOptions = dataSheet.datasheetPages.Select(i => i.sheetName).ToArray();
            string[] options = new string[originalOptions.Length];
            for (int i = 0; i < options.Length; i++)
                options[i] = originalOptions[i].Replace('/', '\u2215');

            int currentOption = Array.IndexOf(originalOptions, selectedPage.dataSheetIdentifier);
            int selectedOption = EditorGUI.Popup(buttonRect, currentOption, options);

            if (currentOption == selectedOption)
                return;

            CloseSubWindow();

            string selectedPageString = originalOptions[selectedOption];
            selectedPage = loadedPages[selectedPageString];
        }

        private void CloseSubWindow()
        {
            if (subWindow == null)
                return;

            subWindow.Close();
            subWindow = null;
        }

        private void OnCallback_AddRow(SheetRow sheetRow, int insertIndex)
        {
            DeselectCell();

            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            int currentWidth = widthDimensions[0];

            GUIStyle guiStyle = new GUIStyle();
            int columnWidth = (int)guiStyle.CalcSize(new GUIContent(sheetRow.identifier)).x;

            int height = Mathf.CeilToInt((columnWidth + WindowSettings.WIDTH_PADDING) / (float)WindowSettings.MAXWIDTH_INITALIZATION) * WindowSettings.ONE_LINE_HEIGHT;
            height = Mathf.Clamp(height, WindowSettings.MINHEIGHT, WindowSettings.MAXHEIGHT_INITALIZATION);

            if (insertIndex <= heightDimensions.Count)
                heightDimensions.Insert(insertIndex + 1, height);
            else
                heightDimensions.Add(height);
            verticalScrollValue = totalHeight - visibleHeight;
            Repaint();
        }

        private void OnCallback_AddColumn(SheetColumn sheetColumn, int insertIndex)
        {
            DeselectCell();

            List<int> widthDimensions = selectedPage.widthDimensions;

            GUIStyle guiStyle = new GUIStyle();
            int columnWidth = (int)guiStyle.CalcSize(new GUIContent(sheetColumn.serializationName)).x;
            int width = Mathf.Clamp(columnWidth, WindowSettings.MINWIDTH + WindowSettings.WIDTH_PADDING, WindowSettings.MAXWIDTH_INITALIZATION);
            
            if (insertIndex <= widthDimensions.Count)
                widthDimensions.Insert(insertIndex + 1, width);
            else
                widthDimensions.Add(width);
            horizontalScrollValue = totalWidth - visibleWidth;
            Repaint();
        }

        private void OnCallback_AddSheetPage(SheetPage sheetPage)
        {
            DeselectCell();

            dataSheet.dirty = true;
            dataSheet.datasheetPages = dataSheet.datasheetPages.Append(sheetPage).ToArray();

            LoadedPageData pageData = new LoadedPageData();
            pageData.widthDimensions = new List<int>() { (int)WindowSettings.GUI_STYLE.CalculateTotalSize("Row Name").x };
            pageData.heightDimensions = new List<int>() { WindowSettings.ONE_LINE_HEIGHT * 2 + WindowSettings.HEIGHT_PADDING };
            pageData.sheetPage = sheetPage;
            pageData.dataSheetIdentifier = sheetPage.sheetName;

            loadedPages.Add(sheetPage.sheetName, pageData);
            selectedPage = pageData;
            Repaint();
        }
        #endregion
        #endregion

        #region Draw Range
        private DrawRange GetDrawRange()
        {
            List<int> heightDimensions = selectedPage.heightDimensions;
            List<int> widthDimensions = selectedPage.widthDimensions;

            int startWidthIndex = GetStartWidthIndex();
            int endWidthIndex = GetEndWidthIndex(startWidthIndex);

            int startHeightIndex = GetStartHeightIndex();
            int endHeightIndex = GetEndHeightIndex(startHeightIndex);

            return new DrawRange(startWidthIndex - 1, endWidthIndex - 1, startHeightIndex - 1, endHeightIndex - 1);
        }

        private int GetStartHeightIndex()
        {
            List<int> heightDimensions = selectedPage.heightDimensions;
            if (heightDimensions.Count == 1)
                return 1;

            if (totalHeight <= visibleHeight)
                return 1;

            int maxIndex = 0;
            int currentHeight = 0;
            int heightOverflow = 0;
            for (int i = heightDimensions.Count - 1; i > 0; i--)
            {
                int previousHeight = currentHeight;
                currentHeight += heightDimensions[i] + WindowSettings.BORDER_SIZE;
                if (currentHeight < visibleHeight)
                    continue;

                maxIndex = i + 1;
                heightOverflow = visibleHeight - previousHeight;
                break;
            }

            int maxScrollValue = totalHeight - visibleHeight;
            float verticalFactor = verticalScrollValue / maxScrollValue;
            float scrollValue = heightOverflow * verticalFactor + verticalScrollValue;

            int endIndex = heightDimensions.Count - 1;
            currentHeight = 0;
            for (int i = 1; i < heightDimensions.Count; i++)
            {
                currentHeight += heightDimensions[i] + WindowSettings.BORDER_SIZE;
                if (currentHeight < scrollValue)
                    continue;

                float overflowAmount = currentHeight - scrollValue;
                float overflowFactor = overflowAmount / (heightDimensions[i] + WindowSettings.BORDER_SIZE);
                if (overflowFactor <= verticalFactor && i < heightDimensions.Count - 1)
                    endIndex = i + 1;
                else
                    endIndex = i;
                break;
            }

            return Mathf.Clamp(endIndex, 0, maxIndex);
        }

        private int GetEndHeightIndex(int startHeightIndex)
        {
            List<int> heightDimensions = selectedPage.heightDimensions;
            if (heightDimensions.Count == 1)
                return 0;

            int currentHeight = 0;
            for (int i = startHeightIndex; i < heightDimensions.Count; i++)
            {
                float previousHeight = currentHeight;
                if (previousHeight + WindowSettings.MINHEIGHT < visibleHeight)
                {
                    currentHeight += heightDimensions[i] + WindowSettings.BORDER_SIZE;
                    continue;
                }

                return i;
            }

            return heightDimensions.Count;
        }

        private int GetStartWidthIndex()
        {
            List<int> widthDimensions = selectedPage.widthDimensions;
            if (widthDimensions.Count == 1)
                return 1;

            if (totalWidth <= visibleWidth)
                return 1;

            int maxIndex = 0;
            int currentWidth = 0;
            int widthOverFlow = 0;
            for (int i = widthDimensions.Count - 1; i > 0; i--)
            {
                int previousWidth = currentWidth;
                currentWidth += widthDimensions[i] + WindowSettings.BORDER_SIZE;
                if (currentWidth <= visibleWidth)
                    continue;

                maxIndex = i + 1;
                widthOverFlow = visibleWidth - previousWidth;
                break;
            }

            int maxScrollValue = totalWidth - visibleWidth;
            float horizontalFactor = horizontalScrollValue / maxScrollValue;
            float scrollValue = widthOverFlow * (horizontalScrollValue / maxScrollValue) + horizontalScrollValue;

            int endIndex = widthDimensions.Count - 1;
            currentWidth = 0;
            for (int i = 1; i < widthDimensions.Count; i++)
            {
                currentWidth += widthDimensions[i] + WindowSettings.BORDER_SIZE;
                if (currentWidth <= scrollValue)
                    continue;

                float overflowAmount = currentWidth - scrollValue;
                float overflowFactor = overflowAmount / (widthDimensions[i] + WindowSettings.BORDER_SIZE);
                if (overflowFactor <= horizontalFactor && i < widthDimensions.Count - 1)
                    endIndex = i + 1;
                else
                    endIndex = i;
                break;
            }

            return Mathf.Clamp(endIndex, 0, maxIndex);
        }

        private int GetEndWidthIndex(int startWidthIndex)
        {
            List<int> widthDimensions = selectedPage.widthDimensions;
            if (widthDimensions.Count == 1)
                return 0;

            int currentWidth = 0;
            for (int i = startWidthIndex; i < widthDimensions.Count; i++)
            {
                float previousWidth = currentWidth;
                if (previousWidth + WindowSettings.MINWIDTH < visibleWidth)
                {
                    currentWidth += widthDimensions[i] + WindowSettings.BORDER_SIZE;
                    continue;
                }

                return i;
            }

            return widthDimensions.Count;
        }
        #endregion

        #region Cell Displays
        private object ShowGUILayout(Rect drawRect, SheetColumn sheetColumn, object data, bool selected)
        {
            if (sheetColumn.isCollection)
                return ShowGUILayout_Collection(drawRect, (Array)data, selected);
            else
                return ShowGUILayout_Single(drawRect, sheetColumn, data);
        }

        private object ShowGUILayout_Single(Rect drawRect, SheetColumn sheetColumn, object data)
        {
            switch (sheetColumn.dataType)
            {
                case (SheetDataType.Boolean):
                    return ShowGUILayout_Boolean(drawRect, (bool)data);
                case (SheetDataType.String):
                    return ShowGUILayout_String(drawRect, (string)data);
                case (SheetDataType.Byte):
                    return ShowGUILayout_Byte(drawRect, (byte)data);
                case (SheetDataType.Short):
                    return ShowGUILayout_Short(drawRect, (short)data);
                case (SheetDataType.Int):
                    return ShowGUILayout_Int(drawRect, (int)data);
                case (SheetDataType.Long):
                    return ShowGUILayout_Long(drawRect, (long)data);
                case (SheetDataType.Float):
                    return ShowGUILayout_Float(drawRect, (float)data);
                case (SheetDataType.Double):
                    return ShowGUILayout_Double(drawRect, (double)data);
                case (SheetDataType.Reference):
                    return ShowGUILayout_Reference(drawRect, sheetColumn.referenceSheet, (string)data);
            }
            throw new MissingReferenceException(string.Format(Localization.EXCEPTION_SHOWGUILAYOUT, sheetColumn.dataType));
        }

        private Array ShowGUILayout_Collection(Rect drawRect, Array data, bool selected)
        {
            string result = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                if (!string.IsNullOrEmpty(result))
                    result += "\n";

                object arrayData = data.GetValue(i);
                result += i + ":";
                result += arrayData != null ? arrayData.ToString() : Localization.NULLOBJECT_TO_STRING;
            }
            GUIStyle guiStyle = GUI.skin.textField;
            if (selected)
            {
                guiStyle = new GUIStyle(guiStyle);
                if (selected)
                    guiStyle.normal.background = guiStyle.focused.background;
            }
            GUI.Label(drawRect, result, guiStyle);
            return data;
        }

        private bool ShowGUILayout_Boolean(Rect drawRect, bool data)
        {
            return GUI.Toggle(drawRect, data, string.Empty);
        }

        private byte ShowGUILayout_Byte(Rect drawRect, byte data)
        {
            int value = EditorGUI.IntField(drawRect, data);
            return (byte)Mathf.Clamp(value, byte.MinValue, byte.MaxValue);
        }

        private short ShowGUILayout_Short(Rect drawRect, short data)
        {
            int value = EditorGUI.IntField(drawRect, data);
            return (short)Mathf.Clamp(value, short.MinValue, short.MaxValue);
        }

        private int ShowGUILayout_Int(Rect drawRect, int data)
        {
            return EditorGUI.IntField(drawRect, data);
        }

        private long ShowGUILayout_Long(Rect drawRect, long data)
        {
            return EditorGUI.LongField(drawRect, data);
        }

        private float ShowGUILayout_Float(Rect drawRect, float data)
        {
            return EditorGUI.FloatField(drawRect, data);
        }

        private double ShowGUILayout_Double(Rect drawRect, double data)
        {
            return EditorGUI.DoubleField(drawRect, data);
        }

        private string ShowGUILayout_String(Rect drawRect, string data)
        {
            return EditorGUI.TextField(drawRect, data);
        }

        private string ShowGUILayout_Reference(Rect drawRect, string referenceSheet, string data)
        {
            drawRect.height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, new GUIContent(""));
            drawRect.y += 1;
            LoadedPageData pageData = loadedPages[referenceSheet];
            string[] originalOptions = pageData.sheetPage.rows.Select(i => i.identifier).Prepend(SheetStringDefinitions.IDENTIFIER_DEFAULT_VALUE).ToArray();
            string[] options = new string[originalOptions.Length];
            for (int i = 0; i < options.Length; i++)
                options[i] = originalOptions[i].Replace('/', '\u2215');

            int currentOption = Array.IndexOf(originalOptions, data);
            int selectedOption = EditorGUI.Popup(drawRect, currentOption, options);
            return originalOptions[selectedOption];
        }
        #endregion

        #region Cell Size Calculations

        private bool IsOverflowing(SheetColumn sheetColumn, object data, Rect drawRect)
        {
            Vector2 minSize = GetMinSize(sheetColumn, data);
            bool overflowRight = drawRect.width + WindowSettings.OVERFLOW_WIDTH_MARGIN < minSize.x;
            bool overflowDown = drawRect.height + WindowSettings.OVERFLOW_HEIGHT_MARGIN < minSize.y;

            if (overflowRight)
                return true;

            if (overflowDown)
                return true;

            return false;
        }

        private bool IsOverflowing(string data, Rect drawRect)
        {
            Vector2 minSize = WindowSettings.GUI_STYLE.CalculateTotalSize(data);
            bool overflowRight = drawRect.width + WindowSettings.OVERFLOW_WIDTH_MARGIN < minSize.x;
            bool overflowDown = drawRect.height + WindowSettings.OVERFLOW_HEIGHT_MARGIN < minSize.y;
            
            if (overflowRight)
                return true;

            if (overflowDown)
                return true;

            return false;
        }

        private Vector2 GetMinSize(SheetColumn sheetColumn, object data)
        {
            if (sheetColumn.isCollection)
                return GetMinSize_Collection(sheetColumn, data);
            else
                return GetMinSize_Single(sheetColumn, data);
        }

        private Vector2 GetMinSize_Collection(SheetColumn sheetColumn, object data)
        {
            switch (sheetColumn.dataType)
            {
                case (SheetDataType.Boolean):
                    return GetMinSize_Collection_Bool((bool[])data);
                case (SheetDataType.Byte):
                    return GetMinSize_Collection_Byte((byte[])data);
                case (SheetDataType.Double):
                    return GetMinSize_Collection_Double((double[])data);
                case (SheetDataType.Float):
                    return GetMinSize_Collection_Float((float[])data);
                case (SheetDataType.Int):
                    return GetMinSize_Collection_Int((int[])data);
                case (SheetDataType.Long):
                    return GetMinSize_Collection_Long((long[])data);
                case (SheetDataType.Short):
                    return GetMinSize_Collection_Short((short[])data);
                case (SheetDataType.String):
                    return GetMinSize_Collection_String((string[])data);
                case (SheetDataType.Reference):
                    return GetMinSize_Collection_String((string[])data);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETMINSIZE_COLLECTION, sheetColumn.dataType));
        }

        private Vector2 GetMinSize_Single(SheetColumn sheetColumn, object data)
        {
            switch (sheetColumn.dataType)
            {
                case (SheetDataType.Boolean):
                    return GetMinSize_Single_Bool((bool)data);
                case (SheetDataType.Byte):
                    return GetMinSize_Single_Byte((byte)data);
                case (SheetDataType.Double):
                    return GetMinSize_Single_Double((double)data);
                case (SheetDataType.Float):
                    return GetMinSize_Single_Float((float)data);
                case (SheetDataType.Int):
                    return GetMinSize_Single_Int((int)data);
                case (SheetDataType.Long):
                    return GetMinSize_Single_Long((long)data);
                case (SheetDataType.Short):
                    return GetMinSize_Single_Short((short)data);
                case (SheetDataType.String):
                    return GetMinSize_Single_String((string)data);

                //Never show overflow indicators for Reference singles
                case (SheetDataType.Reference):
                    return new Vector2();
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETMINSIZE_SINGLE, sheetColumn.dataType));
        }


        private Vector2 GetMinSize_Collection_Bool(bool[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (bool data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Bool(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Bool(bool data)
        {
            string content = data ? Localization.TRUE : Localization.FALSE;
            return WindowSettings.GUI_STYLE.CalculateTotalSize(content);
        }

        private Vector2 GetMinSize_Collection_Byte(byte[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (byte data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Byte(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Byte(byte data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data.ToString());
        }

        private Vector2 GetMinSize_Collection_Short(short[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (short data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Short(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Short(short data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data.ToString());
        }

        private Vector2 GetMinSize_Collection_Int(int[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (int data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Int(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Int(int data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data.ToString());
        }

        private Vector2 GetMinSize_Collection_Long(long[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (long data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Long(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Long(long data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data.ToString());
        }

        private Vector2 GetMinSize_Collection_String(string[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (string data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_String(data).x);
            return new Vector2(minWidth, minHeight);
        }
        
        private Vector2 GetMinSize_Single_String(string data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data);
        }

        private Vector2 GetMinSize_Collection_Float(float[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (float data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Float(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Float(float data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data.ToString());
        }

        private Vector2 GetMinSize_Collection_Double(double[] datas)
        {
            float minHeight = datas.Length * WindowSettings.ONE_LINE_HEIGHT + WindowSettings.HEIGHT_PADDING;
            float minWidth = 0;
            foreach (double data in datas)
                minWidth = Math.Max(minWidth, GetMinSize_Single_Double(data).x);
            return new Vector2(minWidth, minHeight);
        }

        private Vector2 GetMinSize_Single_Double(double data)
        {
            return WindowSettings.GUI_STYLE.CalculateTotalSize(data.ToString());
        }
        #endregion
    }
}