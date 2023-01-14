using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using TreeMine.Services;

namespace TreeMine
{
    internal static class Extensions
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



        /// <summary>
        /// Allows to report enumeration progress
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="every"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        internal static IEnumerable<TSource> GetProgress<TSource>(this IEnumerable<TSource> source, int every, Action<int, TSource> progress)
        {
            int count = 0;

            foreach (var item in source)
            {
                if ((++count % every) == 0)
                    progress(count, item);

                yield return item;
            }
        }
    }
}
