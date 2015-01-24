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
		public const string BaseAddress = "http://localhost:8080/";

		[SetUp]
		public void WireUpWebApiEndpoint()
		{
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
