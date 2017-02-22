using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;

namespace WindowsFormsApplication1
{
    public partial class MainView : Form
    {
        const string apikey = "D5D66847FA5F20AFE3BA4A9280502A13";
        const string baseurlsteam = "api.steampowered.com";
        public MainView()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient w = new WebClient();
            string UserName = LoginText.Text;
            string vLink = "http://steamcommunity.com/id/" + UserName + "/";
            string page = w.DownloadString(vLink);
            int n = page.IndexOf("\"steamid\":");
            if (n != 0)
            {
                page = page.Substring(n + 11, 50);
                page = page.Substring(0, page.IndexOf("\""));
                string steamId64 = page;
            }
            else
            {
                page = "Необходимо в настроках профиля указать персональную ссылку";
                string steamId64 = "0";
            }
            SteamIDText.Text = page;
            parserDocumentation();
        }
        private void parserDocumentation()
        {
            WebClient w = new WebClient();
            Stream data = w.OpenRead(new Uri("http://steamwebapi.azurewebsites.net/"));
            StreamReader reader = new StreamReader(data);
            string htmlContent = reader.ReadToEnd().ToLower();
            data.Close();
            reader.Close();
            string findbeg = "<div style=\"margin-bottom: 75px\">";
            string findend = "<div class=\"footer\">";
            int ibeg = htmlContent.IndexOf(findbeg);
            int iend = htmlContent.IndexOf(findend);
            htmlContent = htmlContent.Substring(ibeg+34);
            ibeg = htmlContent.IndexOf(findbeg);
            htmlContent = htmlContent.Substring(0, ibeg);
            testbox.Text = htmlContent;
            IHTMLDocument2 htmlDocument = (IHTMLDocument2)new mshtml.HTMLDocument();// 1.создаем объект html
            htmlDocument.write(htmlContent);                                        // 2.в объект html записываем код
            IHTMLElementCollection allElements = htmlDocument.all;                  // 3.из кода вычленяем элементы HTML
            foreach (IHTMLElement element in allElements)                           // 4.перебираем эти элементы
            {
                listBox1.Items.Add(element.tagName);
            }
        }

    }
}
