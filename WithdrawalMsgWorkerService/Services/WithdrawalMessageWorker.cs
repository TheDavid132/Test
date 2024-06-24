using SanlamTest.BLL.Interfaces;
using SanlamTest.Common.Interfaces;
using WithdrawalMsgWorkerService.Models;

namespace WithdrawalMsgWorkerService.Services;

public class WithdrawalMessageWorker : BackgroundService
{
    private readonly ILoggingService _loggingService;
    private readonly AppSettings _appSettings;
    private readonly ITransactionMessageService _transactionMessageService;

    public WithdrawalMessageWorker(ILoggingService loggingService, ITransactionMessageService transactionMessageService, AppSettings appSettings)
    {
        _loggingService = loggingService;
        _appSettings = appSettings;
        _transactionMessageService = transactionMessageService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _loggingService.Information($"TransactionHistoryWorker started {DateTime.Now}");
        while (!stoppingToken.IsCancellationRequested)
        {
            await _transactionMessageService.SendWithdrawalMessages(_appSettings.ConcurrencyCount,_appSettings.RetryAttempts, stoppingToken);

            await Task.Delay(_appSettings.ServiceSleepTimeInMs, stoppingToken);
        }
        _loggingService.Error($"TransactionHistoryWorker Stopped {DateTime.Now}");
    }
}
