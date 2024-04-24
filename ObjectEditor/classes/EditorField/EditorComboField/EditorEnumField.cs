using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal class EditorEnumField : EditorComboField<Enum>
    {
        private string NullValueDescriptor;
        private Dictionary<string, Enum> options;

        internal EditorEnumField(string Description, string Category, double SortIndex, string NullValueDescriptor, Type enumType, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.NullValueDescriptor = NullValueDescriptor;

            options = new Dictionary<string, Enum>();
            if (NullValueDescriptor != null)
            {
                options.Add(NullValueDescriptor, null);
            }
            foreach (Enum val in Enum.GetValues(enumType))
            {
                if (val.IsHidden())
                    continue;

                options.Add(val.FriendlyName(), val);
            }
        }
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
            cell.Value = Text(ObjectBeingEditted);
        }

        public override List<string> GetOptions()
        {
            return options.Keys.ToList();
        }
        public override string Text(object ObjectBeingEditted)
        {
            Enum value = GetValue(ObjectBeingEditted);
            if (value == null)
                return NullValueDescriptor;
            return value.FriendlyName();
        }

        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            if (options.TryGetValue(text, out Enum val))
                SetValue(ObjectBeingEditted, val, true);
        }
    }
}
