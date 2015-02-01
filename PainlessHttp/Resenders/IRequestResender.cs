using System.Threading.Tasks;
using PainlessHttp.Integration;

namespace PainlessHttp.Resenders
{
	public interface IRequestResender
	{
		bool IsApplicable(IHttpWebResponse response);
		Task<IHttpWebResponse> ResendRequestAsync(IHttpWebResponse response, WebRequestSpecifications specs);
	}
}