using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SQLite;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        

        public Service1()
        {
            InitializeComponent();
        }


        protected override async void OnStart(string[] args)
        {
            //TODO: create a function elsewhere to do all these 
            new WindowsService1.Database();//create new department table in new database
            string selectQuery = @"SELECT * FROM Department";

            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source=|DataDirectory|/test1.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(conn))
                {
                    conn.Open();
                    com.CommandText = selectQuery;
                    com.ExecuteNonQuery();

                    using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                    {
                        //string postJSON;
                        Department department = new Department();
                        while (reader.Read())
                        {
                            department.Name = reader["Name"].ToString();
                            department.DepartmentId = reader["DepartmentId"].ToString();

                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri("http://webapi.local/api/");
                                var postTask = client.PostAsJsonAsync<string>("values", @"{" + "\"name\":\"" + department.Name + "\"," + "\"department_id\":\"" + department.DepartmentId + "\"}");
                                postTask.Wait();

                                /*var content = new StringContent(@"{ Name: "", DepartmentID: ""}", Encoding.UTF8, "application/json");
                                var httpClient = new HttpClient();
                                HttpResponseMessage response = httpClient.PostAsync(URL, content).Result;
                                var responseString = await response.Content.ReadAsStringAsync();*/

                            }
                        }
                    }
                    conn.Close();
                }
            }
        }







        protected override void OnStop()
        {
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
