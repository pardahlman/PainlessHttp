using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PainlessHttp.Client;

namespace PainlessHttp.Utils
{
	public class ClientUtils
	{
		private const string _painlessAgent = "Painless Http Client";
		private static Version _version;

		public static string GetUserAgent()
		{
			if (_version == null)
			{
				_version = Assembly.GetAssembly(typeof (HttpClient)).GetName().Version;
			}
			return string.Format("{0} {1}", _painlessAgent, _version);
		}
	}
}
