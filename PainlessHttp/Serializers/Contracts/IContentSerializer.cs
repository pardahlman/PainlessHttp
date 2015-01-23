using System.Collections.Generic;
using PainlessHttp.Http;

namespace PainlessHttp.Serializers.Contracts
{
	public interface IContentSerializer
	{
		IEnumerable<ContentType> ContentType { get; }
		string Serialize(object data);
		T Deserialize<T>(string data);
	}
}
