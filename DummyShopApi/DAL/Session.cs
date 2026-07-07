using DummyShopApi.DAL.Entities;
using Npgsql;
using System.Data;

namespace DummyShopApi.DAL
{
    public class Session : ISession
    {
        private EDbType edbType;
        public EDbType EDBType => edbType;

        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; set; }

        public Session(string connectionString, EDbType eDBType)
        {
            edbType = eDBType;

            switch (edbType)
            {
                case EDbType.POSTGRESQL:
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                    dataSourceBuilder.MapEnum<EOrderProductStatus>("product_order_status");
                    dataSourceBuilder.MapEnum<ERole>("roles");
                    var dataSource = dataSourceBuilder.Build();
                    Connection = dataSource.OpenConnection();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
