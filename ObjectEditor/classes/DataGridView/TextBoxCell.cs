using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;
using ObjectEditor.classes.DataGridView;

namespace ObjectEditor
{
    internal class TextBoxCell : DataGridViewTextBoxCell
    {
        public StringModes StringMode = StringModes.NoChange;
        public string EmptyString = null;
        public string ButtonText = null;

        public delegate void ClickEvent(TextBoxCell cell);
        public event ClickEvent Clicked;

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (StringMode == StringModes.Password)
            {
                if (value is string s)
                    return new string(EditorField.PassChar, s.Length);
                return "";
            }
            return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }
        protected override void Paint(Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (formattedValue is string s)
            {
                if (EmptyString != null && s.Length == 0)
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
                    return typeof(TextBoxCellEditingControl);
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
                labelButton.SetMultiline(StringMode == StringModes.Multiline);
            }
            else if (DataGridView.EditingControl is TextBoxCellEditingControl txt)
            {
                txt.UseSystemPasswordChar = (StringMode == StringModes.Password);
                if (this.Value != null)
                    txt.Text = this.Value.ToString();
                else
                    txt.Text = null;
                txt.SetMultiline(StringMode == StringModes.Multiline);
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
