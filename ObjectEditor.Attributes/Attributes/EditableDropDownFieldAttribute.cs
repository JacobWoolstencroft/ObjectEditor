using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditableDropDownFieldAttribute : ObjectEditorMemberAttribute
    {
        public string NullValueDescriptor;
        /// <summary>
        /// The field specified by this attribute will be used to retrieve the values that will appear in the drop down list.  The field should be either List&lt;T&gt; or T[].
        /// </summary>
        public string OptionsField = null;
        public string OnChangeMethod = null;
        public string ValidateField = null;

        public string ToolTipText = null;
    }
}
