namespace DummyShopApi.DAL.Entities
{
    public class OrderStatus: IEntity
    {
        public int orderStatusId { get; set; }
        public string label { get; set; }
    }
}
