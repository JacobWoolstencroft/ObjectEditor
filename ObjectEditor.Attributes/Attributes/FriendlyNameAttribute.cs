using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FriendlyNameAttribute : Attribute
    {
        public readonly string FriendlyName;
        public bool Hidden;

        public FriendlyNameAttribute(string FriendlyName)
        {
            this.FriendlyName = FriendlyName;
        }
    }
}
