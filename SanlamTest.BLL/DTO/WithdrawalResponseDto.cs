using SanlamTest.Common.Enums;
using SanlamTest.DAL.Models;

namespace SanlamTest.BLL.DTO
{
    public class WithdrawalResponseDto
    {
        private static readonly Dictionary<WithdrawalStatus, string> WithdrawalStatusMessage=new Dictionary<WithdrawalStatus, string>()
        {
            {WithdrawalStatus.CompletedSuccessfully,"Completed" },
            {WithdrawalStatus.FailedInsufficientBalance,"Insufficient Balance" },
            {WithdrawalStatus.FailedAccountDisabled,"Account Disabled" },
            {WithdrawalStatus.FailedInternalError,"The system is unavailable please try again later" },
            {WithdrawalStatus.FailedAccountNotFound,"Account not found" }            
        };

        public WithdrawalResponseDto(WithdrawalResult withdrawResult)
        {            
            Result = withdrawResult.Status;
            Balance = withdrawResult.Balance;
        }

        public WithdrawalResponseDto(WithdrawalStatus withdrawalStatus,decimal? balance)
        {
            Result = withdrawalStatus;
            Balance = balance;
        }

        public WithdrawalResponseDto(WithdrawalStatus withdrawalStatus)
        {
            Result = withdrawalStatus;            
        }

        public WithdrawalStatus Result { get; init; }
        public string? Message 
        { 
            get 
            {
                return WithdrawalStatusMessage[Result]; 
            } 
        }
        public decimal? Balance { get; init; }
    }
}