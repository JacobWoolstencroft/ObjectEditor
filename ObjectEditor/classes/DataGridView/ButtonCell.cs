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
    internal class ButtonCell : DataGridViewButtonCell
    {
        private bool enabledValue;
        public bool Enabled
        {
            get
            {
                return enabledValue;
            }
            set
            {
                bool refresh = enabledValue != value;
                enabledValue = value;
                if (refresh && this.DataGridView != null)
                    this.DataGridView.InvalidateCell(this);
            }
        }

        // Override the Clone method so that the Enabled property is copied.
        public override object Clone()
        {
            ButtonCell cell = (ButtonCell)base.Clone();
            cell.Enabled = this.Enabled;
            return cell;
        }

        public ButtonCell()
        {
            this.enabledValue = true;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = style.SelectionBackColor = SystemColors.Control;
            style.ForeColor = style.SelectionForeColor = SystemColors.ControlText;
            this.Style = style;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            // The button cell is disabled, so paint the border,
            // background, and disabled button for the cell.
            if (!this.enabledValue)
            {
                // Draw the cell background, if specified.
                if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                {
                    SolidBrush cellBackground = new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // Draw the cell borders, if specified.
                if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
                }

                // Calculate the area in which to draw the button.
                Rectangle buttonArea = cellBounds;
                Rectangle buttonAdjustment = this.BorderWidths(advancedBorderStyle);
                buttonArea.X += buttonAdjustment.X;
                buttonArea.Y += buttonAdjustment.Y;
                buttonArea.Height -= buttonAdjustment.Height;
                buttonArea.Width -= buttonAdjustment.Width;

                // Draw the disabled button.
                ButtonRenderer.DrawButton(graphics, buttonArea, PushButtonState.Disabled);

                buttonArea.X += 4;
                buttonArea.Width -= 8;

                // Draw the disabled button text.
                TextRenderer.DrawText(graphics, this.FormattedValue.ToString(), this.DataGridView.Font, buttonArea, SystemColors.GrayText, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                // The button cell is enabled, so let the base class handle the painting.
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
}
