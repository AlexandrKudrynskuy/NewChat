using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatClient.Model;
using Microsoft.VisualBasic;
using static System.Net.WebRequestMethods;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClientService clientService;
         public ObservableCollection<VievMessage> ViewMessages { get; set;}
        public ObservableCollection<VievUser> ViewUsers { get; set;}
        public VievUser MyVievUser { get; set; } 
        public MainWindow()
        {
            InitializeComponent();
            clientService = new ClientService();         
            ViewMessages = new ObservableCollection<VievMessage>();
            ViewUsers = new ObservableCollection<VievUser>();
            VievMessageListViev.ItemsSource = ViewMessages;
            UserMessageListViev.ItemsSource = ViewUsers;
       
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            clientService.Connect("127.0.0.1", 2424);

            //clientService.SendNewUser += ClientService_SendNewUser;
            clientService.GetAllUsers += ClientService_GetAllUsers; ; 
            clientService.GetPrivate += ClientService_GetPrivate; ;
            clientService.GetPublic += ClientService_GetPublic; ;
            clientService.LogClient += ClientService_LogClient;
            clientService.SendConnected += ClientService_SendConnected;
            clientService.Disconnected += ClientService_Disconnected;
            MyVievUser = new VievUser(); 



            clientService.StartReceive();

        }

     

        //Log to ListView
        private void ClientService_LogClient(string obj)
        {
        }

        // Conect To Server
        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            MyVievUser.Sender = nameTextBox.Text;
            clientService.Send(new Model.Message { Content = MyVievUser.Sender, PhotoPathSender= MyVievUser.SenderPhoto, Type = Model.MessageType.NewUser });
            ConnectBtn.IsEnabled = false;
            nameTextBox.IsEnabled = false;
            UserMessageListViev.IsEnabled = true;
            VievMessageListViev.IsEnabled = true;
            MMSGTextBox.IsEnabled=true;
            SendTextBtn.IsEnabled = true;
        }
        private void ClientService_GetPrivate(Responce obj)
        {
            if (obj.Content != null)
            {
                var time= "\t" +  DateTime.Now.Hour +":"+ DateTime.Now.Minute + "\t";
                var vMes = new VievMessage { Message= "Приватне\t" + obj.Content, Sender=obj.Sender, SenderPhoto=obj.PhotoPathSender, Time= time };
                ViewMessages.Add(vMes);
            }
        }

        private void ClientService_GetPublic(Responce obj)
        {
            if (obj.Content != null)
            {
                var time = "\t" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "\t";

                var vMes = new VievMessage {Message = "Публічне\t" + obj.Content, Time=time, Sender = obj.Sender, SenderPhoto = obj.PhotoPathSender };
                ViewMessages.Add(vMes);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clientService.Send(new Model.Message { Content = nameTextBox.Text, Type = Model.MessageType.Disconect });
            clientService.Stop();
        }
     
        // send Public Message
        private void SendTextBtn_Click(object sender, RoutedEventArgs e)
        {
            clientService.Send(new Model.Message { Content = MMSGTextBox.Text, Recipient = nameTextBox.Text, Type = MessageType.PublicMessage });
            MMSGTextBox.Text = string.Empty;

        }
        private void SendPrivateTextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserMessageListViev.SelectedValue is VievUser vUser)
            {
                clientService.Send(new Model.Message { Content = MMSGTextBox.Text, Recipient = vUser.Sender, Sender = MyVievUser.Sender, PhotoPathSender = MyVievUser.SenderPhoto, Type = Model.MessageType.PrivateMessage });
                SendPrivateTextBtn.IsEnabled = false;
                MMSGTextBox.Text = string.Empty;
            }
        }
        private void ClientService_SendConnected(Responce obj)
        {
            if (obj.Sender != string.Empty)
            {
                var time = "\t" + DateTime.Now.Hour + ":" + DateTime.Now.Minute +"\t";

                ViewUsers.Add(new VievUser { Sender= obj.Sender, SenderPhoto=obj.PhotoPathSender });
                var vMes = new VievMessage { Message = "Приєднався до чату",Time=time, Sender = obj.Sender, SenderPhoto = obj.PhotoPathSender };
                ViewMessages.Add(vMes);
            }
        }
        private void ClientService_Disconnected(Responce obj)
        {
            if (obj.Sender != string.Empty)
            {
                var time = "\t" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "\t";
                var vMes = new VievMessage { Message = "Вийшов з чату",Time= time, Sender = obj.Sender, SenderPhoto = obj.PhotoPathSender };
                ViewMessages.Add(vMes);
                var currentVUser = ViewUsers.First(x => x.Sender == obj.Sender);
                ViewUsers.Remove(currentVUser);
                //    Dispatcher.Invoke(() => { UserListViev.UpdateLayout(); });
            }
        }

        private void ClientService_GetAllUsers(Responce obj)
        {
            if (obj.Sender != string.Empty)
            {
                ViewUsers.Add(new VievUser { Sender = obj.Sender, SenderPhoto = obj.PhotoPathSender });
            }
        }
        private void AddUrlBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UrlPhotoPath.Text != string.Empty)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(UrlPhotoPath.Text);
                bitmap.EndInit();
                MyVievUser.SenderPhoto = UrlPhotoPath.Text;
                PhotoImage.Source =bitmap;
                nameTextBox.IsEnabled = true;
                ConnectBtn.IsEnabled = true;    
                AddUrlBtn.IsEnabled = false;
                UrlPhotoPath.IsEnabled = false;
            }
        }

        private void UserMessageListViev_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MMSGTextBox.Text != string.Empty)
            {
                SendPrivateTextBtn.IsEnabled = true;
            }
        }

      


    }
}
