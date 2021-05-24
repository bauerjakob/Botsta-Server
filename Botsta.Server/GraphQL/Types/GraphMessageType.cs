using System;
using Botsta.DataStorage.Entities;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class GraphMessageType : ObjectGraphType<Message>
    {
        public GraphMessageType()
        {
            Field(x => x.Id);
            Field("message", x => x.Msg);
            Field("sendTime", x => x.SendTime);
            Field("chatroomId", x => x.ChatroomId);
            Field<ChatPracticantGraphType>("sender", resolve: x => x.Source.Sender);
        }
    }
}
