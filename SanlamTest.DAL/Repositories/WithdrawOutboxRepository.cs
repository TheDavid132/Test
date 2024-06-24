using Dapper;
using SanlamTest.Common.DTO;
using SanlamTest.Common.Enums;
using SanlamTest.Common.Interfaces;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Models;
using System.Data;

namespace SanlamTest.DAL.Interfaces
{
    public class WithdrawOutboxRepository : IWithdrawOutboxRepository
    {
        private IConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public WithdrawOutboxRepository(IConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public Response<IEnumerable<WithdrawOutbox>> GetPendingMessagesSetStatusInProgress(int count)
        {
            try
            {
                string sql = @$"UPDATE ""WithdrawOutbox"" 
                            SET     ""MessageServiceStatusId"" = 1 , ""MessageServiceStatusChangeDate"" ='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                            WHERE   ""MessageServiceStatusId"" = 0 and 
                                    ""Id"" in (	
                                                select ""Id"" from ""WithdrawOutbox"" 
		                                        where ""MessageServiceStatusId"" = 0
                                                FOR UPDATE SKIP LOCKED
		                                        limit {count}
                                                )
                            RETURNING 	""Id"", 
				                        ""TransactionGuid"", 
				                        ""AccountId"", 
				                        ""Amount"", 
				                        ""CreatedDate"", 
				                        ""RunningBalance""
  				                        ""TransactionHistoryStatusId"",
  				                        ""MessageServiceStatusId"",
  				                        ""TransactionHistoryStatusChangeDate"",
  				                        ""MessageServiceStatusChangeDate""";

                using (IDbConnection db = _connectionFactory.CreateSanlamDbConnection())
                {
                    var withdrawOutbox = db.Query<WithdrawOutbox>(sql);
                    return new Response<IEnumerable<WithdrawOutbox>>(withdrawOutbox, Status.Success);
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, "Error in GetPendingMessagesSetStatusInProgress");
                return new Response<IEnumerable<WithdrawOutbox>>(Status.FailedInternalError);
            }
        }

        public Status ResetMessageServiceStatus(int WithdrawOutboxId)
        {
            try
            {
                string sql = @$"UPDATE ""WithdrawOutbox"" 
                            SET     ""MessageServiceStatusId"" = 0 
                            WHERE   ""Id"" = {WithdrawOutboxId}";

                using (IDbConnection db = _connectionFactory.CreateSanlamDbConnection())
                {
                    db.Execute(sql);
                    return Status.Success;
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, "Error in GetPendingMessagesSetStatusInProgress");
                return Status.FailedInternalError;
            }
        }
    }
}