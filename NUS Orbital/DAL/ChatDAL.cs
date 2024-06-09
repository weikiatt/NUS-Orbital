using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using NUS_Orbital.Models;
using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NUS_Orbital.DAL
{
    public class ChatDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public ChatDAL()
        {
            string connstring = "Server=tcp:nus-orbital-tft.database.windows.net,1433;Initial Catalog=NUS_Orbital;Persist Security Info=False;User ID=nus-orbital-tft-admin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            this.conn = new SqlConnection();
            this.conn.ConnectionString = connstring;
        }

        public List<StudentChat> GetAllChatsForUser(Student student)
        {
            StudentDAL studentContext = new StudentDAL();
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM CHATS WHERE CurrStudentID = @studentId", conn);
            cmd.Parameters.AddWithValue("@studentId", student.studentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            List<StudentChat> studentList = new List<StudentChat>();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                Student otherStudent = studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["OtherStudentID"]));
                studentList.Add(new StudentChat(
                    otherStudent,
                    GetChatLogWithUser(student, otherStudent)
                ));
            }
            return studentList;
        }

        public List<ChatLog> GetChatLogWithUser(Student currStudent, Student otherStudent)
        {
            StudentDAL studentContext = new StudentDAL();
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM CHAT_LOG WHERE (SenderID = @currStudentId AND ReceiverID = @otherStudentID) OR " +
            "(SenderID = @otherStudentID AND ReceiverID = @currStudentId)", conn);
            cmd.Parameters.AddWithValue("@currStudentId", currStudent.studentId);
            cmd.Parameters.AddWithValue("@otherStudentID", otherStudent.studentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ChatLog");
            conn.Close();
            List<ChatLog> chatLog = new List<ChatLog>();
            foreach (DataRow row in result.Tables["ChatLog"].Rows)
            {
                chatLog.Add(new ChatLog(
                    studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["SenderID"])),
                    studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["ReceiverID"])),
                    row["Description"].ToString(),
                    Convert.ToDateTime(row["TimeSent"])
                ));
            }
            return chatLog;
        }

        public void SendMessage(int currStudId, int otherStudId, string message, DateTime currentTime)
        {
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO CHAT_LOG(SenderID, ReceiverID, [Description], TimeSent) " +
                "VALUES (@currStudId, @otherStudId, @message, @currentTime)", conn);

            cmd.Parameters.AddWithValue("@currStudId", currStudId);
            cmd.Parameters.AddWithValue("@otherStudId", otherStudId);
            cmd.Parameters.AddWithValue("@message", message);
            cmd.Parameters.AddWithValue("@currentTime", currentTime);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public void AddChat(int currStudId, int otherStudId)
        {
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO CHATS (CurrStudentID, OtherStudentID)" +
                "VALUES (@currStudId, @otherStudId)", conn);

            cmd.Parameters.AddWithValue("@currStudId", currStudId);
            cmd.Parameters.AddWithValue("@otherStudId", otherStudId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public bool DoesChatExist(int currStudId, int otherStudId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM CHATS WHERE CurrStudentID=@currStudId AND OtherStudentID=@otherStudId", conn);
            cmd.Parameters.AddWithValue("@currStudId", currStudId);
            cmd.Parameters.AddWithValue("@otherStudId", otherStudId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Chat");
            conn.Close();
            if (result.Tables["Chat"].Rows.Count > 0)
                return true; // Module code exists
            return false; // Module code does not exist
        }
    }
}
