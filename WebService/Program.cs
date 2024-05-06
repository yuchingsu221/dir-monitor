using CommonLibrary;
using CommonLibrary.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog.Web;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tag = "Program.Main";
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            LogUtility.LogInfo($"=========================", tag);
            LogUtility.LogInfo($"DIR_MONITOR_API Web Service 啟動!", tag);
            LogUtility.LogInfo($"=========================", tag);
            LogUtility.LogInfo($"Ver: {version}");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // 取得環境變數
                    var env = hostContext.HostingEnvironment;
                    LogUtility.LogInfo($"env:{env.EnvironmentName}");

                    var jsonPath = $"{Directory.GetCurrentDirectory()}/appsettings.json";
                    var jsonEnvPath = $"{Directory.GetCurrentDirectory()}/appsettings.{env.EnvironmentName}.json";

                    config.SetBasePath(Directory.GetCurrentDirectory())
                    .CustomAddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                    .CustomAddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .ConfigureLogging(logging =>
                    {
                        //清除其他 logger provide 如預設 console (一經設定 log 就不會在 console 上出現)
                        logging.ClearProviders();
                        //設定最低 log 輸出 level
                        logging.SetMinimumLevel(LogLevel.Warning);
                    })
                    .UseNLog();
                });
    }
}
