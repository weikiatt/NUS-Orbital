namespace NUS_Orbital.Models
{
    public class ChatLog
    {
        public Student sender;
        public Student receiver;
        public string description;
        public DateTime timeSent;
        public ChatLog(Student sender, Student receiver, string description, DateTime timeSent)
        {
            this.sender = sender;
            this.receiver = receiver; 
            this.description = description;
            this.timeSent = timeSent;
        }
    }
}
