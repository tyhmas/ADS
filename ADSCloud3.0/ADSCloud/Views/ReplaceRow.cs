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

    public class RowDataEventArgs : EventArgs
    {
        public readonly int oldRowNumber;
        public readonly int newRowNumber;

        public RowDataEventArgs(int oldRowNumber, int newRowNumber)
        {
            this.oldRowNumber = oldRowNumber;
            this.newRowNumber = newRowNumber;
        }
    }

    public partial class ReplaceRow : Form
    {
        /// <summary>
        /// Private variables
        /// </summary>
        /// <param name="username"></param>
        /// <param name="type"></param>
        //private string _username_txt;
        //private string _type_txt;
        //Dictionary<string, List<string>> _logindict = new Dictionary<string, List<string>>();
        //private CreateDatabase _createdatabase = new CreateDatabase();
        public delegate void RowDataEventHandler(RowDataEventArgs e);
        public event RowDataEventHandler dataReady;
        private RowDataEventArgs ee;

        public void OnFetchRowData(RowDataEventArgs e)
        {
            dataReady(e);
        }

        public ReplaceRow()
        {
            InitializeComponent();
        }

        //public ReplaceRow(string username,string type)
        //{
        //    InitializeComponent();
        //    _username_txt = username;
        //    _type_txt = type;
        //    _logindict = _createdatabase.Retrievedata();
        //}
        /// <summary>
        /// OK to replace a row with new data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, EventArgs e)
        {
            int oldNumber = Int16.Parse(oldRowNumber.Text);
            int newNumber = Int16.Parse(newRowNumber.Text);

            if (oldNumber > 0 && newNumber > 0)
            {
                ee = new RowDataEventArgs(oldNumber, newNumber);
                OnFetchRowData(ee);
            }
        }

        private void usertext_TextChanged(object sender, EventArgs e)
        {
            if (oldRowNumber.Text.Trim().Length >= 3 && newRowNumber.Text.Trim().Length >= 3)
            {
                OK.Enabled = true;
            }
            else
            {
                OK.Enabled = false;
            }
        }

        private void passwordtext_TextChanged(object sender, EventArgs e)
        {
            if (oldRowNumber.Text.Trim().Length >= 3 && newRowNumber.Text.Trim().Length >= 3)
            {
                OK.Enabled = true;
            }
            else
            {
                OK.Enabled = false;
            }
        }
        /// <summary>
        /// Cancel replacement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, EventArgs e)
        {
            oldRowNumber.Text = "";
            newRowNumber.Text = "";
        }
        ///// <summary>
        ///// Support Functions
        ///// </summary>
        ///// <returns></returns>


        //private bool checkifsamedata()
        //{
        //    if (_logindict.ContainsKey(oldRowNumber.Text))
        //    {
        //        MessageBox.Show("Username already exists");
        //        _createdatabase.Addtoaudit(_username_txt, "Add User", "Username already exists");
        //        return false;
        //    }
        //    if (oldRowNumber.Text == newRowNumber.Text)
        //    {
        //        MessageBox.Show("Username and Password cannot be same");
        //        _createdatabase.Addtoaudit(_username_txt, "Add User", "Username and Password cannot be same");
        //        return false;
        //    }
        //    return true;

        //}

    }
}
