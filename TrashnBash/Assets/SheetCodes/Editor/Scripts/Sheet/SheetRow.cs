using SheetCodes;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SheetCodesEditor
{
    public class SheetRow
    {
        public int index;
        public List<SheetCell> cells;

        public string identifier;
        public string enumValue;

        public SheetRow() { }
        public SheetRow(SheetPage sheetPage, int index, string identifier, string enumValue, int insertIndex)
        {
            this.index = index;
            this.identifier = identifier;
            this.enumValue = enumValue;

            cells = new List<SheetCell>(sheetPage.columns.Count);

            for (int i = 0; i < sheetPage.columns.Count; i++)
            {
                SheetColumn column = sheetPage.columns[i];
                object value = column.GetDefaultCellValue();
                cells.Add(new SheetCell(value));
            }

            if (insertIndex < sheetPage.rows.Count)
                sheetPage.rows.Insert(insertIndex, this);
            else
                sheetPage.rows.Add(this);
        }

        public SheetRow(SheetPage sheetPage, SheetRowJsonable sheetRowJson)
        {
            index = sheetRowJson.index;
            identifier = sheetRowJson.identifier;

            cells = new List<SheetCell>(sheetPage.columns.Count);

            for (int i = 0; i < sheetPage.columns.Count; i++)
            {
                SheetColumn column = sheetPage.columns[i];
                object value = column.dataType.GetDataValue(sheetRowJson.data[i], column.isCollection);
                cells.Add(new SheetCell(value));
            }
        }

        public void ChangeIdentifier(DataSheet dataSheet, SheetPage sheetPage, string newIdentifier)
        {
            foreach (SheetPage datasheetPage in dataSheet.datasheetPages)
            {
                for (int i = 0; i < datasheetPage.columns.Count; i++)
                {
                    SheetColumn datasheetColumn = datasheetPage.columns[i];

                    if (datasheetColumn.dataType != SheetDataType.Reference)
                        continue;

                    if (datasheetColumn.referenceSheet != sheetPage.sheetName)
                        continue;

                    if (datasheetColumn.isCollection)
                    {
                        for (int j = 0; j < datasheetPage.rows.Count; j++)
                        {
                            SheetRow datasheetRow = datasheetPage.rows[j];
                            SheetCell datasheetCell = datasheetRow.cells[i];
                            string[] dataArray = datasheetCell.data as string[];
                            for (int h = 0; h < dataArray.Length; h++)
                            {
                                if (dataArray[h] == identifier)
                                    dataArray[h] = newIdentifier;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < datasheetPage.rows.Count; j++)
                        {
                            SheetRow datasheetRow = datasheetPage.rows[j];
                            SheetCell datasheetCell = datasheetRow.cells[i];
                            if ((string)datasheetCell.data == identifier)
                                datasheetCell.data = newIdentifier;
                        }
                    }
                }
            }
        }

        public bool CheckIfSameCodebase(SheetRow other)
        {
            if (identifier != other.identifier)
                return false;

            if (enumValue != other.enumValue)
                return false;

            if (index != other.index)
                return false;

            return true;
        }
    }
}