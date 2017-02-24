using System;
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
            int i = 1;
            cleandb();
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\DEV\C#\Achi\Achi\achidb.mdf;Integrated Security=True;Connect Timeout=30");
            con.Open();
            do
            {
                iend = elements.Substring(1).IndexOf(findbeg) + 1;
                if (iend < 30) { break; }
                string elemdiv = elements.Substring(ibeg, iend - ibeg).Trim();
                int len = elements.Length - iend;
                if (len > 0)
                {
                    elements = elements.Substring(iend, len).Trim();
                    listBox.Text = elements;
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
                SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\DEV\C#\Achi\Achi\achidb.mdf;Integrated Security=True;Connect Timeout=30");
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into Interface (id,interface) values (@id,@interface)", con);
                cmd.Parameters.AddWithValue("@id", i);
                cmd.Parameters.AddWithValue("@interface", interfacename);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        private void cleandb()
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\DEV\C#\Achi\Achi\achidb.mdf;Integrated Security=True;Connect Timeout=30");
            con.Open();
            SqlCommand cmd1 = new SqlCommand("delete from testres", con);
            SqlCommand cmd2 = new SqlCommand("delete from Interface", con);
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            con.Close();
        }
        private void show_categories()
        {
            interfacename.Items.Clear();
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\DEV\C#\Achi\Achi\achidb.mdf;Integrated Security=True;Connect Timeout=30");
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
