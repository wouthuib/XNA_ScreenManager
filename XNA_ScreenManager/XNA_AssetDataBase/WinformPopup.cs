using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XNA_AssetDataBase
{
    public partial class WinformPopup : Form
    {
        public WinformPopup(string exception)
        {
            InitializeComponent();
            this.textBox1.Text = exception;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
