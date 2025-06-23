using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorComboObjectField : EditorValueField
    {
        private List<object> options;
        private string NullValueDescriptor = null;
        private FieldData OptionsField = null;

        internal EditorComboObjectField(string Description, string Category, double SortIndex, List<object> options, string NullValueDescriptor, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.options = new List<object>();

            if (NullValueDescriptor == null)
                NullValueDescriptor = "";
            this.NullValueDescriptor = NullValueDescriptor;

            if (options != null)
            {
                this.options.AddRange(options);
            }
        }
        internal EditorComboObjectField(string Description, string Category, double SortIndex, FieldData OptionsField, string NullValueDescriptor, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.options = new List<object>();
            this.OptionsField = OptionsField;

            if (NullValueDescriptor == null)
                NullValueDescriptor = "";
            this.NullValueDescriptor = NullValueDescriptor;
        }

        public override string ListValue(object ObjectBeingEditted)
        {
            object ob = GetFieldValue(ObjectBeingEditted);
            if (ob == null)
                return NullValueDescriptor;
            return ob.ToString();
        }

        protected override DataGridViewCell MakeDataGridViewCell(object ObjectBeingEditted)
        {
            DataGridViewComboBoxCell cmb = new DataGridViewComboBoxCell();

            if (OptionsField != null)
            {
                foreach (string option in options)
                    cmb.Items.Add(option);
            }
            return cmb;
        }
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
            if (OptionsField != null)
            {
                this.options = new List<object>();
                if (NullValueDescriptor != null)
                    this.options.Add(null);
                List<object> options = OptionsField.GetValue_ObjectList(ObjectBeingEditted, ValueField.memberType);
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
                        if (!this.options.Contains(cmb.Items[x]))
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
            object value = GetFieldValue(ObjectBeingEditted);
            cell.Value = value;
        }

        protected override void CellValueChanging(object value, object ObjectBeingEditted)
        {
            if (options.Contains(value))
                SetFieldValue(ObjectBeingEditted, value, true);
        }
    }
}
