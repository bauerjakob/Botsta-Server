using System;
using Botsta.Server.Dto;

namespace Botsta.Server.GraphQL.Types
{
    public class RefreshTokenGraphType: BaseErrorGraphType<RefreshTokenResponse>
    {
        public RefreshTokenGraphType()
        {
            Field(l => l.Token, true);
        }
    }
}
