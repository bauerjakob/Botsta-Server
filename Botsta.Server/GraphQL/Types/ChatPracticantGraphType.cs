using System;
using System.Linq;
using Botsta.DataStorage.Entities;
using Botsta.Server.Middelware;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class ChatPracticantGraphType : ObjectGraphType<ChatPracticant>
    {
        public ChatPracticantGraphType()
        {
            Field(c => c.Name);
            Field(c => c.Id);
            Field<StringGraphType>("type", resolve: c => c.Source.Type.ToString());
            Field("isUser", c => c.Type == PracticantType.User);
            Field("isBot", c => c.Type == PracticantType.Bot);
        }
    }
}
