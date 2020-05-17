using System;

namespace SheetCodesEditor
{
    [Serializable]
    public class SheetRowJsonable
    {
        public int index;
        public string identifier;
        public string enumIdentifier;
        public string[] data;

        public SheetRowJsonable(SheetPage sheetPage, SheetRow sheetRow)
        {
            index = sheetRow.index;
            identifier = sheetRow.identifier;
            enumIdentifier = sheetRow.enumValue;

            data = new string[sheetPage.columns.Count];

            for (int i = 0; i < sheetPage.columns.Count; i++)
            {
                SheetColumn column = sheetPage.columns[i];
                SheetCell cell = sheetRow.cells[i];
                data[i] = column.dataType.GetStringValue(cell.data, column.isCollection);
            }
        }
    }
}
