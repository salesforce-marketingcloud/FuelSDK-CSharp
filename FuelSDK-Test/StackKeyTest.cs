using NUnit.Framework;

namespace FuelSDK.Test
{
    [TestFixture]
    public class StackKeyTest
    {
        [Test]
        public void MultipleETClientInstancesForTheSameClientIdAndSecretWillHaveTheSameStackKey()
        {
            ETClient client1 = new ETClient();
            ETClient client2 = new ETClient();

            Assert.IsNotNull(client1.Stack);
            Assert.IsNotNull(client2.Stack);
            Assert.AreEqual(client1.Stack, client2.Stack);
        }
    }
}
