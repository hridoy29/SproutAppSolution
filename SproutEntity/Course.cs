using System;
using System.Text;

namespace SproutEntity
{
	public class Course
	{
		public Int32 Id { get; set; }
		public Int32 CourseTypeId { get; set; }
		public string CourseName { get; set; }
		public bool IsActive { get; set; }
		public string Creator { get; set; }
		public string CreationDate { get; set; }
		public string Modifier { get; set; }
		public string ModificationDate { get; set; }
	}
}
