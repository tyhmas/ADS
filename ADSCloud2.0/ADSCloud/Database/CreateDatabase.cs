using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace ADSCloud.Database
{
    public class CreateDatabase
    {
        SQLiteConnection conn;
        SQLiteCommand cmd;
        SQLiteDataReader reader;
        List<string> retList = new List<string>();
        List<string> retListuser = new List<string>();
        List<string> retListcheck = new List<string>();
        Dictionary<string, List<string>> _logindict = new Dictionary<string, List<string>>();
        /// <summary>
        /// Creates the main DB files
        /// </summary>
        public void Createdb()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            try
            {
               
                if (!File.Exists("ADS.db"))
                {
                    SQLiteConnection.CreateFile("ADS.db");
                    using (conn = new SQLiteConnection("Data Source=ADS.db;Version=3;New=True;Compress=True;"))
                    {
                        conn.Open();
                        string commandstring = "CREATE TABLE IF NOT EXISTS Login (Username varchar(20), Password VARCHAR TEXT(10), Type TEXT(10), Date TEXT(10), Setvalue bool(10))";
                        using (cmd = new SQLiteCommand(commandstring, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        commandstring = "INSERT INTO Login ([Username],[Password],[Type],[Date],[Setvalue]) VALUES (@Username,@Password,@Type,@Date,@Setvalue)";
                        using (cmd = new SQLiteCommand(commandstring, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", "Adm");
                            cmd.Parameters.AddWithValue("@Password", "123");
                            cmd.Parameters.AddWithValue("@Type", "Admin");
                            cmd.Parameters.AddWithValue("@Date", "--/--/----");
                            cmd.Parameters.AddWithValue("@Setvalue", true);
                            cmd.ExecuteNonQuery();
                        }
                    }

                }
                if (!File.Exists("Audit.db"))
                {
                    SQLiteConnection.CreateFile("Audit.db");
                    using (conn = new SQLiteConnection("Data Source=Audit.db;Version=3;New=True;Compress=True;"))
                    {
                        conn.Open();
                        string commandstring = "CREATE TABLE IF NOT EXISTS Auditdata (Username VARCHAR(20), Page VARCHAR TEXT(10), Description TEXT(100), Date TEXT(10))";
                        using (cmd = new SQLiteCommand(commandstring, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the Login information needed to Login to application
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> Retrievedata()
        {
            _logindict.Clear();

            try
            {
                using (conn = new SQLiteConnection("Data Source=ADS.db;Version=3;New=True;Compress=True;"))
                {
                    conn.Open();
                    
                    string commandstring = "SELECT * FROM Login";
                    //Dictionary<string, string> _UserType = new Dictionary<string, string>();
                    using (cmd = new SQLiteCommand(commandstring, conn))
                    {
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                List<string> _valuelist = new List<string>();
                                _valuelist.Add(Convert.ToString(reader["Password"]));
                                _valuelist.Add(Convert.ToString(reader["Type"]));
                                _valuelist.Add(Convert.ToString(reader["Date"]));
                                _valuelist.Add(Convert.ToString(reader["Setvalue"]));
                                _logindict.Add(Convert.ToString(reader["Username"]), _valuelist);
                                
                            }
                            return _logindict;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return _logindict;
            }
        }
     
        public bool pushuserdata(string username, string passowrd, string accesstype)
        {
            try
            {
                using (conn = new SQLiteConnection("Data Source=ADS.db;Version=3;New=True;Compress=True;"))
                {
                    conn.Open();
                    string commandstring = "INSERT INTO Login ([Username],[Password],[Type],[Date],[Setvalue]) VALUES (@Username,@Password,@Type,@Date,@Setvalue)";
                    using (cmd = new SQLiteCommand(commandstring, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", passowrd);
                        cmd.Parameters.AddWithValue("@Type", accesstype);
                        DateTime aDate = DateTime.Now;
                        string date = aDate.ToString("dd MMMM yyyy HH:mm:ss");
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.Parameters.AddWithValue("@Setvalue", true);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User Added Successfully!");
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Unable To Add User");
            }
            return false;
        }
      
        
        public void Addtoaudit(string usernametext, string Page, string content)
        {
            DateTime aDate = DateTime.Now;
            if (File.Exists("Audit.db"))
            {
                using (conn = new SQLiteConnection("Data Source=Audit.db;Version=3;New=True;Compress=True;"))
                {
                    conn.Open();
                    string commandstring = "INSERT INTO Auditdata ([Username],[Page],[Description],[Date]) VALUES (@Username ,@Page," +
                        "@Description ,@Date)";
                    using (cmd = new SQLiteCommand(commandstring, conn))
                    {
                        if (usernametext == "Adm")
                        {
                            cmd.Parameters.AddWithValue("@Username", "Admin");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Username", usernametext);
                        }
                        cmd.Parameters.AddWithValue("@Page", Page);
                        cmd.Parameters.AddWithValue("@Description", content);
                        cmd.Parameters.AddWithValue("@Date", aDate.ToString("dd MMMM yyyy HH:mm:ss"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                MessageBox.Show("No audit file created");
            }
        }
    }
}
