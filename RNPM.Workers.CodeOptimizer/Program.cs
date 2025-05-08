using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using RNPM.Common.Data;
using RNPM.Common.Interfaces;
using RNPM.Common.Services;
using RNPM.Workers.CodeOptimizer;
using RNPM.Workers.CodeOptimizer.Scheduling;
using RNPM.Workers.CodeOptimizer.Services;

public class Program
    {
        private IConfiguration _configuration;
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseSerilog()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var env = hostContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName ?? "Production"}.json", true)
                .AddEnvironmentVariables();

                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config.Build())
                .CreateLogger();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();

                services.AddHttpClient();
                services.AddHttpClient("Esolutions", client =>
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", hostContext.Configuration["EsolutionsSettings:BillPaymentsApiToken"]);
                    client.Timeout = TimeSpan.FromMinutes(1);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                    return httpClientHandler;
                });

                var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
                string connectionString = hostContext.Configuration.GetConnectionString("DohweConnection");

                // For Entity Framework
                services.AddDbContext<RnpmDbContext>(options =>
                    options.UseSqlServer(connectionString, x =>
                    {
                        x.MigrationsAssembly(migrationsAssembly);
                    }));
                
                services.AddTransient<IDateTimeService, DateTimeService>();
                services.AddTransient<ICurrentUserService, CurrentUserService>();

                //services.AddTransient<IGeneratorService, GeneratorService>();

                services.AddSingleton<IJobFactory, SchedulerJobFactory>();
                services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                services.AddScoped<CodeOptimizationJob>();

                var minutes = hostContext.Configuration["Scheduler:Minutes"];

                //services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(AirtimeSendingJob), "Airtime Sending Job", $"0 0/{minutes} * * * ?"));
                // services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(CommissionCalculationsJob), "Cimmission Calculation Job", $"0 0/{minutes} * * * ?"));
                //services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(ScheduleCleanupJob), "Schedule Cleanup Job", $"0 0/1 * * * ?"));
                //services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(BalanceReminderJob), "Balance Reminder Job", "0 0 7 * * ?"));
                var optimizationJob = new JobMetadata(Guid.NewGuid(), typeof(CodeOptimizationJob), "ZIMRA Job", $"0 0/{minutes} * * * ?");
                services.AddSingleton(new List<JobMetadata>()
                {
                    optimizationJob
                    
                });
                services.AddHostedService<SchedulerHostedService>();
            });
    }