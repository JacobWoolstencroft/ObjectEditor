using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal class EditorListObjectField<T> : EditorButtonField where T : class, ICloneable, new()
    {
        ObjectEditorInfo editorInfo;
        FieldData ValueField, DisplayField;
        EmptyListModes EmptyListMode;

        //Warning: Changes to this constructor also require changes to reflection calls in ObjectEditor
        internal EditorListObjectField(string Description, string Category, double SortIndex, ObjectEditorInfo editorInfo, FieldData ValueField, FieldData DisplayField, EmptyListModes EmptyListMode)
        {
            this.Description = Description;
            this.Category = Category;
            this.SortIndex = SortIndex;
            this.editorInfo = editorInfo;
            this.ValueField = ValueField;
            this.DisplayField = DisplayField;
            this.EmptyListMode = EmptyListMode;
        }
        public override string GetButtonText(object ObjectBeingEditted)
        {
            if (DisplayField != null)
            {
                return DisplayField.GetValue(ObjectBeingEditted).ToString();
            }
            else
            {
                return ValueField.GetValue(ObjectBeingEditted).ToString();
            }
        }
        public static List<T> Copy(List<T> list)
        {
            if (list == null)
                return null;

            List<T> copy = new List<T>();
            foreach (T val in list)
            {
                if (val == null)
                    copy.Add(null);
                else
                    copy.Add((T)val.Clone());
            }

            return copy;
        }
        public override void Click(object ObjectBeingEditted)
        {
            List<T> list = Copy((List<T>)ValueField.GetValue(ObjectBeingEditted));
            if (ObjectEditors.ShowListEditor("Edit " + Description, list, editorInfo) == DialogResult.OK)
            {
                if (list.Count == 0)
                {
                    switch (EmptyListMode)
                    {
                        case EmptyListModes.Null:
                            list = null;
                            break;
                        case EmptyListModes.EmptyList:
                        case EmptyListModes.Unspecified:
                            //No changes
                            break;
                    }
                }
                ValueField.SetValue(ObjectBeingEditted, list);
            }
        }
        public override bool VisibleOnList()
        {
            if (DisplayField != null)
                return true;
            return base.VisibleOnList();
        }
        public override string ListValue(object ObjectBeingEditted)
        {
            if (DisplayField != null)
            {
                object val = DisplayField.GetValue(ObjectBeingEditted);
                if (val != null)
                    return val.ToString();
                return null;
            }
            return base.ListValue(ObjectBeingEditted);
        }
    }
}
