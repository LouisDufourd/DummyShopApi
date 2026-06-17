using DummyShopApi.DAL.DAO;
using DummyShopApi.DAL.DAO.Postgrsql;

namespace DummyShopApi.DAL
{
    public class UOW : IUOW
    {
        public IInventoryDAO Inventory => CurrentDictionary.GetValueOrDefault(typeof(IInventoryDAO)) as IInventoryDAO;
        public IOrderDAO Order => CurrentDictionary.GetValueOrDefault(typeof(IOrderDAO)) as IOrderDAO;

        private readonly ISession _session;
        private readonly Dictionary<Type, object> CurrentDictionary;

        public UOW(String connectionString, EDBType eDBType)
        {
            _session = new Session(connectionString, eDBType);
            switch(_session.EDBType)
            {
                case EDBType.POSTGRESQL:
                    CurrentDictionary = new Dictionary<Type, object>
                    {
                        {typeof(IInventoryDAO), new InventoryDAOPostgres(_session)},
                        {typeof(IOrderDAO), new OrderDAOPostgres(_session)},
                    };
                    break;
            }
        }

        public void BeginTransaction()
        {
            if (_session.Transaction is null)
                _session.Transaction = _session.Connection.BeginTransaction();
            else
                throw new Exception("Application transaction is already open");
        }

        public void Commit()
        {
            _session.Transaction?.Commit();
        }

        public void RollBack()
        {
            _session.Transaction?.Rollback();
        }
    }
}
