using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SheetCodes
{
    [Serializable]
    public abstract class BaseModel<T1, T2> : ScriptableObject where T1 : BaseRecord<T2> where T2 : struct, IConvertible
    {
        protected abstract T1[] Records { get; }

#if UNITY_EDITOR
        [NonSerialized] protected T1[] editableRecords;
        [NonSerialized] private bool isSavingData = false;
        [NonSerialized] private int editableIndex = 0;

        public void SetEditableCopy(T1 record)
        {
            for (int i = 0; i < editableIndex; i++)
                if (editableRecords[i].Identifier.Equals(record.Identifier))
                    return;

            editableRecords[editableIndex] = record;
            editableIndex++;
        }
#endif

        public void SaveToScriptableObject()
        {
#if UNITY_EDITOR
            isSavingData = true;

            for (int i = 0; i < editableIndex; i++)
                editableRecords[i].SaveToScriptableObject();

            isSavingData = false;

            SaveModel();
#else
            Debug.LogError("SheetCodes: Saving to ScriptableObject does not work in builds. See documentation 'Editing your data at runtime' for information on why this does not work.");
#endif
        }

        public void SaveModel()
        {
#if UNITY_EDITOR
            if (isSavingData)
                return;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#else
            Debug.LogError("SheetCodes: Saving models does not work in builds. See documentation 'Editing your data at runtime' for information on why this does not work.");
#endif
        }

        public T1 GetRecord(T2 identifier, bool editableRecord = false)
        {
#if UNITY_EDITOR
            if (editableRecord)
            {
                if (editableRecords == null)
                    editableRecords = new T1[Records.Length];

                T1 record = null;
                for (int i = 0; i < editableIndex; i++)
                {
                    if (editableRecords[i].Identifier.Equals(identifier))
                    {
                        record = editableRecords[i];
                        break;
                    }
                }

                if (record == null)
                {
                    T1 originalRecord = Records.FirstOrDefault(i => i.Identifier.Equals(identifier));
                    if (originalRecord == null)
                        return null;

                    originalRecord.CreateEditableCopy();
                    for (int i = 0; i < editableIndex; i++)
                    {
                        if (editableRecords[i].Identifier.Equals(identifier))
                        {
                            record = editableRecords[i];
                            break;
                        }
                    }
                }
                return record;
            }
            else
                return Records.FirstOrDefault(i => i.Identifier.Equals(identifier));
#else
            if(editableRecord)
                Debug.LogError("SheetCodes: Editable Records are disabled in builds. See documentation 'Editing your data at runtime' for information on why this does not work.");

            return Records.FirstOrDefault(i => i.Identifier.Equals(identifier));
#endif
        }

        // Justin's Code //
        public int GetTotalUpgrades(UpgradeMenu.Upgrade type)
        {
            int total = 0;

            foreach (var record in Records)
            {
                UpgradesRecord rec = record as UpgradesRecord;
                if (rec.UpgradeType == type.ToString())
                    total++;
            }
            return total;
        }
    }
}