using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectEditor.Attributes;
using Json;

namespace ObjectEditor.Tests.Classes
{
    [EditableObject(Dictionary_KeyDescription = "User")]
    class DictionaryTestClass : ICloneable, IJsonPackable
    {
        public DictionaryTestClass()
        {
        }
        public DictionaryTestClass(DictionaryTestClass copyFrom)
        {
            this.Description = copyFrom.Description;
            this.Password = copyFrom.Password;
            this._x = copyFrom._x;
        }
        public object Clone()
        {
            return new DictionaryTestClass(this);
        }

        [EditableField(EmptyStringMode = EmptyStringModes.Null, StringMode = StringModes.Trim)]
        public string Description = null;

        [EditableField(Description = "Password", StringMode = StringModes.Password, ValidateField = "PasswordValid")]
        public string Password = "";
        public bool PasswordValid
        {
            get
            {
                if (Password == null || Password.Length < 5)
                {
                    throw new Exception("Password must be 5 characters or longer");
                }
                return true;
            }
        }

        [EditableField(Category = "Buttons", Description_Edit_Postfix = "(Greater than 5)", ValidateField = "ValidX")]
        int X => _x;
        int _x;
        bool ValidX => (_x > 5);

        [ClickableButtonMethod(Category = "Buttons")]
        void XPlus() => _x++;

        [ClickableButtonMethod(Category = "Buttons")]
        void XMinus() => _x--;

        public JsonToken Pack()
        {
            JsonMapping map = new JsonMapping();
            map["Description"] = Description;
            map["Password"] = Password;
            map["X"] = _x;

            return map;
        }
        public void Unpack(JsonToken token)
        {
            Description = token.GetString("Description", Description);
            Password = token.GetString("Password", Password);
            _x = token.GetInt("X", _x);
        }
    }
}
