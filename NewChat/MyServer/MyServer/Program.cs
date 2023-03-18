using System.Text.Json;
using MyServer;
using MyServer.Model;

var server = new MServer();
var listUsers = server.GetUsers();
server.ServerLog += Server_ServerLog;
server.SendAll += Server_SendAll;
server.NewUser += Server_NewUser;
server.GetAllUser += Server_GetAllUser;
server.ConnectedUser += Server_ConnectedUser;
server.Disconected += Server_Disconected;

void Server_Disconected(Responce arg1, User arg2)
{
    foreach (var user in listUsers)
    {
        if (user.Name != arg2.Name)
        {
            server.SendAsync(new Responce { Content = arg2.Name, Sender = arg2.Name, Type = ResponceType.Disconect }, user);
        }
    }
}

server.StartAsync("127.0.0.1", 2424);

Console.ReadLine();


void Server_ConnectedUser(Responce arg1, User arg2)
{
    foreach (var user in listUsers)
    {
        if (user.Name != arg1.Sender)
        {
            server.SendAsync(new Responce { Content = arg2.Name, IdSender=arg1.IdSender, PhotoPathSender=arg1.PhotoPathSender, Sender = arg1.Sender, Type = ResponceType.Connected }, user);
        }
    }
}


void Server_GetAllUser(Responce arg1, User arg2)
{
    foreach (var user in listUsers)
    {
        if (user.Name != arg1.Sender)
        {
            server.SendAsync(new Responce { Content = user.Name, Sender = user.Name, PhotoPathSender = user.PhotoPath, Type = ResponceType.GetAll }, arg2);
        }
    }
}

void Server_NewUser(Responce arg1, User arg2)
{
    foreach (var user in listUsers)
    {
        if (user.Name != arg1.Sender)
        {
            server.SendAsync(new Responce { Content = arg1.Content, Sender = arg1.Sender, PhotoPathSender = arg1.PhotoPathSender, Type = ResponceType.NewUser }, user);
        }
    }
}

void Server_SendAll(Responce arg1, User arg2)
{
    foreach (var user in listUsers)
    {  
            server.SendAsync(new Responce { Content = arg1.Content, Sender = arg1.Sender, PhotoPathSender = arg1.PhotoPathSender, Type = ResponceType.GetPublic },user);
    }
}

void Server_ServerLog(string obj)
{
    Console.WriteLine(obj);
}