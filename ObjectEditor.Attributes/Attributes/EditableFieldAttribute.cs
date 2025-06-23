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
        /// <summary>
        /// When StringMode is set to DropDownList, the field specified by this attribute will be used to retrieve the values that will appear in that list.  The field should be either List&lt;string&gt; or string[].
        /// </summary>
        public string StringOptionsField = null;
        public string OnChangeMethod = null;
        public string ValidateField = null;

        public string OnButtonClickMethod = null;
        public string ButtonText = "...";
        public string ToolTipText = null;
    }
}
