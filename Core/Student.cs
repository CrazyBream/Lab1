namespace Core
{
    public class Student : Person, IStudyable, IChessPlayer
    {
        public int Course { get; set; } 
        public string StudentId { get; set; } 
        public double AverageGrade { get; set; }
        public string Country { get; set; } 
        public string RecordBookNumber { get; set; } 

        public Student(string firstName, string lastName, int course, string studentId, double averageGrade, string country, string recordBookNumber)
            : base(firstName, lastName)
        {
            Course = course;
            StudentId = studentId;
            AverageGrade = averageGrade;
            Country = country;
            RecordBookNumber = recordBookNumber;
        }

        public void Study()
        {
            AverageGrade = Math.Min(AverageGrade + 0.1, 5.0);
        }

        public void PlayChess()
        {
            AverageGrade = Math.Min(AverageGrade + 0.05, 5.0);
        }
    }
}
