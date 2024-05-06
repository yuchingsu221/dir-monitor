using CommonLibrary;
using CommonLibrary.Util;

using DataLayer.RelationDB;
using DataLayer.RelationDB.Interfaces;
using Domain.Defines;
using Domain.Models.Config;
using Jobs.Jobs;
using Jobs.Services;
using Jobs.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Net.Http;

namespace Jobs
{
    public class Program
    {
        private static readonly WebServiceSetting _Settings = new WebServiceSetting();

        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var env = hostContext.HostingEnvironment.EnvironmentName;
                    var executePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    LogUtility.LogInfo($"Excute Env:{env}");
                    var config = BuildConfiguration(env, hostContext);
                    config.Bind(_Settings);

                    #region SampleJob
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();

                        var SampleJobKey = new JobKey("SampleJob");

                        // Register the job with the DI container
                        q.AddJob<SampleJob>(opts => opts.WithIdentity(SampleJobKey));

                        // Create a trigger for the job
                        q.AddTrigger(opts => opts
                        .ForJob(SampleJobKey) // link to the Job
                        .WithIdentity("SampleJobTrigger") // give the trigger a unique name
                        .WithSimpleSchedule(x => x
                            .WithInterval(new TimeSpan(0, 0, 10, 0, 0))
                            .RepeatForever()

                        // Create a trigger for the job
                        //q.AddTrigger(opts => opts
                        //.ForJob(SampleJobKey)
                        //.WithIdentity("SampleJobTrigger") // give the trigger a unique name
                        //以下為定期排程的語法設定
                        //(1) .WithCronSchedule(_Settings.Sample));
                        //(2) .WithCronSchedule("0 0 1 * * ?"));
                        //秒 分 時 日 月 星期 年（可選）

                        //.WithSimpleSchedule(x => x
                        //    .WithIntervalInMinutes(10) // 每10分鐘一次
                        //    .RepeatForever()
                        //)); ;  
                        ));
                    });
                    #endregion

                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                    services.AddSingleton(_Settings);
                    //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                    services.AddHttpClient(ExternalServiceEnum.TrueId.ToString(), x =>
                    {
                        x.BaseAddress = new Uri(_Settings.TrueIdSetting.Host);
                        x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        x.Timeout = TimeSpan.FromSeconds(10);
                    });

                    services.AddHttpClient(ExternalServiceEnum.SendSMS.ToString(), x =>
                    {
                        x.BaseAddress = new Uri(_Settings.SMSSetting.Url);
                        x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        x.Timeout = TimeSpan.FromSeconds(10);
                    });

                    services.AddHttpClient(ExternalServiceEnum.Push.ToString(), x =>
                    {
                        x.BaseAddress = new Uri(_Settings.PushSettings.Url);
                        x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        x.Timeout = TimeSpan.FromSeconds(10);
                    }).ConfigurePrimaryHttpMessageHandler(h =>
                    {
                        var handler = new HttpClientHandler();
                        handler.ServerCertificateCustomValidationCallback = delegate { return true; };
                        return handler;
                    });

                    services = DependencyInjectionService(services);

                    GlobalSettings();

                });

        private static IServiceCollection DependencyInjectionService(IServiceCollection services)
        {
            //services.AddScoped<IWriteApLoginLogService, WriteApLoginLogService>();
            services.AddScoped<ISmsService, SmsService>();

            return services;
        }

        // Global
        private static void GlobalSettings()
        {
            ServicePointManager.DefaultConnectionLimit = short.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        private static IConfiguration BuildConfiguration(string env, HostBuilderContext hostContext)
          => new ConfigurationBuilder()
              .CustomAddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
              .CustomAddJsonFile(path: $"appsettings.{env}.json", optional: true, reloadOnChange: true)
              .Build();

    }
}

