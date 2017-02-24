﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Achi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string apikey = "D5D66847FA5F20AFE3BA4A9280502A13";
        const string baseurlsteam = "api.steampowered.com";
        const string dbconn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\DEV\C#\Achi\Achi\achidb.mdf;Integrated Security=True;Connect Timeout=30";
        int j;
        public MainWindow()
        {
            InitializeComponent();
            show_categories();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebClient w = new WebClient();
            string UserName = txLogin.Text;
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
            txSteamId.Text = page;
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
            string findbeg = "<div style=\"margin-bottom: 75px";
            string findend = "<div class=\"footer\">";

            int ibeg = htmlContent.IndexOf(findbeg);
            int iend = htmlContent.IndexOf(findend);
            htmlContent = htmlContent.Substring(ibeg, iend - ibeg).Trim();
            string elements = htmlContent;
            textBox.Text = htmlContent;
            ibeg = 0;
            cleandb();
            j = 1;
            int i = 1;
            SqlConnection con = new SqlConnection(dbconn);
            con.Open();
            do
            {
                iend = elements.Substring(1).IndexOf(findbeg) + 1;
                if (iend < 30) { iend = elements.Length; ; }
                string elemdiv = elements.Substring(ibeg, iend - ibeg).Trim();
                int len = elements.Length - iend;
                if (len > 0)
                {
                    elements = elements.Substring(iend, len).Trim();
                }
                else { elements = ""; }
                ibeg = elements.IndexOf(findbeg);
                parserelementdiv(elemdiv,i);
                SqlCommand cmd = new SqlCommand("insert into testres (id,name) values (@id,@name)", con);
                cmd.Parameters.AddWithValue("@id", i);
                cmd.Parameters.AddWithValue("@name", elemdiv);
                cmd.ExecuteNonQuery();
                i++;
            } while (ibeg != -1);
        }

        private void parserelementdiv(string elemdiv,int i)
        {
            int begh3 = elemdiv.IndexOf("<h3>")+4;
            int endh3 = 0;
            
            if (begh3>4)
            {
                endh3 = elemdiv.IndexOf("</h3>");
                string interfacename = elemdiv.Substring(begh3, endh3 - begh3);
                SqlConnection con = new SqlConnection(dbconn);
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into Interface (id,interface) values (@id,@interface)", con);
                cmd.Parameters.AddWithValue("@id", i);
                cmd.Parameters.AddWithValue("@interface", interfacename);
                cmd.ExecuteNonQuery();
                con.Close();
                /*-----далее делаю парисинг методов и версий
                 * ----elemdiv переписываю в parsdiv для вырезания фрагметов
                */
                string parsdiv = elemdiv;
                string findbeg = "<div class=\"panel-heading\">";
                int begmth = parsdiv.IndexOf(findbeg);
                parsdiv = parsdiv.Substring(begmth);
                begmth = 0;
                do {
                    int endmth = parsdiv.Substring(1).IndexOf(findbeg) + 1;
                    if (endmth < 30) {endmth=parsdiv.Length; }
                    string methoddiv = parsdiv.Substring(begmth, endmth - begmth).Trim();
                    int len = parsdiv.Length - endmth;
                    if (len > 0)
                    {
                        parsdiv = parsdiv.Substring(endmth, len).Trim();
                        listBox.Text = parsdiv;
                    } else { parsdiv = ""; }
                    /*Здесь будет парсер метода с версией*/
                    int ibeg = methoddiv.IndexOf("/"+ interfacename+"/")+2+interfacename.Length;
                    int iend = methoddiv.IndexOf("</pre>");
                    string methodname = methoddiv.Substring(ibeg, iend - ibeg);
                    con.Open();
                    cmd = new SqlCommand("insert into method (id,method,interfaceid) values (@id,@method,@interface)", con);
                    cmd.Parameters.AddWithValue("@id", j);
                    cmd.Parameters.AddWithValue("@method", methodname);
                    cmd.Parameters.AddWithValue("@interface", i);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    j++;
                    // конец парсера метода с версией
                    begmth = parsdiv.IndexOf(findbeg);
                } while (begmth != -1);
            }
        }

        private void cleandb()
        {
            SqlConnection con = new SqlConnection(dbconn);
            con.Open();
            SqlCommand cmd1 = new SqlCommand("delete from testres", con);
            SqlCommand cmd2 = new SqlCommand("delete from Interface", con);
            SqlCommand cmd3 = new SqlCommand("delete from Method", con);
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            cmd3.ExecuteNonQuery();
            con.Close();
        }
        private void show_categories()
        {
            interfacename.Items.Clear();
            SqlConnection con = new SqlConnection(dbconn);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM interface", con);
                DataSet ds = new DataSet();
                da.Fill(ds, "interface");
                interfacename.ItemsSource = ds.Tables[0].DefaultView;
                interfacename.DisplayMemberPath = ds.Tables[0].Columns["interface"].ToString();
                interfacename.SelectedValuePath = ds.Tables[0].Columns["id"].ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
