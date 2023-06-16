namespace SDN.Sdk.Sample.Console
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;

    internal static class Program
    {
        private static readonly Sample sampleApp = new();

#pragma warning disable CA1416
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(loggingBuilder =>
                    loggingBuilder.AddSerilog(dispose: true));

                services.AddSDNClient();
            }).UseConsoleLifetime();

#pragma warning restore CA1416

        
        private static async Task<int> Main(string[] args)
        {
            IHost host = CreateHostBuilder().Build();
            
            SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            await sampleApp.Run(host.Services);

            return 0;
        }
    }
}