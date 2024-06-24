using Microsoft.Extensions.Configuration;
using SanlamTest.DAL.Interfaces;
using System.Collections.Concurrent;

namespace SanlamTest.DAL.Factories
{
    public class InMemoryMessageServiceFactory : IMessageServiceFactory<ConcurrentQueue<string>>
    {
        public InMemoryMessageServiceFactory(IConfiguration configurationManager)
        {
            
        }
        public ConcurrentQueue<string> Create()
        {
            return new ConcurrentQueue<string>();
        }
    }
}
