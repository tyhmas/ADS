using ADSCloud.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSCloud
{
    public partial class Login : Form
    {
        private CreateDatabase _createdatabase = new CreateDatabase();
        Dictionary<string, List<string>> _logindict = new Dictionary<string, List<string>>();
        public Login()
        {
            InitializeComponent();
            _createdatabase.Createdb();
            _logindict = _createdatabase.Retrievedata();
            string pathOp = @"c:\ADS\Report";
            System.IO.Directory.CreateDirectory(pathOp);
            string pathOpAudit = @"c:\ADS\Audit";
            System.IO.Directory.CreateDirectory(pathOpAudit);
        }
        /// <summary>
        /// Login to application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                _createdatabase.Addtoaudit(username.Text, "Login", "User" + "  " + username.Text + " logged in");
                this.Hide();
                var Mfrm = new MainMenu(username.Text, _logindict[username.Text][1]);
                Mfrm.Closed += (s, args) => this.Close(); /* lambda expression*/
                Mfrm.Show();
            }
            else
            {
                _createdatabase.Addtoaudit(username.Text, "Login Page", "Login Failed, Incorrect username or password " + username.Text);
                MessageBox.Show("Login Unsuccessful. Try Again!");
            }
        }
        /// <summary>
        /// Checks the data entered in the textboxes
        /// </summary>
        /// <returns></returns>
        public new bool Validate()
        {
            bool output = false;
            if (_logindict.ContainsKey(username.Text))
            {
                if (_logindict[username.Text][0] == password.Text && bool.Parse(_logindict[username.Text][3]) == true)
                {
                    output = true;
                }
            }
            return output;
        }
        /// <summary>
        /// Checking if the chars > 3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void username_TextChanged(object sender, EventArgs e)
        {
            if (username.Text.Trim().Length >= 3 && password.Text.Trim().Length >= 3)
            {
                loginbutton.Enabled = true;
            }
            else
            {
                loginbutton.Enabled = false;
            }
        }
        private void password_TextChanged(object sender, EventArgs e)
        {
            if (username.Text.Trim().Length >= 3 && password.Text.Trim().Length >= 3)
            {
                loginbutton.Enabled = true;
            }
            else
            {
                loginbutton.Enabled = false;
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
