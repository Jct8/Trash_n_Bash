using System;

namespace SheetCodesEditor
{
    [Serializable]
    public class SheetPageJsonable
    {
        public int index;
        public string sheetName;
        public SheetRowJsonable[] rows;
        public SheetColumnJsonable[] columns;

        public SheetPageJsonable(SheetPage sheetPage)
        {
            index = sheetPage.index;
            sheetName = sheetPage.sheetName;

            rows = new SheetRowJsonable[sheetPage.rows.Count];
            for (int i = 0; i < rows.Length; i++)
                rows[i] = new SheetRowJsonable(sheetPage, sheetPage.rows[i]);

            columns = new SheetColumnJsonable[sheetPage.columns.Count];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = new SheetColumnJsonable(sheetPage.columns[i]);
        }
    }
}
