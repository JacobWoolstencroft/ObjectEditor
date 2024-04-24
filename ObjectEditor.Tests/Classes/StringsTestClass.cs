using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectEditor.Attributes;

namespace ObjectEditor.Tests
{
    class StringsTestClass
    {
        [EditableSubField(Category = "String List")]
        StringListTest StringList = new StringListTest();

        class StringListTest
        {
            [EditableField(EmptyStringMode = EmptyStringModes.Null)]
            List<string> Strings = new List<string>();

            [EditableField(Description = "Strings[0]")]
            string Strings0 => (Strings == null || Strings.Count <= 0 ? "[out of range]" : Strings[0]);
            [EditableField(Description = "Strings[1]")]
            string Strings1 => (Strings == null || Strings.Count <= 1 ? "[out of range]" : Strings[1]);
            [EditableField(Description = "Strings[2]")]
            string Strings2 => (Strings == null || Strings.Count <= 2 ? "[out of range]" : Strings[2]);
        }

        [EditableSubField(Category = "Password Test")]
        PasswordTest password = new PasswordTest();

        class PasswordTest
        {
            [EditableField(StringMode = StringModes.Password, EmptyStringMode = EmptyStringModes.NotAllowed)]
            string Password1;

            [EditableField]
            string Password1Revealed => Password1;

            [EditableField(Description_Edit_Postfix = " (with button)", StringMode = StringModes.Password, EmptyStringMode = EmptyStringModes.Blank, OnButtonClickMethod = "RandomPassword2", ButtonText = "Random Password")]
            string Password2;
            void RandomPassword2()
            {
                Random rnd = new Random();

                Password2 = "";

                int length = rnd.Next(5, 11);
                for (int x = 0; x < length; x++)
                {
                    Password2 += rnd.Next(10).ToString();
                }
            }

            [EditableField]
            string Password2Revealed => Password2;

            [EditableField(Description_Edit_Postfix = " (with NullValueDescriptor)", EmptyStringMode = EmptyStringModes.Null, NullValueDescriptor = "[Not Set]")]
            string Password3;
            [EditableField(NullValueDescriptor = "[Not Set]")]
            string Password3Revealed => Password3;
        }
    }
}
