namespace SproutDAL
{
    public static class Facade
    {
        public static CourseDAO CourseDAO { get { return new CourseDAO(); } }
        public static CourseDetailsDAO CourseDetailsDAO { get { return new CourseDetailsDAO(); } }
        public static CourseTypeDAO CourseTypeDAO { get { return new CourseTypeDAO(); } }
        public static ExamDAO ExamDAO { get { return new ExamDAO(); } }
        public static ExamDetailsDAO ExamDetailsDAO { get { return new ExamDetailsDAO(); } }
        public static ExamTypeDAO ExamTypeDAO { get { return new ExamTypeDAO(); } }
        public static QuestionDAO QuestionDAO { get { return new QuestionDAO(); } }
        public static TakeExamDAO TakeExamDAO { get { return new TakeExamDAO(); } }
        public static TakeExamDetailsDAO TakeExamDetailsDAO { get { return new TakeExamDetailsDAO(); } }
        public static UserDAO UserDAO { get { return new UserDAO(); } }
        public static UserProfileDAO UserProfileDAO { get { return new UserProfileDAO(); } }
        public static UsersDAO UsersDAO { get { return new UsersDAO(); } }
         
    }
}
