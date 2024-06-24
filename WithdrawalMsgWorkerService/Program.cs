using SanlamTest.BLL.Interfaces;
using SanlamTest.BLL.Services;
using SanlamTest.Common.Interfaces;
using SanlamTest.Common.Services;
using SanlamTest.DAL.Factories;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Messaging;
using System.Collections.Concurrent;
using WithdrawalMsgWorkerService.Models;
using WithdrawalMsgWorkerService.Services;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WithdrawalMessageWorker>();


var appSettings = builder.Configuration.Get<AppSettings>();
builder.Services.AddSingleton(appSettings);

// Logging
builder.Services.AddSingleton<ILoggingService, SerilogLoggingService>();

// BLL services
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<ITransactionMessageService, TransactionMessageService>();

// DAL repositories
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IAccountBalanceRepository, AccountBalanceRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IWithdrawOutboxRepository, WithdrawOutboxRepository>();
builder.Services.AddSingleton<IMessageHandler, WithdrawalInMemoryMessageHandler>();

// Infrastructure
builder.Services.AddSingleton<IConnectionFactory, PostgreSqlConnectionFactory>();
builder.Services.AddSingleton<IMessageServiceFactory<ConcurrentQueue<string>>, InMemoryMessageServiceFactory>();

var host = builder.Build();
host.Run();