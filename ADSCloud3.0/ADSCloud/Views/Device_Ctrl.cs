using System;
using System.Windows.Forms;

namespace ADSCloud.Views
{
    public partial class Device_Ctrl : Form
    {
        private string _user;
        private string _user_type;
        public Device_Ctrl(string username, string type)
        {
            InitializeComponent();
            _user = username;
            _user_type = type;
        }
        /// <summary>
        /// Go back to Main Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var Mainfrm = new MainMenu(_user,_user_type);
            Mainfrm.Closed += (s, args) => this.Close(); /*lambda expression*/
            Mainfrm.Show();
        }
    }
}
