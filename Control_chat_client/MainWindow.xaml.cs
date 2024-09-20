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

namespace Control_chat_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void Add_text(string mes);
        public delegate void Change();
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        TcpClient client;
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15);
        NetworkStream stream;
        public MainWindow()
        {
            InitializeComponent();

            txt_mes.Items.Add("Вопрос А");
            txt_mes.Items.Add("Вопрос B");
            txt_mes.Items.Add("Вопрос C");
        }
        public void Add_text_to(string mes)
        {
            txt_output.Text = mes;
        }

        public void Change_visibility()
        {
            main_panel.Visibility = Visibility.Visible;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new();

                Dispatcher.BeginInvoke(new Change(Change_visibility));

                Dispatcher.BeginInvoke(new Add_text(Add_text_to), "Подключаемся к серверу");

                // подсоединяемся к серверу
                await client.ConnectAsync(ipEndPoint);

                // создаем наш поток - получая от клиента поток
                await using NetworkStream stream = client.GetStream();

                while (true)
                {
                    // далее будем считывать сообщения от сервера
                    // создаем буфер
                    var buffer = new byte[1024];
                    // вернется размер того, что считали
                    int recLength = await stream.ReadAsync(buffer);

                    // расшифровываем сообщение
                    var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

                    // выводим сообщение
                    txt_output.Text = msg;
                    txt_output.Text += "\nWould you like to answer? 1 - yes, 2 - no";
                }
            }
            catch (Exception)
            {
                //Console.WriteLine(e.ToString());
            }
            finally
            {
                //tcp_client.Close();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //int answer;

            //do
            //{
            //    // формируем сообщение
            //    answer = Convert.ToInt16(Console.ReadLine());

            //    if (answer != 1 && answer != 2)
            //        Console.WriteLine("Please, push '1' or '2'");

            //} while (answer != 1 && answer != 2);

            //if (answer == 2)
            //    break;

            //txt_output.Text += "\nEnter the answer: ";

            //// формируем сообщение
            //txt_mes = Console.ReadLine();

            // создаем наш поток - получая от клиента поток
            await using NetworkStream stream = client.GetStream();

            // преобразовываем в массив байт
            byte[] byteMsg = Encoding.UTF8.GetBytes(txt_mes.Text);

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream.WriteAsync(byteMsg);

            // выводим
            txt_output.Text += "\nThe message has been sent!";
        }
    }
}
