using ADSCloud.Database;
using ADSCloud.Views;
using System;
using System.Windows.Forms;

namespace ADSCloud
{
    public partial class MainMenu : Form
    {
        /// <summary>
        /// Private variables
        /// </summary>
        /// 

        private string _usernameTxt;
        private string _typrTxt;
        private CreateDatabase _createdatabase = new CreateDatabase();
        



        public MainMenu(string username, string type)
        {
            InitializeComponent();
            _usernameTxt = username;
            _typrTxt = type;
            if(_typrTxt == "Manager")
            {
                button6.Enabled = false;
                Auditbtn.Enabled = false;
            }
            if (_typrTxt == "User")
            {
                button6.Enabled = false;
                Auditbtn.Enabled = false;
            }
        }

        /// <summary>
        /// Generate Report Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void button1_Click(object sender, EventArgs e)
        {
            _createdatabase.Addtoaudit(_usernameTxt, "Main Menu", "Clicked on Generate Dissolution Report Button");
            this.Hide();
            var Gfrm = new Generate_report_tf(_usernameTxt, _typrTxt);
            Gfrm.Closed += (s, args) => this.Close(); /* lambda expression*/
            Gfrm.Show();
        }

        /// <summary>
        /// User Management
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void button6_Click(object sender, EventArgs e)
        {
            _createdatabase.Addtoaudit(_usernameTxt, "Main Menu", "Clicked on User Management Button");
            this.Hide();
            var Ufrm = new UserMgm(_usernameTxt, _typrTxt);
            Ufrm.Closed += (s, args) => this.Close(); /*lambda expression*/
            Ufrm.Show();
        }

        /// <summary>
        /// Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void button5_Click(object sender, EventArgs e)
        { 
            try
            {
                string path = @"c:\ADS\";
                System.IO.Directory.CreateDirectory(path);
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = @"c:\ADS\";
                openFileDialog1.Filter = "PDF document (*.pdf)|*.pdf";
                DialogResult result = openFileDialog1.ShowDialog();
                _createdatabase.Addtoaudit(_usernameTxt, "Open Report", "Clicked on Open Report Button");
                if (result == DialogResult.OK)
                {                   
                    string pdfFile = openFileDialog1.FileName;
                    System.Diagnostics.Process.Start(pdfFile);
                }
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.Message);
            }
        }

        private void Auditbtn_Click(object sender, EventArgs e)
        {
            _createdatabase.Addtoaudit(_usernameTxt, "Main Menu", "Clicked on Audit Button");
            this.Hide();
            var Afrm = new Audit_page(_usernameTxt, _typrTxt);
            Afrm.Closed += (s, args) => this.Close(); /*lambda expression*/
            Afrm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _createdatabase.Addtoaudit(_usernameTxt, "Main Menu", "Clicked on Device Control");
            this.Hide();
            var Dfrm = new Device_Ctrl(_usernameTxt, _typrTxt);
            Dfrm.Closed += (s, args) => this.Close(); /*lambda expression*/
            Dfrm.Show();
        }
    }
}
