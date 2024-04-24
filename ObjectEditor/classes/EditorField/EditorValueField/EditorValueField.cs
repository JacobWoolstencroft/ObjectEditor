using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectEditor
{
    internal abstract class EditorValueField : EditorField
    {
        internal FieldData ValueField;
        internal FieldData ValidateField;
        public object OriginalValue;
        public bool IsReadOnly => (ValueField == null || ValueField.ReadOnly);
        internal MethodData OnChangeMethod = null;

        internal MethodData OnButtonClickMethod = null;
        internal string ButtonText = null;

        internal EditorValueField(FieldData ValueField)
        {
            this.ValueField = ValueField;
        }

        public override bool VisibleOnList()
        {
            return true;
        }
        protected void RaiseOnChange(object ObjectBeingEditted)
        {
            if (OnChangeMethod != null)
            {
                OnChangeMethod.Execute(ObjectBeingEditted);
            }
        }

        public void SetFieldValue(object ObjectBeingEditted, object val, bool TriggerOnChange)
        {
            if (IsReadOnly)
                return;
            ValueField.SetValue(ObjectBeingEditted, val);
            if (TriggerOnChange)
            {
                RaiseOnChange(ObjectBeingEditted);
            }
        }
        public object GetFieldValue(object ObjectBeingEditted)
        {
            if (ValueField == null)
                return null;
            return ValueField.GetValue(ObjectBeingEditted);
        }

        public void ResetFieldValue(object ObjectBeingEditted)
        {
            if (IsReadOnly)
                return;
            ValueField.SetValue(ObjectBeingEditted, OriginalValue);
        }
        public void SaveFieldValue(object ObjectBeingEditted)
        {
            if (ValueField == null)
                return;
            OriginalValue = ValueField.GetValue(ObjectBeingEditted);
        }

        public virtual bool IsValid(object ObjectBeingEditted)
        {
            if (ValidateField != null && !(bool)ValidateField.GetValue(ObjectBeingEditted))
                return false;
            return true;
        }
        public void CellValueChanged(object value, object ObjectBeingEditted)
        {
            try
            {
                CellValueChanging(value, ObjectBeingEditted);
            }
            catch (Exception ex)
            {
            }
        }
        protected abstract void CellValueChanging(object value, object ObjectBeingEditted);
    }
}
