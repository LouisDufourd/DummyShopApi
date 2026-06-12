using Npgsql;
using System.Data;

namespace DummyShopApi.DAL
{
    public class Session : ISession
    {
        private EDBType edbType;
        public EDBType EDBType => edbType;

        public IDbConnection Connection { get; private set; }

        private IDbTransaction _transaction;
        public IDbTransaction Transaction { get => _transaction; set => _transaction = value; }

        public Session(String connectionString, EDBType eDBType)
        {
            edbType = eDBType;

            switch (edbType)
            {
                case EDBType.POSTGRESQL:
                    Connection = new NpgsqlConnection(connectionString);
                    break;
                default:
                    throw new NotImplementedException();
            }

            Connection.Open();
        }
    }
}
