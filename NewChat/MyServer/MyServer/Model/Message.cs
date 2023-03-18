using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Model
{
    public enum MessageType
    {
        Error,
        NewUser,
        Connected,
        PublicMessage,
        PrivateMessage,
        GetAll,
        Disconect
    }
    public class Message
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public string IdSender { get; set; }
        public string PhotoPathSender { get; set; }
        public MessageType Type { get; set; }
        public string Recipient { get; set; }
    }

}
