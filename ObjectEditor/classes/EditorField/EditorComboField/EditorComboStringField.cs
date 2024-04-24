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
        private string NullValueDescriptor = null;

        public EditorComboStringField(string Description, string Category, double SortIndex, List<string> options, EmptyStringModes EmptyStringMode, string NullValueDescriptor, FieldData ValueField) : base(ValueField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.options = new List<string>();

            if (NullValueDescriptor == null)
                NullValueDescriptor = "";
            this.NullValueDescriptor = NullValueDescriptor;

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
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
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
