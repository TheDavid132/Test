using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanlamTest.BLL.Interfaces;
using SanlamTest.Common.Enums;
using SanlamTest.Common.Interfaces;
using SanlamTest.Security;

namespace SanlamTest.Controllers
{
    [ApiController]
    [Route("bank")]
    public class TransactionController:ControllerBase
    {
        private readonly ILoggingService _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILoggingService loggingService,ITransactionService transactionService)
        {
            _logger = loggingService;
            _transactionService = transactionService;
        }
        [Authorize]
        [HttpPost("withdraw")]
        public IActionResult Withdraw(decimal amount)
        {
            // Get accountId from JWT token
            try
            {
                var accountIdClaim = User.FindFirst(CustomClaimTypes.AccountID)?.Value;

                if (amount <= 0)
                    return BadRequest("Withdrawal amount cannot be 0 or less than 0");


                if (string.IsNullOrEmpty(accountIdClaim))
                    return Unauthorized();

                var accountId = Convert.ToInt64(accountIdClaim);
                var responseDto = _transactionService.Withdraw(accountId, amount);

                IActionResult apiResponse;
                switch (responseDto.Result)
                {
                    case WithdrawalStatus.FailedInsufficientBalance:
                        apiResponse= BadRequest(new { responseDto.Message });
                        break;
                    case WithdrawalStatus.FailedAccountDisabled:
                        apiResponse = BadRequest(new { responseDto.Message });
                        break;
                    case WithdrawalStatus.FailedInternalError:
                        apiResponse = StatusCode(500, new { responseDto.Message });
                        break;
                    case WithdrawalStatus.CompletedSuccessfully:
                        apiResponse = Ok(new { responseDto.Message, responseDto.Balance });
                        break;
                    case WithdrawalStatus.FailedAccountNotFound:
                        apiResponse = NotFound(new { responseDto.Message});
                        break;
                    default:
                        apiResponse = StatusCode(500, new { Message = "An unknown error occurred." });
                        break;
                }

                _logger.Information($"Response from Withdraw: {apiResponse}");

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in Transaction=>Withdraw");
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
