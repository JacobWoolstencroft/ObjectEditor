using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorLongField : EditorTextField<long?>
    {
        public EditorLongField(string Description, string Category, double SortIndex, string NullValueDescriptor, FieldData ValueField) : base(ValueField, NullValueDescriptor)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
        }
        public override string Text(object ObjectBeingEditted)
        {
            long? value = GetValue(ObjectBeingEditted);
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
            else if (long.TryParse(text, out long l))
                SetValue(ObjectBeingEditted, l, true);
        }
    }
}
