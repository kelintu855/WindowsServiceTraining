using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace WindowsService1
{
    class Database
    {
        public SQLiteConnection myConnection;

        public Database()
        {
            myConnection = new SQLiteConnection("Data Source=database.sqlite3");
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
                System.Console.WriteLine("Database file created");
            }
            CreateDBTable();
        }

        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Clone();
            }
        }

        public void CreateDBTable()
        {
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS [Department] (
                                        [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                        [Name] NVARCHAR(2048) NULL,
                                        [DepartmentId] VARCHAR(2048) NULL
                                        )";

            //Configure where db file is created
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            System.Data.SQLite.SQLiteConnection.CreateFile(path + "/test1.db3");
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source=|DataDirectory|/test1.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(conn))
                {
                    conn.Open();                             
                    com.CommandText = createTableQuery;     
                    com.ExecuteNonQuery();                  
                    com.CommandText = "INSERT INTO Department (Name,DepartmentId) Values ('Tom','8')";     
                    com.ExecuteNonQuery();      
                    com.CommandText = "INSERT INTO Department (Name,DepartmentId) Values ('Jerry','24')";  
                    com.ExecuteNonQuery();     
                    com.CommandText = "Select * FROM Department";      

                    using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["Name"] + " : " + reader["DepartmentId"]);
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}
