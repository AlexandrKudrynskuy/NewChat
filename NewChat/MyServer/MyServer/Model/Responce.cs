using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Model
{
    public enum ResponceType
    {
        Error,
        NewUser,
        GetAll,
        GetPublic,
        GetPrivate,
        Connected,
        Disconect

    }
    public class Responce
    {
        public string Content { get; set; }
        public ResponceType Type { get; set; }
        public string IdSender { get; set; }
        public string PhotoPathSender { get; set; }
        public string Sender { get; set; }
    }
}
