using System;
using NUnit.Framework;
using Microsoft.Owin.Hosting;
using PainlessHttp.DevServer;

namespace PainlessHttp.IntegrationTests
{
	[SetUpFixture]
	public class WebApiSetupFixture
	{
		private IDisposable _app;
		public static string BaseAddress;

		[SetUp]
		public void WireUpWebApiEndpoint()
		{
			var randomPort = new Random().Next(49152, 65535);
			BaseAddress = string.Format("http://localhost:{0}/", randomPort);
			Console.Write("Development Server running at " + BaseAddress);
			_app = WebApp.Start<Startup>(BaseAddress);
		}

		[TearDown]
		public void DisposeTheApiEndpoinnt()
		{
			if (_app != null)
			{
				_app.Dispose();
			}
		}
	}
}
