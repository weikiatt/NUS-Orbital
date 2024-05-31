using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using NUS_Orbital.Models;
using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;

namespace NUS_Orbital.DAL
{
    public class ModuleDAL
    {
        private IConfiguration Configuration { get; set; }
        private MySqlConnection conn;

        public ModuleDAL()
        {
            string connstring = "server=localhost;uid=root;pwd=password;database=NUS_Orbital";
            this.conn = new MySqlConnection();
            this.conn.ConnectionString = connstring;
        }

        public List<Module> GetAllModules() 
        {
            MySqlCommand cmd = new MySqlCommand(
            "SELECT * FROM MODULES ORDER BY ModuleCode", conn);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ModuleDetails");
            conn.Close();
            List<Module> moduleList = new List<Module>();
            foreach (DataRow row in result.Tables["ModuleDetails"].Rows)
            {
                moduleList.Add(new Module
                (
                    row["ModuleCode"].ToString(),
                    row["ModuleName"].ToString(),
                    row["Description"].ToString()
                ));
            }
            return moduleList;
        }

        public bool DoesModuleExist(string moduleCode)
        {
            MySqlCommand cmd = new MySqlCommand
            ("SELECT ModuleCode FROM MODULES WHERE ModuleCode=@selectedModule", conn);
            cmd.Parameters.AddWithValue("@selectedModule", moduleCode);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ModuleCode");
            conn.Close();
            if (result.Tables["ModuleCode"].Rows.Count > 0)
                return true; // Module code exists
            return false; // Module code does not exist
        }

        public Module GetModuleDetails(string moduleCode)
        {
            MySqlCommand cmd = new MySqlCommand
            ("SELECT * FROM MODULES WHERE ModuleCode=@selectedModule", conn);
            cmd.Parameters.AddWithValue("@selectedModule", moduleCode);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ModuleCode");
            conn.Close();
            if (result.Tables["ModuleCode"].Rows.Count > 0)
            {
                DataTable table = result.Tables["ModuleCode"];
                return new Module(moduleCode, table.Rows[0]["ModuleName"].ToString(), table.Rows[0]["Description"].ToString());
            }
            return new Module(moduleCode, "none", "none");

        }

