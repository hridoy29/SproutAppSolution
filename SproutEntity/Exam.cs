using System;
using System.Text;

namespace SproutEntity
{
	public class Exam
	{
		public Int32 Id { get; set; }
		public Int32 ExamTypeId { get; set; }
		public Int32 CourseId { get; set; }
		public string ExamName { get; set; }
		public string TimeDuration { get; set; }
		public bool IsActive { get; set; }
		public string Creator { get; set; }
		public string CreationDate { get; set; }
		public string Modifier { get; set; }
		public string ModificationDate { get; set; }
	}
}
