using System;

namespace SheetCodes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ColumnName : Attribute
    {
        public readonly string columnName;

        public ColumnName(string columnName)
        {
            this.columnName = columnName;
        }
    }
}