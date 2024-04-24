using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorLabelField : EditorTextField<string>
    {
        private string text;
        public EditorLabelField(string Description, string Category, double SortIndex, string text) : base(null, null)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.text = text;
        }
        public override string Text(object ObjectBeingEditted)
        {
            return text;
        }
        protected override void CellTextChanging(string text, object ObjectBeingEditted)
        {
            //Labels can't be changed
        }
    }
}
