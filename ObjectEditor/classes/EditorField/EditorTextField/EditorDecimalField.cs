using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorDecimalField : EditorTextField<decimal?>
    {
        internal EditorDecimalField(string Description, string Category, double SortIndex, string NullValueDescriptor, FieldData ValueField) : base(ValueField, NullValueDescriptor)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
        }

        public override string Text(object ObjectBeingEditted)
        {
            decimal? value = GetValue(ObjectBeingEditted);
            if (value == null)
                return "";
            return value.Value.ToShortString();
        }

        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (NullValueDescriptor != null)
                    SetValue(ObjectBeingEditted, null, true);
            }
            else if (decimal.TryParse(text, out decimal d))
                SetValue(ObjectBeingEditted, d, true);
        }
    }
}
