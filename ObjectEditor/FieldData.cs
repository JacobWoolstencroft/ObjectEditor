using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

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
        public Type memberType
        {
            get
            {
                if (member is FieldInfo field)
                    return field.FieldType;
                else if (member is PropertyInfo property)
                    return property.PropertyType;
                return null;
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
        public List<string> GetValue_StringList(object ObjectBeingEditted)
        {
            object obj = GetValue(ObjectBeingEditted);
            if (obj != null)
            {
                if (obj is List<string> list)
                    return list;
                if (obj is string[] array)
                    return array.ToList();
            }
            return null;
        }
        public List<object> GetValue_ObjectList(object ObjectBeingEditted, Type memberType)
        {
            object obj = GetValue(ObjectBeingEditted);
            if (obj != null)
            {
                if (obj is Array arr)
                {
                    List<object> r = new List<object>();
                    foreach (object o in arr)
                    {
                        if (o == null || memberType.IsAssignableFrom(o.GetType()))
                            r.Add(o);
                        else
                            throw new Exception("Invalid type cast - Can not cast from " + o.GetType().FullName + " to " + memberType.FullName);
                    }
                    return r;
                }
                else if (obj is IEnumerable list)
                {
                    List<object> r = new List<object>();
                    foreach (object o in list)
                    {
                        if (o == null || memberType.IsAssignableFrom(o.GetType()))
                            r.Add(o);
                        else
                            throw new Exception("Invalid type cast - Can not cast from " + o.GetType().FullName + " to " + memberType.FullName);
                    }
                    return r;
                }
            }
            return null;
        }
        public bool? GetValue_Bool(object ObjectBeingEditted)
        {
            object obj = GetValue(ObjectBeingEditted);
            if (obj is bool b)
                return b;
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
