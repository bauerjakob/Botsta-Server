using System;
using Botsta.Server.Dto;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class LoginGraphType : BaseErrorGraphType<LoginResponse>
    {
        public LoginGraphType()
        {
            Field(l => l.RefreshToken, true);
            Field(l => l.Token, true);
        }
    }
}
