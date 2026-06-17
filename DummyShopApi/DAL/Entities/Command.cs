namespace DummyShopApi.DAL.Entities
{
    public class Command: IEntity
    {
        public int Id { get; set; }
        public EStatus status { get; set; }
        public Dictionary<Product, bool> products;
    }
}
