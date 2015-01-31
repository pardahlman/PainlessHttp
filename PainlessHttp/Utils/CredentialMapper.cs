using System;
using System.Collections.Generic;
using System.Net;
using PainlessHttp.Client.Configuration;

namespace PainlessHttp.Utils
{
	public class CredentialMapper
	{
		public CredentialCache Map(IEnumerable<Credential> credentials)
		{
			var result = new CredentialCache();
			foreach (var credential in credentials)
			{
				foreach (var type in credential.AuthTypes)
				{
					result.Add(new Uri(credential.Domain), type.ToString(), new NetworkCredential(credential.UserName, credential.Password) );
				}
			}
			return result;
		}
	}
}
