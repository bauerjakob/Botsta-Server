﻿using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage;
using Botsta.DataStorage.Entities;
using Botsta.Server.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Server.Middelware;
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
            FieldAsync<StringGraphType>("login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "secret" }
                    ),
                resolve: async context =>
                {
                    var name = context.GetArgument<string>("name");
                    var secret = context.GetArgument<string>("secret");

                    return await identityManager.LoginAsync(name, secret);
                }
            );

            FieldAsync<StringGraphType>("register",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                    ),
                resolve: async context =>
                {
                    var username = context.GetArgument<string>("username");
                    var password = context.GetArgument<string>("password");

                    await identityManager.RegisterUserAsync(username, password);
                    return await identityManager.LoginAsync(username, password);
                }
            );

            FieldAsync<StringGraphType>("registerBot",
               description: "Register new bot",
               arguments: new QueryArguments(
                   new QueryArgument<StringGraphType> { Name = "botName" }
                   ),
               resolve: async context => {
                   var botName = context.GetArgument<string>("botName");
                   var user = session.GetUser();
                   var (apiKey, bot) = await identityManager.RegisterBotAsync(botName, user);

                   return apiKey;
               }
               ).AuthorizeWith(PoliciesExtentions.User);

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
                    new QueryArgument<StringGraphType> { Name = "message" }
                ),
                resolve: async context =>
                {
                    var chatroomId = context.GetArgument<string>("chatroomId");
                    var messageJson = context.GetArgument<string>("message");

                    var user = session.GetUser();

                    var newMessage = new Message
                    {
                        Id = Guid.NewGuid(),
                        Msg = messageJson,
                        ChatroomId = Guid.Parse(chatroomId),
                        SenderId = user.Id,
                        SendTime = DateTimeOffset.UtcNow
                    };

                    await repository.AddMessageToDb(newMessage);

                    notifier.NotifyChat(newMessage);

                    return newMessage;
                }
            ).AuthorizeWith(PoliciesExtentions.User);
        }
    }
}
