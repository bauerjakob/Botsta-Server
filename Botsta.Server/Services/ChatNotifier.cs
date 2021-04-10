using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Botsta.DataStorage.Entities;

namespace Botsta.Server.Services
{
    public class ChatNotifier : IChatNotifier
    {
        private readonly ISubject<Message> _messageStream = new Subject<Message>();

        public Message NotifyChat(Message message)
        {
            _messageStream.OnNext(message);
            return message;
        }
        
        public IObservable<Message> Messages()
        {
            return _messageStream.AsObservable();
        }
    }
}
