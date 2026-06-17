namespace DummyShopApi.DAL.Entities
{
    public class Order: IEntity
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
