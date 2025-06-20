using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ClickableButtonMethodAttribute : ObjectEditorMemberAttribute
    {
        public string Text = null;
        public string DisplayField = null;
        public string EnabledFlagMember = null;
        public string ToolTipText = null;
    }
}
