using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public Socket clSocket { get; set; }

        public User() 
        {
             Id= Guid.NewGuid();
        }
    }
}
