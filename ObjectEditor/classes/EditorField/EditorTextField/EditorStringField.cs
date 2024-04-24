using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal class EditorStringField : EditorTextField<string>
    {
        public string EmptyValueDescriptor = null;
        private EmptyStringModes EmptyStringMode;
        private StringModes StringMode;

        public override string Text(object ObjectBeingEditted)
        {
            return GetValue(ObjectBeingEditted);
        }

        internal EditorStringField(string Description, string Category, double SortIndex, string EmptyValueDescriptor, EmptyStringModes EmptyStringMode, StringModes StringMode, FieldData ValueField) : base(ValueField, EmptyValueDescriptor)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.EmptyValueDescriptor = EmptyValueDescriptor;
            this.EmptyStringMode = EmptyStringMode;
            this.StringMode = StringMode;
        }

        private bool ApplyStringModes(ref string text)
        {
            if (text != null)
            {
                switch (StringMode)
                {
                    case StringModes.Trim:
                        text = text.Trim();
                        break;
                }
            }
            if (string.IsNullOrEmpty(text))
            {
                switch (EmptyStringMode)
                {
                    case EmptyStringModes.Blank:
                        text = "";
                        return true;
                    case EmptyStringModes.Null:
                        text = null;
                        return true;
                    case EmptyStringModes.NotAllowed:
                        return false;
                }
            }
            return true;
        }

        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            if (StringMode == StringModes.Trim && text != null)
                text = text.Trim();
            SetValue(ObjectBeingEditted, text, true);
        }
        protected override DataGridViewCell MakeDataGridViewCell(object ObjectBeingEditted)
        {
            DataGridViewCell cell = base.MakeDataGridViewCell(ObjectBeingEditted);
            if (cell is TextBoxCell txt)
            {
                txt.StringMode = StringMode;
            }

            string str = GetValue(ObjectBeingEditted);
            string formatted = str;
            if (ApplyStringModes(ref formatted))
                SetValue(ObjectBeingEditted, str, false);

            return cell;
        }
        public override bool IsValid(object ObjectBeingEditted)
        {
            string str = GetValue(ObjectBeingEditted);
            if (ApplyStringModes(ref str))
            {
                SetValue(ObjectBeingEditted, str, false);
                return base.IsValid(ObjectBeingEditted);
            }
            else
                return false;
        }
    }
}
