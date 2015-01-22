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

		public static async Task<string> ReadBodyAsync(HttpWebResponse response)
		{
			var responseStream = response.GetResponseStream();
			if (responseStream == null)
			{
				throw new ArgumentNullException("response");
			}

			string raw;
			using (var reader = new StreamReader(responseStream))
			{
				raw = await reader.ReadToEndAsync();
			}
			return raw;
		}
		public async static Task<T> ParseBodyAsync<T>(HttpWebResponse response)
		{
			var readTask = ReadBodyAsync(response);
			var deserializeTask = await readTask.ContinueWith(task =>
							{
								var body = task.Result;
								var typedBody = JsonConvert.DeserializeObject<T>(body);
								return typedBody;
							});
			return deserializeTask;
		}
	}
}
