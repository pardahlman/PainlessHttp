using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PainlessHttp.Http;

namespace PainlessHttp.Utils
{
	public class AcceptHeaderMapper
	{
		public IEnumerable<AcceptHeaderField> Map(string acceptHeader)
		{
			if (string.IsNullOrWhiteSpace(acceptHeader))
			{
				return new List<AcceptHeaderField>();
			}

			return acceptHeader
				.Split(',')
				.Select(MapToHeaderFields)
				.ToList();
		}

		private static AcceptHeaderField MapToHeaderFields(string field)
		{
			var sections = field
				.Split(';')
				.Select(s => s.Trim())
				.ToList();

			if (!sections.Any())
			{
				return null;
			}

			var result = new AcceptHeaderField
			{
				ContentType = sections[0],
				Q = MapHeaderFieldAttribute(sections.FirstOrDefault(s => s.StartsWith("q", StringComparison.InvariantCultureIgnoreCase))),
				Mxb = MapHeaderFieldAttribute(sections.FirstOrDefault(s => s.StartsWith("mxb", StringComparison.InvariantCultureIgnoreCase))),
				Mxs = MapHeaderFieldAttribute(sections.FirstOrDefault(s => s.StartsWith("mxs", StringComparison.InvariantCultureIgnoreCase))),
			};

			return result;
		}

		private static float MapHeaderFieldAttribute(string header)
		{
			float result = 0;
			if (string.IsNullOrWhiteSpace(header))
			{
				return result;
			}
			var valueIndex = header.IndexOf('=') + 1;
			var attrValue = string.Format("0{0}", header.Substring(valueIndex).Trim());
			float.TryParse(attrValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result);
			return result;
		}
	}
}
