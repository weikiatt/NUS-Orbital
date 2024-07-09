using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using NUS_Orbital.Models;
using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto.Paddings;


namespace NUS_Orbital.DAL
{
    public class HomeDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public HomeDAL()
        {
            string connstring = "Server=tcp:nus-orbital-tft.database.windows.net,1433;Initial Catalog=NUS_Orbital;Persist Security Info=False;User ID=nus-orbital-tft-admin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            this.conn = new SqlConnection();
            this.conn.ConnectionString = connstring;
        }

        public void AddVerification(int studentId, String verificationCode, DateTime currTime)
        {
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO STUDENT_VERIFICATION (StudentID, VerificationCode, DateSent) VALUES(@studentId, @verificationCode, @dateSent)", conn);

            cmd.Parameters.AddWithValue("@studentId", studentId);
            cmd.Parameters.AddWithValue("@verificationCode", verificationCode);
            cmd.Parameters.AddWithValue("@dateSent", currTime);

            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public void Expire()
        {
            SqlCommand cmd = new SqlCommand
                ("UPDATE STUDENT_VERIFICATION SET Expired = 1 " +
                "WHERE DATEDIFF(MINUTE, FORMAT(DateSent, 'yyyy-MM-dd HH:mm:ss.fffffff'), FORMAT(GetDate(), 'yyyy-MM-dd HH:mm:ss.fffffff')) > -475", conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public bool VerifyCode(int studentId, string verificationCode)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM STUDENT_VERIFICATION WHERE StudentID = @studentId AND VerificationCode = @verificationCode AND Expired = 'False'", conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            cmd.Parameters.AddWithValue("@verificationCode", verificationCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Verification");
            conn.Close();
            if (result.Tables["Verification"].Rows.Count > 0)
                return true;
            return false;
        }

        public void VerifyStudent(int studentId)
        {
            SqlCommand cmd = new SqlCommand
                ("UPDATE STUDENTS SET Verified = 'True' WHERE StudentID = @studentId", conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void ExpireCode(int studentId)
        {
            SqlCommand cmd = new SqlCommand
                ("UPDATE STUDENT_VERIFICATION SET Expired = 1 WHERE StudentID = @studentId", conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public bool IsStudentVerified(string email)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM STUDENTS WHERE Email = @email AND Verified = 1", conn);
            cmd.Parameters.AddWithValue("@email", email);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Verification");
            conn.Close();
            if (result.Tables["Verification"].Rows.Count > 0)
                return true;
            return false;
        }
    }
}
