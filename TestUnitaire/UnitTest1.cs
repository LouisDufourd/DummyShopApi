namespace TestUnitaire
{
    public class UnitTest1
    {
        [Fact]
        public void Random()
        {
            Random random = new Random();
            var rdm = random.NextDouble();

            Assert.True(rdm > .5);
        }
    }
}