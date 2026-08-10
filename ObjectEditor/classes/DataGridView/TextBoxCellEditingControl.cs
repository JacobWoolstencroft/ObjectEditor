using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor.classes.DataGridView
{
    public class TextBoxCellEditingControl : DataGridViewTextBoxEditingControl
    {
        public void SetMultiline(bool multi)
        {
            base.Multiline = multi;
            base.AcceptsReturn = multi;
        }
        public override bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if (base.Multiline)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Return:
                        return true;
                    case Keys.Right:
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                        return true;
                }
            }
            return base.EditingControlWantsInputKey(keyData, dataGridViewWantsInputKey);
        }
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            if (base.Multiline)
            {
                switch ((Keys)(int)m.WParam)
                {
                    case Keys.Return:
                        if (m.Msg == 258 && (Control.ModifierKeys == Keys.None))
                        {
                            //The base DataGridViewTextBoxEditingControl wants to only process shift+return
                            //This forces the textbox to process return with no modifier keys
                            KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs((char)(long)Keys.Return);
                            OnKeyPress(keyPressEventArgs);

                            return keyPressEventArgs.Handled;
                        }

                        break;
                }
            }

            return base.ProcessKeyEventArgs(ref m);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            if (base.Multiline)
            {
                base.OnTextChanged(e);
                this.EditingControlDataGridView.CurrentCell.Value = Text;
            }
        }
    }
}
