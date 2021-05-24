using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage;
using Botsta.DataStorage.Entities;
using Botsta.Core.Configuration;
using Botsta.Core.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Core.Services;
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
            Field<ChatPracticantGraphType>(
                "whoami",
                description: "Returns informations about the current user.",
                resolve: c => session.GetChatPracticantAsync())
                .RequiresAuthorization();

            Field<ListGraphType<GraphUserType>>(
                "allUsers",
                "Returns list of all registerd users (only users not bots)",
                resolve: c => dbContext.GetAllUsers()
                ).AuthorizeWith(PoliciesExtentions.User);

            Field<ListGraphType<ChatPracticantGraphType>>(
                "allChatPracticants",
                "Returns list of all registerd chat practicants (users and bots)",
                resolve: c => dbContext.GetAllChatPracticants()
                ).AuthorizeWith(PoliciesExtentions.User);

            Field<ListGraphType<GraphChatroomType>>(
                "chatrooms",
                description: "Returns all chatrooms of current user.",
                resolve: c =>
                {
                    var user = session.GetUser();
                    return user.ChatPracticant.Chatrooms;
                }).AuthorizeWith(PoliciesExtentions.User);

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
                }).AuthorizeWith(PoliciesExtentions.User);

        }
    }
}
