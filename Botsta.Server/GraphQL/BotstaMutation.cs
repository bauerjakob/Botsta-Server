using System;
using System.Linq;
using Botsta.DataStorage.Models;
using Botsta.Server.Middelware;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaMutation : ObjectGraphType
    {
        public BotstaMutation(IBotstaDbRepository dbContext, IIdentityService identityManager)
        {
            Field<StringGraphType>("login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                    ),
                resolve: context =>
                {
                    var username = context.Arguments.First(a => a.Key == "username");
                    var password = context.Arguments.First(a => a.Key == "password");

                    var token = identityManager.Login(username.Value.ToString(), password.Value.ToString());

                    return token;
                }
            );

            FieldAsync<StringGraphType>("register",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                    ),
                resolve: async context =>
                {
                    var username = context.Arguments.First(a => a.Key == "username").Value.ToString();
                    var password = context.Arguments.First(a => a.Key == "password").Value.ToString();
                    await identityManager.RegisterAsync(username, password);
                    return identityManager.Login(username, password);
                }
            );
        }
    }
}
