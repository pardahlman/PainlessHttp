using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin;
using PainlessHttp.DevServer.Data;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.DevServer.Controllers
{
	public class TodoController : ApiController
	{
		private readonly ITodoRepository _repo;

		public TodoController()
		{
			_repo = InMemoryTodoRepo.Instance;
		}

		[Route("api/todos")]
		[HttpGet]
		public HttpResponseMessage GetAllTodos(bool includeCompeted = true)
		{
			var todos = _repo.GetAll();
			if (includeCompeted == false)
			{
				todos = todos.Where(t => t.IsCompleted == false).ToList();
			}
			return Request.CreateResponse(HttpStatusCode.OK, todos);
		}

		[Route("api/todos/{id}")]
		[HttpGet]
		public HttpResponseMessage GetTodo(int id)
		{
			var found = _repo.Get(id);
			if (found == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			if (Request.Headers.IfModifiedSince.HasValue && Request.Headers.IfModifiedSince.Value <= found.UpdateDate)
			{
				return Request.CreateResponse(HttpStatusCode.NotModified);
			}

			var response = Request.CreateResponse(HttpStatusCode.OK, found);
			response.Content.Headers.LastModified = new DateTimeOffset(found.UpdateDate);
			return response;
		}

		[Route("api/todos")]
		[HttpPost]
		public HttpResponseMessage AddTodo(Todo todo)
		{
			if (todo == null)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			var created = _repo.Add(todo);
			return Request.CreateResponse(HttpStatusCode.Created, created);
		}

		[Route("api/todos")]
		[HttpPut]
		public HttpResponseMessage UpdateTodo(Todo todo)
		{
			if (todo == null)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			var found = _repo.Get(todo.Id);
			if (found == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			var updated = _repo.Update(todo);
			return Request.CreateResponse(HttpStatusCode.NoContent, updated);
		}

		[Route("api/todos/{id}")]
		[HttpDelete]
		public HttpResponseMessage RemoveTodo(int id)
		{
			var found = _repo.Get(id);
			if (found == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}
			_repo.Remove(found);
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		
	}
}
