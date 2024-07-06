using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using NUS_Orbital.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Xml.Linq;


namespace NUS_Orbital.DAL
{
    public class AdminDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public AdminDAL()
        {
            string connstring = "Server=tcp:nus-orbital-tft.database.windows.net,1433;Initial Catalog=NUS_Orbital;Persist Security Info=False;User ID=nus-orbital-tft-admin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            this.conn = new SqlConnection();
            this.conn.ConnectionString = connstring;
        }
        public bool DoesLoginCredentialExist(string name, string password)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM ADMINS WHERE [Name] = @name AND Password = @password", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "LoginInfo");
            conn.Close();
            if (result.Tables["LoginInfo"].Rows.Count > 0)
                return true; //login info correct
            return false;
        }
    }
}
