namespace DummyShopApi.Domain.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException()
        {
        }

        public InvalidLoginException(string? message) : base(message)
        {
        }
    }
}
