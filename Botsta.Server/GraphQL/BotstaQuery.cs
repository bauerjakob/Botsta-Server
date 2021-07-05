using System;
using System.Collections.Generic;
using System.Linq;
using Botsta.DataStorage;
using Botsta.DataStorage.Entities;
using Botsta.Core.Configuration;
using Botsta.Core.Extentions;
using Botsta.Server.GraphQL.Types;
using Botsta.Core.Services;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;

namespace Botsta.Server.GraphQL
{
    public class BotstaQuery : ObjectGraphType
    {
        public BotstaQuery(IBotstaDbRepository repository, ISessionController session)
        {
            Field<ChatPracticantGraphType>(
                "whoami",
                description: "Returns informations about the current user.",
                resolve: c => session.GetChatPracticantAsync())
                .RequiresAuthorization();

            Field<ListGraphType<GraphUserType>>(
                "allUsers",
                "Returns list of all registerd users (only users not bots)",
                resolve: c => repository.GetAllUsers()
                ).AuthorizeWith(PoliciesExtentions.User);

            Field<ListGraphType<ChatPracticantGraphType>>(
                "allChatPracticants",
                "Returns list of all registerd chat practicants (users and bots)",
                resolve: c =>
                {
                    var user = session.GetUser();
                    var allPracticants = repository.GetAllChatPracticants().ToList();
                    var ret = new List<ChatPracticant>();
                    foreach (var item in allPracticants)
                    {
                        if (item.Type != PracticantType.Bot)
                        {
                            ret.Add(item);
                            continue;
                        }

                        var bot = repository.GetBotById(item.Id.ToString());
                        if (bot.IsPublic || bot.OwnerId == user.Id)
                        {
                            ret.Add(item);
                            continue;
                        }
                    }
                    return ret;
                }
                ).AuthorizeWith(PoliciesExtentions.User);

            Field<ListGraphType<GraphChatroomType>>(
                "chatrooms",
                description: "Returns all chatrooms of current user.",
                resolve: c =>
                {
                    var user = session.GetUser();
                    return user.ChatPracticant.Chatrooms;
                }).AuthorizeWith(PoliciesExtentions.User);

            FieldAsync<GraphChatroomType>(
                "chatroom",
                arguments: new QueryArguments
                {
                    new QueryArgument<StringGraphType> {Name = "chatroomId"}
                },
                description: "Returns the chtroom behind the given id.",
                resolve: async c =>
                {
                    var chatroomId = c.GetArgument<Guid>("chatroomId");

                    var chatroom = await repository.GetChatroomByIdAsync(chatroomId);
                    return chatroom;
                }).AuthorizeWith(PoliciesExtentions.User);

            Field<ListGraphType<BotGraphType>>(
                "getOwnBots",
                description: "Returns all bots where current user is owner",
                resolve: c =>
                {
                    var user = session.GetUser();
                    var bots = repository.GetBots(user.Id);
                    return bots;
                }).AuthorizeWith(PoliciesExtentions.User);

            FieldAsync<ListGraphType<ChatPracticantGraphType>>(
                "getChatPracticantsOfChatroom",
                description: "Returns a list of chat practicants of a chatroom",
                arguments: new QueryArguments
                {
                    new QueryArgument<StringGraphType> {Name = "chatroomId"}
                },
                resolve: async c =>
                {
                    var chatPracticant = await session.GetChatPracticantAsync();

                    var chatroomId = c.GetArgument<Guid>("chatroomId");

                    var chatroom =  await repository.GetChatroomByIdAsync(chatroomId);

                    if (chatroom.ChatPracticants.Select(c => c.Id).Contains(chatPracticant.Id))
                    {
                        return chatroom.ChatPracticants;
                    }

                    return null;
                    
                })
                .RequiresAuthorization();
        }
    }
}
