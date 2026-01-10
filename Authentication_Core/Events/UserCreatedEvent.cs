//using Authentication_Core.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Authentication_Core.Events
//{
//    public record UserCreatedEvent : IBaseEvent
//    {
//        public Guid EventId { get; } = Guid.NewGuid();
//        public DateTime CreatedAt { get; } = DateTime.UtcNow;
//        public int UserId { get; init; }
//        public string Name { get; init; }
//        public string Email { get; init; }
//    }
//}
