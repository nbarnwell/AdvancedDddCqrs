using NUnit.Framework;

namespace ClassLibrary2
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Deserialisation_ShouldNotReturnNull()
        {
            var serialiser = new OrderSerialiser();
            var json = GetJson();
            var document = serialiser.Deserialise<WaiterOrderView>(json);

            Assert.That(document, Is.Not.Null);
        }

        [Test]
        public void Deserialisation_ShouldReadPropertyValues()
        {
            var serialiser = new OrderSerialiser();
            var json = GetJson();
            var document = serialiser.Deserialise<WaiterOrderView>(json);

            Assert.That(document, Is.Not.Null);
            Assert.That(document.ServerId, Is.EqualTo("Neil"));
        }

        [Test]
        public void Serialisation_ShouldWriteAllValues()
        {
            var serialiser = new OrderSerialiser();
            var document = serialiser.Deserialise<WaiterOrderView>(GetJson());
            document.ServerId = "Neil";

            var json = serialiser.Serialise(document);

            Assert.That(json, Is.EqualTo(GetJson()));
        }

        [Test]
        public void Serialisation_ShouldSupportLists()
        {
            var serialiser = new OrderSerialiser();
            var document = serialiser.Deserialise<WaiterOrderView>(GetJson());

        }

        private string GetJson()
        {
            return @"{
  ""ServerId"": ""Neil"",
  ""TableNumber"": 12,
  ""Items"": [
    {
      ""Name"": ""Lamb Chop"",
      ""Quantity"": 1,
      ""TaxRate"": 0.2,
      ""Price"": 2.99
    },
    {
      ""Name"": ""Fish and Chips"",
      ""Quantity"": 2,
      ""TaxRate"": 0.2,
      ""Price"": 1.99
    }
  ],
  ""Discount"": 0.0,
  ""Tip"": 0.5,
  ""Paid"": false
}";
        }
    }
}