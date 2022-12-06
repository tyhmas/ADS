using ADSCloud.Database;
using ADSCloud.Properties;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSCloud.Views
{
    public partial class Audit_page : Form
    {
        /// <summary>
        /// Variables
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type"></param>

        private string _username;
        private string _type;
        private SQLiteConnection sql_conn;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private CreateDatabase _createdatabase = new CreateDatabase();
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="user"></param>
        /// <param name="usertype"></param>
        public Audit_page(string user,string usertype)
        {
            InitializeComponent();
            _username = user;
            _type = usertype;
            datagridprop();
            setdb();
            datagridcoldim();
        }

        //Set SQL connection
        private void setconnection()
        {
            sql_conn = new SQLiteConnection("Data Source=Audit.db;Version=3;New=True;Compress=True;");
        }

        //set execute query
        private void setquery(string txtquery)
        {
            setconnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            sql_cmd.CommandText = txtquery;
            sql_cmd.ExecuteNonQuery();
            sql_conn.Close();
        }
        //set loadDB
        private void setdb()
        {
            setconnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string CommandText = "SELECT * FROM Auditdata";
            DB = new SQLiteDataAdapter(CommandText, sql_conn);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView1.DataSource = DT;
            sql_conn.Close();
            DataGridViewColumn column = dataGridView1.Columns[0];
            column.Width = 100;
            DataGridViewColumn column1 = dataGridView1.Columns[1];
            column1.Width = 60;
            DataGridViewColumn column2 = dataGridView1.Columns[2];
            column2.Width = 430;
            DataGridViewColumn column3 = dataGridView1.Columns[3];
            column3.Width = 200;
        }

        /// <summary>
        /// Main Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mainmenu_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var Mfrm = new MainMenu(_username, _type);
            Mfrm.Closed += (s, args) => this.Close(); /* lambda expression*/
            Mfrm.Show();
        }
        /// <summary>
        /// Save Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void adduser_Click(object sender, EventArgs e)
        {
            try
            {
                string path = @"c:\ADS\Audit";
                System.IO.Directory.CreateDirectory(path);
                string result = "Audit ";
                string filename = result + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".pdf";
                path = System.IO.Path.Combine(path, filename);
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fontpara1 = new iTextSharp.text.Font(bf, 10f, iTextSharp.text.Font.BOLD);
                Document doc = new Document(iTextSharp.text.PageSize.A4, 10, 10, 42, 35);
                PdfWriter write = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 25f, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontpara = new iTextSharp.text.Font(bf, 10f, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontheader = new iTextSharp.text.Font(bf, 15f, iTextSharp.text.Font.NORMAL);
                try
                {
                    doc.Open();
                    Paragraph space = new Paragraph(new Chunk("---------------------------------------------------------------------"));
                    space.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    space.SpacingAfter = 4f;
                    Paragraph p1 = new Paragraph(new Chunk("Audit Report", font));
                    p1.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    doc.Add(p1);
                    doc.Add(space);

                    Paragraph space1 = new Paragraph(new Chunk("                                                                     "));
                    space1.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    space1.SpacingAfter = 5f;
                    doc.Add(space1);
                    int colcount = dataGridView1.Columns.Count;
                    int rowcount = dataGridView1.Rows.Count;
                    if (rowcount > 0 || rowcount > 0)
                    {
                        PdfPTable table = new PdfPTable(dataGridView1.Columns.Count);
                        /*table.TotalWidth = 128f;
                        table.LockedWidth = true;*/
                        for (int k = 0; k < dataGridView1.Columns.Count; k++)
                        {
                            table.AddCell(new Phrase(dataGridView1.Columns[k].HeaderText, fontpara1));
                        }
                        table.HeaderRows = 1;

                        for (int k = 0; k < rowcount; k++)
                        {
                            for (int j = 0; j < colcount; j++)
                            {

                                if (dataGridView1.Rows[k].Cells[j].Value != null)
                                {
                                    table.AddCell(new Phrase(dataGridView1.Rows[k].Cells[j].Value.ToString(), fontpara));
                                }
                            }
                        }
                        doc.Add(table);
                        doc.Add(space);
                    }
                    else
                    {
                        MessageBox.Show("No Data In Table");
                        doc.Close();
                        goto stop;
                    }
                    DateTime Ptime = DateTime.Now;
                    Paragraph para15 = new Paragraph(new Chunk("Report Generated on: " + Ptime.ToString(), fontpara));
                    para15.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
                    para15.SpacingAfter = 30f;
                    doc.Add(para15);
                    doc.Close();
                    AddPageNumber(filename);
                    MessageBox.Show("File Saved");
                    _createdatabase.Addtoaudit(_username, "Audit Page", "Audit Report PDF generated");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    doc.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            stop:
            int a = 0;
        }
        private void AddPageNumber(string filename)
        {
            string path = @"c:\ADS\Audit";
            path = System.IO.Path.Combine(path, filename);
            byte[] bytes = File.ReadAllBytes(path);
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10f, iTextSharp.text.Font.NORMAL);
            var image = iTextSharp.text.Image.GetInstance(Resources.logos_logan, System.Drawing.Imaging.ImageFormat.Bmp);
            image.ScalePercent(15f);
            image.SetAbsolutePosition(490f, 810f);

            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), font), 568f, 15f, 0);
                        var pdfContentByte = stamper.GetOverContent(i);
                        pdfContentByte.AddImage(image);
                    }

                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(path, bytes);
        }
        /// <summary>
        /// Search username
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataView dv = DT.DefaultView;
            dv.RowFilter = string.Format("Username LIKE '%{0}%'", textBox1.Text);
            dataGridView1.DataSource = dv.ToTable();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            DataView dv = DT.DefaultView;
            dv.RowFilter = string.Format("Page LIKE '%{0}%'", textBox2.Text);
            dataGridView1.DataSource = dv.ToTable();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                textBox2.Enabled = false;
                textBox1.Enabled = true;
            }
            else
            {
                textBox2.Enabled = true;
                textBox1.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton2.Checked == true)
            {
                textBox2.Enabled = true;
                textBox1.Enabled = false;
            }
            else
            {
                textBox2.Enabled = false;
                textBox1.Enabled = true;

            }
        }

        private void datagridprop()
        {

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.WhiteSmoke;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
        }

        private void datagridcoldim()
        {
            dataGridView1.Columns[0].Width = 270;
            dataGridView1.Columns[1].Width = 250;
            dataGridView1.Columns[2].Width = 400;
            dataGridView1.Columns[3].Width = 300;
        }
    }
}
