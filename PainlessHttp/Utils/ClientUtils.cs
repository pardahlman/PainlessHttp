using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
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

		public static CredentialCache CreateCredentials(IEnumerable<Credential> credentials)
		{
			var result = new CredentialCache();
			foreach (var credential in credentials)
			{
				foreach (var type in credential.AuthTypes)
				{
					result.Add(new Uri(credential.Domain), type.ToString(), new NetworkCredential(credential.UserName, credential.Password));
				}
			}
			return result;
		}
	}
}
