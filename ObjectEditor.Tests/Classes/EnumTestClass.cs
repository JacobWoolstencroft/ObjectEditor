using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectEditor.Attributes;

namespace ObjectEditor.Tests.Classes
{
    class EnumTestClass
    {
        enum Letter
        {
            A,
            B,
            C
        }

        [EditableField]
        bool Letter1Visible = true, Letter2Visible = true, Letter3Visible = true;

        [EditableField(VisibilityFlagMember = "Letter1Visible")]
        Letter? Letter1 = Letter.A;

        [EditableField(VisibilityFlagMember = "Letter2Visible")]
        Letter? Letter2 = Letter.A;

        [EditableField(VisibilityFlagMember = "Letter3Visible")]
        Letter? Letter3 = Letter.A;
    }
}
