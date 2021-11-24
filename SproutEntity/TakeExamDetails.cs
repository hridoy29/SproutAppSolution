using System;
using System.Text;

namespace SproutEntity
{
	public class TakeExamDetails
	{
		public Int32 Id { get; set; }
		public Int32 QuestionId { get; set; }
		public string UserAnswer { get; set; }
		public Int32 ExamId { get; set; }
		public Int32 TakeExamId { get; set; }
	}
}