        public List<Post> GetAllPosts(Module module, Student student)
        {
            StudentDAL studentContext = new StudentDAL();

            MySqlCommand cmd = new MySqlCommand(
            "SELECT * FROM POSTS WHERE ModuleCode=@selectedModule ORDER BY PostTime DESC", conn);
            cmd.Parameters.AddWithValue("@selectedModule", module.moduleCode);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Posts");
            conn.Close();
            List<Post> postList = new List<Post>();
            foreach (DataRow row in result.Tables["Posts"].Rows)
            {
                postList.Add(new Post
                (
                    module,
                    Convert.ToInt32(row["PostID"]),
                    Convert.ToDateTime(row["PostTime"]),
                    row["Description"].ToString(),
                    GetNumberOfPostUpvotes(Convert.ToInt32(row["PostID"])),
                    studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["StudentID"])),
                    GetAllComments(Convert.ToInt32(row["PostID"]), student),
                    DoesPostUpvoteExist(Convert.ToInt32(row["PostID"]), student.studentId),
                    GetAllTagsForPost(Convert.ToInt32(row["PostID"]))
                ));
            }
            return postList;
        }

        public int GetNumberOfPostUpvotes(int postId)
        {
            MySqlCommand cmd = new MySqlCommand(
            "SELECT * FROM POST_UPVOTES WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Posts");
            conn.Close();
            return result.Tables["Posts"].Rows.Count;
        }


        public void AddPost(String moduleCode, String description, int studentId)
        {
            StudentDAL studentContext = new StudentDAL();
            MySqlCommand cmd = new MySqlCommand
                ("INSERT INTO POSTS(ModuleCode, PostTime, `Description`, StudentID)" +
                "VALUES (@moduleCode, @postTime, @description, @studentID)", conn);

            cmd.Parameters.AddWithValue("@moduleCode", moduleCode);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@studentID", studentId);
            
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public int GetNumberOfCommentUpvotes(int commentId)
        {
            MySqlCommand cmd = new MySqlCommand(
            "SELECT * FROM COMMENT_UPVOTES WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Comments");
            conn.Close();
            return result.Tables["Comments"].Rows.Count;
        }

        public List<Comment> GetAllComments(int postId, Student student)
        {
            StudentDAL studentContext = new StudentDAL();

            MySqlCommand cmd = new MySqlCommand(
            "SELECT * FROM COMMENTS WHERE PostID=@postId ORDER BY CommentTime", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Comments");
            conn.Close();
            List<Comment> comments = new List<Comment>();
            foreach (DataRow row in result.Tables["Comments"].Rows)
            {
                comments.Add(new Comment
                (

                    Convert.ToInt32(row["CommentID"]),
                    Convert.ToDateTime(row["CommentTime"]),
                    row["Description"].ToString(),
                    GetNumberOfCommentUpvotes(Convert.ToInt32(row["CommentID"])),
                    Convert.ToInt32(row["PostID"]),
                    studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["StudentID"])),
                    DoesCommentUpvoteExist(Convert.ToInt32(row["CommentID"]), student.studentId)
                ));
            }
            return comments;
        }

        public void AddCommentToPost(String description, int postId, int studentId)
        {
            StudentDAL studentContext = new StudentDAL();
            MySqlCommand cmd = new MySqlCommand
                ("INSERT INTO COMMENTS(CommentTime, `Description`, PostID, StudentID)" +
                "VALUES (@commentTime, @description, @postId, @studentID)", conn);

            cmd.Parameters.AddWithValue("@commentTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentID", studentId);

            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public void AddUpvoteToPost(int postId, int studentId)
        {
            MySqlCommand cmd = new MySqlCommand
                ("INSERT INTO POST_UPVOTES (PostID, StudentID)" +
                "VALUES (@postId, @studentId)", conn);

            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public void RemoveUpvoteFromPost(int postId, int studentId)
        {
            MySqlCommand cmd = new MySqlCommand
                ("DELETE FROM POST_UPVOTES WHERE PostID=@postId AND StudentID=@studentId", conn);

            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public Boolean DoesPostUpvoteExist(int postId, int studentId)
        {
            MySqlCommand cmd = new MySqlCommand
                ("SELECT * FROM POST_UPVOTES WHERE PostID=@postId AND StudentID=@studentId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "UpvoteDetails");
            conn.Close();
            if (result.Tables["UpvoteDetails"].Rows.Count > 0)
                return true;
            return false;
        }

        public void AddUpvoteToComment(int commentId, int studentId)
        {
            MySqlCommand cmd = new MySqlCommand
                ("INSERT INTO COMMENT_UPVOTES (CommentID, StudentID)" +
                "VALUES (@commentId, @studentId)", conn);

            cmd.Parameters.AddWithValue("@commentId", commentId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public void RemoveUpvoteFromComment(int commentId, int studentId)
        {
            MySqlCommand cmd = new MySqlCommand
                ("DELETE FROM COMMENT_UPVOTES WHERE CommentID=@commentId AND StudentID=@studentId", conn);

            cmd.Parameters.AddWithValue("@commentId", commentId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public Boolean DoesCommentUpvoteExist(int commentId, int studentId)
        {
            MySqlCommand cmd = new MySqlCommand
                ("SELECT * FROM COMMENT_UPVOTES WHERE CommentID=@commentId AND StudentID=@studentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "UpvoteDetails");
            conn.Close();
            if (result.Tables["UpvoteDetails"].Rows.Count > 0)
                return true;
            return false;
        }

        public List<Tag> GetAllTagsForPost(int postId)
        {
            MySqlCommand cmd = new MySqlCommand(
            "SELECT PT.TagID, Tag FROM POST_TAGS PT INNER JOIN TAGS T ON PT.TagID = T.TagID WHERE PostID = @postId ", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Tags");
            conn.Close();
            List<Tag> tagList = new List<Tag>();
            foreach (DataRow row in result.Tables["Tags"].Rows)
            {
                tagList.Add(new Tag
                (
                    Convert.ToInt32(row["TagID"]),
                    row["Tag"].ToString()
                ));
            }
            return tagList;
        }
    }
}