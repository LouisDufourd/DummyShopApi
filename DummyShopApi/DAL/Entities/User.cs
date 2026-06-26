namespace DummyShopApi.DAL.Entities
{
    public class User: IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ERole role { get; set; }
    }
}
