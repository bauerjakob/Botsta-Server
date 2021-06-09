using System;
using Botsta.DataStorage.Entities;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class BotGraphType : ObjectGraphType<Bot>
    {
        public BotGraphType()
        {
            Field(b => b.Id);
            Field("name", b => b.ChatPracticant.Name);
            Field(b => b.OwnerId);
            Field(b => b.IsPublic);
        }
    }
}
