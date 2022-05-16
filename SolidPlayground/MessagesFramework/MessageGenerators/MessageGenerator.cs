using AutoFixture;
using System.Text.Json;

namespace MessagesFramework.MessageGenerators
{
    internal class MessageGenerator<T> : IMessageGenerator
    {
        private readonly Fixture fixture;

        public MessageGenerator()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(
                new StringGenerator(() => Guid.NewGuid().ToString().Substring(8, 3))
            );
            fixture.Customizations.Add(
                new RandomNumericSequenceGenerator(1, 99999999)
            );
        }

        public Message Generate()
        {
            return ToMessage(fixture.Create<T>());
        }

        protected Message ToMessage(T obj)
        {
            return new Message(JsonSerializer.Serialize(obj));
        }
    }
}
