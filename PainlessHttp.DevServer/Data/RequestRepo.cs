using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;

namespace PainlessHttp.DevServer.Data
{
	public class RequestRepo
	{
		public const string RequestIdentifierHeader = "X-Request-Id";
		private readonly Dictionary<string, List<HttpRequestMessage>> _dictionary; 

		private static RequestRepo instace;

		public static RequestRepo Instance
		{
			get
			{
				if (instace == null)
				{
					instace = new RequestRepo();
				}
				return instace;
			}
		}

		private RequestRepo()
		{
			_dictionary = new Dictionary<string, List<HttpRequestMessage>>();
		}

		public void SaveRequestIfPossible(HttpRequestMessage request)
		{
			if (request == null)
			{
				return;
			}
			IEnumerable<string> values;
			if (!request.Headers.TryGetValues(RequestIdentifierHeader, out values))
			{
				return;
			}

			foreach (var value in values)
			{
				if (_dictionary.ContainsKey(value))
				{
					_dictionary[value].Add(request);
				}
				else
				{
					_dictionary.Add(value, new List<HttpRequestMessage>{ request});
				}
			}
		}

		public IEnumerable<HttpRequestMessage> GetRequestForSession(string sessionId)
		{
			if (_dictionary.ContainsKey(sessionId))
			{
				return _dictionary[sessionId];
			}
			return new List<HttpRequestMessage>();
		} 
	}
}
