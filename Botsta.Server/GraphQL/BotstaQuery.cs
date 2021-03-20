using System;
using System.Collections.Generic;
using Botsta.DataStorage.Models;
using Botsta.Server.Configuration;
using Botsta.Server.Extentions;
using Botsta.Server.GraphQL.Types;
using GraphQL.Authorization;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaQuery : ObjectGraphType
    {
        public BotstaQuery(IBotstaDbRepository dbContext)
        {
            this.AuthorizeWith(PoliciesExtentions.User);
            Field(
                name: "messages",
                type: typeof(ListGraphType<MessageType>),
                resolve: context => dbContext.GetMessages(),
                description: "Returns a list all messages"
            );
        }
    }
}
