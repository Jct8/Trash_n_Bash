namespace SheetCodesEditor
{
    public static class Localization
    {
        public const string COMPILER_ERRORS_TITLE = "Compiler errors!";
        public const string COMPILER_ERRORS_MESSAGE = "This tool does not work while there are compiler errors.";
        public const string UNSAVED_CHANGES_TITLE = "Unsaved changes";
        public const string UNSAVED_CHANGES_MESSAGE = "Do you want to save any unsaved changes?";

        public const string OK = "Ok";
        public const string YES = "Yes";
        public const string NO = "No";

        public const string EDIT = "Edit";
        public const string INSERT_LEFT = "Insert Left";
        public const string INSERT_RIGHT = "Insert Right";
        public const string INSERT_ABOVE = "Insert Above";
        public const string INSERT_BELOW = "Insert Below";
        public const string DELETE = "Delete";

        public const string ADD = "Add";
        public const string REMOVE = "Remove";
        public const string CLEAR = "Clear";

        public const string ADDITION_ARRAY = "{0} Array";
        public const string ADDITION_REFERENCE = "Reference: {0}";
        public const string ADDITION_REFERENCE_ARRAY = "Reference Array: {0}";

        public const string IDENTIFIER = "Row Name";
        public const string IDENTIFIER_ENUM_INDEX = "Row Name\nEnum Value\nIndex";

        public const string SHOW_ENUM_VALUE = "Show Enum Value";
        public const string SAVE = "Save";
        public const string ADD_ROW = "Add Row";
        public const string ADD_COLUMN = "Add Column";
        public const string ADD_SHEET = "Add Sheet";
        public const string DELETE_SHEET = "Delete Sheet";

        public const string GENERATE = "Generate";

        public const string DELETE_SHEET_MESSAGE_TITLE = "Delete current sheet";
        public const string DELETE_SHEET_MESSAGE_MESSAGE = "Deleting this sheet will also remove reference columns to this sheet in other sheets. Are you sure you want to delete this sheet?";

        public const string EXCEPTION_SHOWGUILAYOUT = "Missing ShowGUILayout implementation for SheetData Type {0}";
        public const string EXCEPTION_GETMINSIZE_COLLECTION = "GetMinSize_Collection is missing implementation for {0}";
        public const string EXCEPTION_GETMINSIZE_SINGLE = "GetMinSize_Sngle is missing implementation for {0}";
        public const string EXCEPTION_GETDEFAULTVALUE_SINGLE = "GetDefaultValue_Single value to string is missing implementation for {0}";
        public const string EXCEPTION_GETDEFAULTVALUE_COLLECTION = "GetDefaultValue_Collection value to string is missing implementation for {0}";
        public const string EXCEPTION_GETDATAVALUE_SINGLE = "GetDataValue_Single value to string is missing implementation for {0}";
        public const string EXCEPTION_GETDATAVALUE_COLLECTION = "GetDataValue_Collection value to string is missing implementation for {0}";
        public const string EXCEPTION_GETSTRINGVALUE_SINGLE = "GetStringValue_Single value to string is missing implementation for {0}";
        public const string EXCEPTION_GETSTRINGVALUE_COLLECTION = "GetStringValue_Collection value to string is missing implementation for {0}";
        public const string EXCEPTION_CONVERTDATA = "Convert Data is missing implementation for {0}";

        public const string NULLOBJECT_TO_STRING = "Empty";
        public const string TRUE = "True";
        public const string FALSE = "False";

        public const string ONLY_ADJUSTABLE_WITH_TOOL = "RECORDS CAN ONLY BE ADJUSTED USING THE DEDICATED TOOL";
        public const string OPEN_WITH_TOOL = "Open in Sheet Codes Free";

        public const string ROW_NAME = "Row Name:";
        public const string ROW_ENUMVALUE = "Enum Value:";
        public const string ROW_INDEX = "Index:";

        public const string ERROR_ROW_IDENTIFIER_EMPTY = "Row Name cannot be empty.";
        public const string ERROR_ROW_IDENTIFIER_MATCHES_NONE = "Row Name: 'None' is a default identifier and is therefore not allowed.";
        public const string ERROR_ROW_IDENTIFIER_MATCH = "Row Name cannot match any other Row Name already in the sheet page.";
        public const string ERROR_ROW_ENUMVALUE_EMPTY = "Enum Value cannot be empty.";
        public const string ERROR_ROW_ENUMVALUE_MATCHES_NONE = "Enum Value 'None' is a default identifier and is therefore not allowed.";
        public const string ERROR_ROW_ENUMVALUE_MATCH = "Enum Value cannot match any other Enum Values already in the sheet page";
        public const string ERROR_ROW_INDEX_LOWER_THAN_ONE = "Index must be higher than 0";
        public const string ERROR_ROW_INDEX_MATCH = "Index cannot match any other index already in the sheet page";

        public const string COLUMN_COLUMN_NAME = "Column Name:";
        public const string COLUMN_PROPERTY_NAME = "Property Name:";
        public const string COLUMN_IS_ARRAY = "Array:";
        public const string COLUMN_DATA_TYPE = "Data type:";
        public const string COLUMN_REFERENCE_SHEET = "Reference Sheet:";
        public const string COLUMN_FILTER = "Filter:";
        public const string COLUMN_COMPONENT_LIST = "Component List:";
        public const string COLUMN_COMPONENT_TYPE = "Component Type:";
        public const string COLUMN_ENUM_TYPE = "Enum Type:";

        public const string ERROR_COLUMN_SERIALIZATIONNAME_EMPTY = "Column Name cannot be empty.";
        public const string ERROR_COLUMN_PROPERTYNAME_EMPTY = "Property Name cannot be empty.";
        public const string ERROR_COLUMN_PROPERTYNAME_MATCHES_IDENTIFIER = "Property Name cannot match any other Property Names already in the sheet page.";
        public const string ERROR_COLUMN_SERIALIZATIONNAME_MATCH = "Column Name cannot match any other Column Names already in the sheet page";
        public const string ERROR_COLUMN_PROPERTYNAME_MATCH = "Property Name cannot match any other Property Names already in the sheet page.";
        public const string ERROR_COLUMN_PROPERTYNAME_MATCH_EXTRA = "Another column that generates an extra property is matching your Property Name.";
        public const string ERROR_COLUMN_COMPONENT_EMPTY = "Select a class type for your column. This can be any class that inherits from UnityEngine.Object.";
        public const string ERROR_COLUMN_ENUM_EMPTY = "Select an Enum Type for your column. This can be any Enum within your project";

        public const string WARNING_COLUMN_CHANGES_RESET_DATA = "Current changes to the column will change all cell values to their default value.";
        public const string WARNING_COLUMN_CHANGES_CAST_DATA = "Current changes to the column will cast it's cell values from {0} to {1}. Some precision data might be lost depending on the cast.";
        public const string WARNING_COLUMN_CHANGES_CASTCOMPONENT_DATA = "Current changes to the column will cause cell values to retrieve {0} from {1}'s GameObject. If the component does not exist the cell value will be set to default";
        public const string WARNING_COLUMN_CHANGES_CASTOBJECT_DATA = "Current changes to the column will set cell values to null if the current cell value is not of type {0}.";
        public const string WARNING_COLUMN_CHANGES_ARRAY_TO_NONARRAY = "Current changes to the column will take the first element from each cell's array and use that as the new cell value.\n";
        public const string WARNING_COLUMN_CHANGES_NONARRAY_TO_ARRAY = "Current changes to the column will take the cell's value and put it as the first element of the Array.\n";

        public const string SHEET_SHEETNAME = "Sheet name";
        public const string SHEET_MODELNAME = "Model Name:";
        public const string SHEET_RECORDNAME = "Record Name:";
        public const string SHEET_IDENTIFIERNAME = "Identifier Name:";

        public const string ERROR_SHEET_SHEETNAME_EMPTY = "Sheet Name cannot be empty.";
        public const string ERROR_SHEET_ENUMVALUE_EMPTY = "Generated script names (model, record, identifier) cannot be empty.";
        public const string ERROR_SHEET_ENUMVALUE_BASECLASSES = "BaseClasses folder is reserved for special generated scripts. Please choose another sheet name.";
        public const string ERROR_SHEET_SHEETNAME_MATCH = "Sheet name cannot match that of another sheet.";
        public const string ERROR_SHEET_ENUMVALUE_MATCH = "Generated script names (model, record, identifier) cannot match that of another sheet.";
    }
}
