using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal abstract class EditorComboField<T> : EditorValueField
    {
        public EditorComboField(FieldData ValueField) : base(ValueField)
        {
        }

        public abstract List<string> GetOptions();
        public abstract string Text(object ObjectBeingEditted);
        public override string ListValue(object ObjectBeingEditted)
        {
            return Text(ObjectBeingEditted);
        }
        protected override DataGridViewCell MakeDataGridViewCell(object ObjectBeingEditted)
        {
            ComboBoxCell cmb = new ComboBoxCell();

            List<string> options = GetOptions();
            foreach (string option in options)
                cmb.Items.Add(option);
            return cmb;
        }

        public T GetValue(object ObjectBeingEditted)
        {
            return (T)GetFieldValue(ObjectBeingEditted);
        }
        public void SetValue(object ObjectBeingEditted, T Value, bool TriggerOnChange)
        {
            SetFieldValue(ObjectBeingEditted, Value, TriggerOnChange);
        }
        protected override void CellValueChanging(object value, object ObjectBeingEditted)
        {
            CellTextChanging(value == null ? null : value.ToString(), ObjectBeingEditted);
        }
        protected abstract void CellTextChanging(string text, object ObjectBeingEditted);
    }
}
