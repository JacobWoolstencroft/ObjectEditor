using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal partial class frmObjectEditor : Form
    {
        private class FieldCell
        {
            public FieldCell(EditorField field, DataGridViewCell cell)
            {
                this.field = field;
                this.cell = cell;
            }
            public EditorField field;
            public DataGridViewCell cell;
        }

        private List<FieldCell> FieldCells;
        private List<DataGridView> Grids;
        private List<TabPage> Tabs;

        private ObjectEditorInfo editorInfo = new ObjectEditorInfo();
        private object ObjectBeingEditted = null;

        internal frmObjectEditor(string Title, List<EditorField> Fields, object ObjectBeingEditted, List<string> PreferredCategoryOrder, ObjectEditorInfo editorInfo)
        {
            InitializeComponent();
            if (editorInfo.HideFromScreenShare)
                this.SetHiddenFromScreenShare(true);
            this.Text = Title;
            this.editorInfo = editorInfo;
            this.ObjectBeingEditted = ObjectBeingEditted;

            btnOK.Enabled = editorInfo.Editable;

            SetupDataViews(PreferredCategoryOrder, Fields);
            UpdateValues();
        }

        #region Setup
        private void SetupDataViews(List<string> PreferredCategoryOrder, List<EditorField> Fields)
        {
            Dictionary<string, List<EditorField>> FieldsByCategory = new Dictionary<string, List<EditorField>>();
            foreach (EditorField field in Fields)
            {
                if (!FieldsByCategory.TryGetValue(field.Category, out List<EditorField> categoryFields))
                    FieldsByCategory[field.Category] = categoryFields = new List<EditorField>();
                categoryFields.Add(field);
            }

            FieldCells = new List<FieldCell>();
            Grids = new List<DataGridView>();
            Tabs = new List<TabPage>();

            HashSet<string> CategoriesAdded = new HashSet<string>();
            if (PreferredCategoryOrder != null)
            {
                foreach (string category in PreferredCategoryOrder)
                {
                    string cat = string.IsNullOrEmpty(category) ? EditorField.DEFAULT_CATEGORY : category;
                    if (CategoriesAdded.Contains(cat))
                        continue;
                    if (FieldsByCategory.TryGetValue(cat, out List<EditorField> categoryFields))
                    {
                        SetupCategory(cat, categoryFields);
                        CategoriesAdded.Add(cat);
                    }
                }
            }
            foreach (string category in FieldsByCategory.Keys)
            {
                if (CategoriesAdded.Contains(category))
                    continue;
                SetupCategory(category, FieldsByCategory[category]);
            }
        }
        private void SetupCategory(string category, List<EditorField> fields)
        {
            if (fields == null || fields.Count == 0)
                return;

            TabPage tab = new TabPage(category);
            tabCategories.TabPages.Add(tab);

            DataGridViewExtended grid = new DataGridViewExtended();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.ColumnHeadersVisible = false;
            grid.RowHeadersVisible = false;
            grid.Dock = System.Windows.Forms.DockStyle.Fill;
            grid.Location = new System.Drawing.Point(3, 3);
            grid.Size = new System.Drawing.Size(554, 352);
            grid.TabIndex = 0;
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.UpdateValues += Grid_UpdateValues;

            tab.Controls.Add(grid);

            DataGridViewColumn LabelsCol = new DataGridViewColumn();
            LabelsCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            LabelsCol.DefaultCellStyle = new DataGridViewCellStyle();
            grid.Columns.Add(LabelsCol);
            LabelsCol.ReadOnly = true;

            DataGridViewColumn ValuesCol = new DataGridViewColumn();
            ValuesCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns.Add(ValuesCol);

            foreach (EditorField field in fields)
            {
                grid.Rows.Add(SetupDataRow(field));
            }
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.CellValidating += DataGridView_CellValidating;
            grid.CellClick += Grid_CellClick;
            grid.CellDoubleClick += Grid_CellDoubleClick;
            grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;

            Grids.Add(grid);
            Tabs.Add(tab);
        }

        private void Grid_UpdateValues()
        {
            UpdateValues();
        }

        private DataGridViewRow SetupDataRow(EditorField field)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell label = new DataGridViewTextBoxCell();
            label.Value = field.Description;
            if (field.ToolTipText != null)
                label.ToolTipText = field.ToolTipText;
            row.Cells.Add(label);
            label.ReadOnly = true;

            DataGridViewCell cell = field.CreateDataGridViewCell(ObjectBeingEditted);
            if (field.ToolTipText != null)
                cell.ToolTipText = field.ToolTipText;
            row.Cells.Add(cell);
            if ((field is EditorValueField valueField && valueField.IsReadOnly) || !editorInfo.Editable)
                cell.ReadOnly = true;
            cell.Tag = FieldCells.Count;
            FieldCells.Add(new FieldCell(field, cell));

            return row;
        }
        #endregion

        #region Event handlers
        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView grid)
            {
                if (grid.EditingControl is DataGridViewComboBoxEditingControl cmb)
                    cmb.DroppedDown = true;
                if (grid[e.ColumnIndex, e.RowIndex] is ButtonCell btnCell)
                {
                    if (grid[e.ColumnIndex, e.RowIndex].Tag is int fieldCellIndex && FieldCells[fieldCellIndex].field is EditorButtonField btnField)
                    {
                        if (btnField.Enabled)
                        {
                            btnField.Click(ObjectBeingEditted);
                            UpdateValues();
                        }
                    }
                }
            }
        }
        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView grid)
            {
                if (grid[e.ColumnIndex, e.RowIndex] is ButtonCell btnCell)
                {
                    if (grid[e.ColumnIndex, e.RowIndex].Tag is int fieldCellIndex && FieldCells[fieldCellIndex].field is EditorButtonField btnField)
                    {
                        if (btnField.Enabled)
                        {
                            btnField.Click(ObjectBeingEditted);
                            UpdateValues();
                        }
                    }
                }
            }
        }

        private void DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (sender is DataGridView grid)
            {
                DataGridViewCell cell = grid[e.ColumnIndex, e.RowIndex];
                if (cell.Tag is int fieldCellIndex && FieldCells[fieldCellIndex].field is EditorValueField field && !field.IsReadOnly)
                {
                    field.CellValueChanged(e.FormattedValue, ObjectBeingEditted);
                    UpdateValues();
                }
            }
        }
        private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView grid)
            {
                if (grid.CurrentCell is ComboBoxCell cmbCell)
                {
                    if (grid.IsCurrentCellDirty)
                    {
                        if (cmbCell.Tag is int fieldCellIndex && FieldCells[fieldCellIndex].field is EditorValueField field && !field.IsReadOnly)
                        {
                            field.CellValueChanged(cmbCell.EditedFormattedValue, ObjectBeingEditted);
                            UpdateValues();
                            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                        }
                    }
                }
            }
        }

        private void UpdateValues()
        {
            UpdateFlags();
            foreach (FieldCell fieldCell in FieldCells)
            {
                fieldCell.field.UpdateCellValue(fieldCell.cell, ObjectBeingEditted);
            }
        }
        private bool UpdatingFlags = false;
        private void UpdateFlags()
        {
            if (UpdatingFlags)
                return;
            UpdatingFlags = true;

            try
            {
                foreach (FieldCell fieldCell in FieldCells)
                {
                    bool visible = fieldCell.field.IsVisible(ObjectBeingEditted);
                    DataGridViewRow row = fieldCell.cell.OwningRow;
                    if (visible != row.Visible)
                    {
                        if (fieldCell.cell.DataGridView.CurrentRow == row)
                            fieldCell.cell.DataGridView.CurrentCell = null;
                        row.Visible = visible;
                    }
                }

                UpdateVisibleTabs();
            }
            finally
            {
                UpdatingFlags = false;
            }
        }
        private void UpdateVisibleTabs()
        {
            List<TabPage> VisibleTabs = new List<TabPage>();
            foreach (DataGridView grid in Grids)
            {
                if (grid.Parent is TabPage tab)
                {
                    int visibleRows = 0;
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.Visible)
                            visibleRows++;
                    }
                    if (visibleRows > 0)
                        VisibleTabs.Add(tab);
                }
            }
            foreach (TabPage tab in Tabs)
            {
                int VisibleIndex = VisibleTabs.IndexOf(tab);
                if (VisibleIndex >= 0 && !tabCategories.TabPages.Contains(tab))
                {
                    //Add the tab
                    if (VisibleIndex > 0)
                    {
                        int PreviousTabIndex = tabCategories.TabPages.IndexOf(VisibleTabs[VisibleIndex - 1]);
                        tabCategories.TabPages.Insert(PreviousTabIndex + 1, tab);
                    }
                    else
                        tabCategories.TabPages.Insert(0, tab);
                }
                else if (VisibleIndex < 0 && tabCategories.TabPages.Contains(tab))
                {
                    //Remove the tab
                    tabCategories.TabPages.Remove(tab);
                }
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            DataGridViewCell InvalidCell = null;
            string ErrorMessage = null;

            foreach (FieldCell fieldCell in FieldCells)
            {
                if (!fieldCell.cell.OwningRow.Visible)
                    continue;
                if (fieldCell.field is EditorValueField ValueField)
                {
                    try
                    {
                        if (!ValueField.IsValid(ObjectBeingEditted))
                        {
                            InvalidCell = fieldCell.cell;
                            ErrorMessage = null;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            ex = ex.InnerException;
                        InvalidCell = fieldCell.cell;
                        ErrorMessage = ex.Message;
                        break;
                    }
                }
            }

            if (InvalidCell != null)
            {
                if (InvalidCell.DataGridView.Parent is TabPage page)
                {
                    if (page.Parent is TabControl tabControl)
                    {
                        tabControl.SelectTab(page);
                    }
                    page.Focus();
                }

                InvalidCell.DataGridView.CurrentCell = InvalidCell;
                InvalidCell.DataGridView.Focus();

                if (ErrorMessage != null)
                {
                    MessageBox.Show(ErrorMessage, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InvalidCell.DataGridView.CurrentCell = InvalidCell;
                }
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void tabCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Deselect each grid's current cell when the tab is changed.
            //This prevents an editting control from holding onto an old value
            foreach (DataGridView grid in Grids)
            {
                grid.EndEdit();
                grid.CurrentCell = null;
            }
        }
        #endregion
    }
}
