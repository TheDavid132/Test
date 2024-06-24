using Dapper;
using SanlamTest.Common.DTO;
using SanlamTest.Common.Enums;
using SanlamTest.Common.Interfaces;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Models;
using System.Data;

namespace SanlamTest.DAL.Interfaces
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public AccountRepository(IConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }
        public Response<Account> GetById(Int64 accountId)
        {
            try
            {
                using (IDbConnection db = _connectionFactory.CreateSanlamDbConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("accountId", accountId, DbType.Int64);                   

                    Account? account = db.QueryFirstOrDefault<Account>("Select \"Id\",\"Enabled\" from \"Account\"", commandType: CommandType.Text);                    

                    return new Response<Account>(account);
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex, $"Failed to retrieve account details for {accountId}", accountId);
                return new Response<Account>(Status.FailedInternalError);
            }
        }
    }
}