// Created with Janda.Go http://github.com/Jandini/Janda.Go
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Serilog;
using CommandLine;

namespace DirectoryNavigator
{
    class Program
    {
        static void Main(string[] args)
        {            
            try
            {
                Parser.Default.ParseArguments<Options.Run, Options.Create>(args)
                    .WithParsed((parameters) =>
                    {
                        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddAppSettingsJson("appsettings.json")
                            .Build();

                        using var provider = new ServiceCollection()
                            .AddServices()
                            .AddConfiguration(configuration)
                            .AddLogging(builder => builder
                                .AddSerilog(new LoggerConfiguration()
                                    .ReadFrom.Configuration(configuration)
                                    .CreateLogger(), dispose: true))
                            .BuildServiceProvider();

                        provider.LogVersion();

                        try
                        {
                            var main = provider.GetRequiredService<IMain>();

                            switch (parameters)
                            {
                                case Options.Run:
                                    main.Run();
                                    break;

                                case Options.Create options:
                                    main.Create(options.Name);
                                    break;

                            };
                        }
                        catch (Exception ex)
                        {
                            provider.GetService<ILogger<Program>>()?
                                .LogCritical(ex, ex.Message);
                        }
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}