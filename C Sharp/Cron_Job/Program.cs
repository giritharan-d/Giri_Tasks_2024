using Quartz.Impl;
using Quartz;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;


namespace Cron_Job
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                    .AddJsonFile($"appsettings.json");
            var config = configuration.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<Job>();
            serviceCollection.AddSingleton<IConfiguration>(_ => config);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //Cron Job Code:
            StdSchedulerFactory factory = new();
            IScheduler scheduler = await factory.GetScheduler();
            scheduler.JobFactory = new JobFactory(serviceProvider);
            await scheduler.Start();

            
            IJobDetail job = JobBuilder.Create<Job>()
                .WithIdentity("DataUpdateJob", "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 2 Minutes....
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("DataUpdatetrigger", "group1")
               .StartNow()
               .WithCronSchedule("0 0/2 * * * ?")   
               .Build();

            await scheduler.ScheduleJob(job, trigger);

            Console.ReadKey();
        } 
    }



}
