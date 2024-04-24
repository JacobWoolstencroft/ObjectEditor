using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EditableSubFieldAttribute : ObjectEditorMemberAttribute
    {
    }
}
