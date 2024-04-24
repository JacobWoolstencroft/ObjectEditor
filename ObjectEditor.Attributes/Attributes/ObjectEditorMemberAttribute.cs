using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectEditor.Attributes
{
    public abstract class ObjectEditorMemberAttribute : Attribute
    {
        public string Description;
        public string Description_List_Prefix;
        public string Description_List_Postfix;
        public string Description_Edit_Prefix;
        public string Description_Edit_Postfix;
        public string Category;
        public double SortIndex = double.MaxValue;
        public string VisibilityFlagMember = null;
        public ModesFlags VisibleModes = ModesFlags.All;
    }
}
