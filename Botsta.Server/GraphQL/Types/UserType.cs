using System;
using Botsta.DataStorage.Models;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field("userId", u => u.Id.ToString());
            Field(u => u.Username);
        }
    }
}
