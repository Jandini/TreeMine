using Microsoft.Extensions.Logging;
using System.IO;
using Serilog;

namespace DirectoryNavigator
{
    internal class Main : IMain
    {
        private readonly ILogger<Main> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public Main(ILogger<Main> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public void Run()
        {
            
        }


        public void Create(string name)
        {            
            _logger.LogDebug("Adding file logger");

            _loggerFactory
              .AddSerilog(new LoggerConfiguration()
              .WriteTo.File(Path.ChangeExtension(Path.Combine(name, name), "log"))
              .CreateLogger(), dispose: true);
            
            _logger.LogDebug("New logger was added");
            _logger.LogInformation("Creating {name} directory in {path}", name, Directory.GetCurrentDirectory());
            var info = Directory.CreateDirectory(name);
            _logger.LogInformation("Directory {name} created successfully", info.FullName);            
        }
    }
}