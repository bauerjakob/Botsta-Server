using System;
using Botsta.DataStorage.Entities;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class KeyExchangeGraphType : ObjectGraphType<KeyExchange>
    {
        public KeyExchangeGraphType()
        {
            Field("publicKey", i => i.PublicKey);
            Field("chatPracticantId", i => i.ChatPracticantId);
            Field("sessionId", i => i.SessionId);
        }
    }
}
