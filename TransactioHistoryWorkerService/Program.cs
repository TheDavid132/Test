using SanlamTest.BLL.Interfaces;
using SanlamTest.BLL.Services;
using SanlamTest.Common.Interfaces;
using SanlamTest.Common.Services;
using SanlamTest.DAL.Factories;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Interfaces;
using TransactioHistoryWorkerService.Models;
using TransactioHistoryWorkerService.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Add services
        builder.Services.AddHostedService<TransactionHistoryWorker>();

        var appSettings = builder.Configuration.Get<AppSettings>();
        builder.Services.AddSingleton(appSettings);

        // Logging
        builder.Services.AddSingleton<ILoggingService, SerilogLoggingService>();

        // BLL services
        builder.Services.AddSingleton<ITransactionService, TransactionService>();

        // DAL repositories
        builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
        builder.Services.AddSingleton<IAccountBalanceRepository, AccountBalanceRepository>();
        builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

        // Infrastructure
        builder.Services.AddSingleton<IConnectionFactory, PostgreSqlConnectionFactory>();

        var host = builder.Build();
        host.Run();
    }
}