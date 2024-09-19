using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        int number;

        Client client = new Client(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15));

        // тк мы прослушиваем - то указываем любой адрес
        Server server = new Server(new IPEndPoint(IPAddress.Any, 15));

        Console.WriteLine("Select the operation mode: \n1 - Server, 2 - Client \n\nAt first - to start 'Server' and then to start 'Client'");

        number = Convert.ToInt16(Console.ReadLine());

        do
        {
            if (number == 1)
                await server.ServerWorking();
            else if (number == 2)
                await client.ClientWorking();
            else
                Console.WriteLine("Please, select the operation mode!");

        } while (number != 1 && number != 2);
        
    }

    class Server
    {
        public IPEndPoint ipEndPoint;
        public TcpListener tcp_listener;

        public Server(IPEndPoint iPEndPoint)
        {
            ipEndPoint = iPEndPoint;
            tcp_listener = new TcpListener(ipEndPoint);
        }

        public async Task ServerWorking()
        {
            try
            {
                // запускаем 'listener', который будет слушать входящие соед-ия
                tcp_listener.Start();

                // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
                // получаем его с помощью 'Accept' - вернем 'TcpClient'
                // (также как с помощью него можно вернуть и сокет)
                using TcpClient handler = await tcp_listener.AcceptTcpClientAsync();
                // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

                // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
                await using NetworkStream stream = handler.GetStream();

                // когда произойдет 'рукопожатие' - соединение установится

                int answer;
                Console.WriteLine("\nEnter the message:");

                while (true)
                {
                    // формируем сообщение
                    string msg = Console.ReadLine();

                    // преобразовываем в массив байт
                    byte[] byteMsg = Encoding.UTF8.GetBytes(msg);

                    // передаем это сообщение с помощью метода 'WriteAsync'
                    await stream.WriteAsync(byteMsg);

                    // выводим
                    Console.WriteLine("The message has been sent!");

                    // далее будем считывать сообщения от сервера
                    // создаем буфер
                    var buffer = new byte[1024];
                    // вернется размер того, что считали
                    int recLength = await stream.ReadAsync(buffer);

                    // расшифровываем сообщение
                    msg = Encoding.UTF8.GetString(buffer, 0, recLength);

                    // выводим сообщение
                    Console.WriteLine($"The received message: {msg}");
                    Console.WriteLine("\nWould you likr to answer? 1 - yes, 2 - no");

                    do
                    {
                        // формируем сообщение
                        answer = Convert.ToInt16(Console.ReadLine());

                        if (answer != 1 && answer != 2)
                            Console.WriteLine("Please, push '1' or '2'");

                    } while (answer != 1 && answer != 2);

                    if (answer == 2)
                        break;

                    Console.WriteLine("\nEnter the answer:");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                tcp_listener.Stop();
            }
        }
    }

    class Client
    {
        public IPEndPoint ipEndPoint;
        public TcpClient tcp_client;

        public Client(IPEndPoint iPEndPoint)
        {
            ipEndPoint = iPEndPoint;
            tcp_client = new TcpClient();
        }

        public async Task ClientWorking()
        {
            try
            {
                // подсоединяемся к серверу
                await tcp_client.ConnectAsync(ipEndPoint);

                // создаем наш поток - получая от клиента поток
                await using NetworkStream stream = tcp_client.GetStream();

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
                    Console.WriteLine($"The received message: {msg}");
                    Console.WriteLine("\nWould you likr to answer? 1 - yes, 2 - no");

                    int answer;

                    do
                    {
                        // формируем сообщение
                        answer = Convert.ToInt16(Console.ReadLine());

                        if (answer != 1 && answer != 2)
                            Console.WriteLine("Please, push '1' or '2'");

                    } while (answer != 1 && answer != 2);

                    if (answer == 2)
                        break;

                    Console.WriteLine("Enter the answer: ");

                    // формируем сообщение
                    msg = Console.ReadLine();

                    // преобразовываем в массив байт
                    byte[] byteMsg = Encoding.UTF8.GetBytes(msg);

                    // передаем это сообщение с помощью метода 'WriteAsync'
                    await stream.WriteAsync(byteMsg);

                    // выводим
                    Console.WriteLine("The message has been sent!");
                }
  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                tcp_client.Close();
            }
        }
    }
}