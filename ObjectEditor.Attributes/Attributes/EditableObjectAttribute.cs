using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class EditableObjectAttribute : Attribute
    {
        public string Description;
        public string Category;
        public string NullValueDescriptor;
        public EmptyStringModes EmptyStringMode = EmptyStringModes.Unspecified;
        public StringModes StringMode = StringModes.Unspecified;
        public string[] PreferredCategoryOrder;
        public string Dictionary_KeyDescription = "Key";
        public string Dictionary_KeyCategory = "General";
        public double Dictionary_KeySortOrder = double.MinValue;
    }
}
