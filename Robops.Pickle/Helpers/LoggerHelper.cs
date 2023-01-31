using Serilog;
using Serilog.Events;

namespace Robops.Pickle.Helpers
{
    public class LoggerHelper
    {
        public static ILogger Criar(string path) 
            => new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(path, rollingInterval: RollingInterval.Day)
                .CreateLogger();
    }
}
