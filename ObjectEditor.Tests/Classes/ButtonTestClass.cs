using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectEditor.Attributes;

namespace ObjectEditor.Tests
{
    class ButtonTestClass
    {
        [EditableField(Category = "Location", SortIndex = 1)]
        int X = 0, Y = 0;

        [EditableField(Category = "Location", Description = "X * Y", SortIndex = 1.2)]
        int XY => X * Y;

        [ClickableButtonMethod(Category = "Location", Description = "", Text = "X+", SortIndex = 2, EnabledFlagMember = "XPlusEnable")]
        void XPlus() => X++;
        bool XPlusEnable => X < 10;

        [ClickableButtonMethod(Category = "Location", Description = "", Text = "X-", SortIndex = 3, EnabledFlagMember = "XMinusEnable")]
        void XMinus() => X--;
        bool XMinusEnable => X > 0;

        [ClickableButtonMethod(Category = "Location", Description = "", Text = "Y+", SortIndex = 4, EnabledFlagMember = "YPlusEnable")]
        void YPlus() => Y++;
        bool YPlusEnable => Y < 10;

        [ClickableButtonMethod(Category = "Location", Description = "", Text = "Y-", SortIndex = 5, EnabledFlagMember = "YMinusEnable")]
        void YMinus() => Y--;
        bool YMinusEnable => Y > 0;

        [EditableField(Category = "Location", OnButtonClickMethod = "ZPlus")]
        int Z = 0;
        void ZPlus() => Z++;

        [EditableField(Category = "Location", Description = "Z * 2")]
        int DistanceTimes2 => Z * 2;

        [ClickableButtonMethod(Category = "Bad Buttons")]
        void BadParametersTest(int i = 5)
        {
            X += i;
        }

        [ClickableButtonMethod(Category = "Bad Buttons")]
        int BadReturnType()
        {
            return X;
        }

        [EditableField(Category = "Bad Buttons", OnButtonClickMethod = "TestStringClick")]
        string TestStringWithButton = "7";
        void TestStringClick(string str)
        {
            TestStringWithButton = TestStringWithButton + str;
        }

        [EditableField(Category = "Bad Buttons", OnButtonClickMethod = "TestStringClick2")]
        string TestStringWithButton2 = "2";
        string TestStringClick2()
        {
            return TestStringWithButton2;
        }

        [EditableField(Category = "Strings", EmptyStringMode = EmptyStringModes.NotAllowed)]
        string testText;


        [EditableField(Category = "Text with Button", OnButtonClickMethod = "x1Click", ButtonText = "Random")]
        int x1;
        void x1Click()
        {
            Random rnd = new Random();
            x1 = rnd.Next(1000);
        }
        [EditableField(Category = "Text with Button", OnButtonClickMethod = "x2Click", ButtonText = "Random", VisibilityFlagMember = "x2Visible")]
        int x2;
        void x2Click()
        {
            Random rnd = new Random();
            x1 = rnd.Next(1000);
        }
        bool x2Visible => x1 >= 500;
    }
}
