namespace NUS_Orbital.Models
{
    public class StudentView
    {
        public Student currStud;
        public Student studToView;
        public StudentView(Student currStud, Student studToView) {
            this.currStud = currStud;
            this.studToView = studToView;
        }
    }
}
