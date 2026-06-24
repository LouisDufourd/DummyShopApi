namespace DummyShopApi.Domain.Exceptions
{
    public class NotFoundEntityException : Exception
    {
        public NotFoundEntityException()
        {
        }
        public NotFoundEntityException(string? message) : base(message)
        {
        }
    }
}
