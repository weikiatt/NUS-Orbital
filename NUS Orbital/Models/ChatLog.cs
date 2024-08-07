namespace NUS_Orbital.Models
{
    public class ChatLog
    {
        public Student sender { get; set; }
        public Student receiver { get; set; }
        public string description { get; set; }
        public DateTime timeSent { get; set; }
        /*
        public ChatLog(Student sender, Student receiver, string description, DateTime timeSent)
        {
            this.sender = sender;
            this.receiver = receiver; 
            this.description = description;
            this.timeSent = timeSent;
        }*/
    }
}
