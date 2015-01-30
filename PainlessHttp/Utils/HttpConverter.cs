using System;
using System.Collections.Generic;
using System.Linq;
using PainlessHttp.Http;

namespace PainlessHttp.Utils
{
	public class HttpConverter
	{

		static readonly List<Tuple<ContentType, string>> ContentTypes = new List<Tuple<ContentType, string>>
		{
			new Tuple<ContentType, string>(Http.ContentType.ApplicationJson, Http.ContentTypes.ApplicationJson),
			new Tuple<ContentType, string>(Http.ContentType.ApplicationXml, Http.ContentTypes.ApplicationXml),
			new Tuple<ContentType, string>(Http.ContentType.TextCsv, Http.ContentTypes.TextCsv),
			new Tuple<ContentType, string>(Http.ContentType.TextHtml, Http.ContentTypes.TextHtml),
			new Tuple<ContentType, string>(Http.ContentType.TextPlain, Http.ContentTypes.TextPlain),
		};

		static readonly List<Tuple<HttpMethod, string>> Methods = new List<Tuple<HttpMethod, string>>
		{
			new Tuple<HttpMethod, string>(Http.HttpMethod.Get, HttpMethods.Get),
			new Tuple<HttpMethod, string>(Http.HttpMethod.Post, HttpMethods.Post),
			new Tuple<HttpMethod, string>(Http.HttpMethod.Put, HttpMethods.Put),
			new Tuple<HttpMethod, string>(Http.HttpMethod.Delete, HttpMethods.Delete),
			new Tuple<HttpMethod, string>(Http.HttpMethod.Options, HttpMethods.Options),
		};
	
		public static string ContentType(ContentType type)
		{
			string result;
			if (TryParseContentType(type, out result))
			{
				return result;
			}

			throw new ArgumentException(string.Format("Unable to convert content type {0} to string.", type));
		}

		public static bool TryParseContentType(ContentType type, out string result)
		{
			result = ContentTypes
								.Where(ct => ct.Item1 == type)
								.Select(ct => ct.Item2)
								.FirstOrDefault();

			return result != null;
		}

		public static bool TryParseContentType(string type, out ContentType result)
		{
			result = ContentTypes
								.Where(ct => type.IndexOf(ct.Item2, StringComparison.InvariantCultureIgnoreCase) != -1)
								.Select(ct => ct.Item1)
								.FirstOrDefault();

			return result != Http.ContentType.Unknown;
		}

		public static ContentType ContentType(string type)
		{
			ContentType result;
			if (TryParseContentType(type, out result))
			{
				return result;
			}

			throw new ArgumentException(string.Format("Unable to convert content type {0} to enum.", type));
		}

		public static ContentType ContentTypeOrDefault(string type)
		{
			ContentType result;
			if (TryParseContentType(type, out result))
			{
				return result;
			}

			return default (ContentType);
		}

		public static HttpMethod HttpMethod(string method)
		{
			var result = Methods
								.Where(hm => hm.Item2 == method)
								.Select(hm => hm.Item1)
								.FirstOrDefault();

			if (result != Http.HttpMethod.Unknown)
			{
				return result;
			}

			throw new ArgumentException(string.Format("Unable to convert method {0} to string.", method));
		}

		public static string HttpMethod(HttpMethod method)
		{
			var result = Methods
								.Where(hm => hm.Item1 == method)
								.Select(hm => hm.Item2)
								.FirstOrDefault();

			if (!string.IsNullOrWhiteSpace(result))
			{
				return result;
			}

			throw new ArgumentException(string.Format("Unable to convert string {0} to method.", method));
		}
	}
}
