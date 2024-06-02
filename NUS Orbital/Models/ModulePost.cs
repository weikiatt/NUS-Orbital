namespace NUS_Orbital.Models
{
    public class ModulePost
    {
        public Module module;
        public List<Post> posts;
        public Student student;
        public ModulePost(Module module, List<Post> posts, Student student)
        {
            this.module = module;
            this.posts = posts;
            this.student = student;
        }
    }
}
