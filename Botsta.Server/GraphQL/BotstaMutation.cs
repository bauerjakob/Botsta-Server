using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage;
using Botsta.DataStorage.Entities;
using Botsta.Core.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Core.Services;
using Botsta.Server.Services;
using GraphQL; 
using GraphQL.Authorization;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaMutation : ObjectGraphType
    {
        public BotstaMutation(IBotstaDbRepository repository, IIdentityService identityManager, ISessionController session, IChatNotifier notifier)
        {
            FieldAsync<LoginGraphType>("login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "secret" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "publicKey" }
                    ),
                resolve: async context =>
                {
                    var name = context.GetArgument<string>("name");
                    var secret = context.GetArgument<string>("secret");
                    var publicKey = context.GetArgument<string>("publicKey");

                    var response =  await identityManager.LoginAsync(name, secret, publicKey);
                    return response;
                }
            );

            FieldAsync<LoginGraphType>("register",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "publicKey" }
                    ),
                resolve: async context =>
                {
                    var username = context.GetArgument<string>("username");
                    var password = context.GetArgument<string>("password");
                    var publicKey = context.GetArgument<string>("publicKey");

                    await identityManager.RegisterUserAsync(username, password);
                    return await identityManager.LoginAsync(username, password, publicKey);
                }
            );

            FieldAsync<StringGraphType>("registerBot",
               description: "Register new bot",
               arguments: new QueryArguments(
                   new QueryArgument<StringGraphType> { Name = "botName" },
                   new QueryArgument<BooleanGraphType> { Name = "isPublic" }
                   ),
               resolve: async context => {
                   var botName = context.GetArgument<string>("botName");
                   var isPublic = context.GetArgument<bool>("isPublic");
                   var user = session.GetUser();

                   var (apiKey, bot) = await identityManager.RegisterBotAsync(botName, user, isPublic);

                   return apiKey;
               }
               ).AuthorizeWith(PoliciesExtentions.User);

            FieldAsync<RefreshTokenGraphType>("refreshToken",
                arguments: new QueryArguments(
                    //new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "refreshToken" }
                    ),
                resolve: async context =>
                {
                    //var refreshToken = context.GetArgument<string>("refreshToken");
                    var claims = session.GetClaims();
                    var response = await identityManager.RefreshTokenAsync(claims);
                    return response;
                }
            ).AuthorizeWith(PoliciesExtentions.RefreshToken);

            FieldAsync<GraphChatroomType>("newChatroomSingle",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "practicantId" }
                    ),
                description: "Create a new chatroom",
                resolve: async context =>
                {
                    var currentUser = session.GetUser();

                    var practicantId = context.GetArgument<string>("practicantId");


                    var practicant = await repository.GetChatPracticantAsync(Guid.Parse(practicantId));

                    var chatroom = new Chatroom
                    {
                        Id = Guid.NewGuid(),
                        ChatPracticants = new[] { currentUser.ChatPracticant, practicant },
                        Type = ChatroomType.Single
                    };

                    await repository.AddChatroomToDbAsync(chatroom);

                    return chatroom;
                }
            ).AuthorizeWith(PoliciesExtentions.User);

            FieldAsync<GraphChatroomType>("newChatroomGroup",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "name" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "practicantIds" }
                    ),
                description: "Create a new chatroom",
                resolve: async context =>
                {
                    var currentUser = session.GetUser();

                    string chatroomName = context.GetArgument<string>("name");

                    var practicantIds = context.GetArgument<string[]>("practicantIds")?
                    .ToList().ToHashSet();

                    practicantIds.Add(currentUser.Id.ToString());

                    var practicants = repository.GetChatPracticants(practicantIds.Select(id => Guid.Parse(id))).ToList();

                    var chatroom = new Chatroom
                    {
                        Id = Guid.NewGuid(),
                        ChatPracticants = practicants,
                        Type = ChatroomType.Group,
                        Name = chatroomName
                    };

                    await repository.AddChatroomToDbAsync(chatroom);

                    return chatroom;
                }
            ).AuthorizeWith(PoliciesExtentions.User);


            FieldAsync<GraphMessageType>("postMessage",
                description: "Post message to chatroom",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "chatroomId" },
                    new QueryArgument<StringGraphType> { Name = "message" },
                    new QueryArgument<StringGraphType> { Name = "receiverSessionId" }
                ),
                resolve: async context =>
                {
                    var chatroomId = context.GetArgument<string>("chatroomId");
                    var messageJson = context.GetArgument<string>("message");
                    var receiverSessionId = context.GetArgument<Guid>("receiverSessionId");

                    var chatPracticant = await session.GetChatPracticantAsync();
                    var sessionId = session.GetSessionId();
                    var publicKey = chatPracticant.KeyExchange.Single(k => k.SessionId == sessionId).PublicKey;

                    var newMessage = new Message
                    {
                        Id = Guid.NewGuid(),
                        Msg = messageJson,
                        ChatroomId = Guid.Parse(chatroomId),
                        SenderId = chatPracticant.Id,
                        SendTime = DateTimeOffset.UtcNow,
                        SenderPublicKey = publicKey,
                        ReceiverSessionId = receiverSessionId
                    };

                    await repository.AddMessageToDb(newMessage);

                    notifier.NotifyChat(newMessage);

                    return newMessage;
                }
            ).RequiresAuthorization();

            FieldAsync<StringGraphType>("deleteMessages",
                description: "Delete messages from server",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "messageIds" }
                    ),
                resolve: async context =>
                {
                    var sessionId = session.GetSessionId();
                    var messageIds = context.GetArgument<Guid[]>("messageIds");

                    await repository.DeleteMessagesAsync(messageIds, sessionId);

                    return "ok";
                }).RequiresAuthorization();

            FieldAsync<StringGraphType>("logout",
               description: "Logout",
               resolve: async context =>
               {
                   var sessionId = session.GetSessionId(); 

                   await repository.DeleteKeyExchangeAsync(sessionId);

                   return "ok";
               }).RequiresAuthorization();
        }
    }
}
