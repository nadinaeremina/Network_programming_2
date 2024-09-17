using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_json_Client
{
    // нужен для обновления инф-ции в текстовом поле
    delegate void AppendText(string user);

    public partial class MainWindow : Window
    {
        Socket sock;
        IPAddress ip;
        IPEndPoint ipep;

        void AppendTextToOutput(string user)
        {
            RecTB.Text = user;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sock == null)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                // создаем новый ip-адрес
                ip = IPAddress.Parse("127.0.0.1");

                // создаем 'EndPoint' для прослушки
                ipep = new IPEndPoint(ip, 1024);
            }
            
            try
            {
                sock.Connect(ipep);

                if (sock.Connected)
                {
                    string json = "";
                    byte[] buff = new byte[1024];
                    int l;

                    // здесь пр-ма встанет и будет ждать, пока не прийдет ответ от сервера
                    l = sock.Receive(buff);
                    // 'Receive' измеряет размер сообщения, в то время как буффер яв-ся массивом, поэтому он будет изменяться внутри метода

                    string user = Encoding.ASCII.GetString(buff, 0, l);

                    this.Dispatcher.Invoke(new AppendText(AppendTextToOutput), user);
                    // обращаемся в главном потоке

                    sock.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server is unaviable");
            }
        }
    }
}