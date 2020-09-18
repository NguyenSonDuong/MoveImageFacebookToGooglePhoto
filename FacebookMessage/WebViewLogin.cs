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
    public partial class WebViewLogin : Form
    {
        public WebViewLogin()
        {
            InitializeComponent();
        }

        private void WebViewLogin_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = false;
            webBrowser1.Navigate(new Uri("https://dictionarivietnam.herokuapp.com/getAccessToken"));
            webBrowser1.ProgressChanged += new WebBrowserProgressChangedEventHandler((Object ob, WebBrowserProgressChangedEventArgs ex)=> {
                label1.Text = ex.CurrentProgress+"/"+ex.MaximumProgress;
                if (ex.CurrentProgress == ex.MaximumProgress)
                {
                    if (webBrowser1.Url.OriginalString.Contains("dictionarivietnam.herokuapp.com/getCode"))
                        MessageBox.Show(webBrowser1.DocumentText);
                }
            });
            
        }


    }
}
