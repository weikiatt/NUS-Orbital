namespace NUS_Orbital.Models
{
    public class StudentChat
    {
        public Student otherStudent { get; set; }
        public List<ChatLog> chatLog { get; set; }
        public StudentChat(Student otherStudent, List<ChatLog> chatLog)
        {
            this.otherStudent = otherStudent;
            this.chatLog = chatLog; 
        }
    }
}
