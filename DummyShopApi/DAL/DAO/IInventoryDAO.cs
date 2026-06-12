using DummyShopApi.DAL.Entities;

namespace DummyShopApi.DAL.DAO
{
    public interface IInventoryDAO: IGenericReadRepository<Product, int>
    {
    }
}
