using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System;

namespace Chat_Listener_Client_WPF
{

    public partial class MainWindow : Window
    {
        delegate void AppendText(string text);

        public MainWindow()
        {
            InitializeComponent();
        }

        void AddText(string text)
        {
            txt_data.Text = text;
        }

        async void Client_Mode()
        {
            // 1

            Client_connect client = new Client_connect();

            client.IpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15);
            client.Client = new();

            // подсоединяемся к серверу
            client.Connect();


            // 2

            Server_connect server = new Server_connect();

            // тк мы прослушиваем - то указываем любой адрес
            server.IpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15);

            server.Listener = new TcpListener(server.IpEndPoint);


            // 3

            // создаем наш поток - получая от клиента поток
            await using NetworkStream stream_client = client.Client.GetStream();

            // далее будем считывать сообщения от сервера
            // создаем буфер
            // вернется размер того, что считали


            // 4 

            // запускаем 'listener', который будет слушать входящие соед-ия
            server.Start();


            // 5 

            // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
            // получаем его с помощью 'Accept' - вернем 'TcpClient'
            // (также как с помощью него можно вернуть и сокет)
            using TcpClient handler = await server.Listener.AcceptTcpClientAsync();
            // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

            // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
            await using NetworkStream stream_server = handler.GetStream();


            // 5

            client.Buffer_length = await stream_client.ReadAsync(client.Buffer);

            // расшифровываем сообщение
            client.Message = Encoding.UTF8.GetString(client.Buffer, 0, client.Buffer_length);


            // 6

            // формируем сообщение
            server.Message = $"Current Time: 📅{DateTime.Now}";

            // преобразовываем в массив байт
            var byteMsg = Encoding.UTF8.GetBytes(server.Message);

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream_server.WriteAsync(byteMsg);

            // выводим

            txt_data.Text = $"Sent message: {server.Message}";

            // 7

            // выводим сообщение
            this.Dispatcher.Invoke(new AppendText(AddText), client.Message);

            // преобразовываем в массив байт
            var byteMsg2 = Encoding.UTF8.GetBytes(txt_answer.Text);

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream_server.WriteAsync(byteMsg2);

            // тк использовали 'using' - у нас вызовется метод 'Dispose' и закроются сокеты 
        }

        async void Server_Mode()
        {
            Server_connect server = new Server_connect();

            // тк мы прослушиваем - то указываем любой адрес
            server.IpEndPoint = new IPEndPoint(IPAddress.Any, 15);

            server.Listener = new TcpListener(server.IpEndPoint);

            try
            {
                // запускаем 'listener', который будет слушать входящие соед-ия
                server.Start();

                // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
                // получаем его с помощью 'Accept' - вернем 'TcpClient'
                // (также как с помощью него можно вернуть и сокет)
                using TcpClient handler = await server.Listener.AcceptTcpClientAsync();
                // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

                // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
                await using NetworkStream stream = handler.GetStream();


                //var ipEndPoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15);

                //using TcpClient client = new();

                //// подсоединяемся к серверу
                //await client.ConnectAsync(ipEndPoint);

                //// создаем наш поток - получая от клиента поток
                //await using NetworkStream stream2 = client.GetStream();


                // когда произойдет 'рукопожатие' - соединение установится

                // формируем сообщение
                server.Message = $"Current Time: 📅{DateTime.Now}";

                // преобразовываем в массив байт
                var byteMsg = Encoding.UTF8.GetBytes(server.Message);

                // передаем это сообщение с помощью метода 'WriteAsync'
                await stream.WriteAsync(byteMsg);

                // выводим

                txt_data.Text = $"Sent message: {server.Message}";


                //// далее будем считывать сообщения от сервера
                //// создаем буфер
                //var buffer = new byte[1024];
                //// вернется размер того, что считали
                //int recLength = await stream.ReadAsync(buffer);

                //// расшифровываем сообщение
                //var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

                //// выводим сообщение
                //txt_data.Text = msg;
            }
            catch (Exception)
            {

            }
            finally
            {
                server.Stop();
            }
        }
        private async void btn_client_Click(object sender, RoutedEventArgs e)
        {
            Client_Mode();
        }

        private async void btn_server_Click(object sender, RoutedEventArgs e)
        {
            Server_Mode();
        }
    }
}