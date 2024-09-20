using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Control_chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void Add_text(string mes);
        Thread receiver = new Thread(Receiver_mothod);
        TcpListener server = new TcpListener(IPAddress.Any, 15);
        TcpClient client;

        private async void Receiver_mothod(object? obj)
        {
            while (true)
            {
                TcpListener server = new TcpListener(IPAddress.Any, 15);

                // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
                // получаем его с помощью 'Accept' - вернем 'TcpClient'
                // (также как с помощью него можно вернуть и сокет)
                using TcpClient handler = await server.AcceptTcpClientAsync();
                // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

                // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
                await using NetworkStream stream = handler.GetStream();

                // преобразовываем в массив байт
                byte[] byteMsg = Encoding.UTF8.GetBytes("gthfddtfh");

                // передаем это сообщение с помощью метода 'WriteAsync'
                await stream.WriteAsync(byteMsg);

                Dispatcher.BeginInvoke(new Add_text(Add_text_to), "The message has been sent!");

                // далее будем считывать сообщения от сервера
                // создаем буфер
                var buffer = new byte[1024];
                // вернется размер того, что считали
                int recLength = await stream.ReadAsync(buffer);

                // расшифровываем сообщение
                msg = Encoding.UTF8.GetString(buffer, 0, recLength);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Background = receiver;
        }
        public void Add_text_to(string mes)
        {
            txt_output.Text = mes;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                server.Start();

                Dispatcher.BeginInvoke(new Add_text(Add_text_to), "Сервер запущен. Ожидание подключений...");

                receiver.Start();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        // send
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //int answer;
            //txt_output.Text = "\nEnter the message:";

            while (true)
            {
                // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
                // получаем его с помощью 'Accept' - вернем 'TcpClient'
                // (также как с помощью него можно вернуть и сокет)
                using TcpClient handler = await server.AcceptTcpClientAsync();
                // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

                // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
                await using NetworkStream stream = handler.GetStream();

                // когда произойдет 'рукопожатие' - соединение установится

                // формируем сообщение
                //string msg = Console.ReadLine();

                // преобразовываем в массив байт
                byte[] byteMsg = Encoding.UTF8.GetBytes(txt_mes.Text);

                // передаем это сообщение с помощью метода 'WriteAsync'
                await stream.WriteAsync(byteMsg);

                // выводим
                txt_output.Text += "\nThe message has been sent!";

                // далее будем считывать сообщения от сервера
                // создаем буфер
                var buffer = new byte[1024];
                // вернется размер того, что считали
                int recLength = await stream.ReadAsync(buffer);

                // расшифровываем сообщение
                txt_mes.Text = Encoding.UTF8.GetString(buffer, 0, recLength);

                // выводим сообщение
                txt_output.Text += "\nThe received message: {txt_mes.Text}";
                txt_output.Text += "\nWould you likr to answer? 1 - yes, 2 - no";

                //do
                //{
                //    // формируем сообщение
                //    answer = Convert.ToInt16(Console.ReadLine());

                //    if (answer != 1 && answer != 2)
                //        Console.WriteLine("Please, push '1' or '2'");

                //} while (answer != 1 && answer != 2);

                //if (answer == 2)
                //    break;

                txt_output.Text += "\nEnter the answer:";
            }
        }
    }
}