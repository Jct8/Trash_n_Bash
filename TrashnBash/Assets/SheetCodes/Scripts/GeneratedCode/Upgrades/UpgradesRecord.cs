using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class UpgradesRecord : BaseRecord<UpgradesIdentifier>
	{
		[ColumnName("UpgradeLevel")] [SerializeField] private int _upgradelevel = default;
		public int Upgradelevel { get { return _upgradelevel; } set { if(!CheckEdit()) return; _upgradelevel = value; }}

		[ColumnName("Upgrade Type")] [SerializeField] private string _upgradeType = default;
		public string UpgradeType { get { return _upgradeType; } set { if(!CheckEdit()) return; _upgradeType = value; }}

		[ColumnName("Description")] [SerializeField] private string _description = default;
		public string Description { get { return _description; } set { if(!CheckEdit()) return; _description = value; }}

		[ColumnName("Trash Cost")] [SerializeField] private float _trashCost = default;
		public float TrashCost { get { return _trashCost; } set { if(!CheckEdit()) return; _trashCost = value; }}

		[ColumnName("Modifier Value")] [SerializeField] private float _modifierValue = default;
		public float ModifierValue { get { return _modifierValue; } set { if(!CheckEdit()) return; _modifierValue = value; }}

		[ColumnName("Target")] [SerializeField] private string _target = default;
		public string Target { get { return _target; } set { if(!CheckEdit()) return; _target = value; }}
		/*PROPERTIES*/

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public UpgradesModel model { get { return ModelManager.UpgradesModel; } }
        private UpgradesRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            UpgradesRecord editableCopy = new UpgradesRecord();
            editableCopy.Identifier = Identifier;
            editableCopy.originalRecord = this;
            CopyData(editableCopy);
            model.SetEditableCopy(editableCopy);
#else
            Debug.LogError("SheetCodes: Creating an editable record does not work in buolds. See documentation 'Editing your data at runtime' for more information.");
#endif
        }

        public override void SaveToScriptableObject()
        {
#if UNITY_EDITOR
            if (!runtimeEditingEnabled)
            {
                Debug.LogWarning("SheetCodes: Runtime Editing is not enabled for this object. Either you are not using the editable copy or you're trying to edit in a build.");
                return;
            }
            CopyData(originalRecord);
            model.SaveModel();
#else
            Debug.LogError("SheetCodes: Saving to ScriptableObject does not work in builds. See documentation 'Editing your data at runtime' for more information.");
#endif
        }

        private void CopyData(UpgradesRecord record)
        {
			/*COPY_PROPERTIES*/
        }

        private bool CheckEdit()
        {
            if (runtimeEditingEnabled)
                return true;

            Debug.LogWarning("SheetCodes: Runtime Editing is not enabled for this object. Either you are not using the editable copy or you're trying to edit in a build.");
            return false;
        }
    }
}
