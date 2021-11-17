using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using RestSharp;
using Timer = System.Windows.Forms.Timer;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        KeyboardHook k_hook;

        public Form1()
        {
            InitializeComponent();
            var p = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var url = new System.Uri(@"file://" + p + "\\web\\index.html");
            this.webView21.Source = url;
            this.InitializeAsync();
            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new System.Windows.Forms.KeyEventHandler(hook_KeyDown); //钩住键按下 
            // k_hook.KeyPressEvent += K_hook_KeyPressEvent;
            k_hook.Start(); //安装键盘钩子
        }

        private static bool IsHanZi(string ch)
        {
            if (ch.Length < 1)
            {
                return false;
            }

            byte[] byteLen = System.Text.Encoding.Default.GetBytes(ch[0].ToString());
            if (byteLen.Length == 2)
            {
                return true;
            }

            return false;
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //判断按下的键（Alt + A） 
            if (e.KeyValue == (int)Keys.Space && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Control)
            {
                IDataObject data = Clipboard.GetDataObject();
                if (data != null)
                {
                    if (data.GetDataPresent(DataFormats.Text))
                    {
                        var articleUri = (String)data.GetData(DataFormats.Text);
                        var from = "zh";
                        var to = "en";
                        if (!IsHanZi(articleUri))
                        {
                            from = "en";
                            to = "zh";
                        }

                        var fdata = Fanyi(articleUri, from, to);
                        Post(fdata);
                    }
                }
            }
        }

        private void K_hook_KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            //tb1.Text += e.KeyChar;
            int i = (int)e.KeyChar;
            MessageBox.Show(i.ToString());
        }

        void LoopPost()
        {
            while (true)
            {
                Post(new DateTime().ToString());
                Thread.Sleep(1000);
            }
        }

        async void InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.WebMessageReceived += MessageReceived;
            // var th = new Thread(new ThreadStart(LoopPost));
            // th.Start();
        }

        private void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
        }

        private void Post(string s)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(Post), s);
            }
            else
            {
                webView21.CoreWebView2.PostWebMessageAsString(s);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Post(e.ToString());
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        private string getMd5(string s)
        {
            //初始化MD5对象
            MD5 md5 = MD5.Create();

            //将源字符串转化为byte数组
            Byte[] soucebyte = Encoding.GetEncoding("UTF-8").GetBytes(s);

            //soucebyte转化为mf5的byte数组
            Byte[] md5Bytes = md5.ComputeHash(soucebyte);

            //将md5的byte数组再转化为MD5数组
            StringBuilder sb = new StringBuilder();
            foreach (Byte b in md5Bytes)
            {
                //x表示16进制，2表示2位
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        private string Fanyi(string q, string f, string t)
        {
            var salt = GetTimeStamp();
            var s1 = "20211116001000647" + q + salt + "WNb_OmCUjSy7Ive3wCcB";
            var sign = getMd5(s1);
            var url = HttpUtility.UrlEncode(q);
            var client = new RestClient(@"http://api.fanyi.baidu.com/api/trans/vip/translate?" + "q=" + url + "&from=" +
                                        f +
                                        "&to=" + t +
                                        "&appid=20211116001000647&salt=" + salt + "&sign=" + sign);
            var req = new RestRequest(Method.GET);
            var data = client.Execute(req);
            return data.Content;
        }
    }
}