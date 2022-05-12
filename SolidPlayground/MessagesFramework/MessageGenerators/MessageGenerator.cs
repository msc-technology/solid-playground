using AutoFixture;
using System.Text.Json;

namespace MessagesFramework.MessageGenerators
{
    internal class MessageGenerator<T> : IMessageGenerator
    {
        private readonly Fixture Fixture;

        public MessageGenerator()
        {
            var range = Range.EndAt(99999999);
            
            Fixture = new Fixture();
            Fixture.Customizations.Add(
                new StringGenerator(() => Guid.NewGuid().ToString().Substring(8, 3))
            );
            Fixture.Customizations.Add(
                new RandomNumericSequenceGenerator(1, 9999999)
            );
        }

        public Message Generate()
        {
            return ToMessage(Fixture.Create<T>());
        }

        protected Message ToMessage(T obj)
        {
            return new Message(JsonSerializer.Serialize(obj));
        }
    }
}
