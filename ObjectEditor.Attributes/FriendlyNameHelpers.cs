using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ObjectEditor.Attributes
{
    public static class FriendlyNameHelpers
    {
        public static string FriendlyName(this Enum e)
        {
            MemberInfo[] members = e.GetType().GetMember(e.ToString());
            if (members != null && members.Length > 0)
            {
                FriendlyNameAttribute friendlyName = members[0].GetCustomAttribute<FriendlyNameAttribute>();
                if (friendlyName != null)
                    return friendlyName.FriendlyName;
            }
            return e.ToString();
        }
        public static bool IsHidden(this Enum e)
        {
            MemberInfo[] members = e.GetType().GetMember(e.ToString());
            if (members != null && members.Length > 0)
            {
                FriendlyNameAttribute friendlyName = members[0].GetCustomAttribute<FriendlyNameAttribute>();
                if (friendlyName != null)
                    return friendlyName.Hidden;
            }
            return false;
        }
    }
}
