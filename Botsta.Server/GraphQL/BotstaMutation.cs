using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage.Models;
using Botsta.Server.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Server.Middelware;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaMutation : ObjectGraphType
    {
        public BotstaMutation(IBotstaDbRepository dbContext, IIdentityService identityManager, ISessionController session)
        {
            Field<StringGraphType>("login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                    ),
                resolve: context =>
                {
                    var username = context.GetArgument<string>("username");
                    var password = context.GetArgument<string>("password");

                    return identityManager.LoginUser(username, password);
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
                    return identityManager.LoginUser(username, password);
                }
            );

            Field<StringGraphType>("loginBot",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "botName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "apiKey" }
                    ),
                resolve: context =>
                {
                    var botName = context.GetArgument<string>("botName");
                    var apiKey = context.GetArgument<string>("apiKey");
                    return identityManager.LoginBot(botName, apiKey);
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

            FieldAsync<ChatroomType>("newChatroom",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "userIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "botIds" }
                    ),
                description: "Create a new chatroom",
                resolve: async context =>
                {
                    var userIds = context.GetArgument<string[]>("userIds");

                    var botIds = context.GetArgument<string[]>("botIds");

                    var users = userIds.Select(u => dbContext.GetUserById(u));
                    var bots = botIds.Select(u => dbContext.GetBotById(u));

                    var chatroom = new Chatroom {
                        Id = Guid.NewGuid(),
                        Users = users,
                        Bots = bots
                    };

                    await dbContext.AddChatroomToDbAsync(chatroom);

                    return chatroom;
                }
            ).AuthorizeWith(PoliciesExtentions.User);


            FieldAsync<MessageType>("newMessage",
                description: "Post message to chatroom",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "chatroomId" },
                    new QueryArgument<StringGraphType> { Name = "message" }
                ),
                resolve: async context =>
                {
                    var chatroomId = context.GetArgument<string>("chatroomId");
                    var messageJson = context.GetArgument<string>("message");

                    var newMessage = new Message
                    {
                        Id = Guid.NewGuid(),
                        MessageJson = messageJson,
                        ChatroomId = Guid.Parse(chatroomId)
                    };

                    await dbContext.AddMessageToDb(newMessage);

                    return newMessage;
                }
            ).AuthorizeWith(PoliciesExtentions.User);
        }
    }
}
