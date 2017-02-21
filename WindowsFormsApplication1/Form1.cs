﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        const string apikey = "D5D66847FA5F20AFE3BA4A9280502A13";
        const string baseurlsteam = "api.steampowered.com";

        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient w = new WebClient();
            string UserName = textEdit1.Text;
            string vLink = "http://steamcommunity.com/id/" + UserName + "/";
            string page = w.DownloadString(vLink);
            int n = page.IndexOf("\"steamid\":");
            if (n!=0) {
                page = page.Substring(n + 11, 50);
                page = page.Substring(0, page.IndexOf("\""));
                string  steamId64 = page;
            }
            else
            {
                page = "Необходимо в настроках профиля указать персональную ссылку";
                string  steamId64 = "0";
            }
            
            UrlApi u = new WindowsFormsApplication1.UrlApi();
            u.Fbaseurl = baseurlsteam;
            u.Finterface = "ISteamUser";
            u.Fmethod = "GetPlayerSummaries";
            u.Fversion = "v2";
            u.Fkey = apikey;
            u.Fsteamid = page;
            string url = u.getUrl();
            memoEdit1.Text = url;
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}