using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectEditor.Attributes;

namespace ObjectEditor.Tests.Classes
{
    [EditableObject(Category = "Doom", Description = "Test List Object")]
    class ListTestClass : ICloneable
    {
        public ListTestClass()
        {
        }
        public ListTestClass(ListTestClass copyFrom)
        {
            this.user = copyFrom.user;
            this.x = copyFrom.x;
        }
        public object Clone()
        {
            return new ListTestClass(this);
        }

        [EditableField(ValidateField = "UserValid", EmptyStringMode = EmptyStringModes.Blank, Description_Edit_Postfix = " (At least length 3)")]
        string user;
        bool UserValid => (user != null && user.Length >= 3);

        [EditableField(Description_Edit_Postfix = " (At least 7)", ValidateField = "ValidX")]
        int X => x;
        bool ValidX => (X >= 7);

        int x = 0;

        [ClickableButtonMethod(Category = "Buttons")]
        void XPlus() => x++;

        [ClickableButtonMethod(Category = "Buttons")]
        void XMinus()
        {
            x--;
        }
        


    }
}
