using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage;
using Botsta.DataStorage.Entities;
using Botsta.Server.Configuration;
using Botsta.Server.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Server.Middelware;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;

namespace Botsta.Server.GraphQL
{
    public class BotstaQuery : ObjectGraphType
    {
        public BotstaQuery(IBotstaDbRepository dbContext, ISessionController session)
        {
            //Field(
            //    name: "messages",
            //    type: typeof(ListGraphType<MessageType>),
            //    resolve: context => dbContext.GetMessages(),
            //    description: "Returns a list all messages"
            //).AuthorizeWith(PoliciesExtentions.User);

            this.AuthorizeWith(PoliciesExtentions.User);

            Field<GraphUserType>(
                "whoami",
                description: "Returns informations about the current user.",
                resolve: c => session.GetUser());

            Field<ListGraphType<GraphUserType>>(
                "allUsers",
                "Returns list of all registerd users",
                resolve: c => dbContext.GetAllUsers()
                );

            Field<ListGraphType<GraphChatroomType>>(
                "chatrooms",
                description: "Returns all chatrooms of current user.",
                resolve: c =>
                {
                    var user = session.GetUser();
                    return user.ChatPracticant.Chatrooms;
                });

            FieldAsync<GraphChatroomType>(
                "chatroom",
                arguments: new QueryArguments
                {
                    new QueryArgument<StringGraphType> {Name = "chatroomId"}
                },
                description: "Returns the chtroom behind the given id.",
                resolve: async c =>
                {
                    var chatroomId = c.GetArgument<Guid>("chatroomId");

                    var chatroom = await dbContext.GetChatroomByIdAsync(chatroomId);
                    return chatroom;
                });

        }
    }
}
