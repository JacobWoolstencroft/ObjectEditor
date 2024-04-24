using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal abstract class EditorTextField<T> : EditorValueField
    {
        public readonly string NullValueDescriptor;

        internal EditorTextField(FieldData ValueField, string NullValueDescriptor) : base(ValueField)
        {
            this.NullValueDescriptor = NullValueDescriptor;
        }
        public abstract string Text(object ObjectBeingEditted);
        public override string ListValue(object ObjectBeingEditted)
        {
            return Text(ObjectBeingEditted);
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

        protected override DataGridViewCell MakeDataGridViewCell(object ObjectBeingEditted)
        {
            TextBoxCell txt = new TextBoxCell();
            txt.EmptyString = NullValueDescriptor;
            if (OnButtonClickMethod != null)
            {
                txt.ButtonText = this.ButtonText;
                txt.Clicked += (TextBoxCell cell) => btn_Clicked(cell, ObjectBeingEditted);
            }
            return txt;
        }
        private void btn_Clicked(TextBoxCell cell, object ObjectBeingEditted)
        {
            OnButtonClickMethod.Execute(ObjectBeingEditted);
            UpdateCellValue(cell, ObjectBeingEditted);
        }

        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
            cell.Value = Text(ObjectBeingEditted);
        }

    }
}
