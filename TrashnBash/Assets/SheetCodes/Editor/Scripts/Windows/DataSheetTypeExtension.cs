using SheetCodes;
using System;

namespace SheetCodesEditor
{
    public static class DatasheetTypeExtension
    {
        public static Type GetIdentifierType(this DatasheetType datasheetType)
        {
            string identifier = datasheetType.ToString() + SheetStringDefinitions.IDENTIFIER_SUFFIX;
            Type type = typeof(DatasheetType);

            return Type.GetType(SheetStringDefinitions.NAMESPACE + "." + identifier + "," + type.Assembly);
        }

        public static Type[] GetAllIdentifierTypes()
        {
            DatasheetType[] datasheetTypes = (DatasheetType[])Enum.GetValues(typeof(DatasheetType));
            Type[] identifierTypes = new Type[datasheetTypes.Length];
            for (int i = 0; i < datasheetTypes.Length; i++)
                identifierTypes[i] = datasheetTypes[i].GetIdentifierType();

            return identifierTypes;
        }
    }
}