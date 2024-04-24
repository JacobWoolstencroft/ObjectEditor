using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectEditor
{
    internal class FieldData
    {
        public List<MemberInfo> ParentMembers;
        public MemberInfo member;
        public bool ReadOnly
        {
            get
            {
                if (member is FieldInfo field)
                    return field.IsInitOnly;
                else if (member is PropertyInfo property)
                    return !property.CanWrite;
                return true;
            }
        }

        public FieldData(List<MemberInfo> ParentMembers, MemberInfo member)
        {
            this.ParentMembers = ParentMembers;
            this.member = member;
        }

        private object ParentObject(object ObjectBeingEditted)
        {
            object obj = ObjectBeingEditted;
            for (int x = 0; x < ParentMembers.Count; x++)
            {
                if (obj == null)
                    return null;
                if (ParentMembers[x] is FieldInfo field)
                    obj = field.GetValue(obj);
                else if (ParentMembers[x] is PropertyInfo property)
                    obj = property.GetValue(obj);
                else
                    return null;
            }
            return obj;
        }
        public object GetValue(object ObjectBeingEditted)
        {
            object obj = ParentObject(ObjectBeingEditted);
            if (obj != null)
            {
                if (member is FieldInfo field)
                    return field.GetValue(obj);
                else if (member is PropertyInfo property)
                    return property.GetValue(obj);
                return null;
            }
            return null;
        }
        public void SetValue(object ObjectBeingEditted, object val)
        {
            if (ReadOnly)
                return;
            object ob = ParentObject(ObjectBeingEditted);
            if (ob != null)
            {
                if (member is FieldInfo field)
                    field.SetValue(ob, val);
                else if (member is PropertyInfo property)
                    property.SetValue(ob, val);
            }
        }
    }
}
