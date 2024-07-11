namespace NUS_Orbital.Models
{
    public class ChatList
    {
        public Student currStudent { get; set;}
        // chats currStudent have with other students
        public List<StudentChat> students;
        public ChatList(Student currStudent, List<StudentChat> students)
        {
            this.currStudent = currStudent;
            this.students = students;
        }
    }
}
