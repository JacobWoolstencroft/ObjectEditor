using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ObjectEditor
{
    internal class DataGridViewExtended : DataGridView
    {
        public DataGridViewExtended()
        {
            this.DoubleBuffered = true;
        }

        public Color NullValueColor = Color.LightSlateGray;
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Tab)
            {
                SelectNextCell((keyData & Keys.Modifiers) != Keys.Shift);
                return true;

            }
            return base.ProcessDialogKey(keyData);
        }
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                SelectNextCell((e.KeyData & Keys.Modifiers) != Keys.Shift);
                return true;
            }
            return base.ProcessDataGridViewKey(e);
        }
        private void SelectNextCell(bool forward)
        {
            int col, row;
            int direction = forward ? 1 : -1;
            if (CurrentCell is ComboBoxCell cmbCell)
            {
                if (EditingControl is ComboBox cmb && cmb.DroppedDown)
                    return;
            }

            col = this.CurrentCell.ColumnIndex;
            row = this.CurrentCell.RowIndex;
            do
            {
                col = col + direction;
                if (col >= this.Columns.Count)
                {
                    col = 0;
                    row += 1;
                }
                if (col < 0)
                {
                    col = this.Columns.Count - 1;
                    row -= 1;
                }
            } while (row >= 0 && row < this.Rows.Count && (this[col, row].ReadOnly || this[col, row].Visible == false));

            if (row >= 0 && row < this.Rows.Count)
                this.CurrentCell = this[col, row];
        }
        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            base.OnDataError(false, e);
        }

        public delegate void VoidVoid();
        public event VoidVoid UpdateValues;

        public void TriggerUpdateValues()
        {
            if (UpdateValues != null)
                UpdateValues();
        }
    }
}
