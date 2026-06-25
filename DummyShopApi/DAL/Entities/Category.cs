namespace DummyShopApi.DAL.Entities
{
    public class Category: IEntity
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
