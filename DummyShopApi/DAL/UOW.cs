using DummyShopApi.DAL.DAO;
using DummyShopApi.DAL.DAO.Postgrsql;

namespace DummyShopApi.DAL
{
    public class UOW : IUOW
    {
        public IInventoryDAO Inventory => CurrentDictionary.GetValueOrDefault(typeof(IInventoryDAO)) as IInventoryDAO;
        public IOrderDAO Order => CurrentDictionary.GetValueOrDefault(typeof(IOrderDAO)) as IOrderDAO;

        private readonly ISession _db;
        private readonly Dictionary<Type, object> CurrentDictionary;

        public UOW(String connectionString, EDBType eDBType)
        {
            _db = new Session(connectionString, eDBType);
            switch(_db.EDBType)
            {
                case EDBType.POSTGRESQL:
                    CurrentDictionary = new Dictionary<Type, object>
                    {
                        {typeof(IInventoryDAO), new InventoryDAOPostgres(_db)},
                        {typeof(IOrderDAO), new OrderDAOPostgres(_db)},
                    };
                    break;
            }
        }

        public void BeginTransaction()
        {
            if (_db.Transaction is null)
                _db.Transaction = _db.Connection.BeginTransaction();
            else
                throw new Exception("Application transaction is already open");
        }

        public void Commit()
        {
            if(_db.Transaction is not null)
            {
                _db.Transaction.Commit();
                _db.Transaction = null;
            }
        }

        public void RollBack()
        {
            if (_db.Transaction is not null)
            {
                _db.Transaction.Rollback();
                _db.Transaction = null;
            }
        }
    }
}
