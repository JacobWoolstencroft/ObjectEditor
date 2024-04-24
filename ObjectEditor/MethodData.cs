using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectEditor
{
    internal class MethodData
    {
        public List<MemberInfo> ParentMembers;
        public MethodInfo MethodMember;

        public MethodData(List<MemberInfo> ParentMembers, MethodInfo MethodMember)
        {
            this.ParentMembers = ParentMembers;
            this.MethodMember = MethodMember;
        }

        public void Execute(object ObjectBeingEditted)
        {
            object obj = ObjectBeingEditted;
            if (ParentMembers != null)
            {
                for (int x = 0; obj != null && x < ParentMembers.Count; x++)
                {
                    if (ParentMembers[x] is FieldInfo field)
                        obj = field.GetValue(obj);
                    else if (ParentMembers[x] is PropertyInfo property)
                        obj = property.GetValue(obj);
                    else
                        return;
                }
            }
            if (obj != null)
                MethodMember.Invoke(obj, null);
        }
    }
}
