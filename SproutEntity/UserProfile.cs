using System;
using System.Text;

namespace SproutEntity
{
	public class UserProfile
	{
		public Int32 Id { get; set; }
		public Int32 UserId { get; set; }
		public string Name { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }


      //  private DateTime dob;


        public Int32 age { get; set; }
        public string pic { get; set; }
      //  public DateTime Dob { get => dob; set => dob = value; }
    }
}
