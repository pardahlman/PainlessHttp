using System.Net;
using System.Threading.Tasks;

namespace PainlessHttp.Utils
{
	public class UnauthorizedResender : IRequestResender
	{
		public bool IsApplicable(IHttpWebResponse response)
		{
			if (response == null)
			{
				return false;
			}
			if (response.StatusCode != HttpStatusCode.Unauthorized)
			{
				return false;
			}

			return true;
		}

		public Task<IHttpWebResponse> ResendRequestAsync(IHttpWebResponse response, WebRequestSpecifications specs)
		{
			throw new System.NotImplementedException();
		}
	}
}
