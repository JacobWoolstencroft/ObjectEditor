using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorIntField : EditorTextField<int?>
    {
        public EditorIntField(string Description, string Category, double SortIndex, string NullValueDescriptor, FieldData ValueField) : base(ValueField, NullValueDescriptor)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
        }
        public override string Text(object ObjectBeingEditted)
        {
            int? value = GetValue(ObjectBeingEditted);
            if (value == null)
                return "";
            return value.Value.ToString();
        }

        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (NullValueDescriptor != null)
                    SetValue(ObjectBeingEditted, null, true);
            }
            else if (int.TryParse(text, out int i))
                SetValue(ObjectBeingEditted, i, true);
        }
    }
}
