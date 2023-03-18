using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        string PhotoPath { get; set; }
        public Socket clSocket { get; set; }

    }

}
