using System;
using System.Collections.Generic;
using System.Reflection;

namespace SheetCodes
{
    public static class IdentifierExtensions
    {
        private static Type identifierType = typeof(Identifier);
        private static Dictionary<Type, Dictionary<string, object>> identifierToEnumCollection;
        private static Dictionary<Type, Dictionary<object, string>> enumToIdentifierCollection;

        static IdentifierExtensions()
        {
            identifierToEnumCollection = new Dictionary<Type, Dictionary<string, object>>();
            enumToIdentifierCollection = new Dictionary<Type, Dictionary<object, string>>();
        }

        public static string GetIdentifier(this object enumValue)
        {
            Type enumType = enumValue.GetType();

            Dictionary<object, string> cachedConversion;
            if (!enumToIdentifierCollection.TryGetValue(enumType, out cachedConversion))
            {
                CreateCacheForEnum(enumType);
                cachedConversion = enumToIdentifierCollection[enumType];
            }

            return cachedConversion[enumValue];
        }

        public static string GetIdentifier<T>(this T enumValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);

            Dictionary<object, string> cachedConversion;
            if (!enumToIdentifierCollection.TryGetValue(enumType, out cachedConversion))
            {
                CreateCacheForEnum<T>();
                cachedConversion = enumToIdentifierCollection[enumType];
            }

            return cachedConversion[enumValue];
        }

        public static T GetIdentifierEnum<T>(this string identifier) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            return (T)GetIdentifierEnum(identifier, enumType);
        }

        public static object GetIdentifierEnum(this string identifier, Type enumType)
        {
            Dictionary<string, object> cachedConversion;

            if (!identifierToEnumCollection.TryGetValue(enumType, out cachedConversion))
            {
                CreateCacheForEnum(enumType);
                cachedConversion = identifierToEnumCollection[enumType];
            }
            return cachedConversion[identifier];
        }

        public static bool TryGetIdentifierEnum<T>(this string identifier, out T result) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            object resultObject;
            if (!TryGetIdentifierEnum(identifier, enumType, out resultObject))
            {
                result = default;
                return false;
            }
            result = (T)resultObject;
            return true;
        }

        public static bool TryGetIdentifierEnum(this string identifier, Type enumType, out object result)
        {
            Dictionary<string, object> cachedConversion;

            if (!identifierToEnumCollection.TryGetValue(enumType, out cachedConversion))
            {
                CreateCacheForEnum(enumType);
                cachedConversion = identifierToEnumCollection[enumType];
            }

            return cachedConversion.TryGetValue(identifier, out result);
        }

        private static void CreateCacheForEnum<T>() where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            CreateCacheForEnum(enumType);
        }

        private static void CreateCacheForEnum(Type enumType)
        {
            Dictionary<object, string> enumToIdentifier = new Dictionary<object, string>();
            Dictionary<string, object> identifierToEnum = new Dictionary<string, object>();

            enumToIdentifierCollection.Add(enumType, enumToIdentifier);
            identifierToEnumCollection.Add(enumType, identifierToEnum);

            Array enumValues = Enum.GetValues(enumType);
            for (int i = 0; i < enumValues.Length; i++)
            {
                object enumValue = enumValues.GetValue(i);

                Type type = enumValue.GetType();
                MemberInfo[] memInfos = type.GetMember(enumValue.ToString());
                Identifier identifier = memInfos[0].GetCustomAttribute(identifierType, false) as Identifier;

                enumToIdentifier.Add(enumValue, identifier.enumIdentifier);
                identifierToEnum.Add(identifier.enumIdentifier, enumValue);
            }

            Type test = typeof(BaseRecord<>);
        }
    }
}