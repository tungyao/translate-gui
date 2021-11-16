using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var url = new System.Uri("file://C:\\Users\\yao19\\RiderProjects\\WindowsFormsApp1\\web\\index.html");
            this.webView21.Source = url;
            this.InitializeAsync();
        }
        async void InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.WebMessageReceived += MessageReceived;
            this.Post();

        }

        private void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            
        }

        private void Post()
        {
            this.webView21.CoreWebView2.PostWebMessageAsString("{\"hello\":\"world\"}");
        }
    }
}