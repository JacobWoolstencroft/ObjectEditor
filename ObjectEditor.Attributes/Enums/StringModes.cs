using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectEditor.Attributes
{
    public enum StringModes
    {
        Unspecified,
        NoChange,
        Trim,
        Password,
        [Obsolete("Use EditableDropDownField instead")]
        DropDownList,
        [Obsolete("Use EditableDropDownField instead")]
        DropDownObjectList
    }
}
