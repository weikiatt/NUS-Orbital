using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using NUS_Orbital.Models;

namespace NUS_Orbital.DAL
{
    public class StudentDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        //Constructor
        public StudentDAL()
        {
            //Locate the appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            //Read ConnectionString from appsettings.json file
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("NUS_Orbital");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }


        public bool DoesEmailExist(string email)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT id FROM Student WHERE Email=@selectedEmail", conn);
            cmd.Parameters.AddWithValue("@selectedEmail", email);
            SqlDataAdapter daEmail = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            //Use DataAdapter to fetch data to a table "EmailDetails" in DataSet.
            daEmail.Fill(result, "Email");
            conn.Close();
            if (result.Tables["Email"].Rows.Count > 0)
                return true; //The email exists 
            else
                return false; // The email given does not exist
        }

    }
}
