using System;
using System.Collections.Generic;

namespace Botsta.DataStorage.Models
{
    public interface IBotstaDbRepository
    {
        IEnumerable<Message> GetMessages();
    }
}
