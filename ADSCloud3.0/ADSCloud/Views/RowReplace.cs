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
    public partial class RowReplace : Form
    {
        List<TextBox> textBoxes;
        Dictionary<int, int> res = new Dictionary<int, int>();

        public RowReplace()
        {
            InitializeComponent();
            textBoxes = new List<TextBox>() { textBox1, textBox3, textBox5 };
            //textBox1.Enabled = false;
            //textBox2.Enabled = false;
            //textBox3.Enabled = false;
            //textBox4.Enabled = false;
            //textBox5.Enabled = false;
            //textBox6.Enabled = false;
        }

        public void Receive(List<int> rowData)
        {
            int i = 0;
            foreach (int row in rowData)
            {
                textBoxes[i].Enabled = true;
                textBoxes[i + 1].Enabled = true;
                textBoxes[i].Text = "" + row;
                i += 1;
            }
        }

        public Dictionary<int, int> GetData()
        {
            int oldRow, newRow;
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                oldRow = short.Parse(textBox1.Text);
                newRow = short.Parse(textBox2.Text);
                res[oldRow] = newRow;
            }
            if (textBox3.Text != "" && textBox4.Text != "")
            {
                oldRow = short.Parse(textBox3.Text);
                newRow = short.Parse(textBox4.Text);
                res[oldRow] = newRow;
            }
            if (textBox5.Text != "" && textBox6.Text != "")
            {
                oldRow = short.Parse(textBox5.Text);
                newRow = short.Parse(textBox6.Text);
                res[oldRow] = newRow;
            }
            return res;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
