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
    public class ModuleDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public ModuleDAL()
        {
            string connstring = "Server=tcp:nus-orbital-tft.database.windows.net,1433;Initial Catalog=NUS_Orbital;Persist Security Info=False;User ID=nus-orbital-tft-admin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            this.conn = new SqlConnection();
            this.conn.ConnectionString = connstring;
        }

        public List<Module> GetAllModulesAsAdmin()
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM MODULES ORDER BY ModuleCode", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ModuleDetails");
            conn.Close();
            List<Module> moduleList = new List<Module>();
            foreach (DataRow row in result.Tables["ModuleDetails"].Rows)
            {
                Module module = new Module();
                module.moduleName = row["ModuleName"].ToString();
                module.moduleCode = row["ModuleCode"].ToString();
                module.description = row["Description"].ToString();
                module.units = Convert.ToInt32(row["Units"]);
                module.graded = Convert.ToInt32(row["Graded"]) == 0 ? "Completed Satisfactory/Unsatisfactory" : "Graded";
                module.hidden = Convert.ToInt32(row["Hidden"]) == 0 ? false : true;
                moduleList.Add(module);
            }
            return moduleList;
        }

        public List<Module> GetAllModulesAsStudent()
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM MODULES WHERE Hidden=0 ORDER BY ModuleCode", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ModuleDetails");
            conn.Close();
            List<Module> moduleList = new List<Module>();
            foreach (DataRow row in result.Tables["ModuleDetails"].Rows)
            {
                Module module = new Module();
                module.moduleName = row["ModuleName"].ToString();
                module.moduleCode = row["ModuleCode"].ToString();
                module.description = row["Description"].ToString();
                module.units = Convert.ToInt32(row["Units"]);
                module.graded = Convert.ToInt32(row["Graded"]) == 0 ? "Completed Satisfactory/Unsatisfactory" : "Graded";
                module.hidden = Convert.ToInt32(row["Hidden"]) == 0 ? false : true;
                moduleList.Add(module);
            }
            return moduleList;
        }

        public bool DoesModuleExist(string moduleCode)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT ModuleCode FROM MODULES WHERE ModuleCode=@selectedModule", conn);
            cmd.Parameters.AddWithValue("@selectedModule", moduleCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM MODULES WHERE ModuleCode=@selectedModule", conn);
            cmd.Parameters.AddWithValue("@selectedModule", moduleCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ModuleCode");
            conn.Close();
            Module module = new Module();
            if (result.Tables["ModuleCode"].Rows.Count > 0)
            {
                DataTable table = result.Tables["ModuleCode"];
                module.moduleName = table.Rows[0]["ModuleName"].ToString();
                module.moduleCode = table.Rows[0]["ModuleCode"].ToString();
                module.description = table.Rows[0]["Description"].ToString();
                module.units = Convert.ToInt32(table.Rows[0]["Units"]);
                module.graded = Convert.ToInt32(table.Rows[0]["Graded"]) == 0 ? "Completed Satisfactory/Unsatisfactory" : "Graded";
                module.hidden = Convert.ToInt32(table.Rows[0]["Hidden"]) == 0 ? false : true;
            }
            return module;

        }

        public List<Post> GetAllPosts(Module module, Student student)
        {
            StudentDAL studentContext = new StudentDAL();

            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM POSTS WHERE ModuleCode=@selectedModule ORDER BY PostTime DESC", conn);
            cmd.Parameters.AddWithValue("@selectedModule", module.moduleCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Posts");
            conn.Close();
            List<Post> postList = new List<Post>();
            foreach (DataRow row in result.Tables["Posts"].Rows)
            {
                Post post = new Post(
                    module,
                    Convert.ToInt32(row["PostID"]),
                    Convert.ToDateTime(row["PostTime"]),
                    row["Title"].ToString(),
                    row["Description"].ToString(),
                    GetNumberOfPostUpvotes(Convert.ToInt32(row["PostID"])),
                    studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["StudentID"])),
                    GetAllComments(Convert.ToInt32(row["PostID"]), student),
                    DoesPostUpvoteExist(Convert.ToInt32(row["PostID"]), student.StudentId),
                    GetAllTagsForPost(Convert.ToInt32(row["PostID"])),
                    Convert.ToInt32(row["Edited"]) == 0 ? false : true,
                    Convert.ToInt32(row["Deleted"]) == 0 ? false : true
                );
                if (DoesPostFileExist(post.postId))
                {
                    post.file = GetPostFile(post.postId);
                }
                postList.Add(post);
            }
            return postList;
        }

        public bool DoesPostFileExist(int postId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM POST_FILES WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "PostFiles");
            conn.Close();
            if (result.Tables["PostFiles"].Rows.Count > 0)
                return true; // Module code exists
            return false; // Module code does not exist
        }

        public FileDataModel GetPostFile(int postId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM POST_FILES WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "File");
            conn.Close();
            if (result.Tables["File"].Rows.Count > 0)
            {
                DataTable table = result.Tables["File"];
                return new FileDataModel(
                    table.Rows[0]["FileName"].ToString(),
                    table.Rows[0]["ContentType"].ToString(),
                    (byte[])table.Rows[0]["FileData"]
                );
            }
            return null;
        }

        public int GetNumberOfPostUpvotes(int postId)
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM POST_UPVOTES WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Posts");
            conn.Close();
            return result.Tables["Posts"].Rows.Count;
        }


        public int AddPost(String moduleCode, String title, String description, int studentId)
        {
            StudentDAL studentContext = new StudentDAL();
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO POSTS(ModuleCode, PostTime, [Title], [Description], StudentID) " +
                "OUTPUT INSERTED.PostID " +
                "VALUES (@moduleCode, @postTime, @title, @description, @studentID)", conn);

            cmd.Parameters.AddWithValue("@moduleCode", moduleCode);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@studentID", studentId);

            conn.Open();
            int postId = (int)cmd.ExecuteScalar();
            conn.Close();
            return postId;
        }

        public int GetNumberOfCommentUpvotes(int commentId)
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM COMMENT_UPVOTES WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Comments");
            conn.Close();
            return result.Tables["Comments"].Rows.Count;
        }

        public List<Comment> GetAllComments(int postId, Student student)
        {
            StudentDAL studentContext = new StudentDAL();

            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM COMMENTS WHERE PostID=@postId ORDER BY CommentTime", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Comments");
            conn.Close();
            List<Comment> comments = new List<Comment>();
            foreach (DataRow row in result.Tables["Comments"].Rows)
            {

                Comment comment = new Comment(
                    Convert.ToInt32(row["CommentID"]),
                    Convert.ToDateTime(row["CommentTime"]),
                    row["Description"].ToString(),
                    GetNumberOfCommentUpvotes(Convert.ToInt32(row["CommentID"])),
                    Convert.ToInt32(row["PostID"]),
                    studentContext.GetStudentDetailsWithID(Convert.ToInt32(row["StudentID"])),
                    DoesCommentUpvoteExist(Convert.ToInt32(row["CommentID"]), student.StudentId),
                    Convert.ToInt32(row["Edited"]) == 0 ? false : true,
                    Convert.ToInt32(row["Deleted"]) == 0 ? false : true
                );
                if (DoesCommentFileExist(comment.commentId))
                {
                    comment.file = GetCommentFile(comment.commentId);
                };
                comments.Add(comment);
            }
            return comments;
        }


        public bool DoesCommentFileExist(int commentId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM COMMENT_FILES WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "CommentFiles");
            conn.Close();
            if (result.Tables["CommentFiles"].Rows.Count > 0)
                return true; // Module code exists
            return false; // Module code does not exist
        }

        public FileDataModel GetCommentFile(int commentId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM COMMENT_FILES WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "File");
            conn.Close();
            if (result.Tables["File"].Rows.Count > 0)
            {
                DataTable table = result.Tables["File"];
                return new FileDataModel(
                    table.Rows[0]["FileName"].ToString(),
                    table.Rows[0]["ContentType"].ToString(),
                    (byte[])table.Rows[0]["FileData"]
                );
            }
            return null;
        }

        public int AddCommentToPost(String description, int postId, int studentId)
        {
            StudentDAL studentContext = new StudentDAL();
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO COMMENTS(CommentTime, Description, PostID, StudentID)" +
                "OUTPUT INSERTED.CommentID " +
                "VALUES (@commentTime, @description, @postId, @studentID)", conn);

            cmd.Parameters.AddWithValue("@commentTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentID", studentId);

            conn.Open();
            int id = (int)cmd.ExecuteScalar();
            conn.Close();
            return id;
        }

        public void AddUpvoteToPost(int postId, int studentId)
        {
            SqlCommand cmd = new SqlCommand
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
            SqlCommand cmd = new SqlCommand
                ("DELETE FROM POST_UPVOTES WHERE PostID=@postId AND StudentID=@studentId", conn);

            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public Boolean DoesPostUpvoteExist(int postId, int studentId)
        {
            SqlCommand cmd = new SqlCommand
                ("SELECT * FROM POST_UPVOTES WHERE PostID=@postId AND StudentID=@studentId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
            SqlCommand cmd = new SqlCommand
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
            SqlCommand cmd = new SqlCommand
                ("DELETE FROM COMMENT_UPVOTES WHERE CommentID=@commentId AND StudentID=@studentId", conn);

            cmd.Parameters.AddWithValue("@commentId", commentId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public Boolean DoesCommentUpvoteExist(int commentId, int studentId)
        {
            SqlCommand cmd = new SqlCommand
                ("SELECT * FROM COMMENT_UPVOTES WHERE CommentID=@commentId AND StudentID=@studentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
            SqlCommand cmd = new SqlCommand(
            "SELECT PT.TagID, Tag FROM POST_TAGS PT INNER JOIN TAGS T ON PT.TagID = T.TagID WHERE PostID = @postId ", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
                    row["Tag"].ToString(),
                    false
                ));
            }
            return tagList;
        }

        public void AddPostTag(int postId, int tagId)
        {
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO POST_TAGS (PostID, TagID)" +
                "VALUES (@postId, @tagId)", conn);

            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@tagId", tagId);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public List<Tag> GetAllTags()
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM TAGS", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
                    row["Tag"].ToString(),
                    false
                ));
            }
            return tagList;
        }

        public Tag GetTagById(int tagId)
        {

            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM TAGS WHERE TagID = @tagId", conn);
            cmd.Parameters.AddWithValue("@tagId", tagId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
                    row["Tag"].ToString(),
                    true
                ));
            }
            return tagList[0];
        }


        public void UpdatePost(int postId, string postTitle, string postDescription)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE POSTS SET [Title]=@postTitle, [Description]=@postDescription, Edited=1 WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postTitle", postTitle);
            cmd.Parameters.AddWithValue("@postDescription", postDescription);
            cmd.Parameters.AddWithValue("@postId", postId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateComment(int commentId, string commentDescription)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE COMMENTS SET [Description]=@commentDescription, Edited=1 WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentDescription", commentDescription);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public List<String> GetPostDetails(int postId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM POSTS WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Post");
            conn.Close();
            List<String> postDetails = new List<String>();
            if (result.Tables["Post"].Rows.Count > 0)
            {
                DataTable table = result.Tables["Post"];
                if (table.Rows[0]["Title"].ToString() != null)
                {
                    postDetails.Add(table.Rows[0]["Title"].ToString());
                }
                if (table.Rows[0]["Description"].ToString() != null)
                {
                    postDetails.Add(table.Rows[0]["Description"].ToString());
                }
                return postDetails;
            }
            return postDetails;
        }

        public String GetCommentDescription(int commentId)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT * FROM COMMENTS WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Comment");
            conn.Close();
            if (result.Tables["Comment"].Rows.Count > 0)
            {
                DataTable table = result.Tables["Comment"];
                if (table.Rows[0]["Description"].ToString() != null)
                {
                    return table.Rows[0]["Description"].ToString();
                }
            }
            return "";
        }

        public void DeletePost(int postId)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE POSTS SET Deleted=1 WHERE PostID=@postId", conn);
            cmd.Parameters.AddWithValue("@postId", postId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void DeleteComment(int commentId)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE COMMENTS SET Deleted=1 WHERE CommentID=@commentId", conn);
            cmd.Parameters.AddWithValue("@commentId", commentId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        public void AddPostFile(int postId, string fileName, string contentType, byte[] fileData)
        {
            SqlCommand cmd = new SqlCommand
                 ("INSERT INTO POST_FILES (PostID, FileName, ContentType, FileData) " +
                 "VALUES (@postId, @fileName, @contentType, @fileData)", conn);

            cmd.Parameters.AddWithValue("@postId", postId);
            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.AddWithValue("@fileData", fileData);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void AddCommentFile(int commentId, string fileName, string contentType, byte[] fileData)
        {
            SqlCommand cmd = new SqlCommand
                 ("INSERT INTO COMMENT_FILES (CommentID, FileName, ContentType, FileData) " +
                 "VALUES (@commentId, @fileName, @contentType, @fileData)", conn);

            cmd.Parameters.AddWithValue("@commentId", commentId);
            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.AddWithValue("@fileData", fileData);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void AddModule(String moduleCode, String moduleName, String description, bool hidden)
        {
            StudentDAL studentContext = new StudentDAL();
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO MODULES(ModuleCode, ModuleName, Description, Hidden) " +
                "VALUES (@moduleCode, @moduleName, @description, @hidden)", conn);

            cmd.Parameters.AddWithValue("@moduleCode", moduleCode);
            cmd.Parameters.AddWithValue("@moduleName", moduleName);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@hidden", hidden);

            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }

        public void UpdateModule(string moduleCode, string moduleName, string description, bool hidden)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE MODULES SET ModuleName=@moduleName, Description=@description, Hidden=@hidden WHERE ModuleCode=@moduleCode", conn);
            cmd.Parameters.AddWithValue("@moduleCode", moduleCode);
            cmd.Parameters.AddWithValue("@moduleName", moduleName);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@hidden", hidden);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}