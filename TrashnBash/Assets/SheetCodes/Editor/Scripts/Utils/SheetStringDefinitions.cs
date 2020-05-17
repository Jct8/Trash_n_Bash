namespace SheetCodesEditor
{
    public static class SheetStringDefinitions
    {
        public const char SPLIT_CHARACTER = ',';
        public const string SPLIT_STRING = ",";
        public const string IDENTIFIER_DEFAULT_VALUE = "None";

        public const string BASE_CLASSES_DIRECTORY = "Assets/SheetCodes/Scripts/GeneratedCode/BaseClasses/";
        public const string TEMPLATE_DIRECTORY = "Assets/SheetCodes/Editor/Templates/";
        public const string MODEL_DIRECTORY = "Assets/SheetCodes/Scripts/GeneratedCode/";
        public const string MODEL_DIRECTORY_BASE = "Assets/SheetCodes/Scripts/GeneratedCode";
        public const string SCRIPTABLEOBJECT_DIRECTORY = "Assets/SheetCodes/Resources/ScriptableObjects/";
        public const string MODELMANAGER_SCRIPTABLEOBJECT_DIRECTORY = "Assets/SheetCodes/Resources/ScriptableObjects/ModelManager.asset";
        public const string ASSEMBLY_LOCATION = ", Assembly-CSharp";

        public const string TEXTURE_DIRECTORY = "Assets/SheetCodes/Editor/Textures/";
        public const string DOTS_TEXTURE_FILE_NAME = "Dots.png";
        public const string COLLECTIONEDIT_TEXTURE_ICON_FILE_NAME = "CollectionEditIcon.png";

        public const string SCRIPT_FILE_EXTENSION = ".cs";
        public const string SCRIPTABLEOBJECT_FILE_EXTENSION = ".asset";
        public const string META_FILE_EXTENSION = ".meta";
        public const string MODELMANAGER_NAME = "ModelManager";

        public const string IDENTIFIER_SUFFIX = "Identifier";
        public const string ARRAY_VARIABLE_SUFFIX = "[]";
        public const string RECORD_IDENTIFIER_VARIABLE = "identifier";

        public const string DATASHEET_IDENTIFIER_FILE_NAME = "DatasheetType.cs";
        public const string MODEL_MANAGER_FILE_NAME = "ModelManager.cs";

        public const string TEMPLATE_REPLACE_MODEL_NAME = "#MODEL_NAME#";
        public const string TEMPLATE_REPLACE_COMPONENT_TYPE = "#COMPONENT_TYPE#";
        public const string TEMPLATE_REPLACE_PROPERTY_IDENTIFIER = "#PROPERTY_IDENTIFIER#";
        public const string TEMPLATE_REPLACE_PROPERTY_NAME_UPPER = "#PROPERTY_NAME_UPPER#";
        public const string TEMPLATE_REPLACE_PROPERTY_NAME_LOWER = "#PROPERTY_NAME_LOWER#";
        public const string TEMPLATE_REPLACE_PROPERTY_TYPE = "#PROPERTY_TYPE#";
        public const string TEMPLATE_REPLACE_REFERENCE_MODEL_NAME = "#REFERENCE_MODEL_NAME#";
        public const string TEMPLATE_REPLACE_ENUM_TYPE = "#ENUM_TYPE#";

        public const string TEMPLATE_REPLACE_IDENTIFIER_NAME = "#RECORD_NAME#";
        public const string TEMPLATE_REPLACE_IDENTIFIER_INDEX = "#RECORD_INDEX#";
        public const string TEMPLATE_REPLACE_IDENTIFIER_NAME_TITLE_CASE = "#RECORD_NAME_TITLE_CASE#";

        public const string TEMPLATE_REPLACE_MODEL_RECORD_TITLE_CASE = "#MODEL_NAME_TITLE_CASE#";
        public const string TEMPLATE_REPLACE_MODEL_RECORD_CAMEL_CASE = "#MODEL_NAME_CAMEL_CASE#";

        public const string TEMPLATE_FILE_MODELMANAGER = "TemplateModelManager.txt";
        public const string TEMPLATE_FILE_MODELMANAGER_RECORD = "TemplateModelManagerRecord.txt";
        public const string TEMPLATE_FILE_MODELMANAGER_ASYNC = "TemplateModelManagerAsync.txt";
        public const string TEMPLATE_FILE_MODELMANAGER_INITIALIZE = "TemplateModelManagerInitialize.txt";
        public const string TEMPLATE_FILE_MODELMANAGER_UNLOAD = "TemplateModelManagerUnload.txt";

        public const string TEMPLATE_FILE_MODEL = "TemplateModel.txt";
        public const string TEMPLATE_FILE_RECORD = "TemplateRecord.txt";

        public const string TEMPLATE_FILE_RECORD_PROPERTY = "TemplateRecordProperty.txt";
        public const string TEMPLATE_FILE_RECORD_REFERENCE = "TemplateRecordReference.txt";

        public const string TEMPLATE_FILE_RECORD_PROPERTY_COLLECTION = "TemplateRecordPropertyCollection.txt";
        public const string TEMPLATE_FILE_RECORD_REFERENCE_COLLECTION = "TemplateRecordReferenceCollection.txt";
        
        public const string TEMPLATE_FILE_IDENTIFIER = "TemplateIdentifier.txt";
        public const string TEMPLATE_FILE_IDENTIFIER_LINE = "TemplateIdentifierLine.txt";

        public const string TEMPLATE_FILE_DATASHEET_TYPE = "TemplateDatasheetType.txt";
        public const string TEMPLATE_FILE_DATASHEET_LINE = "TemplateDatasheetTypeLine.txt";

        public const string TEMPLATE_INSERT_DATASHEETS = "/*DATASHEETS*/";
        public const string TEMPLATE_INSERT_MODELS = "/*MODELS*/";
        public const string TEMPLATE_INSERT_INITIALIZE = "/*INITIALIZE*/";
        public const string TEMPLATE_INSERT_UNLOAD = "/*UNLOAD*/";
        public const string TEMPLATE_INSERT_ASYNC = "/*ASYNC*/";
        public const string TEMPLATE_INSERT_PROPERTIES = "/*PROPERTIES*/";
        public const string TEMPLATE_INSERT_IDENTIFIERS = "/*IDENTIFIERS*/";
        public const string TEMPLATE_INSERT_USINGS = "/*USINGS*/";

        public const string TEMPLATE_REPLACE_DATASHEET_LINE_MODEL_NAME = "#MODEL_NAME#";
        public const string TEMPLATE_REPLACE_DATASHEET_LINE_MODEL_NAME_TITLE_CASE = "#MODEL_NAME_TITLE_CASE#";
        public const string TEMPLATE_REPLACE_DATASHEET_LINE_MODEL_INDEX = "#MODEL_INDEX#";

        public const string RECORDS_PROPERTY_NAME = "records";

        public const string MODEL_RECORD_LOOKUP = "private {0}";

        public const string DATASHEET_LINE_LOOKUP = "[Identifier(\"{0}\")]";

        public const string FILE_LOCATION_TARGET_DATASHEET = "Assets/SheetCodes/Editor/SheetCodesJson.txt";

        public const string NAMESPACE = "SheetCodes";
        public const string NAMESPACE_EDITOR = "SheetCodesEditor";
    }
}