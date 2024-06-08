namespace NUS_Orbital.Models
{
    public class StudentChat
    {
        public Student otherStudent;
        public List<ChatLog> chatLog;
        public StudentChat(Student otherStudent, List<ChatLog> chatLog)
        {
            this.otherStudent = otherStudent;
            this.chatLog = chatLog; 
        }
    }
}
