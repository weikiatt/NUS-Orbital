using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto.Paddings;

namespace NUS_Orbital.Controllers
{
    public class ModuleController : Controller
    {
        private ModuleDAL moduleContext = new ModuleDAL();
        private StudentDAL studentContext = new StudentDAL();

        [HttpGet]
        public IActionResult Index(string query)
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                List<Module> moduleList = moduleContext.GetAllModules();
                if (query == null)
                {
                    return View(moduleList);
                }
                query = query.ToLower();
                List<Module> filteredModuleList = new List<Module>();
                foreach (Module module in  moduleList)
                {
                    if(module.moduleCode.ToLower().Contains(query)  || module.moduleName.ToLower().Contains(query))
                    {
                        filteredModuleList.Add(module);
                    }
                }
                return View(filteredModuleList);
            }
            TempData["Login"] = "Login to view more info about modules";
            return RedirectToAction("Login", "Home");
        }

        public ActionResult View(string ModuleCode, List<int> filterTag)
        {
            if (HttpContext.Session.GetString("authenticated") == "true")
            {
                Student currStud = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email"));
                Module module = moduleContext.GetModuleDetails(ModuleCode);
                List<Post> postList = moduleContext.GetAllPosts(module, currStud);
                //List<string> selectedTags = filterTag.Select(tagId => moduleContext.GetTagNameById(Convert.ToInt32(tagId))).ToList();
                if (filterTag != null && filterTag.Count() > 0)
                {
                    List<Tag> tags = moduleContext.GetAllTags();
                    foreach(Tag tag in tags) 
                    {
                        if (filterTag.Contains(tag.tagId))
                        {
                            tag.filtered = true;
                        }
                    }

                    /*
                    foreach(int tagId in filterTag)
                    {
                        tags.Add(moduleContext.GetTagById(tagId));
                    }*/
                    List<Post> finalPostList = new List<Post>();
                    foreach (var post in postList) {
                        if (post.TagExistInPost(filterTag))
                        {
                            finalPostList.Add(post);
                        }
                    }
                    return View(new ModulePost(module, finalPostList, currStud, tags));
                }

                return View(new ModulePost(module, postList, currStud));
            }
            TempData["Login"] = "Login to view more info about modules";
            return RedirectToAction("Login", "Home");
        }

        public IActionResult DownloadPostFile(int postId)
        {
            var fileData = moduleContext.GetPostFile(postId);

            if (fileData == null)
            {
                return NotFound();
            }

            return File(fileData.FileData, fileData.ContentType, fileData.FileName);
        }

        public IActionResult Post(IFormCollection formData)
        {
            string title = formData["title"].ToString();
            string description = formData["description"].ToString();
            string moduleCode = formData["moduleCode"].ToString();
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).StudentId;
            if (description != null)
            {
                int postId = moduleContext.AddPost(moduleCode, title, description, studentId);
                TempData["postid"] = postId;
                
                if (formData["tag1"].Count > 0)
                {
                    moduleContext.AddPostTag(postId, 1);
                }
                if (formData["tag2"].Count > 0)
                {
                    moduleContext.AddPostTag(postId, 2);
                }
                if (formData["tag3"].Count > 0)
                {
                    moduleContext.AddPostTag(postId, 3);
                }
                if (formData["tag4"].Count > 0)
                {
                    moduleContext.AddPostTag(postId, 4);
                }
                if (formData["tag5"].Count > 0)
                {
                    moduleContext.AddPostTag(postId, 5);
                }

                var file = formData.Files["document"];
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        moduleContext.AddPostFile (postId, file.FileName, file.ContentType, fileBytes);
                    }
                }
            }

            return RedirectToAction("View", "Module", new {ModuleCode = moduleCode});
        }

        /*
        [HttpPost]
        public JsonResult Post(string description, string moduleCode)
        {
            TempData["TEST"] = "POST TEST";
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
            if (description != null)
            {
                moduleContext.AddPost(moduleCode, description, studentId);
            }
            return Json(new { success = true });
        }*/

        public ActionResult Comment(IFormCollection formData)
        {
            string description = formData["description"].ToString();
            string moduleCode = formData["moduleCode"].ToString();
            int postId = Convert.ToInt32(formData["postId"]);
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).StudentId;
            if (description != null)
            {
                int commentId = moduleContext.AddCommentToPost(description, postId, studentId);
                string str = "commentFor(" + postId.ToString() + ")";
                var file = formData.Files[str];
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        moduleContext.AddCommentFile(commentId, file.FileName, file.ContentType, fileBytes);
                    }
                }
            }
            return RedirectToAction("View", "Module", new { ModuleCode = moduleCode });
        }

        public IActionResult DownloadCommentFile(int commentId)
        {
            var fileData = moduleContext.GetCommentFile(commentId);

            if (fileData == null)
            {
                return NotFound();
            }

            return File(fileData.FileData, fileData.ContentType, fileData.FileName);
        }

        [HttpPost]
        public JsonResult UpvotePost(int postId)
        {   
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).StudentId;
            if (moduleContext.DoesPostUpvoteExist(postId, studentId))
            {
                moduleContext.RemoveUpvoteFromPost(postId, studentId);
            } else
            {
                moduleContext.AddUpvoteToPost(postId, studentId);
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult UpvoteComment(int commentId)
        {
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).StudentId;
            if (moduleContext.DoesCommentUpvoteExist(commentId, studentId))
            {
                moduleContext.RemoveUpvoteFromComment(commentId, studentId);
            }
            else
            {
                moduleContext.AddUpvoteToComment(commentId, studentId);
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult EditPost(int postId, string postTitle, string postDescription)
        {
            if (postTitle == "" || postDescription == "")
            {
                return Json(new { success = false });
            }
            moduleContext.UpdatePost(postId, postTitle, postDescription);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditComment(int commentId, string commentDescription)
        {
            if (commentDescription == "")
            {
                return Json(new { success = false });
            }
            moduleContext.UpdateComment(commentId, commentDescription);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult GetPostDetails(int postId)
        {
            List<String> titleAndDesc = moduleContext.GetPostDetails(postId);
            return Json(new { success = true, originalTitle = titleAndDesc[0], originalDescription = titleAndDesc[1] });
        }

        [HttpPost]
        public JsonResult GetCommentDetails(int commentId)
        {
            String description = moduleContext.GetCommentDescription(commentId);
            return Json(new { success = true, originalDescription = description });
        }

        [HttpPost]
        public JsonResult DeletePost(int postId)
        {
            moduleContext.DeletePost(postId);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteComment(int commentId)
        {
            moduleContext.DeleteComment(commentId);
            return Json(new { success = true });
        }
    }
}
