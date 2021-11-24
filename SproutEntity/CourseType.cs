using System;
using System.Text;

namespace SproutEntity
{
	public class CourseType
	{
		public Int32 Id { get; set; }
		public string CourseTypeName { get; set; }
		public bool IsActive { get; set; }
		public string Creator { get; set; }
		public string CreationDate { get; set; }
		public string Modifier { get; set; }
		public string ModificationDate { get; set; }
	}
}
