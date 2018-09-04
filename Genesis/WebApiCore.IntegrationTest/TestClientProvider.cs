using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace WebApiCore.IntegrationTest
{
    public class TestClientProvider:IDisposable
    {

        #region Variables

        public HttpClient Client { get; private set; }
        private TestServer server;

        #endregion

        #region Constructor

        public TestClientProvider()
        {
            var projectDir = System.IO.Directory.GetCurrentDirectory();
            server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(projectDir)
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(projectDir)
                    .AddJsonFile("appsettings.json")
                    .Build()
                )
                .UseStartup<Startup>());
            //server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            server.CreateClient();
            Client = server.CreateClient();
        }

        #endregion

        public void Dispose()
        {
            server?.Dispose();
            Client?.Dispose();
        }
    }
}
