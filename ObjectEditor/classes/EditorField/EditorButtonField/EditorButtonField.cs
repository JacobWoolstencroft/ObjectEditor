using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ObjectEditor
{
    internal abstract class EditorButtonField : EditorField
    {
        public abstract void Click(object ObjectBeingEditted);
        public abstract string GetButtonText(object ObjectBeingEditted);
        public bool Enabled = true;

        protected override DataGridViewCell MakeDataGridViewCell(object ObjectBeingEditted)
        {
            ButtonCell btn = new ButtonCell();

            return btn;
        }
        public override void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted)
        {
            if (EnabledFlagMember != null)
                Enabled = (bool)EnabledFlagMember.GetValue(ObjectBeingEditted);

            if (cell is ButtonCell btn)
            {
                btn.Enabled = Enabled;
            }
            else
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                Color forecolor = SystemColors.ControlText;
                if (!Enabled)
                {
                    forecolor = SystemColors.GrayText;
                }
                style.ForeColor = style.SelectionForeColor = forecolor;
                style.BackColor = style.SelectionBackColor = SystemColors.Control;

                cell.Style = style;
            }

            cell.Value = GetButtonText(ObjectBeingEditted);
        }

        public override bool VisibleOnList()
        {
            return false;
        }
        public override string ListValue(object ObjectBeingEditted)
        {
            return null;
        }
    }
}
