using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ListEditorAttribute : ObjectEditorMemberAttribute
    {
        public string NullValueDescriptor;
        public EmptyListModes EmptyListMode = EmptyListModes.Unspecified;
        public string DisplayField;
    }
}
