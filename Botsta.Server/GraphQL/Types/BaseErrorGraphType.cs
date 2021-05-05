using System;
using System.Linq;
using Botsta.DataStorage.Entities;
using Botsta.Server.Dto;
using Botsta.Server.Middelware;
using GraphQL.Types;

namespace Botsta.Server.GraphQL.Types
{
    public class BaseErrorGraphType<T> : ObjectGraphType<T> where T: ErrorResponse
    {
        public BaseErrorGraphType()
        {
            Field(e => e.HasError);
            Field(e => e.ErrorCode, true);
            Field(e => e.ErrorMessage, true);
        }
    }
}
