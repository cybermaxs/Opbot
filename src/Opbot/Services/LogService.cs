using Serilog;
using Serilog.Events;

namespace Opbot.Services
{
    internal class LogService : ILogService
    {
        private readonly ILogger innerLogger;

        public LogService(Options options)
        {
            var log = new LoggerConfiguration().MinimumLevel.Is(options.Verbose ? LogEventLevel.Verbose : LogEventLevel.Information)
            .WriteTo.ColoredConsole()
            .CreateLogger();


            this.innerLogger = log;
        }

        public void Verbose(string pattern, params object[] parameters)
        {
            innerLogger.Verbose(pattern, parameters);
        }

        public void Info(string pattern, params object[] parameters)
        {
            innerLogger.Information(pattern, parameters);
        }

        public void Warning(string pattern, params object[] parameters)
        {
            innerLogger.Warning(pattern, parameters);
        }

        public void Error(string pattern, params object[] parameters)
        {
            innerLogger.Error(pattern, parameters);
        }
    }
}
