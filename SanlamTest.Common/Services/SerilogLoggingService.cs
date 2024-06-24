using Microsoft.Extensions.Configuration;
using SanlamTest.Common.Interfaces;
using Serilog;
using Serilog.Formatting.Compact;


namespace SanlamTest.Common.Services
{
    public class SerilogLoggingService : ILoggingService
    {
        private readonly ILogger _logger;

        public SerilogLoggingService(IConfiguration configuration)
        {
            _logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    "c:/logs/log-.json",
                    rollingInterval: RollingInterval.Hour,
                    retainedFileCountLimit: 168,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Error(exception,$"{messageTemplate} (Exception Message {exception.Message}, Call Stack {exception.StackTrace})", propertyValues);            
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            _logger.Error($"{messageTemplate}", propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Fatal(exception,$"{messageTemplate} (Exception Message {exception.Message}, Call Stack {exception.StackTrace})", propertyValues);            
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            _logger.Fatal($"{messageTemplate})", propertyValues);
        }

        public void Information(string messageTemplate,params object[] propertyValues)
        {
            _logger.Information($"{messageTemplate}", propertyValues);            
        }
        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            _logger.Warning(messageTemplate, propertyValues);            
        }
    }
}