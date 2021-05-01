using System;
using Botsta.DataStorage.Entities;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class GraphUserType : ObjectGraphType<User>
    {
        public GraphUserType()
        {
            Field("id", u => u.Id.ToString());
            Field("username", u => u.ChatPracticant.Name);
        }
    }
}
