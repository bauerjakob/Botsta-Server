using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage.Models;
using Botsta.Server.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Server.Middelware;
using GraphQL.Authorization;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaMutation : ObjectGraphType
    {
        public BotstaMutation(IBotstaDbRepository dbContext, IIdentityService identityManager)
        {
            Field<StringGraphType>("login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                    ),
                resolve: context =>
                {
                    var username = context.Arguments.First(a => a.Key == "username");
                    var password = context.Arguments.First(a => a.Key == "password");

                    var token = identityManager.Login(username.Value.ToString(), password.Value.ToString());

                    return token;
                }
            );

            FieldAsync<StringGraphType>("register",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                    ),
                resolve: async context =>
                {
                    var username = context.Arguments.First(a => a.Key == "username").Value.ToString();
                    var password = context.Arguments.First(a => a.Key == "password").Value.ToString();
                    await identityManager.RegisterAsync(username, password);
                    return identityManager.Login(username, password);
                }
            );

            FieldAsync<ChatroomType>("newChatroom",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "userIds" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "botIds" }
                    ),
                description: "Create a new chatroom",
                resolve: async context =>
                {
                    var userIds = context.Arguments
                        .First(a => a.Key == "userIds").Value as List<string>;

                    var botIds = context.Arguments
                        .First(a => a.Key == "botIds").Value as List<string>;

                    var users = userIds.Select(u => dbContext.GetUserById(u));
                    var bots = botIds.Select(u => dbContext.GetBotById(u));

                    var chatroom = new Chatroom {
                        ChatroomId = Guid.NewGuid(),
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
                    var chatroomId = context.Arguments.Single(a => a.Key == "chatroomId").Value.ToString();
                    var messageJson = context.Arguments.Single(a => a.Key == "message").Value.ToString();

                    var newMessage = new Message
                    {
                        MessageId = Guid.NewGuid(),
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
