using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal partial class frmObjectDictionaryEditor<T> : Form where T : ICloneable, new()
    {
        Dictionary<string, T> objectDict;
        string ObjectDesc;
        private ObjectEditorInfo editorInfo = new ObjectEditorInfo();
        private ObjectEditors.ImportDictionaryDelegate<T> importFunction = null;
        private ObjectEditors.ExportDictionaryDelegate<T> exportFunction = null;

        private string keyDescription = "TEMP_DESC";
        private string keyCategory = EditorField.DEFAULT_CATEGORY;
        private double keySortIndex = double.MinValue;

        private class ColumnInfo
        {
            public ColumnInfo(EditorField field, ColumnHeader columnHeader)
            {
                this.field = field;
                this.columnHeader = columnHeader;
            }

            public EditorField field;
            public ColumnHeader columnHeader;
        }
        private class KeyValuePair
        {
            [EditableField(EmptyStringMode = EmptyStringModes.Blank)]
            public string Key = "";
            [EditableSubField]
            public T Value;
        }

        internal frmObjectDictionaryEditor(string Title, Dictionary<string, T> objectDict, ObjectEditorInfo editorInfo, ObjectEditors.ImportDictionaryDelegate<T> importFunction, ObjectEditors.ExportDictionaryDelegate<T> exportFunction)
        {
            InitializeComponent();
            if (Title != null)
                this.Text = Title;
            this.objectDict = objectDict;
            this.editorInfo = editorInfo;
            this.importFunction = importFunction;
            this.exportFunction = exportFunction;

            EditableObjectAttribute editableObject = typeof(T).GetCustomAttribute<EditableObjectAttribute>();
            if (editableObject != null)
            {
                ObjectDesc = editableObject.Description;
                keyCategory = editableObject.Dictionary_KeyCategory;
                keyDescription = editableObject.Dictionary_KeyDescription;
                keySortIndex = editableObject.Dictionary_KeySortOrder;
            }
            if (ObjectDesc == null)
                ObjectDesc = typeof(T).Name;

            string DefaultCategory = null;
            string[] PreferredCategoryOrder = null;
            if (editableObject != null)
            {
                DefaultCategory = editableObject.Category;
                PreferredCategoryOrder = editableObject.PreferredCategoryOrder;
            }

            Dictionary<string, List<ColumnInfo>> category_columns = CreateCategoryColumns(DefaultCategory);

            HashSet<string> CategoriesAdded = new HashSet<string>();
            if (PreferredCategoryOrder != null)
            {
                foreach (string category in PreferredCategoryOrder)
                {
                    string cat = string.IsNullOrEmpty(category) ? EditorField.DEFAULT_CATEGORY : category;
                    if (CategoriesAdded.Contains(cat))
                        continue;
                    if (category_columns.TryGetValue(cat, out List<ColumnInfo> clms))
                    {
                        foreach (ColumnInfo clm in clms)
                        {
                            lstRules.Columns.Add(clm.columnHeader);
                        }
                        CategoriesAdded.Add(cat);
                    }
                }
            }
            foreach (string category in category_columns.Keys)
            {
                if (CategoriesAdded.Contains(category))
                    continue;

                if (category_columns.TryGetValue(category, out List<ColumnInfo> clms))
                {
                    foreach (ColumnInfo clm in clms)
                    {
                        lstRules.Columns.Add(clm.columnHeader);
                    }
                }
                CategoriesAdded.Add(category);
            }


            RefreshList();
            lstRules.ResizeColumns();

            btnImport.Visible = (importFunction != null);
            btnExport.Visible = (exportFunction != null);

            if (!editorInfo.Editable)
            {
                btnAddRule.Enabled = false;
                btnRemoveRule.Enabled = false;
                btnCopy.Enabled = false;
                btnEditRule.Text = "View";
                btnImport.Enabled = false;

                btnOK.Enabled = false;
            }
        }
        private Dictionary<string, List<ColumnInfo>> CreateCategoryColumns(string DefaultCategory)
        {
            EditorField.GetDefaults(typeof(T), out EditorField.DefaultEditorFieldValues defaults, out List<string> PreferredCategoryOrder);
            EditorField.CreateFields(typeof(KeyValuePair), out List<EditorField> fields, defaults, ModesFlags.List, editorInfo);

            EditorField keyEditorField = FindKeyEditorField(fields);
            keyEditorField.Category = keyCategory;
            keyEditorField.Description = keyEditorField.ListDescription = keyDescription;
            keyEditorField.SortIndex = keySortIndex;

            Dictionary<string, List<ColumnInfo>> Category_ColumnInfos = new Dictionary<string, List<ColumnInfo>>();
            foreach (EditorField field in fields)
            {
                if (!field.VisibleOnList())
                    continue;

                ColumnHeader clm = new ColumnHeader();
                clm.Text = (field.ListDescription == null ? field.Description : field.ListDescription);
                ColumnInfo columnInfo = new ColumnInfo(field, clm);
                clm.Tag = columnInfo;

                if (!Category_ColumnInfos.TryGetValue(field.Category, out List<ColumnInfo> clmList))
                    Category_ColumnInfos[field.Category] = clmList = new List<ColumnInfo>();
                clmList.Add(columnInfo);
            }

            foreach (string category in Category_ColumnInfos.Keys)
            {
                Category_ColumnInfos[category].Sort((a, b) => a.field.SortIndex.CompareTo(b.field.SortIndex));
            }

            return Category_ColumnInfos;
        }

        private void RefreshList()
        {
            lstRules.Items.Clear();
            foreach (KeyValuePair<string, T> ob in objectDict)
            {
                ListViewItem li = new ListViewItem();
                UpdateListViewItem(li, ob);
                lstRules.Items.Add(li);
            }
        }
        private string GetColText(ColumnHeader col, KeyValuePair keyVal)
        {
            if (col == null || col.Tag == null)
                return null;
            else if (col.Tag is ColumnInfo info)
            {
                return info.field.ListValue(keyVal);
            }

            return null;
        }
        private void UpdateListViewItem(ListViewItem li, KeyValuePair<string, T> keyVal)
        {
            KeyValuePair kv = new KeyValuePair();
            kv.Key = keyVal.Key;
            kv.Value = keyVal.Value;
            UpdateListViewItem(li, kv);
        }
        private void UpdateListViewItem(ListViewItem li, KeyValuePair keyVal)
        {
            string s;
            li.Tag = keyVal.Key;
            for (int x = 0; x < lstRules.Columns.Count; x++)
            {
                ColumnHeader col;
                col = lstRules.Columns[x];
                s = GetColText(col, keyVal);

                while (li.SubItems.Count <= x)
                    li.SubItems.Add("");
                li.SubItems[x].Text = s;
            }
        }

        private static EditorField FindKeyEditorField(List<EditorField> fields)
        {
            foreach (EditorField field in fields)
            {
                if (field is EditorValueField valueField)
                {
                    //The Key will be the only field in the top-level, so look for a field with no parent members
                    if (valueField.ValueField.ParentMembers.Count == 0)
                        return field;
                }
            }
            return null;
        }
        private void CreateFieldsForObjectEditor(KeyValuePair ob, out List<EditorField> fields, out List<string> PreferredCategoryOrder, ObjectEditorInfo editorInfo)
        {
            EditorField.GetDefaults(typeof(T), out EditorField.DefaultEditorFieldValues defaults, out PreferredCategoryOrder);
            EditorField.CreateFields(typeof(KeyValuePair), out fields, defaults, ModesFlags.Object, editorInfo);

            EditorField keyEditorField = FindKeyEditorField(fields);
            keyEditorField.Category = keyCategory;
            keyEditorField.Description = keyDescription;
            keyEditorField.SortIndex = keySortIndex;

            fields.Sort((a, b) => a.SortIndex.CompareTo(b.SortIndex));

            EditorField.SaveFieldValues(fields, ob);
        }

        #region Event Handlers
        private void btnAddRule_Click(object sender, EventArgs e)
        {
            KeyValuePair ob = new KeyValuePair();
            ob.Value = new T();

            CreateFieldsForObjectEditor(ob, out List<EditorField> fields, out List<string> PreferredCategoryOrder, editorInfo);

            while (true)
            {
                if (ObjectEditors.ShowEditor("Add " + ObjectDesc, fields, ob, editorInfo, PreferredCategoryOrder) == DialogResult.OK)
                {
                    if (objectDict.ContainsKey(ob.Key))
                    {
                        MessageBox.Show(keyDescription + " must be unique", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    objectDict.Add(ob.Key, ob.Value);
                    ListViewItem li = new ListViewItem();
                    UpdateListViewItem(li, ob);
                    lstRules.Items.Add(li);
                    lstRules.ResizeColumns();
                }
                else
                    EditorField.ResetFieldValues(fields, ob);
                break;
            }
        }

        private void btnEditRule_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;
            int index = lstRules.SelectedIndices[0];
            ListViewItem li = lstRules.Items[index];
            if (!(li.Tag is string key))
                return;

            KeyValuePair ob = new KeyValuePair();
            ob.Key = key;
            ob.Value = objectDict[key];

            CreateFieldsForObjectEditor(ob, out List<EditorField> fields, out List<string> PreferredCategoryOrder, editorInfo);

            while (true)
            {
                if (ObjectEditors.ShowEditor((editorInfo.Editable ? "Edit " : "Add ") + ObjectDesc, fields, ob, editorInfo, PreferredCategoryOrder) == DialogResult.OK && editorInfo.Editable)
                {
                    if (key != ob.Key && objectDict.ContainsKey(ob.Key))
                    {
                        MessageBox.Show(keyDescription + " must be unique", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    if (key != ob.Key)
                        objectDict.Remove(key);
                    objectDict[ob.Key] = ob.Value;

                    UpdateListViewItem(li, ob);
                    lstRules.ResizeColumns();
                }
                else
                    EditorField.ResetFieldValues(fields, ob);
                break;
            }
        }

        private void btnRemoveRule_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;
            int index = lstRules.SelectedIndices[0];
            ListViewItem li = lstRules.Items[index];
            if (!(li.Tag is string key))
                return;

            if (MessageBox.Show("Are you sure you want to remove this " + ObjectDesc + "?", "Remove " + ObjectDesc, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                objectDict.Remove(key);
                lstRules.Items.Remove(li);
            }
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;
            int index = lstRules.SelectedIndices[0];
            if (!(lstRules.Items[index].Tag is string key))
                return;

            KeyValuePair ob = new KeyValuePair();
            ob.Key = key;
            ob.Value = (T)objectDict[key].Clone();

            CreateFieldsForObjectEditor(ob, out List<EditorField> fields, out List<string> PreferredCategoryOrder, editorInfo);

            while (true)
            {
                if (ObjectEditors.ShowEditor("Add " + ObjectDesc, fields, ob, editorInfo, PreferredCategoryOrder) == DialogResult.OK)
                {
                    if (objectDict.ContainsKey(ob.Key))
                    {
                        MessageBox.Show(keyDescription + " must be unique", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    objectDict.Add(ob.Key, ob.Value);
                    ListViewItem li = new ListViewItem();
                    UpdateListViewItem(li, ob);
                    lstRules.Items.Add(li);
                    lstRules.ResizeColumns();
                }
                else
                    EditorField.ResetFieldValues(fields, ob);
                break;
            }
        }

        private void lstRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enable = lstRules.SelectedIndices.Count == 1;

            btnEditRule.Enabled = enable;
            if (editorInfo.Editable)
            {
                btnRemoveRule.Enabled = enable;
                btnCopy.Enabled = enable;
            }
        }

        private void lstRules_DoubleClick(object sender, EventArgs e)
        {
            btnEditRule_Click(null, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (exportFunction != null)
            {
                try
                {
                    exportFunction(objectDict);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (importFunction != null)
            {
                try
                {
                    Dictionary<string, T> newDict;

                    newDict = importFunction();
                    if (newDict == null)
                        return;

                    objectDict.Clear();
                    foreach (KeyValuePair<string, T> ob in newDict)
                    {
                        objectDict.Add(ob.Key, ob.Value);
                    }
                    RefreshList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
