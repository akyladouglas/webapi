using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using AtendTeleMedicina.DistributedInterfaces.Services;
using System;
using System.IO;

namespace AtendTeleMedicina
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();
            var host = new WebHostBuilder()
             .UseKestrel(
                options =>
                {
                    options.Limits.MaxRequestBodySize = 30 * 1000 * 1000;
                    options.Limits.MaxRequestBufferSize = 8 * 1024 * 1024;
                    options.Limits.MaxRequestHeaderCount = 300;
                    options.Limits.MaxRequestHeadersTotalSize = 64 * 1024;
                    options.Limits.MaxRequestLineSize = 16 * 1024;
                    options.Limits.MaxResponseBufferSize = 128 * 1024;
                    options.Limits.MaxConcurrentConnections = 600;
                    options.Limits.MaxConcurrentUpgradedConnections = 600;
                    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
                    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(60);

                    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(360);
                    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(180);

                })
             .UseContentRoot(Directory.GetCurrentDirectory())
             .UseIISIntegration()
             .UseStartup<Startup>()
             .Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();
    }
}
