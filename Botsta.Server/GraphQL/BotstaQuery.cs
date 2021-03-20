using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage.Models;
using Botsta.Server.Configuration;
using Botsta.Server.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Server.Middelware;
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

            Field<UserType>(
                "whoami",
                description: "Returns informations to the current user.",
                resolve: c => session.GetUser());

            Field<ListGraphType<UserType>>(
                "getAllUsers",
                "Returns list of all registerd users",
                resolve: c => dbContext.GetAllUsers()
                );

            Field<ListGraphType<StringGraphType>>(
                "chatrooms",
                description: "Returns all chatrooms of current user.",
                resolve: c =>
                {
                    var user = session.GetUser();
                    return user.Chatrooms.Select(u => u.ChatroomId.ToString());
                });

        }
    }
}
