using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditableFieldAttribute : ObjectEditorMemberAttribute
    {
        public string NullValueDescriptor;
        public EmptyStringModes EmptyStringMode = EmptyStringModes.Unspecified;
        public StringModes StringMode = StringModes.Unspecified;
        public string DropDownListKey;
        public string OnChangeMethod = null;
        public string ValidateField = null;

        public string OnButtonClickMethod = null;
        public string ButtonText = "...";
    }
}
