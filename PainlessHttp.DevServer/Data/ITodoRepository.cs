using System.Collections.Generic;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.DevServer.Data
{
	public interface ITodoRepository
	{
		Todo Add(Todo todo);
		Todo Update(Todo todo);
		void Remove(Todo todo);
		Todo Get(int id);
		List<Todo> GetAll();
	}
}
