using SanlamTest.BLL.DTO;
using SanlamTest.BLL.Interfaces;
using SanlamTest.Common.DTO;
using SanlamTest.Common.Enums;
using SanlamTest.Common.Interfaces;
using SanlamTest.DAL.Interfaces;

namespace SanlamTest.BLL.Services
{

    public class TransactionService : ITransactionService
    {
        private readonly ILoggingService _loggingService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountBalanceRepository _accountBalanceRepository;

        public TransactionService(
            ILoggingService loggingService, 
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IAccountBalanceRepository accountBalanceRepository
            )
        {
            _loggingService = loggingService;
            _transactionRepository = transactionRepository;
            _accountRepository= accountRepository;
            _accountBalanceRepository= accountBalanceRepository;
        }

        public WithdrawalResponseDto Withdraw(long accountId, decimal amount)
        {
            try
            {
                var withdrawalResponseDto = ValidateWithdrawl(accountId, amount);

                if (withdrawalResponseDto != null)
                    return withdrawalResponseDto;

                var withdraw = _transactionRepository.Withdraw(accountId, amount);

                return new WithdrawalResponseDto(withdraw);
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, "Error occured in TransactionManager=>Withdraw");
                return new WithdrawalResponseDto(WithdrawalStatus.FailedInternalError);
            }
        }
        public async Task InsertTransactionHistoryFromWithdrawOutboxAsync(int concurrencyCount)
        {
            try
            {
                var tasks = new List<Task<Response<string?>>>();

                for (int i = 0; i < concurrencyCount; i++)
                    tasks.Add(_transactionRepository.InsertTransactionHistoryFromWithdrawOutboxAsync());

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, "Error in TransactionService");
            }
        }
        private WithdrawalResponseDto ValidateWithdrawl(long accountId, decimal amount)
        {
            var accountResponse = _accountRepository.GetById(accountId);

            if (accountResponse.ResponseStatus == Status.FailedInternalError)
                return new WithdrawalResponseDto(WithdrawalStatus.FailedInternalError);

            if (accountResponse.Value == null)
            {
                _loggingService.Error($"TransactionService => Withdraw Error Account {accountId} not found");
                return new WithdrawalResponseDto(WithdrawalStatus.FailedAccountNotFound);
            }

            if (!accountResponse.Value.Enabled)
            {
                _loggingService.Information($"TransactionService => Withdraw Error Account {accountId} disabled");
                return new WithdrawalResponseDto(WithdrawalStatus.FailedAccountDisabled);
            }

            var accountBalanceResponse = _accountBalanceRepository.GetById(accountId);

            if (accountBalanceResponse.ResponseStatus == Status.FailedInternalError)
                return new WithdrawalResponseDto(WithdrawalStatus.FailedInternalError);

            if (accountBalanceResponse.Value == null)
            {
                _loggingService.Error($"TransactionService => Withdraw Error AccountBalance {accountId} not found");
                return new WithdrawalResponseDto(WithdrawalStatus.FailedAccountNotFound);
            }

            if (accountBalanceResponse.Value.Balance < amount)
            {
                _loggingService.Information($"TransactionService => Withdraw Error Account {accountId} Insufficient Balance");
                return new WithdrawalResponseDto(WithdrawalStatus.FailedInsufficientBalance);
            }

            return null;
        }
    }
}
