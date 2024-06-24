using SanlamTest.BLL.Interfaces;
using SanlamTest.Common.Enums;
using SanlamTest.Common.Helpers;
using SanlamTest.Common.Interfaces;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Models;

namespace SanlamTest.BLL.Services
{
    public class TransactionMessageService : ITransactionMessageService
    {
        private readonly ILoggingService _loggingService;
        private readonly IMessageHandler _messageHandler;
        private readonly IWithdrawOutboxRepository _withdrawOutboxRepository;

        public TransactionMessageService(ILoggingService loggingService, IMessageHandler messageHandler, IWithdrawOutboxRepository withdrawOutboxRepository)
        {
            _loggingService = loggingService;
            _messageHandler = messageHandler;
            _withdrawOutboxRepository = withdrawOutboxRepository;
        }

        public async Task SendWithdrawalMessages(int concurrentMessages, int failedRetryCount, CancellationToken cancellationToken)
        {
            try
            {
                var response = _withdrawOutboxRepository.GetPendingMessagesSetStatusInProgress(concurrentMessages);

                if (response.ResponseStatus == Status.Failed || response.ResponseStatus == Status.FailedInternalError)
                    //error already logged in repository
                    return;


                if (response?.Value == null)
                {
                    _loggingService.Error("SendWithdrawalMessages : response or response value is null check if there are any messages to send");
                    return;
                }

                if (!response.Value.Any())
                    return;

                List<Task> tasks = new List<Task>();
                foreach (var message in response.Value)
                {
                    var withdrawalEvent = new WithdrawalEvent(message.Amount, message.AccountId, "SUCCESSFUL");
                    tasks.Add(
                        RetryHelper.PerformOperationWithRetryAsync(
                            async () =>
                            {
                                await _messageHandler.PublishWithdrawalEventAsync(withdrawalEvent);
                                return new OperationResult(true);
                            },
                        failedRetryCount,
                        new TimeSpan(0, 0, 10),
                        cancellationToken,
                        async () =>
                        {
                            await Task.Run(
                                () =>
                                    {
                                        var status = _withdrawOutboxRepository.ResetMessageServiceStatus(message.Id);
                                        if (status == Status.Failed || status == Status.FailedInternalError)
                                            _loggingService.Fatal($"Error resetting Message ID:{message.Id}");
                                    });
                            return new OperationResult(true);
                        }));
                }

                await Task.WhenAll(tasks);

            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, $"Error sending messages SendWithdrawalMessages");
            }
        }
    }
}
