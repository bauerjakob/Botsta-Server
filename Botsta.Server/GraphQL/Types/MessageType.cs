﻿using System;
using Botsta.DataStorage.Models;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class MessageType : ObjectGraphType<Message>
    {
        public MessageType()
        {
            Field(x => x.MessageId);
            Field(x => x.MessageJson);
        }
    }
}