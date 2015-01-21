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
	
		public static string ContentType(ContentType type)
		{
			var result =  ContentTypes
								.Where(ct => ct.Item1 == type)
								.Select(ct => ct.Item2)
								.FirstOrDefault();

			if (!string.IsNullOrWhiteSpace(result))
			{
				return result;
			}

			throw new ArgumentException(string.Format("Unable to convert content type {0} to string.", type));
		}

		public static ContentType ContentType(string type)
		{
			var result = ContentTypes
								.Where(ct => ct.Item2 == type)
								.Select(ct => ct.Item1)
								.FirstOrDefault();

			if (result != Http.ContentType.Unknown)
			{
				return result;
			}

			throw new ArgumentException(string.Format("Unable to convert content type {0} to enum.", type));
		}
	}
}
