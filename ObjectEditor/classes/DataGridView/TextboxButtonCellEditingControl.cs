using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ObjectEditor
{
    public class TextboxButtonCellEditingControl : TextboxButtonControl, IDataGridViewEditingControl
    {
        public TextboxButtonCellEditingControl()
        {
            this.textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.LabelText;
            set
            {
                if (value is string s)
                    this.LabelText = s;
                else if (value is null)
                    this.LabelText = null;
                else
                    this.LabelText = value.ToString();
            }
        }

        DataGridView dataGridView;
        public DataGridView EditingControlDataGridView { get => dataGridView; set => dataGridView = value; }

        private int RowIndex;
        public int EditingControlRowIndex
        {
            get => RowIndex;
            set => RowIndex = value;
        }

        private bool ValueChanged;
        public bool EditingControlValueChanged { get => ValueChanged; set => ValueChanged = value; }

        public Cursor EditingPanelCursor => base.Cursor;

        public bool RepositionEditingControlOnValueChange => false;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle cellStyle)
        {
            this.LabelFont = cellStyle.Font;
            this.LabelForeColor = cellStyle.ForeColor;
            this.LabelBackColor = cellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public virtual void PrepareEditingControlForEdit(bool selectAll)
        {
            this.textBox1.Focus();
            if (selectAll)
                this.textBox1.SelectAll();
        }

        public override void OnButtonClicked()
        {
            if (dataGridView.CurrentCell is TextBoxCell txtCell)
                txtCell.Click(this);
        }
    }
}
