using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEditor
{
    public partial class TextboxButtonControl : UserControl
    {
        public TextboxButtonControl()
        {
            InitializeComponent();

            button1.ForeColor = SystemColors.ControlText;
            button1.BackColor = SystemColors.Control;

            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OnButtonClicked();
        }

        public string LabelText
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        public Font LabelFont
        {
            get
            {
                return textBox1.Font;
            }
            set
            {
                textBox1.Font = value;
            }
        }
        public Color LabelBackColor
        {
            get
            {
                return textBox1.BackColor;
            }
            set
            {
                textBox1.BackColor = value;
            }
        }
        public Color LabelForeColor
        {
            get
            {
                return textBox1.ForeColor;
            }
            set
            {
                textBox1.ForeColor = value;
            }
        }

        public string ButtonText
        {
            get
            {
                return button1.Text;
            }
            set
            {
                button1.Text = value;
            }
        }

        public virtual void OnButtonClicked()
        {
        }
    }
}
