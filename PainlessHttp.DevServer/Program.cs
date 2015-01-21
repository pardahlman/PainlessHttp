using System;
using Microsoft.Owin.Hosting;

namespace PainlessHttp.DevServer
{
	class Program
	{
		static void Main()
		{
			const string baseAddress = "http://localhost:1337/";

			var app = WebApp.Start<Startup>(baseAddress);
			Console.WriteLine("Painless Http Development Appserver is started at {0}.", baseAddress);
			Console.Write("Press enter to shut down client");
			Console.ReadLine();
			app.Dispose();
		} 
	}
}
