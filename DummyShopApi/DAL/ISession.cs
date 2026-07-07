using System.Data;

namespace DummyShopApi.DAL
{
    public interface ISession
    {
        EDbType EDBType { get; }
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; set; }
    }
}
