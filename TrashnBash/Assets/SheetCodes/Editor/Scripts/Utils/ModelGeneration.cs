using SheetCodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SheetCodesEditor
{
    [InitializeOnLoad]
    public static class ModelGeneration
    {
        #region Generate Model

        public static void UpdateModelCode(SheetPage[] updatedPages)
        {
            List<SheetPage> deletedSheetPages = new List<SheetPage>();
            List<SheetPage> newSheetPages = new List<SheetPage>();
            List<SheetPage> changedPages = new List<SheetPage>();

            DataSheet dataSheet = new DataSheet();

            foreach (SheetPage sheetPage in updatedPages)
            {
                SheetPage originalPage = Array.Find(dataSheet.datasheetPages, i => i.sheetName == sheetPage.sheetName);
                if (originalPage == null)
                {
                    newSheetPages.Add(sheetPage);
                    continue;
                }

                if (originalPage.CheckIfSameCodebase(sheetPage))
                    continue;

                changedPages.Add(sheetPage);
            }

            foreach (SheetPage sheetPage in dataSheet.datasheetPages)
            {
                if (updatedPages.Any(i => i.sheetName == sheetPage.sheetName))
                    continue;

                deletedSheetPages.Add(sheetPage);
            }

            int totalChanges = newSheetPages.Count + deletedSheetPages.Count + changedPages.Count;
            if (totalChanges > 0)
            {
                foreach (SheetPage sheetPage in newSheetPages)
                {
                    Directory.CreateDirectory(sheetPage.sheetDirectoryLocation);
                    GenerateCode_Model(sheetPage);
                    GenerateCode_Record(dataSheet, newSheetPages, sheetPage);
                    GenerateCode_Identifier(sheetPage);
                }

                foreach (SheetPage sheetPage in changedPages)
                {
                    GenerateCode_Record(dataSheet, newSheetPages, sheetPage);
                    GenerateCode_Identifier(sheetPage);
                }

                foreach (SheetPage sheetPage in deletedSheetPages)
                {
                    string directory = SheetStringDefinitions.MODEL_DIRECTORY + sheetPage.sheetEnumCase;
                    Directory.Delete(directory, true);
                    File.Delete(directory + SheetStringDefinitions.META_FILE_EXTENSION);
                    File.Delete(SheetStringDefinitions.SCRIPTABLEOBJECT_DIRECTORY + sheetPage.sheetEnumCase + SheetStringDefinitions.SCRIPTABLEOBJECT_FILE_EXTENSION);
                    File.Delete(SheetStringDefinitions.SCRIPTABLEOBJECT_DIRECTORY + sheetPage.sheetEnumCase + SheetStringDefinitions.SCRIPTABLEOBJECT_FILE_EXTENSION + SheetStringDefinitions.META_FILE_EXTENSION);
                }

                GenerateCode_ModelManager(updatedPages);
                GenerateCode_DataSheetType(updatedPages);

                string[] updatedPagesJson = new string[updatedPages.Length];
                for (int i = 0; i < updatedPages.Length; i++)
                    updatedPagesJson[i] = JsonUtility.ToJson(new SheetPageJsonable(updatedPages[i]));

                string json = SheetDataType.String.GetStringValue(updatedPagesJson, true);
                File.WriteAllText(SheetStringDefinitions.FILE_LOCATION_TARGET_DATASHEET, json);
            }
            else
            {
                foreach (SheetPage sheetPage in updatedPages)
                    CreateModelInstance(sheetPage);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [DidReloadScripts]
        private static void UpdateModelData()
        {
            if (!File.Exists(SheetStringDefinitions.FILE_LOCATION_TARGET_DATASHEET))
                return;

            string json = File.ReadAllText(SheetStringDefinitions.FILE_LOCATION_TARGET_DATASHEET);

            string[] updatedPagesJson = SheetDataType.String.GetDataValue(json, true) as string[];
            SheetPage[] updatedPages = new SheetPage[updatedPagesJson.Length];
            for (int i = 0; i < updatedPagesJson.Length; i++)
                updatedPages[i] = new SheetPage(JsonUtility.FromJson<SheetPageJsonable>(updatedPagesJson[i]));

            foreach (SheetPage sheetPage in updatedPages)
                CreateModelInstance(sheetPage);

            File.Delete(SheetStringDefinitions.FILE_LOCATION_TARGET_DATASHEET);
            File.Delete(SheetStringDefinitions.FILE_LOCATION_TARGET_DATASHEET + SheetStringDefinitions.META_FILE_EXTENSION);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void GenerateCode_Model(SheetPage sheetPage)
        {
            if (!Directory.Exists(sheetPage.sheetDirectoryLocation))
                Directory.CreateDirectory(sheetPage.sheetDirectoryLocation);

            string[] modelTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_MODEL);

            for (int i = 0; i < modelTemplate.Length; i++)
            {
                string line = modelTemplate[i];
                modelTemplate[i] = line.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_NAME, sheetPage.sheetNameTrimmedUpperCase);
            }

            File.WriteAllLines(sheetPage.sheetModelFileLocation, modelTemplate);
        }

        private static void GenerateCode_Record(DataSheet dataSheet, List<SheetPage> newPages, SheetPage sheetPage)
        {
            List<string> recordTemplate = new List<string>(File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_RECORD));

            for (int i = 0; i < recordTemplate.Count; i++)
            {
                string line = recordTemplate[i];
                recordTemplate[i] = line.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_NAME, sheetPage.sheetNameTrimmedUpperCase);
            }

            string[] propertyTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_RECORD_PROPERTY);
            string[] propertyCollectionTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_RECORD_PROPERTY_COLLECTION);

            string[] referenceTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_RECORD_REFERENCE);
            string[] referenceCollectionTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_RECORD_REFERENCE_COLLECTION);
            
            List<string> recordProperties = new List<string>();

            foreach (SheetColumn sheetColumn in sheetPage.columns)
            {
                if (recordProperties.Count > 0)
                    recordProperties.Add(string.Empty);

                string[] matchingTemplate;
                if (sheetColumn.dataType == SheetDataType.Reference)
                    matchingTemplate = sheetColumn.isCollection ? referenceCollectionTemplate : referenceTemplate;
                else
                    matchingTemplate = sheetColumn.isCollection ? propertyCollectionTemplate : propertyTemplate;

                foreach (string templateLine in matchingTemplate)
                {
                    if (sheetColumn.dataType == SheetDataType.Reference)
                    {
                        string referenceSheet;
                        SheetPage[] sheetPages = dataSheet.datasheetPages.Where(i => i.sheetName == sheetColumn.referenceSheet).ToArray();
                        if (sheetPages.Length > 0)
                            referenceSheet = sheetPages[0].sheetEnumCase;
                        else
                            referenceSheet = newPages.Find(i => i.sheetName == sheetColumn.referenceSheet).sheetEnumCase;

                        recordProperties.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_IDENTIFIER, sheetColumn.serializationName)
                                                         .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_NAME_UPPER, sheetColumn.propertyName)
                                                         .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_NAME_LOWER, sheetColumn.propertyName.FirstLetterToLower())
                                                         .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_REFERENCE_MODEL_NAME, referenceSheet));
                    }
                    else
                    {
                        recordProperties.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_IDENTIFIER, sheetColumn.serializationName)
                                                         .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_NAME_UPPER, sheetColumn.propertyName)
                                                         .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_NAME_LOWER, sheetColumn.propertyName.FirstLetterToLower())
                                                         .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_PROPERTY_TYPE, sheetColumn.dataType.GetIdentifier()));
                    }
                }
            }

            int replaceIndex = recordTemplate.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_PROPERTIES));

            recordTemplate.InsertRange(replaceIndex, recordProperties);

            File.WriteAllLines(sheetPage.sheetRecordFileLocation, recordTemplate);
        }

        private static void GenerateCode_Identifier(SheetPage sheetPage)
        {
            List<string> recordTemplate = new List<string>(File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_IDENTIFIER));

            for (int i = 0; i < recordTemplate.Count; i++)
            {
                string line = recordTemplate[i];
                recordTemplate[i] = line.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_NAME, sheetPage.sheetNameTrimmedUpperCase);
            }

            string[] identifierLineTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_IDENTIFIER_LINE);

            List<string> recordProperties = new List<string>();

            foreach (SheetRow sheetRow in sheetPage.rows)
            {
                foreach (string templateLine in identifierLineTemplate)
                {
                    recordProperties.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_IDENTIFIER_NAME, sheetRow.identifier)
                                                     .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_IDENTIFIER_NAME_TITLE_CASE, sheetRow.enumValue)
                                                     .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_IDENTIFIER_INDEX, sheetRow.index.ToString()));
                }
            }

            int replaceIndex = recordTemplate.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_IDENTIFIERS));

            recordTemplate.InsertRange(replaceIndex, recordProperties);

            File.WriteAllLines(sheetPage.sheetIdentifierFileLocation, recordTemplate);
        }

        private static void GenerateCode_ModelManager(SheetPage[] sheetPages)
        {
            List<string> modelManagerCode = new List<string>(File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_MODELMANAGER));

            string[] recordTemplateRecord = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_MODELMANAGER_RECORD);
            string[] recordTemplateAsync = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_MODELMANAGER_ASYNC);
            string[] recordTemplateInitialize = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_MODELMANAGER_INITIALIZE);
            string[] recordTemplateUnload = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_MODELMANAGER_UNLOAD);

            List<string> recordCode = new List<string>();
            List<string> asyncCode = new List<string>();
            List<string> initializeCode = new List<string>();
            List<string> unloadCode = new List<string>();

            foreach (SheetPage sheetPage in sheetPages)
            {
                foreach (string templateLine in recordTemplateRecord)
                {
                    recordCode.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_TITLE_CASE, sheetPage.sheetNameTrimmedUpperCase)
                                               .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_CAMEL_CASE, sheetPage.sheetNameTrimmedLowerCase));
                }
                foreach (string templateLine in recordTemplateAsync)
                {
                    asyncCode.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_TITLE_CASE, sheetPage.sheetNameTrimmedUpperCase)
                                               .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_CAMEL_CASE, sheetPage.sheetNameTrimmedLowerCase));
                }
                foreach (string templateLine in recordTemplateInitialize)
                {
                    initializeCode.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_TITLE_CASE, sheetPage.sheetNameTrimmedUpperCase)
                                               .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_CAMEL_CASE, sheetPage.sheetNameTrimmedLowerCase));
                }
                foreach (string templateLine in recordTemplateUnload)
                {
                    unloadCode.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_TITLE_CASE, sheetPage.sheetNameTrimmedUpperCase)
                                               .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_MODEL_RECORD_CAMEL_CASE, sheetPage.sheetNameTrimmedLowerCase));
                }
            }

            int replaceIndex = modelManagerCode.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_MODELS));
            modelManagerCode.RemoveAt(replaceIndex);
            modelManagerCode.InsertRange(replaceIndex, recordCode);

            replaceIndex = modelManagerCode.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_INITIALIZE));
            modelManagerCode.RemoveAt(replaceIndex);
            modelManagerCode.InsertRange(replaceIndex, initializeCode);

            replaceIndex = modelManagerCode.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_UNLOAD));
            modelManagerCode.RemoveAt(replaceIndex);
            modelManagerCode.InsertRange(replaceIndex, unloadCode);

            replaceIndex = modelManagerCode.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_ASYNC));
            modelManagerCode.RemoveAt(replaceIndex);
            modelManagerCode.InsertRange(replaceIndex, asyncCode);

            File.WriteAllLines(SheetStringDefinitions.BASE_CLASSES_DIRECTORY + SheetStringDefinitions.MODEL_MANAGER_FILE_NAME, modelManagerCode);
        }

        private static void GenerateCode_DataSheetType(SheetPage[] sheetPages)
        {
            List<string> datasheetTypeCode = new List<string>(File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_DATASHEET_TYPE));

            int replaceIndex = datasheetTypeCode.FindIndex(i => i.Contains(SheetStringDefinitions.TEMPLATE_INSERT_DATASHEETS));

            string[] recordTemplate = File.ReadAllLines(SheetStringDefinitions.TEMPLATE_DIRECTORY + SheetStringDefinitions.TEMPLATE_FILE_DATASHEET_LINE);

            List<string> dataSheetTypeCode = new List<string>();

            foreach (SheetPage sheetPage in sheetPages)
            {
                foreach (string templateLine in recordTemplate)
                {
                    dataSheetTypeCode.Add(templateLine.Replace(SheetStringDefinitions.TEMPLATE_REPLACE_DATASHEET_LINE_MODEL_NAME, sheetPage.sheetName)
                                                      .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_DATASHEET_LINE_MODEL_NAME_TITLE_CASE, sheetPage.sheetNameTrimmedUpperCase)
                                                      .Replace(SheetStringDefinitions.TEMPLATE_REPLACE_DATASHEET_LINE_MODEL_INDEX, sheetPage.index.ToString()));
                }
            }
            datasheetTypeCode.InsertRange(replaceIndex, dataSheetTypeCode);

            File.WriteAllLines(SheetStringDefinitions.BASE_CLASSES_DIRECTORY + SheetStringDefinitions.DATASHEET_IDENTIFIER_FILE_NAME, datasheetTypeCode);
        }

        private static void CreateModelInstance(SheetPage sheetPage)
        {
            ScriptableObject instance = AssetDatabase.LoadAssetAtPath<ScriptableObject>(sheetPage.scriptableObjectFileLocation);
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance(sheetPage.sheetModelNameUpperCase);
                Directory.CreateDirectory(SheetStringDefinitions.SCRIPTABLEOBJECT_DIRECTORY);
                AssetDatabase.CreateAsset(instance, sheetPage.scriptableObjectFileLocation);
            }

            Type typeModel = instance.GetType();
            FieldInfo recordsMemberInfo = typeModel.GetField(SheetStringDefinitions.RECORDS_PROPERTY_NAME, BindingFlags.NonPublic | BindingFlags.Instance);

            Type typeRecord = recordsMemberInfo.FieldType.GetElementType();
            Type baseType = typeRecord.BaseType;
            Array recordCollection = Array.CreateInstance(typeRecord, sheetPage.rows.Count);

            FieldInfo identifierFieldInfo = baseType.GetField(SheetStringDefinitions.RECORD_IDENTIFIER_VARIABLE, BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < sheetPage.rows.Count; i++)
            {
                SheetRow sheetRow = sheetPage.rows[i];
                object recordInstance = Activator.CreateInstance(typeRecord);

                identifierFieldInfo.SetValue(recordInstance, sheetRow.identifier.GetIdentifierEnum(identifierFieldInfo.FieldType));

                for (int j = 0; j < sheetPage.columns.Count; j++)
                {
                    SheetColumn sheetColumn = sheetPage.columns[j];
                    FieldInfo propertyInfo = typeRecord.GetField("_" + sheetColumn.propertyName.FirstLetterToLower(), BindingFlags.NonPublic | BindingFlags.Instance);
                    object data = sheetRow.cells[j].data;
                    if (sheetColumn.dataType == SheetDataType.Reference)
                    {
                        if (sheetColumn.isCollection)
                        {
                            Type identifierType = propertyInfo.FieldType.GetElementType();
                            string[] dataStrings = data as string[];
                            Array dataArray = Array.CreateInstance(identifierType, dataStrings.Length);
                            for (int h = 0; h < dataStrings.Length; h++)
                                dataArray.SetValue(dataStrings[h].GetIdentifierEnum(identifierType), h);

                            data = dataArray;
                        }
                        else
                        {
                            Type identifierType = propertyInfo.FieldType;
                            data = ((string)data).GetIdentifierEnum(identifierType);
                        }
                    }
                    propertyInfo.SetValue(recordInstance, data);
                }

                recordCollection.SetValue(recordInstance, i);
            }

            recordsMemberInfo.SetValue(instance, recordCollection);
            EditorUtility.SetDirty(instance);
        }
        #endregion

        #region Model Deconstruction

        public static SheetPage DeconstructDatasheetCode(DatasheetType datasheetType)
        {
            SheetPage sheetPage = new SheetPage();
            sheetPage.sheetName = datasheetType.GetIdentifier();
            sheetPage.index = (int)datasheetType;

            Type recordType = Type.GetType(SheetStringDefinitions.NAMESPACE + "." + sheetPage.sheetRecordName + SheetStringDefinitions.ASSEMBLY_LOCATION);

            FieldInfo[] fieldInfos = recordType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfos = fieldInfos.Where(i => i.IsPublic || i.GetCustomAttribute<SerializeField>() != null).ToArray();
            sheetPage.columns = new List<SheetColumn>(fieldInfos.Length);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];

                SheetColumn column = new SheetColumn();
                sheetPage.columns.Add(column);
                ColumnName columnNameAttribute = fieldInfo.GetCustomAttribute<ColumnName>();
                column.serializationName = columnNameAttribute.columnName;
                column.propertyName = fieldInfo.Name.Remove(0, 1).FirstLetterToUpper();
                column.isCollection = fieldInfo.FieldType.IsArray;
                column.dataType = GetDataType(fieldInfo.FieldType);
                if (column.dataType == SheetDataType.Reference)
                {
                    string typeName = fieldInfo.FieldType.Name;
                    string datasheetName;
                    if (column.isCollection)
                        datasheetName = typeName.Remove(typeName.Length - SheetStringDefinitions.IDENTIFIER_SUFFIX.Length - SheetStringDefinitions.ARRAY_VARIABLE_SUFFIX.Length);
                    else
                        datasheetName = typeName.Remove(typeName.Length - SheetStringDefinitions.IDENTIFIER_SUFFIX.Length);

                    column.referenceSheet = Enum.Parse(typeof(DatasheetType), datasheetName).GetIdentifier();
                }
            }

            Type modelType = Type.GetType(SheetStringDefinitions.NAMESPACE + "." + sheetPage.sheetModelNameUpperCase + SheetStringDefinitions.ASSEMBLY_LOCATION);
            object modelObject = AssetDatabase.LoadAssetAtPath(sheetPage.scriptableObjectFileLocation, modelType);
            FieldInfo recordsFieldInfo = modelType.GetField(SheetStringDefinitions.RECORDS_PROPERTY_NAME, BindingFlags.NonPublic | BindingFlags.Instance);
            Array modelRecords = (Array)recordsFieldInfo.GetValue(modelObject);

            Type recordBaseType = recordType.BaseType;
            FieldInfo recordIdentifierFieldInfo = recordBaseType.GetField(SheetStringDefinitions.RECORD_IDENTIFIER_VARIABLE, BindingFlags.NonPublic | BindingFlags.Instance);

            Type identifierType = Type.GetType(sheetPage.sheetIdentifierName + SheetStringDefinitions.ASSEMBLY_LOCATION);
            sheetPage.rows = new List<SheetRow>(modelRecords.Length);
            for (int i = 0; i < modelRecords.Length; i++)
            {
                object record = modelRecords.GetValue(i);

                SheetRow sheetRow = new SheetRow();
                sheetPage.rows.Add(sheetRow);

                object identifierEnumValue = recordIdentifierFieldInfo.GetValue(record);
                sheetRow.index = (int)identifierEnumValue;
                sheetRow.identifier = identifierEnumValue.GetIdentifier();
                sheetRow.enumValue = identifierEnumValue.ToString();

                sheetRow.cells = new List<SheetCell>(fieldInfos.Length);
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    FieldInfo fieldInfo = fieldInfos[j];
                    SheetColumn column = sheetPage.columns[j];
                    object data = fieldInfo.GetValue(record);
                    if (data == null)
                        data = column.GetDefaultCellValue();
                    if (column.dataType == SheetDataType.Reference)
                    {
                        if (column.isCollection)
                        {
                            Array arrayData = ((Array)data);
                            string[] stringData = new string[arrayData.Length];
                            for (int h = 0; h < arrayData.Length; h++)
                                stringData[h] = arrayData.GetValue(h).GetIdentifier();

                            data = stringData;
                        }
                        else
                            data = data.GetIdentifier();
                    }

                    SheetCell cell = new SheetCell(data);
                    sheetRow.cells.Add(cell);
                }
            }

            return sheetPage;
        }

        private static SheetDataType GetDataType(Type type)
        {
            if (type == typeof(long) || type == typeof(long[]))
                return SheetDataType.Long;
            if (type == typeof(int) || type == typeof(int[]))
                return SheetDataType.Int;
            if (type == typeof(short) || type == typeof(short[]))
                return SheetDataType.Short;
            if (type == typeof(byte) || type == typeof(byte[]))
                return SheetDataType.Byte;
            if (type == typeof(double) || type == typeof(double[]))
                return SheetDataType.Double;
            if (type == typeof(float) || type == typeof(float[]))
                return SheetDataType.Float;
            if (type == typeof(string) || type == typeof(string[]))
                return SheetDataType.String;
            if (type == typeof(bool) || type == typeof(bool[]))
                return SheetDataType.Boolean;
            if (type.IsEnum)
                return SheetDataType.Reference;
            if (type.IsArray && type.GetElementType().IsEnum)
                return SheetDataType.Reference;

            throw new MissingMemberException(string.Format("GetDataType could not convert to a proper type {0}", type.Name));
        }

        #endregion
    }
}