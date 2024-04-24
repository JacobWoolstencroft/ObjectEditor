using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal class EditorSubField
    {
        internal EditorSubField Parent;
        internal FieldData VisibilityFlag;

        internal EditorSubField(EditorSubField Parent, FieldData VisibilityFlag)
        {
            this.Parent = Parent;
            this.VisibilityFlag = VisibilityFlag;
        }

        public bool IsVisible(object ObjectBeingEditted)
        {
            if (Parent != null && !Parent.IsVisible(ObjectBeingEditted))
                return false;
            if (VisibilityFlag != null)
                return (bool)VisibilityFlag.GetValue(ObjectBeingEditted);
            return true;
        }

    }
}
