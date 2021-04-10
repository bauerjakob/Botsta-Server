using System;
using GraphQL.Types;
using GraphQL.Utilities;

namespace Botsta.Server.GraphQL
{
    public class BotstaSchema : Schema
    {
        public BotstaSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<BotstaQuery>();
            Mutation = provider.GetRequiredService<BotstaMutation>();
            Subscription = provider.GetRequiredService<BotstaSubscription>();
        }
    }
}
