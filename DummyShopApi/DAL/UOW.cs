using DummyShopApi.DAL.DAO.Interfaces;
using DummyShopApi.DAL.DAO.Postgrsql;

namespace DummyShopApi.DAL
{
    /// <summary>
    /// Represents a Unit Of Work used to manage data access objects and database transactions.
    /// </summary>
    public class UOW : IUOW
    {
        /// <summary>
        /// Gets the inventory data access object.
        /// </summary>
        public IInventoryDAO Inventory => CurrentDictionary.GetValueOrDefault(typeof(IInventoryDAO)) as IInventoryDAO;

        /// <summary>
        /// Gets the order data access object.
        /// </summary>
        public IOrderDAO Order => CurrentDictionary.GetValueOrDefault(typeof(IOrderDAO)) as IOrderDAO;

        /// <summary>
        /// Gets the user data access object.
        /// </summary>
        public IUserDAO User => CurrentDictionary.GetValueOrDefault(typeof(IUserDAO)) as IUserDAO;

        private readonly ISession _db;
        private readonly Dictionary<Type, object> CurrentDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UOW"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="eDBType">The type of database provider to use.</param>
        public UOW(string connectionString, EDbType eDBType)
        {
            _db = new Session(connectionString, eDBType);

            switch (_db.EDBType)
            {
                case EDbType.POSTGRESQL:
                    CurrentDictionary = new Dictionary<Type, object>
                    {
                        { typeof(IInventoryDAO), new InventoryDAOPostgres(_db) },
                        { typeof(IOrderDAO), new OrderDAOPostgres(_db) },
                        { typeof(IUserDAO), new UserDAOPostgres(_db) }
                    };
                    break;
            }
        }

        /// <summary>
        /// Starts a new database transaction.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown when a transaction is already active.
        /// </exception>
        public void BeginTransaction()
        {
            if (_db.Transaction is null)
                _db.Transaction = _db.Connection.BeginTransaction();
            else
                throw new Exception("Application transaction is already open");
        }

        /// <summary>
        /// Commits the current database transaction.
        /// </summary>
        public void Commit()
        {
            if (_db.Transaction is not null)
            {
                _db.Transaction.Commit();
                _db.Transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current database transaction.
        /// </summary>
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