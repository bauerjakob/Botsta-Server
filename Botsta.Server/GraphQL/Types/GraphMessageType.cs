using System;
using Botsta.DataStorage.Entities;
using Botsta.Server.Middelware;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class GraphMessageType : ObjectGraphType<Message>
    {
        public GraphMessageType(ISessionController session)
        {
            Field(x => x.Id);
            Field("message", x => x.Msg);
            Field("senderId", x => x.SenderId);
            Field("sendTime", x => x.SendTime);
            Field("chatroomId", x => x.ChatroomId);
        }
    }
}
