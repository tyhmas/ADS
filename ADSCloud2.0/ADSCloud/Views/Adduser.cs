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

namespace ADSCloud.Views
{
    public partial class Adduser : Form
    {
        /// <summary>
        /// Private variables
        /// </summary>
        /// <param name="username"></param>
        /// <param name="type"></param>
        private string _username_txt;
        private string _type_txt;
        Dictionary<string, List<string>> _logindict = new Dictionary<string, List<string>>();
        private CreateDatabase _createdatabase = new CreateDatabase();
        public Adduser(string username,string type)
        {
            InitializeComponent();
            _username_txt = username;
            _type_txt = type;
            _logindict = _createdatabase.Retrievedata();
        }
        /// <summary>
        /// Add user to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if(useracctext.Text =="")
            {
                useracctext.Text = "User";
            }
            if(checkifsamedata())
            {
                bool pushflag = _createdatabase.pushuserdata(usertext.Text, passwordtext.Text, useracctext.Text);
                if (pushflag == false)
                {
                    MessageBox.Show("User Added Successfully");
                    _createdatabase.Addtoaudit(_username_txt, "Add User", "New User "+ usertext.Text + " added");
                }
            }
           
        }

        private void usertext_TextChanged(object sender, EventArgs e)
        {
            if (usertext.Text.Trim().Length >= 3 && passwordtext.Text.Trim().Length >= 3)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void passwordtext_TextChanged(object sender, EventArgs e)
        {
            if (usertext.Text.Trim().Length >= 3 && passwordtext.Text.Trim().Length >= 3)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }
        /// <summary>
        /// Back to user management screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var Ufrm = new UserMgm(_username_txt, _type_txt);
            Ufrm.Closed += (s, args) => this.Close(); /* lambda expression*/
            Ufrm.Show();
        }
        /// <summary>
        /// Support Functions
        /// </summary>
        /// <returns></returns>


        private bool checkifsamedata()
        {
            if (_logindict.ContainsKey(usertext.Text))
            {
                MessageBox.Show("Username already exists");
                _createdatabase.Addtoaudit(_username_txt, "Add User", "Username already exists");
                return false;
            }
            if (usertext.Text == passwordtext.Text)
            {
                MessageBox.Show("Username and Password cannot be same");
                _createdatabase.Addtoaudit(_username_txt, "Add User", "Username and Password cannot be same");
                return false;
            }
            return true;

        }

    }
}
