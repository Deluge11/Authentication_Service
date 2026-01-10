using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstantsLib.Interfaces;

namespace Authentication_Core.Interfaces
{
    public interface IEventBus
    {
        Task Publish<T>(T message, string exName, string routingKey) where T : IBaseEvent;
    }
}
