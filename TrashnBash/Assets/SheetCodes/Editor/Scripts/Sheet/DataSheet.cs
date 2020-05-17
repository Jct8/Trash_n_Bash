using SheetCodes;
using System;

namespace SheetCodesEditor
{
    public class DataSheet
    {
        public SheetPage[] datasheetPages;
        public bool dirty;

        public DataSheet()
        {
            DatasheetType[] datasheetTypes = Enum.GetValues(typeof(DatasheetType)) as DatasheetType[];

            datasheetPages = new SheetPage[datasheetTypes.Length];
            for (int i = 0; i < datasheetTypes.Length; i++)
                datasheetPages[i] = ModelGeneration.DeconstructDatasheetCode(datasheetTypes[i]);
        }
    }
}
