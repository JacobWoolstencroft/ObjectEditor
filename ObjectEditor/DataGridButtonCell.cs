using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    class DataGridButtonCell1 : DataGridViewTextBoxCell
    {
        public DataGridButtonCell1() : base()
        {
        }

    //Protected Overrides ReadOnly Property ThemeEffectiveType As Type
    //    Get
    //        Return GetType(GridDataCellElement)
    //    End Get
    //End Property

    //Public Overrides Function IsCompatible(ByVal data As GridViewColumn, ByVal context As Object) As Boolean
    //    Return TypeOf data Is CustomGridViewDataColumn AndAlso TypeOf context Is GridDataRowElement
    //End Function

    //Private container As StackLayoutElement
    //Private progressBarElement As RadProgressBarElement
    //Private buttonElement As RadButtonElement
    //Private textElement As LightVisualElement
        
    //Protected Overrides Sub CreateChildElements()
    //    container = New StackLayoutElement()
    //    container.Orientation = Orientation.Horizontal
    //    container.StretchHorizontally = True
    //    progressBarElement = New RadProgressBarElement()
    //    progressBarElement.StretchHorizontally = False
    //    Dim s As Size = New System.Drawing.Size(50, 20)
    //    progressBarElement.MinSize = s
    //    progressBarElement.MaxSize = s
    //    progressBarElement.Minimum = 0
    //    progressBarElement.Maximum = 1100
    //    textElement = New LightVisualElement()
    //    textElement.StretchHorizontally = True
    //    buttonElement = New RadButtonElement()
    //    buttonElement.Text = "..."
    //    buttonElement.Margin = New System.Windows.Forms.Padding(5, 0, 0, 0)
    //    buttonElement.StretchHorizontally = False
    //    AddHandler buttonElement.Click, AddressOf buttonElement_Click
    //    container.Children.Add(textElement)
    //    container.Children.Add(progressBarElement)
    //    container.Children.Add(buttonElement)
    //    Me.Children.Add(container)
    //    MyBase.CreateChildElements()
    //End Sub

    //Private Sub buttonElement_Click(ByVal sender As Object, ByVal e As EventArgs)
    //    RadMessageBox.Show(Me.Value & "")
    //End Sub

    //Protected Overrides Sub SetContentCore(ByVal value As Object)
    //    MyBase.SetContentCore(value)
    //    Me.DrawText = False

    //    If Me.RowInfo IsNot Nothing AndAlso Me.RowInfo.DataBoundItem IsNot Nothing AndAlso Me.Value IsNot Nothing Then
    //        Dim freight As Decimal = 0

    //        If Decimal.TryParse(Me.Value.ToString(), freight) Then
    //            progressBarElement.Value1 = CInt(freight)
    //        End If

    //        Me.textElement.Text = "Freight = " & Me.Value.ToString()
    //    End If
    //End Sub
    }

    //public class CalendarCell : DataGridViewTextBoxCell
    //{

    //    public CalendarCell() : base()
    //    {
    //        // Use the short date format.
    //        this.Style.Format = "d";
    //    }

    //    public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    //    {
    //        // Set the value of the editing control to the current cell value.
    //        base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
    //        CalendarEditingControl ctl = DataGridView.EditingControl as CalendarEditingControl;
    //        // Use the default row value when Value property is null.
    //        if (this.Value == null)
    //        {
    //            ctl.Value = (DateTime)this.DefaultNewRowValue;
    //        }
    //        else
    //        {
    //            ctl.Value = (DateTime)this.Value;
    //        }
    //    }

    //    public override Type EditType
    //    {
    //        get
    //        {
    //            // Return the type of the editing control that CalendarCell uses.
    //            return typeof(CalendarEditingControl);
    //        }
    //    }

    //    public override Type ValueType
    //    {
    //        get
    //        {
    //            // Return the type of the value that CalendarCell contains.
    //            return typeof(DateTime);
    //        }
    //    }

    //    public override object DefaultNewRowValue
    //    {
    //        get
    //        {
    //            // Use the current date and time as the default value.
    //            return DateTime.Now;
    //        }
    //    }
    //}

    //public class LabelButtonControl : UserControl
    //{
    //    // Create the controls.
    //    private System.Windows.Forms.ErrorProvider errorProvider1;
    //    private Label label;
    //    private Button button;
    //    private System.ComponentModel.IContainer components;

    //    public LabelButtonControl()
    //    {
    //        InitializeComponent();
    //    }

    //    // Initialize the control elements.
    //    public void InitializeComponent()
    //    {
    //        // Initialize the controls.
    //        components = new System.ComponentModel.Container();
    //        errorProvider1 = new System.Windows.Forms.ErrorProvider();
    //        label = new System.Windows.Forms.Label();

    //        label.Location = new System.Drawing.Point(8, 8);
    //        label.Size = new System.Drawing.Size(112, 23);
    //        label.Text = "Name:";
    //        label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

    //        // Add the Validating and Validated handlers for textEmail.
    //        textEmail.Validating += new System.ComponentModel.CancelEventHandler(textEmail_Validating);
    //        textEmail.Validated += new System.EventHandler(textEmail_Validated);

    //        // Add the controls to the user control.
    //        Controls.Add(label);
    //        Controls.Add(button);

    //        // Size the user control.
    //        Size = new System.Drawing.Size(375, 150);
    //    }

    //} // End Class
}
