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
    internal partial class frmObjectListEditor<T> : Form where T : ICloneable, new()
    {
        private const string DEFAULT_CATEGORY = "General";

        List<T> objectList;
        string ObjectDesc;
        private ObjectEditorInfo editorInfo = new ObjectEditorInfo();
        private ObjectEditors.ImportListDelegate<T> importFunction = null;
        private ObjectEditors.ExportListDelegate<T> exportFunction = null;

        private class ColumnInfo
        {
            public ColumnInfo(EditorField DisplayField, ColumnHeader columnHeader)
            {
                this.DisplayField = DisplayField;
                this.columnHeader = columnHeader;
            }

            public EditorField DisplayField;
            public ColumnHeader columnHeader;
        }

        internal frmObjectListEditor(string Title, List<T> objectList, ObjectEditorInfo editorInfo, ObjectEditors.ImportListDelegate<T> importFunction, ObjectEditors.ExportListDelegate<T> exportFunction)
        {
            InitializeComponent();
            if (Title != null)
                this.Text = Title;
            this.objectList = objectList;
            this.editorInfo = editorInfo;
            this.importFunction = importFunction;
            this.exportFunction = exportFunction;

            EditableObjectAttribute editableObject = typeof(T).GetCustomAttribute<EditableObjectAttribute>();
            if (editableObject != null)
                ObjectDesc = editableObject.Description;
            if (ObjectDesc == null)
                ObjectDesc = typeof(T).Name;

            string DefaultCategory = null;
            string[] PreferredCategoryOrder = null;
            if (editableObject != null)
            {
                DefaultCategory = editableObject.Category;
                PreferredCategoryOrder = editableObject.PreferredCategoryOrder;
            }
            if (DefaultCategory == null)
                DefaultCategory = DEFAULT_CATEGORY;

            Dictionary<string, List<ColumnInfo>> category_columns = CreateCategoryColumns(DefaultCategory);

            HashSet<string> CategoriesAdded = new HashSet<string>();
            if (PreferredCategoryOrder != null)
            {
                foreach (string category in PreferredCategoryOrder)
                {
                    string cat = string.IsNullOrEmpty(category) ? DEFAULT_CATEGORY : category;
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
                btnMoveRuleUp.Enabled = false;
                btnMoveRuleDown.Enabled = false;
                btnEditRule.Text = "View";
                btnImport.Enabled = false;

                btnOK.Enabled = false;
            }
        }
        private Dictionary<string, List<ColumnInfo>> CreateCategoryColumns(string DefaultCategory)
        {
            EditorField.GetDefaults(typeof(T), out EditorField.DefaultEditorFieldValues defaults, out List<string> PreferredCategoryOrder);
            EditorField.CreateFields(typeof(T), out List<EditorField> fields, defaults, ModesFlags.List, editorInfo);

            Dictionary<string, List<ColumnInfo>> Category_ColumnInfos = new Dictionary<string, List<ColumnInfo>>();
            foreach (EditorField field in fields)
            {
                if (!field.VisibleOnList())
                    continue;

                ColumnHeader clm = new ColumnHeader();
                clm.Text = (field.ListDescription == null ? field.Description : field.ListDescription);
                ColumnInfo columnInfo = new ColumnInfo(field, clm);
                clm.Tag = columnInfo;

                string cat;
                if (!string.IsNullOrEmpty(field.Category))
                    cat = field.Category;
                else if (!string.IsNullOrEmpty(DefaultCategory))
                    cat = DefaultCategory;
                else
                    cat = DEFAULT_CATEGORY;

                if (!Category_ColumnInfos.ContainsKey(cat))
                    Category_ColumnInfos[cat] = new List<ColumnInfo>();
                Category_ColumnInfos[cat].Add(columnInfo);
            }

            foreach (string category in Category_ColumnInfos.Keys)
            {
                Category_ColumnInfos[category].Sort((a, b) => a.DisplayField.SortIndex.CompareTo(b.DisplayField.SortIndex));
            }

            return Category_ColumnInfos;
        }

        private void RefreshList()
        {
            lstRules.Items.Clear();
            foreach (T ob in objectList)
            {
                ListViewItem li = new ListViewItem();
                UpdateListViewItem(li, ob);
                lstRules.Items.Add(li);
            }
        }
        private string GetColText(ColumnHeader col, T obj)
        {
            if (col == null || col.Tag == null)
                return null;
            if (col.Tag is ColumnInfo info)
                return info.DisplayField.ListValue(obj);

            return null;
        }
        private void UpdateListViewItem(ListViewItem li, T obj)
        {
            string s;
            for (int x = 0; x < lstRules.Columns.Count; x++)
            {
                ColumnHeader col;
                col = lstRules.Columns[x];
                s = GetColText(col, obj);

                while (li.SubItems.Count <= x)
                    li.SubItems.Add("");
                li.SubItems[x].Text = s;
            }
        }

        #region Event Handlers
        private void btnAddRule_Click(object sender, EventArgs e)
        {
            T ob = new T();
            if (ObjectEditors.ShowObjectEditor("Add " + ObjectDesc, ob, editorInfo) == DialogResult.OK)
            {
                objectList.Add(ob);
                ListViewItem li = new ListViewItem();
                UpdateListViewItem(li, ob);
                lstRules.Items.Add(li);
                lstRules.ResizeColumns();
            }
        }

        private void btnEditRule_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;
            int index = lstRules.SelectedIndices[0];
            ListViewItem li = lstRules.Items[index];
            T ob = (T)objectList[index].Clone();
            if (ObjectEditors.ShowObjectEditor((editorInfo.Editable ? "Edit " : "View ") + ObjectDesc, ob, editorInfo) == DialogResult.OK && editorInfo.Editable)
            {
                objectList[index] = ob;
                UpdateListViewItem(li, ob);
                lstRules.ResizeColumns();
            }
        }

        private void btnRemoveRule_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;
            int index = lstRules.SelectedIndices[0];
            ListViewItem li = lstRules.Items[index];
            if (MessageBox.Show("Are you sure you want to remove this " + ObjectDesc + "?", "Remove " + ObjectDesc, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                objectList.RemoveAt(index);
                lstRules.Items.RemoveAt(index);
            }
        }

        private void lstRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enable = lstRules.SelectedIndices.Count == 1;

            btnEditRule.Enabled = enable;
            if (editorInfo.Editable)
            {
                btnRemoveRule.Enabled = enable;
                btnMoveRuleUp.Enabled = enable;
                btnMoveRuleDown.Enabled = enable;
            }
        }

        private void lstRules_DoubleClick(object sender, EventArgs e)
        {
            btnEditRule_Click(null, null);
        }

        private void btnMoveRuleUp_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;

            int index = lstRules.SelectedIndices[0];
            if (index > 0)
            {
                T temp;
                temp = objectList[index];
                objectList[index] = objectList[index - 1];
                objectList[index - 1] = temp;

                ListViewItem li1, li2;
                li1 = lstRules.Items[index];
                li2 = lstRules.Items[index - 1];
                lstRules.Items.Remove(li1);
                lstRules.Items.Remove(li2);
                lstRules.Items.Insert(index - 1, li1);
                lstRules.Items.Insert(index, li2);

                li1.Selected = true;
            }
        }

        private void btnMoveRuleDown_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count != 1)
                return;

            int index = lstRules.SelectedIndices[0];
            if (index < objectList.Count - 1)
            {
                T temp;
                temp = objectList[index];
                objectList[index] = objectList[index + 1];
                objectList[index + 1] = temp;

                ListViewItem li1, li2;
                li1 = lstRules.Items[index];
                li2 = lstRules.Items[index + 1];
                lstRules.Items.Remove(li1);
                lstRules.Items.Remove(li2);
                lstRules.Items.Insert(index, li2);
                lstRules.Items.Insert(index + 1, li1);

                li1.Selected = true;
            }

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
                    exportFunction(objectList);
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
                    List<T> list = importFunction();

                    if (list == null)
                        return;

                    objectList.Clear();
                    foreach (T ob in list)
                    {
                        objectList.Add(ob);
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
