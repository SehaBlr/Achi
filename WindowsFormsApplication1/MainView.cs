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
            string htmlContent = reader.ReadToEnd();
            // textBox2.Text = htmlContent;
            data.Close();
            reader.Close();
            IHTMLDocument2 htmlDocument = (IHTMLDocument2)new mshtml.HTMLDocument();
            IHTMLDocument2 allElementsDiv = (IHTMLDocument2)new mshtml.HTMLDocument();
            htmlDocument.write(htmlContent);
            IHTMLElementCollection allElements = htmlDocument.all;
            IHTMLElementCollection divcontainer = null;
            foreach (IHTMLElement element in allElements)
            {
                if (element.className== "container body-content") {
                    testbox.Text=element.innerHTML;
                    allElementsDiv.write(element.innerHTML);
                    divcontainer = allElementsDiv.all;
                    break;
                }
            }
           /* if (divcontainer != null)
            {
                foreach (IHTMLAnchorElement element in divcontainer)
                {

                }
            }*/
        }

    }
}
