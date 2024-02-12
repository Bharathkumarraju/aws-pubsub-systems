using Amazon.Runtime;
using AWS.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudNotes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    AWSLoggerConfig config = new AWSLoggerConfig()
                    {
                        LogGroup = "CloudNotes",
                        Region = "ap-southeast-2",
                        Credentials = new BasicAWSCredentials(
                                "YOUR AWS ACCESS KEY",
                                "YOUR AWS SECRET KEY"
                            ),
                    };

                    logging.AddAWSProvider(config);

                }).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
