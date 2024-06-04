namespace NUS_Orbital.Models
{
    public class ModulePost
    {
        public Module module;
        public List<Post> posts;
        public Student student;
        public List<Tag> tags;
        public ModulePost(Module module, List<Post> posts, Student student)
        {
            this.module = module;
            this.posts = posts;
            this.student = student;
        }
    }
}
