namespace NUS_Orbital.Models
{
    public class Post
    {
        public Module module;
        public int postId;
        public DateTime postTime;
        public string description;
        public int upvotes;
        public int downvotes;
        public Student student;
        public List<Comment> comments;
        public Post(Module module, int postId, DateTime postTime, string description, int upvotes, int downvotes, Student student, List<Comment> comments)
        {
            this.module = module;
            this.postId = postId;
            this.postTime = postTime;
            this.description = description;
            this.upvotes = upvotes;
            this.downvotes = downvotes;
            this.student = student;
            this.comments = comments;
        }
    }
}
