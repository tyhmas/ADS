using ADSCloud.Database;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ADSCloud.Views
{
    public partial class UserMgm : Form
    {
        /// <summary>
        /// Variables
        /// </summary>
        private CreateDatabase _createdatabase = new CreateDatabase();
        Dictionary<string, List<string>> _logindict = new Dictionary<string, List<string>>();
        private string _Username;
        private string _usertype;
        SQLiteConnection conn;
        SQLiteCommand cmd;
        /**************************************************************************/
        public UserMgm(string _user,string _type)
        {
            InitializeComponent();
            _datagridviewProp();
            _Username = _user;
            _usertype = _type; 
            _logindict = _createdatabase.Retrievedata();
            int i = 0;
            foreach (KeyValuePair<string, List<string>> dataGrid in _logindict)
            {

                if (dataGrid.Key != "Adm")
                {
                    string username = dataGrid.Key;
                    string Password = dataGrid.Value[0];
                    string AccType = dataGrid.Value[1];
                    string Date = dataGrid.Value[2];
                    string status = dataGrid.Value[3];
                    this.dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = username;
                    dataGridView1.Rows[i].Cells[1].Value = Password;
                    dataGridView1.Rows[i].Cells[2].Value = AccType;
                    dataGridView1.Rows[i].Cells[3].Value = Date;
                    dataGridView1.Rows[i].Cells[4].Value = status;
                    i++;
                }
            }
        }
        /// <summary>
        /// DatagridView Properties
        /// </summary>
        private void _datagridviewProp()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            //dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.WhiteSmoke;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dataGridView1.Columns[0].DefaultCellStyle = new DataGridViewCellStyle { ForeColor = Color.Black };
        }
        /// <summary>
        /// Main Menu Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mainmenu_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var Mfrm = new MainMenu(_Username, _usertype);
            Mfrm.Closed += (s, args) => this.Close(); /* lambda expression*/
            Mfrm.Show();
        }
        /// <summary>
        /// Add User
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void adduser_Click(object sender, EventArgs e)
        {
            this.Hide();
            var Addfrm = new Adduser(_Username, _usertype);
            Addfrm.Closed += (s, args) => this.Close(); /* lambda expression*/
            Addfrm.Show();
        }
        /// <summary>
        /// Enable and disable users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _logindict = _createdatabase.Retrievedata();
            int rowindex = e.RowIndex;
            DataGridViewRow selectedrow = dataGridView1.Rows[rowindex];
            string names = selectedrow.Cells[0].Value.ToString();
            bool enbalevalue = bool.Parse(_logindict[names][3]);
            if(enbalevalue)
            {
                enbalevalue = false;
            }
            else
            {
                enbalevalue = true;
            }
            try
            {
                using (conn = new SQLiteConnection("Data Source=ADS.db;Version=3;New=True;Compress=True;"))
                {
                    conn.Open();
                    string commandstring = "UPDATE Login Set Setvalue = @Setvalue WHERE Username ='" + names + "'";
                    using (cmd = new SQLiteCommand(commandstring, conn))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@Setvalue", enbalevalue));
                        cmd.ExecuteNonQuery();
                        if (!enbalevalue)
                        {
                            MessageBox.Show("User " + names + " profile dectivated");
                            dataGridView1.Rows[e.RowIndex].Cells[4].Value = false;
                        }
                        else
                        {
                            MessageBox.Show("User " + names + " profile activated");
                            dataGridView1.Rows[e.RowIndex].Cells[4].Value = true;
                        }
                    }
                }
                _createdatabase.Addtoaudit(_Username, "User Management", "Profile status of "+ names+" changed ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UserMgm_Load(object sender, EventArgs e)
        {

        }
    }
}
