namespace NUS_Orbital.Models
{
    public class StudentView
    {
        public Student currStud { get; set; }
        public Student studToView { get; set; }
        public StudentView(Student currStud, Student studToView) {
            this.currStud = currStud;
            this.studToView = studToView;
        }
    }
}
