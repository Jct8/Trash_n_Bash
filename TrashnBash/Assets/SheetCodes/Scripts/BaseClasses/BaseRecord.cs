using System;
using UnityEngine;

namespace SheetCodes
{
    [Serializable]
    public abstract class BaseRecord<T> where T : struct, IConvertible
    {
        [SerializeField] private T identifier = default;
        public T Identifier { get { return identifier; } protected set { identifier = value; } }

        public abstract void CreateEditableCopy();
        public abstract void SaveToScriptableObject();
    }
}