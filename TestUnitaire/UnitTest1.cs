namespace TestUnitaire
{
    public class UnitTest1
    {
        [Fact]
        public void Random()
        {
            Random random = new Random();
            var rdn = random.NextDouble();

            Assert.True(rdn > .5);
        }
    }
}