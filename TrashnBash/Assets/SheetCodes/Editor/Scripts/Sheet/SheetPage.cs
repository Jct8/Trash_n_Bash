using SheetCodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SheetCodesEditor
{
    public class SheetPage
    {
        public int index;
        public List<SheetColumn> columns;
        public List<SheetRow> rows;

        public string sheetEnumCase { get { return sheetName.ConvertStringToEnumString(); } }
        public string sheetNameTrimmedLowerCase { get { return sheetEnumCase.FirstLetterToLower(); } }
        public string sheetNameTrimmedUpperCase { get { return sheetEnumCase.FirstLetterToUpper(); } }
        public string sheetModelNameLowerCase { get { return sheetNameTrimmedLowerCase + "Model"; } }
        public string sheetModelNameUpperCase { get { return sheetNameTrimmedUpperCase + "Model"; } }
        public string sheetRecordName { get { return sheetEnumCase + "Record"; } }
        public string sheetIdentifierName { get { return sheetEnumCase + "Identifier"; } }
        public string sheetDirectoryLocation { get { return SheetStringDefinitions.MODEL_DIRECTORY + sheetNameTrimmedUpperCase; } }
        public string sheetModelFileLocation { get { return SheetStringDefinitions.MODEL_DIRECTORY + sheetNameTrimmedUpperCase + "/" + sheetModelNameUpperCase + SheetStringDefinitions.SCRIPT_FILE_EXTENSION; } }
        public string sheetRecordFileLocation { get { return SheetStringDefinitions.MODEL_DIRECTORY + sheetNameTrimmedUpperCase + "/" + sheetRecordName + SheetStringDefinitions.SCRIPT_FILE_EXTENSION; } }
        public string sheetIdentifierFileLocation { get { return SheetStringDefinitions.MODEL_DIRECTORY + sheetNameTrimmedUpperCase + "/" + sheetIdentifierName + SheetStringDefinitions.SCRIPT_FILE_EXTENSION; } }
        public string scriptableObjectFileLocation { get { return SheetStringDefinitions.SCRIPTABLEOBJECT_DIRECTORY + sheetNameTrimmedUpperCase + SheetStringDefinitions.SCRIPTABLEOBJECT_FILE_EXTENSION; } }

        public string sheetName;

        public SheetPage()
        {
            columns = new List<SheetColumn>();
            rows = new List<SheetRow>();
        }

        public SheetPage(SheetPageJsonable jsonable)
        {
            sheetName = jsonable.sheetName;
            index = jsonable.index;

            columns = new List<SheetColumn>(jsonable.columns.Length);
            for (int i = 0; i < jsonable.columns.Length; i++)
                columns.Add(new SheetColumn(jsonable.columns[i]));

            rows = new List<SheetRow>(jsonable.rows.Length);
            for (int i = 0; i < jsonable.rows.Length; i++)
                rows.Add(new SheetRow(this, jsonable.rows[i]));
        }


        public string ToJson()
        {
            SheetPageJsonable jsonable = new SheetPageJsonable(this);
            return JsonUtility.ToJson(jsonable);
        }

        public int GetAvailableRowIndex()
        {
            int index = rows.Count > 0 ? rows.Max(i => i.index) : 0;
            return index + 1;
        }

        public static SheetPage FromJson(string json)
        {
            SheetPageJsonable jsonable = JsonUtility.FromJson<SheetPageJsonable>(json);
            return new SheetPage(jsonable);
        }

        public bool CheckIfSameCodebase(SheetPage other)
        {
            if (sheetName != other.sheetName)
                return false;

            if (rows.Count != other.rows.Count)
                return false;

            for (int i = 0; i < rows.Count; i++)
            {
                if (!rows[i].CheckIfSameCodebase(other.rows[i]))
                    return false;
            }

            if (columns.Count != other.columns.Count)
                return false;

            for (int i = 0; i < columns.Count; i++)
            {
                if (!columns[i].CheckIfSameCodebase(other.columns[i]))
                    return false;
            }

            return true;
        }
    }
}