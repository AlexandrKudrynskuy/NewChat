using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ChatClient.Model;

namespace ChatClient
{
    public class ClientService
    {
        public event Action<Responce> SendNewUser;
        public event Action<Responce> GetAllUsers;
        public event Action<Responce> GetPublic;
        public event Action<Responce> GetPrivate;
        public event Action<Responce> SendConnected;
        public event Action<Responce> GetError;
        public event Action<Responce> Disconnected;
        public event Action<string> LogClient;
        private Socket clientSocket;
        public ClientService()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
      
        public async Task Connect(string ipAddress, int port)
        {
            await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            LogClient?.Invoke("Server run");

        }
        public async void StartReceive()
        {
            while (true)
            {
                try
                {
                    if (clientSocket.Connected)
                    {
                        //LogClient?.Invoke("clientSoket Receive");

                        var data = new byte[1024];
                        var bytes = await clientSocket.ReceiveAsync(data, SocketFlags.None);
                        var resp = Encoding.ASCII.GetString(data, 0, bytes);
                        var responce = JsonSerializer.Deserialize<Responce>(resp);
                        await ServerMessageCallBack(responce);
                    }
                }
                catch (Exception ex)
                {
                    LogClient?.Invoke(ex.StackTrace.ToString() + "////"+ ex.Data.ToString() );
                    return;
                }
            }
        }
        private async Task ServerMessageCallBack(Responce responce)
        {
            switch (responce.Type)
            {
                case ResponceType.Connected:
                    SendConnected?.Invoke(responce);
                    //LogClient?.Invoke($"resp={responce}");

                    break;
                //case ResponceType.NewUser:
                //    SendNewUser?.Invoke(responce);
                //    //LogClient?.Invoke($"resp={responce}");

                //    break;
                case ResponceType.GetAll:
                    GetAllUsers?.Invoke(responce);
                    //LogClient?.Invoke($"resp={responce}");

                    break;
                case ResponceType.GetPrivate:
                    GetPrivate?.Invoke(responce);
                    //LogClient?.Invoke($"resp={responce}");

                    break;
                case ResponceType.GetPublic:
                    GetPublic?.Invoke(responce);
                    //LogClient?.Invoke($"resp={responce}");
                    break;
              
                case ResponceType.Disconect:
                    Disconnected?.Invoke(responce);

                    break;
                default:
                    //LogClient?.Invoke($"resp=default");

                    break;
            }

        }
        public async Task Send(Message message)
        {
            var data = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(message));
            await clientSocket.SendAsync(data, SocketFlags.None);
            //LogClient?.Invoke($"mess={message.Content} {message.Type} {message.Recipient} {message.Sender}");

        }

        public void Stop()
        {
            if (clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}
