namespace SanlamTest.Common.Interfaces
{
    public interface ILoggingService
    {        
        void Error(Exception exception, string messageTemplate, params object[] propertyValues);
        void Error(string messageTemplate, params object[] propertyValues);
        void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);
        void Fatal(string messageTemplate, params object[] propertyValues);
        void Information(string messageTemplate, params object[] propertyValues);
        void Warning(string messageTemplate, params object[] propertyValues);
    }
}