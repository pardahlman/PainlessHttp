using System;
using System.Collections.Generic;
using System.Linq;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.DevServer.Data
{
	public class InMemoryTodoRepo : ITodoRepository
	{
		#region Constructor & Singelton
		private static InMemoryTodoRepo _instance;
		public static InMemoryTodoRepo Instance
		{
			get { return _instance ?? (_instance = new InMemoryTodoRepo()); }
		}
		private InMemoryTodoRepo()
		{
			_todos = new List<Todo>
					{
						new Todo { Id = 1, IsCompleted = true, Description = "Start writing awsome todo app", UpdateDate = DateTime.Now.AddDays(-1)},
						new Todo { Id = 2, IsCompleted = false, Description = "Invent awsome idea that makes you a millionare", UpdateDate = DateTime.Now.AddDays(-1)},
						new Todo { Id = 3, IsCompleted = true, Description = "Find out why Redbull sponsors sports event.", UpdateDate = DateTime.Now.AddDays(-1)},
						new Todo { Id = 4, IsCompleted = true, Description = "Make a list of awsome books.", UpdateDate = DateTime.Now.AddDays(-1)},
					};
		}
		#endregion
		
		private readonly List<Todo> _todos;

		public Todo Add(Todo todo)
		{
			if (todo == null)
			{
				return null;
			}
			if (_todos.Any(t => t.Id == todo.Id))
			{
				Update(todo);
			}
			else
			{
				todo.Id = _todos.Count + 1;
				_todos.Add(todo);
			}
			return todo;
		}

		public Todo Update(Todo todo)
		{
			var found = _todos.FirstOrDefault(t => t.Id == todo.Id);
			if(found == null)
				return null;
			found.Description = todo.Description;
			found.IsCompleted = todo.IsCompleted;

			return found;
		}

		public void Remove(Todo todo)
		{
			var found = _todos.FirstOrDefault(t => t.Id == todo.Id);
			if (found == null)
			{
				return;
			}
			_todos.Remove(found);
		}

		public Todo Get(int id)
		{
			return _todos.FirstOrDefault(t => t.Id == id);
		}

		public List<Todo> GetAll()
		{
			var copy = _todos
				.Select(t => new Todo {Id = t.Id, Description = t.Description, IsCompleted = t.IsCompleted})
				.ToList();

			return copy;
		}
	}
}
