using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal class EditorComboStringField : EditorComboField<string>
    {
        private List<string> options;
        private FieldData OptionsField;

        private string NullValueDescriptor = null;

        public EditorComboStringField(string Description, string Category, double SortIndex, List<string> options, EmptyStringModes EmptyStringMode, string NullValueDescriptor, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.options = new List<string>();
            this.OptionsField = null;

            if (NullValueDescriptor == null)
                NullValueDescriptor = "";

            switch (EmptyStringMode)
            {
                case EmptyStringModes.Blank:
                case EmptyStringModes.Null:
                    break;
                case EmptyStringModes.NotAllowed:
                default:
                    NullValueDescriptor = null;
                    break;
            }
            this.NullValueDescriptor = NullValueDescriptor;

            if (NullValueDescriptor != null)
            {
                this.options.Add(null);
            }

            if (options != null)
            {
                this.options.AddRange(options);
            }

            for (int x = 0; x < this.options.Count; x++)
            {
                if (this.options[x] == null)
                    this.options[x] = NullValueDescriptor;
            }
        }
        public EditorComboStringField(string Description, string Category, double SortIndex, FieldData OptionsField, string NullValueDescriptor, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.OptionsField = OptionsField;
            options = new List<string>();

            if (NullValueDescriptor == null)
                NullValueDescriptor = "";
            this.NullValueDescriptor = NullValueDescriptor;
        }
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
            if (OptionsField != null)
            {
                this.options = new List<string>();
                if (NullValueDescriptor != null)
                    this.options.Add(null);
                List<string> options = OptionsField.GetValue_StringList(ObjectBeingEditted);
                if (options != null)
                    this.options.AddRange(options);
                for (int x = 0; x < this.options.Count; x++)
                {
                    if (this.options[x] == null)
                        this.options[x] = NullValueDescriptor;
                }
                if (cell is ComboBoxCell cmb)
                {
                    for (int x = 0; x < cmb.Items.Count; x++)
                    {
                        if (!this.options.Contains(cmb.Items[x].ToString()))
                        {
                            cmb.Items.RemoveAt(x);
                            x--;
                        }
                    }
                    for (int x = 0; x < this.options.Count; x++)
                    {
                        if (!cmb.Items.Contains(this.options[x]))
                            cmb.Items.Add(this.options[x]);
                    }
                }
            }
            cell.Value = Text(ObjectBeingEditted);
        }

        public override List<string> GetOptions()
        {
            return options;
        }

        public override string Text(object ObjectBeingEditted)
        {
            return GetValue(ObjectBeingEditted);
        }

        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            if (NullValueDescriptor != null && text == NullValueDescriptor)
            {
                SetValue(ObjectBeingEditted, null, true);
                return;
            }
            if (options.Contains(text))
                SetValue(ObjectBeingEditted, text, true);
        }
        public override bool IsValid(object ObjectBeingEditted)
        {
            string text = GetValue(ObjectBeingEditted);
            if (NullValueDescriptor != null && text == null)
                return base.IsValid(ObjectBeingEditted);
            if (options.Contains(text))
                return base.IsValid(ObjectBeingEditted);
            return false;
        }
    }
}
