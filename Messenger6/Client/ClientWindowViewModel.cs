using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Messenger6.Client
{
    public class ClientWindowViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { OnPropertyChanged(ref _username, value); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { OnPropertyChanged(ref _address, value); }
        }

        private string _port = "8000";
        public string Port
        {
            get { return _port; }
            set { OnPropertyChanged(ref _port, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { OnPropertyChanged(ref _message, value); }
        }

        private string _colorCode;
        public string ColorCode
        {
            get { return _colorCode; }
            set { OnPropertyChanged(ref _colorCode, value); }
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand SendCommand { get; set; }

        private ChatroomViewModel _chatRoom;
        public ChatroomViewModel ChatRoom
        {
            get { return _chatRoom; }
            set { OnPropertyChanged(ref _chatRoom, value); }
        }

        public ClientWindowViewModel()
        {
            ChatRoom = new ChatroomViewModel();

            ConnectCommand = new AsyncCommand(Connect, CanConnect);
            DisconnectCommand = new AsyncCommand(Disconnect, CanDisconnect);
            SendCommand = new AsyncCommand(Send, CanSend);
        }

        private async Task Connect()
        {
            ChatRoom = new ChatroomViewModel();
            int socketPort = 0;
            var validPort = int.TryParse(Port, out socketPort);

            if (!validPort)
            {
                DisplayError("Пожалуйста предоставьте действующий порт.");
                return;
            }

            if (String.IsNullOrWhiteSpace(Address))
            {
                DisplayError("Пожалуйста предоставьте действующий адресс.");
                return;
            }

            if (String.IsNullOrWhiteSpace(Username))
            {
                DisplayError("Пожалуйста предоставьте имя.");
                return;
            }

            ChatRoom.Clear();
            await Task.Run(() => ChatRoom.Connect(Username, Address, socketPort));
        }

        private async Task Disconnect()
        {
            if (ChatRoom == null)
                DisplayError("Вы не подключенны к серверу.");

            await ChatRoom.Disconnect();
        }

        private async Task Send()
        {
            if (ChatRoom == null)
                DisplayError("Вы не подключились к серверу.");

            await ChatRoom.Send(Username, Message, ColorCode);
            Message = string.Empty;
        }

        private bool CanConnect() => !ChatRoom.IsRunning;
        private bool CanDisconnect() => ChatRoom.IsRunning;
        private bool CanSend() => !String.IsNullOrWhiteSpace(Message) && ChatRoom.IsRunning;

        private void DisplayError(string message) =>
            MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
