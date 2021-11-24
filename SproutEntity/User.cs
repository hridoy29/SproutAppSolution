using System;
using System.Text;

namespace SproutEntity
{
	public class User
	{
		public Int32 Id { get; set; }
		public string MobileNo { get; set; }
		public string Password { get; set; }
		public bool IsActive { get; set; }
		public string Creator { get; set; } = "Admin";
		public DateTime CreationDate { get; set; } = DateTime.Now;
		public string Modifier { get; set; } = "Admin";
		public DateTime ModificationDate { get; set; } = DateTime.Now;
	}
}
