﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace TreeMine
{
    internal static class ProgramExtensions
    {
        internal static IConfigurationBuilder AddAppSettingsJson(this IConfigurationBuilder builder, string name)
        {
            return builder
                .AddJsonStream(new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), typeof(Program).Namespace).GetFileInfo(name).CreateReadStream())
                .AddJsonFile(name, true);
        }

        internal static IServiceProvider LogVersion(this IServiceProvider provider)
        {
            provider
                .GetRequiredService<ILogger<Program>>()
                .LogInformation("TreeMine {version}", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

            return provider;
        }


        internal static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddSingleton(configuration);
        }

        internal static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddTransient<IMain, Main>()
                .AddTransient<IDirectoryMinerService, DirectoryMinerService>()
                .AddTransient<IFileSystemMinerService, FileSystemMinerService>();
        }
    }
}