using DummyShopApi.DAL.DAO.Interfaces;

namespace DummyShopApi.DAL
{
    public interface IUOW
    {

        public IInventoryDAO Inventory { get; }
        public IOrderDAO Order { get; }
        public IUserDAO User { get; }


        /// <summary>
        /// Begin application transaction
        /// </summary>
        public void BeginTransaction();

        /// <summary>
        /// RollBack persitance context
        /// </summary>
        public void RollBack();

        /// <summary>
        /// Save persitance context
        /// </summary>
        public void Commit();
    }
}
