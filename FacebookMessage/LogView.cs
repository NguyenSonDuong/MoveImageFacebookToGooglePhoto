using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookMessage
{
    public partial class LogView : Form
    {
        public LogView()
        {
            InitializeComponent();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void LogView_Load(object sender, EventArgs e)
        {
            richTextBox1.AppendText(File.ReadAllText("log.lg"));
        }
    }
}
