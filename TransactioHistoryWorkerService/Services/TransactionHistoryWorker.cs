using SanlamTest.BLL.Interfaces;
using SanlamTest.Common.Interfaces;
using TransactioHistoryWorkerService.Models;

namespace TransactioHistoryWorkerService.Services;

public class TransactionHistoryWorker : BackgroundService
{    
    private readonly ILoggingService _loggingService;
    private readonly AppSettings _appSettings;
    private readonly ITransactionService _transactionService;

    public TransactionHistoryWorker(ILoggingService loggingService,ITransactionService transactionService,AppSettings appSettings)
    {
        _loggingService = loggingService;
        _appSettings= appSettings;
        _transactionService = transactionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _loggingService.Information($"TransactionHistoryWorker started {DateTime.Now}");
        while (!stoppingToken.IsCancellationRequested)
        {
            await _transactionService.InsertTransactionHistoryFromWithdrawOutboxAsync(_appSettings.ConcurrencyCount);

            await Task.Delay(_appSettings.SleepTimeInMs, stoppingToken);
        }
        _loggingService.Error($"TransactionHistoryWorker Stopped {DateTime.Now}");
    }
}