using DummyShopApi.DAL.Entities;
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
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                    dataSourceBuilder.MapEnum<EOrderProductStatus>("product_order_status");
                    var dataSource = dataSourceBuilder.Build();
                    Connection = dataSource.OpenConnection();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
