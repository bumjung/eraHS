using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using eraHS.Utility;

namespace eraHS
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
            this.richTextBox1.ReadOnly = true;
            this.update();
        }

        public void update()
        {   
            this.richTextBox1.Text = Logger.toString();
        }
    }
}
