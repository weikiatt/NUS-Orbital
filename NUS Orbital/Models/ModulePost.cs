using NUS_Orbital.DAL;

namespace NUS_Orbital.Models
{
    public class ModulePost
    {
        public Module module;
        public List<Post> posts;
        public Student student;
        public List<Tag> tags;
        private ModuleDAL moduleContext = new ModuleDAL();
        public ModulePost(Module module, List<Post> posts, Student student)
        {
            this.module = module;
            this.posts = posts;
            this.student = student;
            this.tags = moduleContext.GetAllTags();
        }

        public ModulePost(Module module, List<Post> posts, Student student, List<Tag> tags)
        {
            this.module = module;
            this.posts = posts;
            this.student = student;
            this.tags = tags;
        }
    }
}
