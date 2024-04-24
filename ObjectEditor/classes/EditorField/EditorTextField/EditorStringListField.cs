using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorStringListField : EditorTextField<List<string>>
    {
        public EditorStringListField(string Description, string Category, double SortIndex, FieldData ValueField) : base(ValueField, null)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
        }
        public override string Text(object ObjectBeingEditted)
        {
            List<string> strs = GetValue(ObjectBeingEditted);
            if (strs == null)
                return "";
            return strs.Collapse(", ");
        }
        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            List<string> strs = text.Expand(',', true, StringSplitOptions.RemoveEmptyEntries, true);
            SetValue(ObjectBeingEditted, strs, true);
        }
    }
}
