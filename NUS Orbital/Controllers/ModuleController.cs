using Microsoft.AspNetCore.Mvc;
using NUS_Orbital.DAL;
using NUS_Orbital.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1;

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


        
        public ActionResult Post(IFormCollection formData)
        {
            string title = formData["title"].ToString();
            string description = formData["description"].ToString();
            string moduleCode = formData["moduleCode"].ToString();
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
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
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
            if (description != null)
            {
                moduleContext.AddCommentToPost(description, postId, studentId);
            }
            return RedirectToAction("View", "Module", new { ModuleCode = moduleCode });
        }

        [HttpPost]
        public JsonResult UpvotePost(int postId)
        {   
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
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
            int studentId = studentContext.GetStudentDetailsWithEmail(HttpContext.Session.GetString("Email")).studentId;
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
    }
}
