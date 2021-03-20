using System;
using Botsta.DataStorage.Models;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field("userId", u => u.UserId.ToString());
            Field(u => u.Username);
        }
    }
}
