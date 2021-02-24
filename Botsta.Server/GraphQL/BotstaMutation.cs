using System;
using Botsta.DataStorage.Models;
using GraphQL.Types;

namespace Botsta.Server.GraphQL
{
    public class BotstaMutation : ObjectGraphType
    {
        public BotstaMutation(IBotstaDbRepository dbContext)
        {
            
        }
    }
}
