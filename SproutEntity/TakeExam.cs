using System;
using System.Text;

namespace SproutEntity
{
	public class TakeExam
	{
		public Int32 Id { get; set; }
		public Int32 ExamId { get; set; }
		public Int32 UserId { get; set; }
		public bool IsActive { get; set; }
		public string Creator { get; set; }
		public string CreationDate { get; set; }
		public string Modifier { get; set; }
		public string ModificationDate { get; set; }
	}
}
