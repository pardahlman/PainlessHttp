using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PainlessHttp.Utils
{
	public class UrlBuilder
	{
		private readonly string _baseUrl;
		private static readonly Type expandoType = typeof (ExpandoObject);

		public UrlBuilder(string baseUrl)
		{
			_baseUrl = baseUrl;
		}

		public string Build(string relativeUrl = null, object query = null)
		{
			var builder = new StringBuilder(_baseUrl);
			if (!string.IsNullOrWhiteSpace(relativeUrl) && !string.IsNullOrWhiteSpace(_baseUrl))
			{
				if (_baseUrl.Last() == '/' && relativeUrl.First() == '/')
				{
					relativeUrl = relativeUrl.Substring(1);
				}
				if (_baseUrl.Last() != '/' && relativeUrl.First() != '/')
				{
					builder.Append('/');
				}
				builder.Append(relativeUrl);
			}
			if (query != null)
			{
				builder.Append(CreateQueryString(query));
			}
			return builder.ToString();
		}

		private static string CreateQueryString(object query)
		{
			var queryType = query.GetType();

			if (queryType == expandoType)
			{
				var dictionary = (IDictionary<string, object>) query;
				if (!dictionary.Keys.Any())
				{
					return string.Empty;
				}
				return dictionary.Keys
					.Select(k => k + "=" + dictionary[k])
					.Aggregate((accumilated, delta) => string.Format("{0}&{1}", accumilated, delta))
					.Insert(0, "?");
			}

			var properties = new List<PropertyInfo>(queryType.GetProperties());
			
			var queryString = properties
				.Select(p => p.Name + "=" + p.GetValue(query))
				.Aggregate((accumilated, delta) => string.Format("{0}&{1}", accumilated, delta))
				.Insert(0, "?");

			return queryString;
		}
	}
}
