using System;

namespace SheetCodesEditor
{
    [Serializable]
    public class SheetColumnJsonable
    {
        public string serializationName;
        public string propertyName;
        public SheetDataType dataType;
        public bool isCollection;
        public string referenceSheet;

        public SheetColumnJsonable(SheetColumn sheetColumn)
        {
            serializationName = sheetColumn.serializationName;
            propertyName = sheetColumn.propertyName;
            dataType = sheetColumn.dataType;
            isCollection = sheetColumn.isCollection;
            referenceSheet = sheetColumn.referenceSheet;
        }
    }
}
