using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal class TextBoxCell : DataGridViewTextBoxCell
    {
        public StringModes StringMode = StringModes.NoChange;
        public string EmptyString = null;
        public string ButtonText = null;

        public delegate void ClickEvent(TextBoxCell cell);
        public event ClickEvent Clicked;

        protected override void Paint(Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (formattedValue is string s)
            {
                if (StringMode == StringModes.Password)
                {
                    if (s.Length > 0)
                        formattedValue = new string(EditorField.PassChar, s.Length);
                }
                else if (EmptyString != null && s.Length == 0)
                {
                    formattedValue = EmptyString;
                    if (this.DataGridView is DataGridViewExtended grid)
                        cellStyle.ForeColor = grid.NullValueColor;
                }
            }
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
        public override Type EditType
        {
            get
            {
                if (ButtonText != null)
                    return typeof(TextboxButtonCellEditingControl);
                else
                    return typeof(DataGridViewTextBoxEditingControl);
            }
        }
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is TextboxButtonCellEditingControl labelButton)
            {
                labelButton.textBox1.UseSystemPasswordChar = (StringMode == StringModes.Password);

                labelButton.ButtonText = this.ButtonText;
                if (this.Value != null)
                    labelButton.LabelText = this.Value.ToString();
                else
                    labelButton.LabelText = null;
            }
            else if (DataGridView.EditingControl is DataGridViewTextBoxEditingControl txt)
            {
                txt.UseSystemPasswordChar = (StringMode == StringModes.Password);
            }
        }
        public void Click(TextboxButtonCellEditingControl control)
        {
            if (Clicked != null)
            {
                Clicked(this);
                if (this.Value != null)
                    control.LabelText = this.Value.ToString();
                else
                    control.LabelText = null;

                if (this.DataGridView is DataGridViewExtended grid)
                    grid.TriggerUpdateValues();
            }
        }
    }
}
