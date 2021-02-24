using System;
using System.Collections.Generic;
using Botsta.DataStorage.Models;
using Botsta.Server.GraphQL.Types;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaQuery : ObjectGraphType
    {
        public BotstaQuery(IBotstaDbRepository dbContext)
        {
            Field(
                name: "messages",
                type: typeof(ListGraphType<MessageType>),
                resolve: context => dbContext.GetMessages()
           );
        }
    }
}
