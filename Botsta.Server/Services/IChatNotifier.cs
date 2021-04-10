using System;
using Botsta.DataStorage.Entities;

namespace Botsta.Server.Services
{
    public interface IChatNotifier
    {
        public Message NotifyChat(Message message);

        public IObservable<Message> Messages();
    }
}
