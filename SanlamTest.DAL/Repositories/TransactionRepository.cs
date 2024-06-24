using Dapper;
using SanlamTest.Common.DTO;
using SanlamTest.Common.Enums;
using SanlamTest.Common.Interfaces;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Models;
using System.Data;

namespace SanlamTest.DAL.Interfaces
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public TransactionRepository(IConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }
        public WithdrawalResult Withdraw(Int64 accountId, decimal amount)
        {
            try
            {
                using (IDbConnection db = _connectionFactory.CreateSanlamDbConnection())
                {
                    Guid transactionGuid = Guid.NewGuid();
                    var parameters = new DynamicParameters();
                    parameters.Add("@accountid", accountId, DbType.Int64, ParameterDirection.Input);
                    parameters.Add("@amount", amount, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@transactionguid", transactionGuid, DbType.Guid, ParameterDirection.Input);
                    parameters.Add("@balance", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                    parameters.Add("@withdrawalstatusid", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@errormessage", dbType: DbType.String, size: 1000, direction: ParameterDirection.Output);

                    db.Execute("\"Withdraw\"", parameters, commandType: CommandType.StoredProcedure);

                    var status = (WithdrawalStatus)parameters.Get<int>("withdrawalstatusid");

                    if (status== WithdrawalStatus.FailedInternalError)
                    {
                        string errorMessage = parameters.Get<string>("errormessage");
                        _loggingService.Error($"Withdraw => Withdraw(Store Proc) Error calling Store Procedure {errorMessage}");
                    }

                    var result = new WithdrawalResult
                    {
                        Balance = parameters.Get<decimal>("balance"),
                        Status = status
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, $"Failed to withdraw {amount} from {accountId}", accountId, amount);
                return new WithdrawalResult() { Status = WithdrawalStatus.FailedInternalError };
            }
        }

        public async Task<Response<string?>> InsertTransactionHistoryFromWithdrawOutboxAsync()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@result_message", dbType: DbType.String, size: 1000, direction: ParameterDirection.Output);

                using (IDbConnection db = _connectionFactory.CreateSanlamDbConnection())
                {
                    await db.ExecuteAsync("\"InsertTransactionHistoryFromWithdrawOutbox\"", parameters, commandType: CommandType.StoredProcedure);                    
                }

                var error = parameters.Get<string>("result_message");

                if (error != null)
                {
                    _loggingService.Error($"Error processing Withdraw Outbox {error}");
                    return new Response<string?>(error,Status.FailedInternalError);
                }

                return new Response<string?>(error, Status.Success);
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, "Error running InsertTransactionHistoryFromWithdrawOutboxAsync");
                return new Response<string?>(ex.Message, Status.FailedInternalError);
            }
        }
    }
}