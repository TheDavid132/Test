using Microsoft.Extensions.Configuration;

namespace SanlamTest.DAL.Interfaces
{
    public interface IMessageServiceFactory<out T>
    {
        T Create();
    }
}
