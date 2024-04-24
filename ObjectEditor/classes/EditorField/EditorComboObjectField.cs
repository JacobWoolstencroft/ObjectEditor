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

            foreach (string option in options)
                cmb.Items.Add(option);
            return cmb;
        }
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
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
