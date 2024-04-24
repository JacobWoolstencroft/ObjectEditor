using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorBoolField : EditorComboField<bool?>
    {
        private string TrueVal = "YES";
        private string FalseVal = "NO";
        private string NullVal = null;
        public EditorBoolField(string Description, string Category, double SortIndex, string NullValueDescriptor, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.NullVal = NullValueDescriptor;
        }
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
            cell.Value = Text(ObjectBeingEditted);
        }
        public override List<string> GetOptions()
        {
            List<string> options = new List<string>();
            if (NullVal != null)
                options.Add(NullVal);
            options.Add(TrueVal);
            options.Add(FalseVal);
            return options;
        }
        public override string Text(object ObjectBeingEditted)
        {
            bool? value = GetValue(ObjectBeingEditted);

            if (value == null)
                return NullVal;
            else
            {
                if (value.Value)
                    return TrueVal;
                else
                    return FalseVal;
            }
        }
        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            if (NullVal != null && text == NullVal)
                SetValue(ObjectBeingEditted, null, true);
            else if (text == TrueVal)
                SetValue(ObjectBeingEditted, true, true);
            else if (text == FalseVal)
                SetValue(ObjectBeingEditted, false, true);
        }
    }
}
