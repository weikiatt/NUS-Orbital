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
    public class StudentDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public StudentDAL()
        {

            string connstring = "Server=tcp:nus-orbital-tft.database.windows.net,1433;Initial Catalog=NUS_Orbital;Persist Security Info=False;User ID=nus-orbital-tft-admin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            this.conn = new SqlConnection();
            this.conn.ConnectionString = connstring;

            /*
            //Locate the appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            //Read ConnectionString from appsettings.json file
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "DefaultConnection");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new MySqlConnection(strConn);*/
        }

        
        public bool DoesLoginCredentialExist(string email, string password)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM STUDENTS WHERE Email = @email AND Password = @password", conn);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();//error
            da.Fill(result, "LoginInfo");
            conn.Close();
            if (result.Tables["LoginInfo"].Rows.Count > 0)
                return true; //login info correct
            else
                return false;
        }

        public bool DoesEmailExist(string email)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM STUDENTS WHERE Email = @selectedEmail", conn);
            cmd.Parameters.AddWithValue("@selectedEmail", email);
            SqlDataAdapter daEmail = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            daEmail.Fill(result, "EmailDetails");
            conn.Close();
            if (result.Tables["EmailDetails"].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void Add(Student student)
        {
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO STUDENTS (Email, Name, Password, ProfilePicture) VALUES(@email, @name, @password, @profilePicture)", conn);

            cmd.Parameters.AddWithValue("@email", student.Email);
            cmd.Parameters.AddWithValue("@name", student.Name);
            cmd.Parameters.AddWithValue("@password", student.Password);
            cmd.Parameters.AddWithValue("@profilePicture", student.ProfilePicture);

            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
        public void Add(String email, String name, String password)
        {
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO STUDENTS (Email, Name, Password) VALUES(@email, @name, @password)", conn);

            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);

            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public string GetName(String email)
        {
            SqlCommand cmd = new SqlCommand
                ("SELECT * FROM STUDENTS WHERE Email=@email", conn);

            cmd.Parameters.AddWithValue("@email", email);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentInfo");
            conn.Close();
            return result.Tables["StudentInfo"].Rows[0]["Name"].ToString();
        }

        public Student GetStudentDetailsWithID(int studentId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM STUDENTS WHERE StudentID=@selectedStudent", conn);
            cmd.Parameters.AddWithValue("@selectedStudent", studentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Student");
            conn.Close();
            if (result.Tables["Student"].Rows.Count > 0)
            {
                DataTable table = result.Tables["Student"];
                return new Student(
                    studentId,
                    table.Rows[0]["Name"].ToString(),
                    table.Rows[0]["Email"].ToString(),
                    table.Rows[0]["Course"].ToString(),
                    table.Rows[0]["Description"].ToString(),
                    $"data:image/jpeg;base64,{Convert.ToBase64String(table.Rows[0]["ProfilePicture"] as byte[])}",
                    table.Rows[0]["ProfilePicture"] as byte[],
                    Convert.ToInt32(table.Rows[0]["Verified"]) == 0 ? false : true
                );
            }
            return null;
        }

        public Student GetStudentDetailsWithEmail(string email)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM STUDENTS WHERE Email=@selectedEmail", conn);
            cmd.Parameters.AddWithValue("@selectedEmail", email);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Student");
            conn.Close();
            if (result.Tables["Student"].Rows.Count > 0)
            {
                DataTable table = result.Tables["Student"];
                Student student = new Student();
                student.StudentId = Convert.ToInt32(table.Rows[0]["StudentID"]);

                return student;
                /* change here
                return new Student(
                    Convert.ToInt32(table.Rows[0]["StudentID"]),
                    table.Rows[0]["Name"].ToString(),
                    email,
                    table.Rows[0]["Course"].ToString(),
                    table.Rows[0]["Description"].ToString(),
                    $"data:image/jpeg;base64,{Convert.ToBase64String(table.Rows[0]["ProfilePicture"] as byte[])}",
                    table.Rows[0]["ProfilePicture"] as byte[],
                    Convert.ToInt32(table.Rows[0]["Verified"]) == 0 ? false : true
                );
                */
            }
            return new Student();
        }

        public int UpdatePhoto2(Student student, byte[] profilePicture)
        {
            SqlCommand cmd = new SqlCommand
                 ("UPDATE STUDENTS SET ProfilePicture=@profilePicture WHERE StudentID = @selectedStudentID", conn);

            cmd.Parameters.AddWithValue("@profilePicture", profilePicture);
            cmd.Parameters.AddWithValue("@selectedStudentID", student.StudentId);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int UpdatePhoto(Student student)
        {
            SqlCommand cmd = new SqlCommand
                 ("UPDATE STUDENTS SET Photo=@photo WHERE StudentID = @selectedStudentID", conn);

            cmd.Parameters.AddWithValue("@photo", student.Photo);
            cmd.Parameters.AddWithValue("@selectedStudentID", student.StudentId);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }


        public void UpdateStudent(Student updatedStudent)
        {
            SqlCommand cmd = new SqlCommand
                 ("UPDATE STUDENTS SET Name=@name, Course=@course, Description=@description" +
                 " WHERE StudentID = @selectedStudentID", conn);

            cmd.Parameters.AddWithValue("@name", updatedStudent.Name);
            cmd.Parameters.AddWithValue("@course", updatedStudent.Course);
            cmd.Parameters.AddWithValue("@description", updatedStudent.Description);
            cmd.Parameters.AddWithValue("@selectedStudentID", updatedStudent.StudentId);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
