using System;
using System.Linq;
using Botsta.DataStorage.Models;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class ChatroomType : ObjectGraphType<Chatroom>
    {
        public ChatroomType()
        {
            Field(c => c.ChatroomId);
            Field(c => c.Users);
            Field(c => c.Messages);
        }
    }
}
