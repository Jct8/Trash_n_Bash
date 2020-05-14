using System;

namespace SheetCodesEditor
{
    public class SheetColumn
    {
        public string serializationName;
        public string propertyName;
        public SheetDataType dataType;
        public bool isCollection;
        public string referenceSheet;

        public SheetColumn() { }

        public SheetColumn(SheetPage sheetPage, string serializationName, string propertyName, SheetDataType dataType, string referenceSheet, bool isCollection, int insertIndex)
        {
            if (insertIndex < sheetPage.columns.Count)
                sheetPage.columns.Insert(insertIndex, this);
            else
                sheetPage.columns.Add(this);

            this.serializationName = serializationName;
            this.propertyName = propertyName;
            this.dataType = dataType;
            this.isCollection = isCollection;
            this.referenceSheet = referenceSheet;

            object defaultData = dataType.GetDefaultValue(isCollection);

            foreach (SheetRow sheetRow in sheetPage.rows)
            {
                SheetCell sheetCell = new SheetCell(defaultData);
                if (insertIndex < sheetPage.columns.Count)
                    sheetRow.cells.Insert(insertIndex, sheetCell);
                else
                    sheetRow.cells.Add(sheetCell);
            }
        }

        public SheetColumn(SheetColumnJsonable jsonable)
        {
            serializationName = jsonable.serializationName;
            propertyName = jsonable.propertyName;
            dataType = jsonable.dataType;
            isCollection = jsonable.isCollection;
            referenceSheet = jsonable.referenceSheet;
        }

        public bool CheckIfSameCodebase(SheetColumn other)
        {
            if (serializationName != other.serializationName)
                return false;

            if (propertyName != other.propertyName)
                return false;

            if (dataType != other.dataType)
                return false;

            if (isCollection != other.isCollection)
                return false;

            if (dataType == SheetDataType.Reference && referenceSheet != other.referenceSheet)
                return false;

            return true;
        }

        public object GetDefaultCellValue()
        {
            return dataType.GetDefaultValue(isCollection);
        }
    }
}