using Newtonsoft.Json;
using SanlamTest.DAL.Factories;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Models;
using System.Collections.Concurrent;

namespace SanlamTest.DAL.Messaging
{
    public class WithdrawalInMemoryMessageHandler : IMessageHandler
    {
        private readonly ConcurrentQueue<string> messages;
        private object consoleLock=new object();

        public WithdrawalInMemoryMessageHandler(IMessageServiceFactory<ConcurrentQueue<string>> inMemoryMessageServiceFactory)
        {
            messages = inMemoryMessageServiceFactory.Create();
        }

        public async Task PublishWithdrawalEventAsync(WithdrawalEvent withdrawalEvent)
        {
            string json = JsonConvert.SerializeObject(withdrawalEvent);
            lock (consoleLock)
            {
                Console.WriteLine(json);
            }

            messages.Enqueue(json);
        }
    }
}
