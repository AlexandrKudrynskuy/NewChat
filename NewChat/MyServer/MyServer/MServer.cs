using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyServer.Model;

namespace MyServer
{
    public class MServer
    {
        private Socket serverSocket;
        private List<User> Users;

        public event Action<string> ServerLog;
        public event Action<Responce, User> NewUser;
        public event Action<Responce, User> GetAllUser;
        public event Action<Responce, User> SendAll;
        public event Action<Responce, User> SendPrivate;
        public event Action<Responce, User> ConnectedUser;
        public event Action<Responce, User> Disconected;

        public List<User> GetUsers() => Users;

        public MServer()
        {
            Users = new List<User>();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async void StartAsync(string IpAdress, int port)
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IpAdress), port));
            ServerLog?.Invoke("serverSocket Bind");
            serverSocket.Listen(port);
            ServerLog?.Invoke("serverSocket listen");
            while (true)
            {
                var clientSoket = await serverSocket.AcceptAsync();
                var user = new User { clSocket = clientSoket };
                Users.Add(user);
                Task.Run(() =>
                {
                    ReceiveClientAsync(user);
                });
            }
        }

        public async Task ReceiveClientAsync(User user)
        {
            while (true)
            {
                var data = new byte[1024];
                var bytes = await user.clSocket.ReceiveAsync(data, SocketFlags.None);
                var message = JsonSerializer.Deserialize<Message>(Encoding.ASCII.GetString(data, 0, bytes));
                await CallbackAsync(message, user);
            }
        }
        private async Task CallbackAsync(Message? message, User user)
        {
            switch (message.Type)
            {
                case MessageType.NewUser:
                    var curentUser = Users.First(x => x.clSocket == user.clSocket);
                    curentUser.Name = message.Content;
                    curentUser.PhotoPath = message.PhotoPathSender;
                    ServerLog?.Invoke($"{user.Name} -> Connect");
                    ConnectedUser?.Invoke(new Responce { Content = "Connected", Sender = curentUser.Name, IdSender = message.IdSender, PhotoPathSender = message.PhotoPathSender, Type = ResponceType.Connected }, user);
                  //ConnectedUser?.Invoke(new Responce { Content = "Connected", Sender = curentUser.Name, IdSender = message.IdSender, PhotoPathSender = message.PhotoPathSender, Type = ResponceType.Connected }, user);
                    GetAllUser?.Invoke(new Responce { Sender = curentUser.Name, IdSender = message.IdSender, PhotoPathSender = message.PhotoPathSender, Type = ResponceType.GetAll }, user);
                  //GetAllUser?.Invoke(new Responce { Sender = curentUser.Name, IdSender = message.IdSender, PhotoPathSender = message.PhotoPathSender, Type = ResponceType.GetAll }, user);
                    //break;
                    break;


                case MessageType.PublicMessage:

                    SendAll?.Invoke(new Responce { Sender = user.Name, IdSender = user.Id.ToString(), PhotoPathSender = user.PhotoPath, Type = ResponceType.GetPublic, Content = message.Content }, user);
                    break;
                case MessageType.PrivateMessage:
                    //recipient in sender
                    curentUser = Users.First(x => x.Name == message.Recipient);

                    SendPrivate?.Invoke(new Responce { Sender = message.Recipient, IdSender = user.Id.ToString(), PhotoPathSender = user.PhotoPath, Type = ResponceType.GetPrivate, Content = message.Content }, curentUser);
                    break;
                case MessageType.Disconect:
                    Disconected?.Invoke(new Responce { Sender = user.Name, IdSender = user.Id.ToString(), PhotoPathSender = user.PhotoPath, Type = ResponceType.Disconect, Content = message.Content }, user);
                    ServerLog?.Invoke($"{user.Name} -> disconnect");
                    Users.Remove(user);
                    break;
                //case MessageType.GetAll: 
                //    break;
                //case MessageType.Error:
                //    break;
                default:
                    break;
            }
        }

        public async Task SendAsync(Responce? responce, User user)
        {
            var data = Encoding.ASCII.GetBytes(JsonSerializer.Serialize<Responce>(responce));
            await user.clSocket.SendAsync(data, SocketFlags.None);
        }
    }
}
