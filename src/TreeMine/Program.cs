// Created with Janda.Go http://github.com/Jandini/Janda.Go
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Serilog;
using CommandLine;

namespace TreeMine
{
    class Program
    {
        static void Main(string[] args)
        {            
            try
            {                
                Parser.Default.ParseArguments<ProgramOptions.Count, ProgramOptions.Scan, ProgramOptions.Hash>(args)
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
                                case ProgramOptions.Count count:
                                    main.Count(count.Path);
                                    break;

                                case ProgramOptions.Scan scan:
                                    main.Scan(scan.Path);
                                    break;

                                case ProgramOptions.Hash hash:
                                    main.Hash(hash.Path);
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