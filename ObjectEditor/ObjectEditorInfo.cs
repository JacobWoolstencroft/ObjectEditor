﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectEditor
{
    public class ObjectEditorInfo
    {
        public bool Editable = true;
        public bool HideFromScreenShare = false;
        [Obsolete("Deprecated - Use EditableDropDownField instead")]
        public Dictionary<string, List<string>> StringLists = null;
        [Obsolete("Deprecated - Use EditableDropDownField instead")]
        public Dictionary<string, List<object>> ObjectLists = null;

        [Obsolete("Deprecated - Use EditableDropDownField instead")]
        public void AddStringList(string key, List<string> list)
        {
            if (StringLists == null)
                StringLists = new Dictionary<string, List<string>>();
            StringLists[key] = list;
        }
        [Obsolete]
        public List<string> GetStringList(string key)
        {
            if (StringLists == null || !StringLists.TryGetValue(key, out List<string> list))
                return null;
            return list;
        }

        [Obsolete("Deprecated - Use EditableDropDownField instead")]
        public void AddObjectList(string key, List<object> list)
        {
            if (ObjectLists == null)
                ObjectLists = new Dictionary<string, List<object>>();
            ObjectLists[key] = list;
        }
        [Obsolete("Deprecated - Use EditableDropDownField instead")]
        public void AddObjectList<T>(string key, List<T> list)
        {
            List<object> obList = new List<object>(list.Capacity);
            foreach (T ob in list)
                obList.Add(ob);
            AddObjectList(key, obList);
        }
        [Obsolete]
        public List<object> GetObjectList(string key)
        {
            if (ObjectLists == null || !ObjectLists.TryGetValue(key, out List<object> list))
                return null;
            return list;
        }
    }
}
