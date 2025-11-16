using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketChat
{
    public class Message
    {
        public string EventName { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; }
        public string Room { get; set; }
        public string Status { get; set; }  
        public bool Connected { get; set; }   

    }
}
