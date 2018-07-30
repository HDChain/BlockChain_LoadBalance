using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LoadBalance
{
    public class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(Program));
        public static void Main(string[] args)
        {
            var fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            var repository = LogManager.CreateRepository(Log4NetCore.CoreRepository);
            XmlConfigurator.Configure(repository, fi);

            Logger.Debug("start ");

            NodeChecker.Instance.Start();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var url = "http://*:50000";

            for (var i = 0; i < args.Length; i++) {
                Console.WriteLine($"{i}@ {args[i]}");

                var tmpArr = args[i].Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (tmpArr.Length != 2)
                    continue;

                switch (tmpArr[0]) {
                    case "-url":
                    case "url":
                    case "--url":
                        url = tmpArr[1];
                        break;
                }
            }

            return WebHost.CreateDefaultBuilder(args)
                .UseUrls(url)
                .UseStartup<Startup>();
        }
    }
}
