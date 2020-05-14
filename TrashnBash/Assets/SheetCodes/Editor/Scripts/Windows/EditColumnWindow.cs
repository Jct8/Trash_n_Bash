using SheetCodes;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public class EditColumnWindow : EditorWindow
    {
        private DataSheet dataSheet;
        private SheetPage sheetPage;
        private SheetColumn sheetColumn;
        private SheetDataType dataType;
        private string serializationName;
        private string propertyName;
        private bool isCollection;
        private string referenceSheet;
        private Action onColumnEdited;
        
        private const int WIDTH = 400;

        private void Awake()
        {
            titleContent.text = "Edit Column";
            minSize = new Vector2(WIDTH, 200);
            maxSize = minSize;
        }
        
        
        public void Initialize(DataSheet dataSheet, SheetPage sheetPage, SheetColumn sheetColumn, Action onColumnEdited)
        {
            this.dataSheet = dataSheet;
            this.sheetPage = sheetPage;
            this.sheetColumn = sheetColumn;
            this.onColumnEdited = onColumnEdited;
            serializationName = sheetColumn.serializationName;
            propertyName = sheetColumn.propertyName;
            isCollection = sheetColumn.isCollection;
            dataType = sheetColumn.dataType;

            if (dataType != SheetDataType.Reference)
            {
                string[] options = dataSheet.datasheetPages.Select(i => i.sheetName).ToArray();
                referenceSheet = options[0];
            }
            else
                referenceSheet = sheetColumn.referenceSheet;
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
            bool containsColumnChanges = ContainsColumnDefinitionChanges();
            if (containsExceptions)
                EditorGUILayout.HelpBox(exception, MessageType.Error);
            else if (containsColumnChanges)
            {
                string warningMessage = GetWarningMessage();
                if (!string.IsNullOrEmpty(warningMessage))
                    EditorGUILayout.HelpBox(GetWarningMessage(), MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(containsExceptions);
            DrawSaveButton(containsColumnChanges);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                minSize = new Vector2(WIDTH, lastRect.yMax - 4);
                maxSize = minSize;
            }
        }

        private string GetWarningMessage()
        {
            string addition = string.Empty;
            if (!sheetColumn.isCollection && isCollection)
                addition = Localization.WARNING_COLUMN_CHANGES_NONARRAY_TO_ARRAY;
            else if (sheetColumn.isCollection && !isCollection)
                addition = Localization.WARNING_COLUMN_CHANGES_ARRAY_TO_NONARRAY;

            if (sheetColumn.dataType == dataType)
            {
                switch (dataType)
                {
                    case SheetDataType.Long:
                    case SheetDataType.Int:
                    case SheetDataType.Short:
                    case SheetDataType.Byte:
                    case SheetDataType.Double:
                    case SheetDataType.Float:
                    case SheetDataType.String:
                    case SheetDataType.Boolean:
                        return addition;
                    case SheetDataType.Reference:
                        if (sheetColumn.referenceSheet != referenceSheet)
                            break;
                        return addition;
                }
            }

            if (sheetColumn.dataType == SheetDataType.Long ||
                sheetColumn.dataType == SheetDataType.Int ||
                sheetColumn.dataType == SheetDataType.Short ||
                sheetColumn.dataType == SheetDataType.Byte ||
                sheetColumn.dataType == SheetDataType.Double ||
                sheetColumn.dataType == SheetDataType.Float ||
                sheetColumn.dataType == SheetDataType.String)
            {
                if (dataType == SheetDataType.Long ||
                    dataType == SheetDataType.Int ||
                    dataType == SheetDataType.Short ||
                    dataType == SheetDataType.Byte ||
                    dataType == SheetDataType.Double ||
                    dataType == SheetDataType.Float ||
                    dataType == SheetDataType.String)
                {
                    return addition + string.Format(Localization.WARNING_COLUMN_CHANGES_CAST_DATA, sheetColumn.dataType, dataType);
                }
            }

            if (sheetColumn.dataType == SheetDataType.Boolean ||
                sheetColumn.dataType == SheetDataType.String)
            {
                if (dataType == SheetDataType.Boolean ||
                    dataType == SheetDataType.String)
                {
                    return addition + string.Format(Localization.WARNING_COLUMN_CHANGES_CAST_DATA, sheetColumn.dataType, dataType);
                }
            }

            return Localization.WARNING_COLUMN_CHANGES_RESET_DATA;
        }

        private void DrawSerializationName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_COLUMN_NAME, GUILayout.Width(120));
            serializationName = GUILayout.TextField(serializationName).RemoveBreakingCharacter();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPropertyName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_PROPERTY_NAME, GUILayout.Width(120));
            propertyName = GUILayout.TextField(propertyName).RemoveBreakingCharacter().CreatePropertyName();
            EditorGUILayout.EndHorizontal();
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

        private void DrawSaveButton(bool containsColumnChanges)
        {
            if (!GUILayout.Button(Localization.SAVE))
                return;

            string oldReferenceSheet = sheetColumn.referenceSheet;
            bool oldIsCollection = sheetColumn.isCollection;
            SheetDataType oldDataType = sheetColumn.dataType;

            sheetColumn.isCollection = isCollection;
            sheetColumn.serializationName = serializationName;
            sheetColumn.propertyName = propertyName;
            sheetColumn.dataType = dataType;

            sheetColumn.referenceSheet = dataType == SheetDataType.Reference ? referenceSheet : "";
            bool sameReferenceSheet = oldReferenceSheet == sheetColumn.referenceSheet;
            if (containsColumnChanges)
            {
                int columnIndex = sheetPage.columns.IndexOf(sheetColumn);

                foreach (SheetRow sheetRow in sheetPage.rows)
                {
                    SheetCell sheetCell = sheetRow.cells[columnIndex];
                    if (oldIsCollection && isCollection)
                        sheetCell.data = ReplaceColumnValuesArrayToArray(sheetCell.data as Array, oldDataType, dataType, sameReferenceSheet);
                    else if (oldIsCollection && !isCollection)
                        sheetCell.data = ReplaceColumnValuesArrayToNonArray(sheetCell.data as Array, oldDataType, dataType, sameReferenceSheet);
                    else if (!oldIsCollection && isCollection)
                        sheetCell.data = ReplaceColumnValuesNonArrayToArray(sheetCell.data, oldDataType, dataType, sameReferenceSheet);
                    else
                        sheetCell.data = ReplaceColumnValuesNonArrayToNonArray(sheetCell.data, oldDataType, dataType, sameReferenceSheet);
                }
            }

            onColumnEdited();
            Close();
        }

        private object ReplaceColumnValuesArrayToNonArray(Array oldData, SheetDataType oldDataType, SheetDataType newDataType, bool sameReferenceSheet)
        {
            if (oldData.Length == 0)
                return newDataType.GetDefaultValue(false);

            object oldValue = oldData.GetValue(0);
            return ConvertData(oldValue, oldDataType, newDataType, sameReferenceSheet);
        }

        private object ReplaceColumnValuesNonArrayToNonArray(object oldData, SheetDataType oldDataType, SheetDataType newDataType, bool sameReferenceSheet)
        {
            return ConvertData(oldData, oldDataType, newDataType, sameReferenceSheet);
        }

        private Array ReplaceColumnValuesArrayToArray(Array oldData, SheetDataType oldDataType, SheetDataType newDataType, bool sameReferenceSheet)
        {
            Array newData = CreateArray(newDataType, oldData.Length);
            for (int i = 0; i < newData.Length; i++)
            {
                object oldValue = oldData.GetValue(i);
                object newValue = ConvertData(oldValue, oldDataType, newDataType, sameReferenceSheet);
                newData.SetValue(newValue, i);
            }
            return newData;
        }

        private Array ReplaceColumnValuesNonArrayToArray(object oldData, SheetDataType oldDataType, SheetDataType newDataType, bool sameReferenceSheet)
        {
            Array newData = CreateArray(newDataType, 1);
            object newValue = ConvertData(oldData, oldDataType, newDataType, sameReferenceSheet);
            newData.SetValue(newValue, 0);
            return newData;
        }

        private Array CreateArray(SheetDataType newDataType, int length)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return new long[length];
                case SheetDataType.Int:
                    return new int[length];
                case SheetDataType.Short:
                    return new short[length];
                case SheetDataType.Byte:
                    return new byte[length];
                case SheetDataType.Double:
                    return new double[length];
                case SheetDataType.Float:
                    return new float[length];
                case SheetDataType.String:
                    return new string[length];
                case SheetDataType.Boolean:
                    return new bool[length];
                case SheetDataType.Reference:
                    return new string[length];
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_CONVERTDATA, newDataType));
        }

        private object ConvertData(object oldData, SheetDataType oldDataType, SheetDataType newDataType, bool sameReferenceSheet)
        {
            switch (oldDataType)
            {
                case SheetDataType.Long:
                    return ConvertLongToDataType((long)oldData, newDataType);
                case SheetDataType.Int:
                    return ConvertIntToDataType((int)oldData, newDataType);
                case SheetDataType.Short:
                    return ConvertShortToDataType((short)oldData, newDataType);
                case SheetDataType.Byte:
                    return ConvertByteToDataType((byte)oldData, newDataType);
                case SheetDataType.Double:
                    return ConvertDoubleToDataType((double)oldData, newDataType);
                case SheetDataType.Float:
                    return ConvertFloatToDataType((float)oldData, newDataType);
                case SheetDataType.String:
                    return ConvertStringToDataType((string)oldData, newDataType);
                case SheetDataType.Boolean:
                    return ConvertBoolToDataType((bool)oldData, newDataType);
                case SheetDataType.Reference:
                    return ConvertReferenceToDataType((string)oldData, newDataType, sameReferenceSheet);
            }

            return newDataType.GetDefaultValue(false);
        }


        private object ConvertLongToDataType(long oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return oldData;
                case SheetDataType.Int:
                    return (int)oldData;
                case SheetDataType.Short:
                    return (short)oldData;
                case SheetDataType.Byte:
                    return (byte)oldData;
                case SheetDataType.Double:
                    return (double)oldData;
                case SheetDataType.Float:
                    return (float)oldData;
                case SheetDataType.String:
                    return oldData.ToString();
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertIntToDataType(int oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return (long)oldData;
                case SheetDataType.Int:
                    return oldData;
                case SheetDataType.Short:
                    return (short)oldData;
                case SheetDataType.Byte:
                    return (byte)oldData;
                case SheetDataType.Double:
                    return (double)oldData;
                case SheetDataType.Float:
                    return (float)oldData;
                case SheetDataType.String:
                    return oldData.ToString();
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertShortToDataType(short oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return (long)oldData;
                case SheetDataType.Int:
                    return (int)oldData;
                case SheetDataType.Short:
                    return oldData;
                case SheetDataType.Byte:
                    return (byte)oldData;
                case SheetDataType.Double:
                    return (double)oldData;
                case SheetDataType.Float:
                    return (float)oldData;
                case SheetDataType.String:
                    return oldData.ToString();
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertByteToDataType(byte oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return (long)oldData;
                case SheetDataType.Int:
                    return (int)oldData;
                case SheetDataType.Short:
                    return (short)oldData;
                case SheetDataType.Byte:
                    return oldData;
                case SheetDataType.Double:
                    return (double)oldData;
                case SheetDataType.Float:
                    return (float)oldData;
                case SheetDataType.String:
                    return oldData.ToString();
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertDoubleToDataType(double oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return (long)oldData;
                case SheetDataType.Int:
                    return (int)oldData;
                case SheetDataType.Short:
                    return (short)oldData;
                case SheetDataType.Byte:
                    return (byte)oldData;
                case SheetDataType.Double:
                    return oldData;
                case SheetDataType.Float:
                    return (float)oldData;
                case SheetDataType.String:
                    return oldData.ToString();
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertFloatToDataType(float oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    return (long)oldData;
                case SheetDataType.Int:
                    return (int)oldData;
                case SheetDataType.Short:
                    return (short)oldData;
                case SheetDataType.Byte:
                    return (byte)oldData;
                case SheetDataType.Double:
                    return (double)oldData;
                case SheetDataType.Float:
                    return oldData;
                case SheetDataType.String:
                    return oldData.ToString();
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertStringToDataType(string oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.Long:
                    {
                        long result;
                        if (long.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.Int:
                    {
                        int result;
                        if (int.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.Short:
                    {
                        short result;
                        if (short.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.Byte:
                    {
                        byte result;
                        if (byte.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.Double:
                    {
                        double result;
                        if (double.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.Float:
                    {
                        float result;
                        if (float.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.Boolean:
                    {
                        bool result;
                        if (bool.TryParse(oldData, out result))
                            return result;
                        break;
                    }
                case SheetDataType.String:
                    {
                        return oldData;
                    }
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertBoolToDataType(bool oldData, SheetDataType newDataType)
        {
            switch (newDataType)
            {
                case SheetDataType.String:
                    return oldData.ToString();
                case SheetDataType.Boolean:
                    return oldData;
            }

            return newDataType.GetDefaultValue(false);
        }

        private object ConvertReferenceToDataType(string oldData, SheetDataType newDataType, bool sameReferenceSheet)
        {
            switch (newDataType)
            {
                case SheetDataType.Reference:
                    {
                        if (!sameReferenceSheet)
                            break;
                        return oldData;
                    }
            }

            return newDataType.GetDefaultValue(false);
        }

        private bool ContainsColumnDefinitionChanges()
        {
            if (sheetColumn.propertyName != propertyName)
                return true;

            if (sheetColumn.dataType != dataType)
                return true;

            if (dataType == SheetDataType.Reference && sheetColumn.referenceSheet != referenceSheet)
                return true;

            if (sheetColumn.isCollection != isCollection)
                return true;

            return false;
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

            if (sheetPage.columns.Any(i => i != sheetColumn && i.serializationName == serializationName))
            {
                error = Localization.ERROR_COLUMN_SERIALIZATIONNAME_MATCH;
                return true;
            }

            foreach (SheetColumn otherColumn in sheetPage.columns)
            {
                if (otherColumn == sheetColumn)
                    continue;

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