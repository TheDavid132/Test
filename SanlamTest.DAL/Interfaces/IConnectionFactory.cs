using System.Data;

namespace SanlamTest.DAL.Interfaces
{
    public interface IConnectionFactory
    {
        IDbConnection CreateSanlamDbConnection();
    }
}
