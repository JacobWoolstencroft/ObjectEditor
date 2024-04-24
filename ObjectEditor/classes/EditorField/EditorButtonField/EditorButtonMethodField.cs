using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal class EditorButtonMethodField : EditorButtonField
    {
        MethodData Method;
        FieldData DisplayField;
        string Text;

        internal EditorButtonMethodField(string Description, string Category, double SortIndex, string Text, MethodData Method, FieldData DisplayField)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.Method = Method;
            this.DisplayField = DisplayField;
            this.Text = Text;
        }
        public override string GetButtonText(object ObjectBeingEditted)
        {
            if (DisplayField != null)
            {
                object DisplayValue = DisplayField.GetValue(ObjectBeingEditted);
                if (DisplayValue is null)
                    return null;
                return DisplayValue.ToString();
            }
            else
            {
                return Text;
            }
        }
        public override void Click(object ObjectBeingEditted)
        {
            Method.Execute(ObjectBeingEditted);
        }
    }
}
