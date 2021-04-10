using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;
using Botsta.Server.GraphQL.Types;
using Botsta.Server.Middelware;
using Botsta.Server.Services;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;
using GraphQL.Authorization;
using Botsta.Server.Extentions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Botsta.Server.GraphQL
{
    public class BotstaSubscription : ObjectGraphType
    {
        private readonly ISessionController _session;
        private readonly IChatNotifier _notifier;

        public BotstaSubscription(ISessionController session, IChatNotifier notifier)
        {
            _session = session;
            _notifier = notifier;

            AddField(new EventStreamFieldType
            {
                Name = "messageReceived",
                Type = typeof(MessageType),
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "token" }
                    ),
                Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                Subscriber = new EventStreamResolver<Message>(Subscribe)
            });
        }

        private IObservable<Message> Subscribe(IResolveEventStreamContext context) {
            var messages =  _notifier.Messages();
            var token = context.GetArgument<string>("token");
            var user = _session.GetUser(token);

            return messages
                .Where(m => !string.IsNullOrEmpty(m?.ChatroomId.ToString())
                    && user.Chatrooms.Select(c => c.Id).Contains(m.ChatroomId));
        }

        private Message ResolveMessage(IResolveFieldContext context)
        {
            return context.Source as Message;
        }
    }
}
