using System.Data;

namespace DummyShopApi.DAL
{
    public interface ISession
    {
        EDBType EDBType { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }
    }
}
