using System;

namespace PainlessHttp.DevServer.Model
{
	public class Todo
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime UpdateDate { get; set; }
	}
}
