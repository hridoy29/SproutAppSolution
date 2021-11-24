using System;
using System.Text;

namespace SproutEntity
{
	public class sysdiagrams
	{
		public string name { get; set; }
		public Int32 principal_id { get; set; }
		public Int32 diagram_id { get; set; }
		public Int32? version { get; set; }
		public Byte[] definition { get; set; }
	}
}
