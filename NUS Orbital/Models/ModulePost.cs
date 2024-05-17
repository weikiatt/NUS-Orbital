namespace NUS_Orbital.Models
{
    public class ModulePost
    {
        public Module module;
        public List<Post> posts;
        public ModulePost(Module module, List<Post> posts)
        {
            this.module = module;
            this.posts = posts;
        }
    }
}
