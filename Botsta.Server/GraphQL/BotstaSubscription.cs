using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;
using Botsta.Server.GraphQL.Types;
using Botsta.Core.Services;
using Botsta.Server.Services;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;
using GraphQL.Language.AST;
using GraphQL.Authorization;
using Botsta.Core.Extentions;
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
                Type = typeof(GraphMessageType),
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "refreshToken" }
                    ),
                Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                Subscriber = new EventStreamResolver<Message>(Subscribe),
            });
        }

        private IObservable<Message> Subscribe(IResolveEventStreamContext context) {
            var refreshToken = context.GetArgument<string>("refreshToken");
            var messages = _notifier.Messages();
            var sessionId = _session.GetSessionId(refreshToken);

            return messages
                .Where(
                m => {
                    if (string.IsNullOrEmpty(m?.ChatroomId.ToString()) || m.ReceiverSessionId != sessionId)
                    {
                        return false;
                    }

                    var practicant = _session.GetChatPracticantFromToken(refreshToken).Result;
                    return practicant.Chatrooms.Select(c => c.Id).Contains(m.ChatroomId);
                });
        }

        private Message ResolveMessage(IResolveFieldContext context)
        {
            return context.Source as Message;
        }
    }
}
