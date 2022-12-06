using ADSCloud.Database;
using ADSCloud.Properties;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ADSCloud.Views
{
    public partial class Generate_report_tf : Form
    {
        /// <summary>
        /// Variables
        /// </summary>
        private string _usernameTxt;
        private string _typrTxt;
        private string _filepath;
        private int _totalsample;
        private int _totalsamples;
        private double _sampleconcentration = 0.0;
        private double _perdissolution = 0.0;
        List<int> TimeData = new List<int>();
        private CreateDatabase _createdatabase = new CreateDatabase();
        /// <summary>
        /// Main entry function to the program
        /// </summary>
        /// constructor to the function
        /// <param name="username"></param>
        /// <param name="type"></param>
        public Generate_report_tf(string username, string type)
        {
            InitializeComponent();
            //user login information -> username and type of user
            _usernameTxt = username;
            _typrTxt = type;
            //Datagrid Properties
            _Datagridcolor();
        }
        /// <summary>
        /// Main Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menubutton_Click(object sender, EventArgs e)
        {
            this.Hide();
            var Mfrm = new MainMenu(_usernameTxt, _typrTxt);
            Mfrm.Closed += (s, args) => this.Close(); /* lambda expression*/
            Mfrm.Show();
        }
        /// <summary>
        /// File Path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void upload_Click(object sender, EventArgs e)
        {
            try
            {
                string path = @"C:\";
                System.IO.Directory.CreateDirectory(path);
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = @"C:\";
                openFileDialog1.Filter = "PDF document (*.pdf)|*.pdf";
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _filepath = openFileDialog1.FileName;
                    extbutton.Enabled = true;
                    MessageBox.Show("File successfully uploaded!");
                }
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.Message);
            }

            _clean_calculate();
        }



        /// <summary>
        /// Extract button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extbutton_Click(object sender, EventArgs e)
        {
            string[] lines;
            string strText = string.Empty;
            if (stpagenum_txt.Text == "" || stdsample_txt.Text == "" || Nsamples_txt.Text == "" || blanksamp.Text == "")
            {
                MessageBox.Show("Enter all the values!");
            }
            else
            {
                List<double> absval = new List<double>();
                dataGridView1.Rows.Clear();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                try
                {
                    int countvalue = 0;
                    int i = 0;
                    int ind = 0;
                    DataGridViewRow row = new DataGridViewRow();
                    PdfReader reader = new PdfReader(_filepath);

                    //Parse data from a determined page range
                    for (int page = int.Parse(stpagenum_txt.Text); page <= reader.NumberOfPages; page++)
                    {
                        int j = 0;
                        ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                        strText = PdfTextExtractor.GetTextFromPage(reader, page, its);
                        //creating the string array and storing the PDF line by line
                        lines = strText.Split('\n');
                        var valueCheck = Regex.Split(lines[4], @"[^0-9\.]+")
                                          .Where(c => c != "." && c.Trim() != "").ToList();
                        if (valueCheck.Count != 0)
                        {
                            j = 4;
                        }

                        else
                        {
                            //foreach (String s in lines){
                            //    Console.WriteLine(s);
                            //}
                            // Locate the line to find data
                            int val;
                            if ((val = Array.FindIndex(lines, x => x.Contains("mAU*min"))) != -1)
                            {
                                ind = val;
                            }
                            else if ((val = Array.FindIndex(lines, x => x.Contains("min"))) != -1)
                            {
                                ind = val;
                            }
                            j = ind + 3 + int.Parse(blanksamp.Text) + int.Parse(stdsample_txt.Text);//Index of min + two lines  + standard value
                        }
                        while (j < lines.Count())
                        {
                            dataGridView1.Rows.Add();
                            var value = Regex.Split(lines[j], @"[^0-9\.]+")
                                          .Where(c => c != "." && c.Trim() != "").ToList();
                            if (value.Count != 0)
                            {
                                dataGridView1.Rows[i].Cells[0].Value = value[0];
                                dataGridView1.Rows[i].Cells[1].Value = value[1];
                                TimeData.Add(int.Parse(value[1]));
                                dataGridView1.Rows[i].Cells[2].Value = value[4];
                                absval.Add(double.Parse(value[4]));
                                i++;
                                j++;
                            }
                            else
                            {
                                j = ind = Array.FindIndex(lines, x => x.Contains("mAU*min"));
                                j = ind + 6;
                            }
                        }
                        var q = TimeData.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count);
                        countvalue = q.ElementAt(0).Count;

                        _totalsample = countvalue / int.Parse(Nsamples_txt.Text);
                        _totalsamples = absval.Count / _totalsample;
                    }
                    //index = TimeData.IndexOf(TimeData[TimeData.Count - 1]);
                    reader.Close();
                    groupBox3.Enabled = true;
                    //chart properties
                    _chartproperties();
                    if (_totalsample >= 2)
                    {
                        Stdconctxttwo.Enabled = true;
                        stdavgtxttwo.Enabled = true;
                        factortwo.Enabled = true;
                        dissolvoltwo.Enabled = true;
                        labelamtapI2.Enabled = true;

                        if (_totalsample == 3)
                        {
                            Stdconctxtthree.Enabled = true;
                            stdavgtxtthree.Enabled = true;
                            factorthree.Enabled = true;
                            dissolvolthree.Enabled = true;
                            labelamtapI3.Enabled = true;
                        }
                    }
                    extbutton.Enabled = false;
                    TimeData = TimeData.Distinct().ToList();
                    _createdatabase.Addtoaudit(_usernameTxt, "Generate Dissolution Page", "Data extracted");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// First Factor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void factorcal_Click(object sender, EventArgs e)
        {
            if (Stdconctxt.Text == "" || stdavgtxt.Text == "")
            {
                MessageBox.Show("Enter all the values!");
            }
            else
            {
                double _Factoronecal = double.Parse(Stdconctxt.Text) / double.Parse(stdavgtxt.Text);
                factorapi.Text = _Factoronecal.ToString().Length > 12 ? _Factoronecal.ToString().Substring(0, 10) : _Factoronecal.ToString();
                caldisso.Enabled = true;
            }
        }
        /// <summary>
        /// Second Factor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void factortwo_Click(object sender, EventArgs e)
        {
            if (Stdconctxttwo.Text == "" || stdavgtxttwo.Text == "")
            {
                MessageBox.Show("Enter all the values!");
            }
            else
            {
                double _Factoronecal = double.Parse(Stdconctxttwo.Text) / double.Parse(stdavgtxttwo.Text);
                factorapitwo.Text = _Factoronecal.ToString().Length > 12 ? _Factoronecal.ToString().Substring(0, 10) : _Factoronecal.ToString();
            }
        }

        /// <summary>
        /// Third Factor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void factorthree_Click(object sender, EventArgs e)
        {
            if (Stdconctxtthree.Text == "" || stdavgtxtthree.Text == "")
            {
                MessageBox.Show("Enter all the values!");
            }
            else
            {
                double _Factoronecal = double.Parse(Stdconctxtthree.Text) / double.Parse(stdavgtxtthree.Text);
                factorapithree.Text = _Factoronecal.ToString().Length > 12 ? _Factoronecal.ToString().Substring(0, 10) : _Factoronecal.ToString();
            }
        }

        /// <summary>
        /// calculate the percentage Dissolution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caldisso_Click(object sender, EventArgs e)
        {

            bool empty1 = Labelamount.Text == "" || dissovolone.Text == "";
            bool empty2 = labelamtapI2.Text == "" || dissolvoltwo.Text == "";
            bool empty3 = labelamtapI3.Text == "" || dissolvolthree.Text == "";


            if (_totalsample == 1 && empty1)
            {
                MessageBox.Show("Enter all the values!");
            }

            else if (_totalsample == 2 && (empty1 || empty2))
            {
                MessageBox.Show("Enter all the values!");

            }

            else if (_totalsample == 3 && (empty1 || empty2 || empty3))
            {
                MessageBox.Show("Enter all the values!");
            }

            else
            {
                chart1.Series.Clear();
                chart2.Series.Clear();
                chart3.Series.Clear();
                _chartproperties();
              
                try
                {
                    if (dissovolone.Text == "" )
                    {
                        dissovolone.Text = "1";
                    }
                    if (dissolvoltwo.Text == "")
                    {
                        dissolvoltwo.Text = "1";
                    }
                    if (dissolvolthree.Text == "")
                    {
                        dissolvolthree.Text = "1";
                    }

                    if (_totalsample > 1)
                    {

                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            if (i < _totalsamples)
                            {
                                _sampleconcentration = double.Parse(factorapi.Text) * double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                                _perdissolution = (_sampleconcentration * double.Parse(dissovolone.Text) / double.Parse(Labelamount.Text))*100;
                            }
                            else
                            {
                                _sampleconcentration = double.Parse(factorapitwo.Text) * double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                                _perdissolution = (_sampleconcentration * double.Parse(dissolvoltwo.Text) / double.Parse(labelamtapI2.Text))*100;
                            }

                            dataGridView1.Rows[i].Cells[3].Value = _perdissolution.ToString().Length > 9 ? _perdissolution.ToString().Substring(0, 7) : _perdissolution.ToString();
                        }
                    }
                    else
                    {
                        if (dissovolone.Text == "")
                        {
                            dissovolone.Text = "1";
                        }
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            _sampleconcentration = double.Parse(factorapi.Text) * double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                            _perdissolution = (_sampleconcentration * double.Parse(dissovolone.Text) / double.Parse(Labelamount.Text))*100;
                            dataGridView1.Rows[i].Cells[3].Value = _perdissolution.ToString().Length > 9 ? _perdissolution.ToString().Substring(0, 7) : _perdissolution.ToString();

                        }
                    }
                    _createdatabase.Addtoaudit(_usernameTxt, "Generate Dissolution Page", "Dissolution data generated");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                _chartdraw();
            }
        }
        ///<summary>
        ///Helper Functions
        /// 
        /// </summary>
        /// 
        private void _Datagridcolor()
        {
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
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
        /// Chart properties and chart
        /// </summary>
        private void _chartproperties()
        {
            /************************************************************************************************/
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.Title = "Time (min) --->";
            chart1.ChartAreas[0].AxisY.Title = "% Dissolution  --->";
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart1.ChartAreas[0].AxisX.IsStartedFromZero = true;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           
            int _countsample = 1;
            try
            {
                if (_totalsample >= 2)
                {
                    _countsample = int.Parse(Nsamples_txt.Text) * _totalsample;
                    /************************************************************************************************/
                    chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
                    chart2.ChartAreas[0].AxisX.Title = "Time (min) --->";
                    chart2.ChartAreas[0].AxisY.Title = "% Dissolution  --->";
                    chart2.ChartAreas[0].AxisX.IsMarginVisible = false;
                    chart2.ChartAreas[0].AxisX.IsStartedFromZero = true;
                    chart2.ChartAreas[0].AxisY.IsStartedFromZero = true;
                    chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                    chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                    chart2.ChartAreas[0].CursorX.AutoScroll = true;
                    chart2.ChartAreas[0].CursorY.AutoScroll = true;
                    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                    /****************************************************************/

                    if (_totalsample == 3)
                    {
                        /************************************************************************************************/
                        chart3.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                        chart3.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
                        chart3.ChartAreas[0].AxisX.Title = "Time (min) --->";
                        chart3.ChartAreas[0].AxisY.Title = "% Dissolution  --->";
                        chart3.ChartAreas[0].AxisX.IsMarginVisible = false;
                        chart3.ChartAreas[0].AxisX.IsStartedFromZero = true;
                        chart3.ChartAreas[0].AxisY.IsStartedFromZero = true;
                        chart3.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                        chart3.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                        chart3.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                        chart3.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                        chart3.ChartAreas[0].CursorX.AutoScroll = true;
                        chart3.ChartAreas[0].CursorY.AutoScroll = true;
                        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                        /****************************************************************/
                    }
                }
                else
                {
                    _countsample = int.Parse(Nsamples_txt.Text);
                }
                for (int i = 0; i < _countsample; i++)
                {
                    int k = i % int.Parse(Nsamples_txt.Text) + 1;
                    if (chart1.Series.IsUniqueName("Vessel " + k))
                    {
                        chart1.Series.Add("Vessel " + k);
                        chart1.Series["Vessel " + k].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                        chart1.Series["Vessel " + k].MarkerStyle = MarkerStyle.Square;
                        chart1.Series["Vessel " + k].MarkerSize = 5;
                        chart1.Series["Vessel " + k]["LineTension"] = "0.1";
                        chart1.Series["Vessel " + k].ToolTip = "Y = #VALY\nX = #VALX";
                    }
                    
                    if (_totalsample >= 2)
                    {
                        if (chart2.Series.IsUniqueName("Vessel " + k))
                        {
                            chart2.Series.Add("Vessel " + k);
                            chart2.Series["Vessel " + k].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                            chart2.Series["Vessel " + k].MarkerStyle = MarkerStyle.Square;
                            chart2.Series["Vessel " + k].MarkerSize = 5;
                            chart2.Series["Vessel " + k]["LineTension"] = "0.1";
                            chart2.Series["Vessel " + k].ToolTip = "Y = #VALY\nX = #VALX";
                        }

                        if (_totalsample == 3)
                        {
                            if (chart3.Series.IsUniqueName("Vessel " + k))
                            {
                                chart3.Series.Add("Vessel " + k);
                                chart3.Series["Vessel " + k].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                                chart3.Series["Vessel " + k].MarkerStyle = MarkerStyle.Square;
                                chart3.Series["Vessel " + k].MarkerSize = 5;
                                chart3.Series["Vessel " + k]["LineTension"] = "0.1";
                                chart3.Series["Vessel " + k].ToolTip = "Y = #VALY\nX = #VALX";
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Draw Dissolution curve
        /// </summary>
        private void _chartdraw()
        {
            int i = 0;
            int j = 1;
            try
            {
                if (_totalsample > 1)
                {
                    for (i = 0; i < dataGridView1.Rows.Count - int.Parse(Nsamples_txt.Text); i = i + int.Parse(Nsamples_txt.Text))
                    {
                        if (i < _totalsamples || _totalsample < 1)
                        {
                            if (i == 0)
                            {
                                //while (j % int.Parse(Nsamples_txt.Text) != 0)
                                for (j = 1; j <= int.Parse(Nsamples_txt.Text); j++)
                                {
                                    chart1.Series["Vessel " + j].Points.AddXY("0", 0);
                                }

                                if (_totalsample >= 2)
                                {
                                    //while (j % int.Parse(Nsamples_txt.Text) != 0)
                                    for (j = 1; j <= int.Parse(Nsamples_txt.Text); j++)
                                    {
                                        chart2.Series["Vessel " + j].Points.AddXY("0", 0);
                                    }

                                    if (_totalsample == 3)
                                    {
                                        //while (j % int.Parse(Nsamples_txt.Text) != 0)
                                        for (j = 1; j <= int.Parse(Nsamples_txt.Text); j++)
                                        {
                                            chart3.Series["Vessel " + j].Points.AddXY("0", 0);
                                        }
                                    }
                                }
                                
                            }

                            j = 1;

                            while (j % int.Parse(Nsamples_txt.Text) != 0)
                            {  
                                chart1.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[i + (j - 1)].Cells[1].Value.ToString(), 
                                    dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                                j++;
                            }
                            chart1.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[j - 1].Cells[1].Value.ToString(), 
                                dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                            //i = i + int.Parse(Nsamples_txt.Text);
                            j = 1;
                        }
                        else
                        {

                            j = 1;

                            while (j % int.Parse(Nsamples_txt.Text) != 0)
                            {
                                chart2.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[i + (j - 1)].Cells[1].Value.ToString(), 
                                    dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                                j++;
                            }
                            chart2.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[j - 1].Cells[1].Value.ToString(), 
                                dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                            //i = i + int.Parse(Nsamples_txt.Text);
                            j = 1;

                            if (_totalsample == 3)
                            {
                                j = 1;

                                while (j % int.Parse(Nsamples_txt.Text) != 0)
                                {
                                    chart3.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[i + (j - 1)].Cells[1].Value.ToString(), 
                                        dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                                    j++;
                                }
                                chart3.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[j - 1].Cells[1].Value.ToString(), 
                                    dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                                //i = i + int.Parse(Nsamples_txt.Text);
                                j = 1;
                            }
                        }
                    }
                }
                else
                {
                    for (i = 0; i < dataGridView1.Rows.Count; i = i + int.Parse(Nsamples_txt.Text))
                    {
                        if (i == 0)
                        {
                            //while (j % int.Parse(Nsamples_txt.Text) != 0)
                            for (j = 1; j <= int.Parse(Nsamples_txt.Text); j++)
                            {
                                chart1.Series["Vessel " + j].Points.AddXY("0", 0);
                            }

                            j = 1;
                        }

                        while (j % int.Parse(Nsamples_txt.Text) != 0)
                        //while (j <= int.Parse(Nsamples_txt.Text))
                        {
                            chart1.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[i + (j - 1)].Cells[1].Value.ToString(), 
                                dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                                                        
                            j++;
                        }
                        chart1.Series["Vessel " + j].Points.AddXY(dataGridView1.Rows[j - 1].Cells[1].Value.ToString(), 
                            dataGridView1.Rows[i + (j - 1)].Cells[3].Value.ToString());
                        //i = i + int.Parse(Nsamples_txt.Text);
                        j = 1;
                    }
                }
                Sbutton.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Generate Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sbutton_Click(object sender, EventArgs e)
        {
            try
            {
                extbutton.Enabled = false;
                factorcal.Enabled = false;
                factortwo.Enabled = false;
                caldisso.Enabled = false;
                string path = @"c:\ADS\Report";
                System.IO.Directory.CreateDirectory(path);
                string [] _nameoffile = _filepath.Split('\\');
                string _methodname = _nameoffile.ElementAt(_nameoffile.Count() - 1).Split('.')[0];
                string filename = _methodname + "  " + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".pdf";
                path = System.IO.Path.Combine(path, filename);
                Document doc = new Document(iTextSharp.text.PageSize.A4, 10, 10, 42, 35);
                PdfWriter write = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 25f, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontpara = new iTextSharp.text.Font(bf, 7.5f, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontheader = new iTextSharp.text.Font(bf, 15f, iTextSharp.text.Font.NORMAL);
                doc.Open();
                /*******************************************************************/
                Paragraph spaceheader = new Paragraph(new Chunk("                                                         "));
                spaceheader.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                spaceheader.SpacingAfter = 4f;
                /******************************************************************/
                Paragraph space = new Paragraph(new Chunk("---------------------------------------------------------------------"));
                space.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                space.SpacingAfter = 4f;
                Paragraph p1 = new Paragraph(new Chunk("ADS Test Report", font));
                p1.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                doc.Add(p1);
                doc.Add(space);
                /***********************************************/
                Paragraph para4 = new Paragraph(new Chunk("Method Name : " + _methodname, fontpara));
                para4.SpacingAfter = 6f;
                doc.Add(para4);
                /***********************************************/
                Paragraph para12 = new Paragraph(new Chunk("Standard Samples: " + stdsample_txt.Text, fontpara));
                para12.TabSettings = new TabSettings(300);
                para12.Add(Chunk.TABBING);
                para12.Add("Number of Samples : " + Nsamples_txt.Text);
                para12.SpacingAfter = 6f;
                doc.Add(para12);
                /*************************************************/
                Paragraph para_stdconcone;                
                if (_totalsample == 1)
                {
                    para_stdconcone = new Paragraph(new Chunk("Standard Concentration: " + Stdconctxt.Text + " mg/mL", fontpara));
                    para_stdconcone.TabSettings = new TabSettings(300);
                    para_stdconcone.Add(Chunk.TABBING);
                    para_stdconcone.Add("Standard Peak Average : " + stdavgtxt.Text);
                }
                else
                {
                    para_stdconcone = new Paragraph(new Chunk("Standard Concentration: " + Stdconctxt.Text +" , "+ Stdconctxttwo.Text + " mg/mL", fontpara));
                    para_stdconcone.TabSettings = new TabSettings(300);
                    para_stdconcone.Add(Chunk.TABBING);
                    para_stdconcone.Add("Standard Peak Average : " + stdavgtxt.Text + " ," + stdavgtxttwo.Text);
                }
              
                para_stdconcone.SpacingAfter = 6f;
                doc.Add(para_stdconcone);
                /***********************************************/
                Paragraph para_Factor;
                if (_totalsample == 1)
                {
                    para_Factor = new Paragraph(new Chunk("Factor : " + factorapi.Text , fontpara));
                    para_Factor.TabSettings = new TabSettings(300);
                    para_Factor.Add(Chunk.TABBING);
                    para_Factor.Add("Dissolution Volume : " + dissolvoltwo.Text + " mL");

                }
                else
                {
                    para_Factor = new Paragraph(new Chunk("Factor : " + factorapi.Text + " , " + factorapitwo.Text , fontpara));
                    para_Factor.TabSettings = new TabSettings(300);
                    para_Factor.Add(Chunk.TABBING);
                    para_Factor.Add("Dissolution Volume : " + dissolvoltwo.Text + " , " + dissovolone.Text + " mL");
                }
                para_Factor.SpacingAfter = 6f;
                doc.Add(para_Factor);
                /*************************************************/
                Paragraph para13;
                if (_totalsample > 1)
                {
                    para13 = new Paragraph(new Chunk("Label API : " + labelamtapI2.Text +" , " + Labelamount.Text, fontpara));
                }
                else
                {
                     para13 = new Paragraph(new Chunk("Label API : " + labelamtapI2.Text, fontpara));
                }
                 para13.TabSettings = new TabSettings(300);
                 para13.Add(Chunk.TABBING);
                 para13.Add("Operator : " + _usernameTxt);
                 para13.SpacingAfter = 6f;
                 doc.Add(para13);
                 doc.Add(space);
                /***********************************************/                
                if (_totalsample> 1)
                {
                    PdfPTable resimtable = new PdfPTable(2); // two colmns create tabble
                    resimtable.WidthPercentage = 100f;//table %100 width
                    var chartimage = new MemoryStream();
                    chart1.SaveImage(chartimage, ChartImageFormat.Png);
                    iTextSharp.text.Image imgsag = iTextSharp.text.Image.GetInstance(chartimage.GetBuffer());
                    imgsag.ScalePercent(30f);
                    resimtable.AddCell(imgsag);//Table One colmns added first image
                    var chartimage1 = new MemoryStream();
                    chart2.SaveImage(chartimage1, ChartImageFormat.Png);
                    iTextSharp.text.Image imgsol = iTextSharp.text.Image.GetInstance(chartimage1.GetBuffer());
                    imgsol.ScalePercent(30f);
                    resimtable.AddCell(imgsol);//Table two columns added second image
                    doc.Add(resimtable);
                    doc.Add(space);
                }
                else
                {
                    var chartimage = new MemoryStream();
                    chart1.SaveImage(chartimage, ChartImageFormat.Png);
                    iTextSharp.text.Image chart_image = iTextSharp.text.Image.GetInstance(chartimage.GetBuffer());
                    chart_image.SpacingAfter = 10f;
                    chart_image.ScalePercent(50f);
                    doc.Add(chart_image);
                    doc.Add(space);
                }

                /**************************************************/

                Paragraph header = new Paragraph(new Chunk("Peak Area - API 1", fontheader));
                header.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                header.SpacingAfter = 8f;
                doc.Add(header);
        
                /**************************************************/
                /*Datagridview table*/
                PdfPTable table = new PdfPTable(int.Parse(Nsamples_txt.Text) + 1);
                List<string> vessel = new List<string>();
                vessel.Add("  ");
                int j = 1;
                for (int i =1;i<= int.Parse(Nsamples_txt.Text);i++)
                {
                    vessel.Add(" Vessel " + i.ToString());
                }


                for (int i = 0; i < vessel.Count(); i++)
                {
                    table.AddCell(new Phrase(vessel[i], fontpara));
                }
                for (int k = 1; k <= TimeData.Count(); k++)
                {
                    table.AddCell(new Phrase(TimeData[k - 1].ToString() + " min", fontpara));
                    while (j % int.Parse(Nsamples_txt.Text) != 0)
                    {
                        table.AddCell(new Phrase(dataGridView1.Rows[j - 1].Cells[2].Value.ToString(), fontpara));
                        j++;
                    }
                    table.AddCell(new Phrase(dataGridView1.Rows[j - 1].Cells[2].Value.ToString(), fontpara));
                    j++;
                }
                doc.Add(table);
                doc.Add(space);
                /*********************************************/
                Paragraph dissheader = new Paragraph(new Chunk("% Dissolution - API 1", fontheader));
                dissheader.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                dissheader.SpacingAfter = 8f;
                doc.Add(dissheader);
                /*Datagridview table*/
                PdfPTable _dissotable = new PdfPTable(int.Parse(Nsamples_txt.Text) + 1);
                List<string> _dissovessel = new List<string>();
                _dissovessel.Add("  ");
                j = 1;
                for (int i = 1; i <= int.Parse(Nsamples_txt.Text); i++)
                {
                    _dissovessel.Add(" Vessel " + i.ToString());
                }
                for (int i = 0; i < vessel.Count(); i++)
                {
                    _dissotable.AddCell(new Phrase(_dissovessel[i], fontpara));
                }
                for (int k = 1; k <= TimeData.Count(); k++)
                {
                    _dissotable.AddCell(new Phrase(TimeData[k - 1].ToString() + " min", fontpara));

                    while (j % int.Parse(Nsamples_txt.Text) != 0)
                    {
                        _dissotable.AddCell(new Phrase(dataGridView1.Rows[j - 1].Cells[3].Value.ToString(), fontpara));
                        j++;
                    }
                    _dissotable.AddCell(new Phrase(dataGridView1.Rows[j - 1].Cells[3].Value.ToString(), fontpara));
                    j++;
                }

                doc.Add(_dissotable);
                doc.Add(space);
                if(_totalsample > 1)
                {

                    Paragraph headertwo = new Paragraph(new Chunk("Peak Area - API 2", fontheader));
                    headertwo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    headertwo.SpacingAfter = 8f;
                    doc.Add(headertwo);
                    int m = j;
                    /*******************************************************************/
                    PdfPTable tabletwo = new PdfPTable(int.Parse(Nsamples_txt.Text) + 1);
                    for (int i = 0; i < vessel.Count(); i++)
                    {
                        tabletwo.AddCell(new Phrase(vessel[i], fontpara));
                    }
                    for (int k = 1; k <= TimeData.Count(); k++)
                    {
                        tabletwo.AddCell(new Phrase(TimeData[k - 1].ToString() + " min", fontpara));
                        while (j % int.Parse(Nsamples_txt.Text) != 0)
                        {
                            tabletwo.AddCell(new Phrase(dataGridView1.Rows[j - 1].Cells[2].Value.ToString(), fontpara));
                            j++;
                        }
                        tabletwo.AddCell(new Phrase(dataGridView1.Rows[j - 1].Cells[2].Value.ToString(), fontpara));
                        j++;
                    }

                    doc.Add(tabletwo);
                    doc.Add(space);
                    /***************************************************************************/
                    Paragraph dissheadertwo = new Paragraph(new Chunk("% Dissolution - API 2", fontheader));
                    dissheadertwo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    dissheadertwo.SpacingAfter = 8f;
                    doc.Add(dissheadertwo);
                    /*Datagridview table*/
                    PdfPTable _dissotabletwo = new PdfPTable(int.Parse(Nsamples_txt.Text) + 1);
                    for (int i = 0; i < vessel.Count(); i++)
                    {
                        _dissotabletwo.AddCell(new Phrase(_dissovessel[i], fontpara));
                    }
                    for (int k = 1; k <= TimeData.Count(); k++)
                    {
                       
                        _dissotabletwo.AddCell(new Phrase(TimeData[k - 1].ToString() + " min", fontpara));

                        while (m % int.Parse(Nsamples_txt.Text) != 0)
                        {
                            _dissotabletwo.AddCell(new Phrase(dataGridView1.Rows[m - 1].Cells[3].Value.ToString(), fontpara));
                            m++;
                        }
                        _dissotabletwo.AddCell(new Phrase(dataGridView1.Rows[m - 1].Cells[3].Value.ToString(), fontpara));
                        m++;
                    }

                    doc.Add(_dissotabletwo);
                    doc.Add(space);
                }
                /*****************************************************************************/
                DateTime Ptime = DateTime.Now;
                /*****************************************************************************/
                Paragraph para16 = new Paragraph(new Chunk("Report Location: " + path, fontpara));
                para16.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
                para16.SpacingAfter = 30f;
                doc.Add(para16);
                /****************************************************************************/
                Paragraph para15 = new Paragraph(new Chunk("Report Generated on: " + Ptime.ToString(), fontpara));
                para15.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
                para15.SpacingAfter = 30f;
                doc.Add(para15);
                doc.Close();
                MessageBox.Show("File Saved");
                _createdatabase.Addtoaudit(_usernameTxt, "Generate Dissolution Page", "Report PDF generated");
                AddPageNumber(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". Unable to Save File");
            }
        }


        private void AddPageNumber(string filename)
        {
            string path = @"c:\ADS\Report";
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), font), 568f, 8f, 0);
                        var pdfContentByte = stamper.GetOverContent(i);
                        pdfContentByte.AddImage(image);
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(path, bytes);
        }
        /// <summary>
        /// Support functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Stpagenum_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Stdsample_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Nsamples_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Stdconctxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Stdconctxttwo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Stdavgtxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Stdavgtxttwo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Factorapi_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Factorapitwo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Labelamount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Dissovolume_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Dissolvolone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Blanksamp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LabelamtapI2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Clean all calculate areas
        /// </summary>
        private void _clean_calculate()
        {
            //clear all other domains
            dataGridView1.Rows.Clear();

            foreach (Control ctrl in groupBox3.Controls)
            {
                if (ctrl is TextBox)
                {
                    (ctrl as TextBox).Text = "";
                }
            }

            groupBox3.Enabled = false;
        }

        /// <summary>
        /// Clear a chart
        /// </summary>
        private void _chart_clear(Chart chart)
        {
            foreach (var series in chart.Series)
            {
                series.Points.Clear();
            }
        }

        private void replace_Click(object sender, EventArgs e)
        {
            ReplaceRow replaceForm = new ReplaceRow();
            replaceForm.dataReady += ReplaceRow;

        }

        private void ReplaceRow(RowDataEventArgs e)
        {
            int oldRowNumber = e.oldRowNumber;
            int newRowNumber = e.newRowNumber;

            var oldRowTimePoint = dataGridView1.Rows[oldRowNumber].Cells[1].Value;
            var oldRow = dataGridView1.Rows[oldRowNumber];
            var newRow = dataGridView1.Rows[newRowNumber];
            dataGridView1.Rows.Remove(oldRow);
            dataGridView1.Rows.Insert(oldRowNumber, newRow);

            dataGridView1.Rows[oldRowNumber].Cells[1].Value = oldRowTimePoint;

        }
    }
}
