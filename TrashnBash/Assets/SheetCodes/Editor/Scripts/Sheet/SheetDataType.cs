using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public enum SheetDataType
    {
        [Identifier("long")] Long = 0,
        [Identifier("int")] Int = 1,
        [Identifier("short")] Short = 2,
        [Identifier("byte")] Byte = 3,
        [Identifier("double")] Double = 4,
        [Identifier("float")] Float = 5,
        [Identifier("string")] String = 6,
        [Identifier("bool")] Boolean = 7,
        [Identifier("reference")] Reference = 8,
    }

    public static class SheetDataTypeExtension
    {
        #region Default Value
        public static object GetDefaultValue(this SheetDataType dataType, bool isCollection)
        {
            if (isCollection)
                return dataType.GetDefaultValue_Collection();
            else
                return dataType.GetDefaultValue_Single();
        }

        private static object GetDefaultValue_Single(this SheetDataType dataType)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return default(long);
                case SheetDataType.Int:
                    return default(int);
                case SheetDataType.Short:
                    return default(short);
                case SheetDataType.Byte:
                    return default(byte);
                case SheetDataType.Double:
                    return default(double);
                case SheetDataType.Float:
                    return default(float);
                case SheetDataType.String:
                    return string.Empty;
                case SheetDataType.Boolean:
                    return default(bool);
                case SheetDataType.Reference:
                    return "None";
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDEFAULTVALUE_SINGLE, dataType));
        }

        private static object GetDefaultValue_Collection(this SheetDataType dataType)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return new long[0];
                case SheetDataType.Int:
                    return new int[0];
                case SheetDataType.Short:
                    return new short[0];
                case SheetDataType.Byte:
                    return new byte[0];
                case SheetDataType.Double:
                    return new double[0];
                case SheetDataType.Float:
                    return new float[0];
                case SheetDataType.String:
                    return new string[0];
                case SheetDataType.Boolean:
                    return new bool[0];
                case SheetDataType.Reference:
                    return new string[0];
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDEFAULTVALUE_COLLECTION, dataType));
        }
        #endregion

        #region String to data
        public static object GetDataValue(this SheetDataType dataType, string value, bool isCollection)
        {
            if (isCollection)
                return dataType.GetDataValue_Collection(value);
            else
                return dataType.GetDataValue_Single(value);
        }

        private static object GetDataValue_Single(this SheetDataType dataType, string value)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return long.Parse(value);
                case SheetDataType.Int:
                    return int.Parse(value);
                case SheetDataType.Short:
                    return short.Parse(value);
                case SheetDataType.Byte:
                    return byte.Parse(value);
                case SheetDataType.Double:
                    return double.Parse(value);
                case SheetDataType.Float:
                    return float.Parse(value);
                case SheetDataType.String:
                    return value;
                case SheetDataType.Boolean:
                    return bool.Parse(value);
                case SheetDataType.Reference:
                    return value;
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDATAVALUE_SINGLE, dataType));
        }

        private static object GetDataValue_Collection(this SheetDataType dataType, string value)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return GetDataCollection_Long(value);
                case SheetDataType.Int:
                    return GetDataCollection_Int(value);
                case SheetDataType.Short:
                    return GetDataCollection_Short(value);
                case SheetDataType.Byte:
                    return GetDataCollection_Byte(value);
                case SheetDataType.Double:
                    return GetDataCollection_Double(value);
                case SheetDataType.Float:
                    return GetDataCollection_Float(value);
                case SheetDataType.String:
                    return GetDataCollection_String(value);
                case SheetDataType.Boolean:
                    return GetDataCollection_Bool(value);
                case SheetDataType.Reference:
                    return GetDataCollection_String(value);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDATAVALUE_COLLECTION, dataType));
        }

        private static long[] GetDataCollection_Long(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            long[] convertedValues = new long[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = long.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static int[] GetDataCollection_Int(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            int[] convertedValues = new int[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = int.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static short[] GetDataCollection_Short(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            short[] convertedValues = new short[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = short.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static float[] GetDataCollection_Float(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            float[] convertedValues = new float[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = float.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static double[] GetDataCollection_Double(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            double[] convertedValues = new double[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = double.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static byte[] GetDataCollection_Byte(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            byte[] convertedValues = new byte[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = byte.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static bool[] GetDataCollection_Bool(string value)
        {
            string[] valueSplit = GetDataCollection_String(value);
            bool[] convertedValues = new bool[valueSplit.Length];

            for (int i = 0; i < valueSplit.Length; i++)
                convertedValues[i] = bool.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static string[] GetDataCollection_String(string value)
        {
            List<string> stringSplit = new List<string>();

            while (value.Length > 0)
            {
                int index = value.IndexOf(' ');
                int stringSize = int.Parse(value.Substring(0, index));
                string stringItem = value.Substring(index + 1, stringSize);
                stringSplit.Add(stringItem);
                int totalSize = index + stringItem.Length + 1;
                value = value.Substring(totalSize, value.Length - totalSize);
            }

            return stringSplit.ToArray();
        }
        #endregion

        #region Data to string
        public static string GetStringValue(this SheetDataType dataType, object value, bool isCollection)
        {
            if (isCollection)
                return dataType.GetStringValue_Collection(value);
            else
                return dataType.GetStringValue_Single(value);
        }

        private static string GetStringValue_Single(this SheetDataType dataType, object value)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return value.ToString();
                case SheetDataType.Int:
                    return value.ToString();
                case SheetDataType.Short:
                    return value.ToString();
                case SheetDataType.Byte:
                    return value.ToString();
                case SheetDataType.Double:
                    return value.ToString();
                case SheetDataType.Float:
                    return value.ToString();
                case SheetDataType.String:
                    return (string)value;
                case SheetDataType.Boolean:
                    return value.ToString();
                case SheetDataType.Reference:
                    return (string)value;
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETSTRINGVALUE_SINGLE, dataType));
        }

        private static string GetStringValue_Collection(this SheetDataType dataType, object value)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return GetStringCollection_Long((long[])value);
                case SheetDataType.Int:
                    return GetStringCollection_Int((int[])value);
                case SheetDataType.Short:
                    return GetStringCollection_Short((short[])value);
                case SheetDataType.Byte:
                    return GetStringCollection_Byte((byte[])value);
                case SheetDataType.Double:
                    return GetStringCollection_Double((double[])value);
                case SheetDataType.Float:
                    return GetStringCollection_Float((float[])value);
                case SheetDataType.String:
                    return GetStringCollection_String((string[])value);
                case SheetDataType.Boolean:
                    return GetStringCollection_Bool((bool[])value);
                case SheetDataType.Reference:
                    return GetStringCollection_String((string[])value);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETSTRINGVALUE_COLLECTION, dataType));
        }

        private static string GetStringCollection_Reference(Array array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertReference(array.GetValue(i)) + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Long(long[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Int(int[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Short(short[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Byte(byte[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Double(double[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Float(float[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Bool(bool[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertString(array[i].ToString());

            return result;
        }

        private static string GetStringCollection_String(string[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertString(array[i]);

            return result;
        }

        private static string ConvertReference(object value)
        {
            return ((short)value).ToString();
        }
        
        private static string ConvertString(string value)
        {
            return value.Length + " " + value;
        }
        #endregion
    }
}