using System.Net.Sockets;
using System.Net;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);

        // using var client = new TcpClient();

        // можем указать, к какому серверу он будет подключаться
        using var client = new TcpClient(ep);
        // можем создать клиента, передав в енго 'AddressFamily'
        // client = new TcpClient(AddressFamily.InterNetwork);
        // 'AddressFamily' иожет быть 'InterNetwork', 'InterNetworkV6'
        // по умолчанию 'Unknown', другие могут вызвать исключения
        // создание экземпляра 'TcpClient' аналогично созданию экземпляра класса 'Socket'
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        
        // создание экземпляра класса 'TcpClient' и передача в него 'EndPoint'
        // - аналогично вызову метода 'Bind'  у сокета
        socket.Bind(ep);

        // затем у клиента вызыватся 'Connect'
        client.Connect(ep);
        // аналогично
        socket.Connect(ep);

        ep = new IPEndPoint(IPAddress.Any, 5000);

        // сокет, который слушает - 'TcpListener'
        //var listener = new TcpListener(IPAddress.Any, 5000);
        // можно передать 'IPEndPoint'
        var listener = new TcpListener(ep);		

        // создаем сокет, создание которого аналогично созданию 'listener'
        var lsock = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        // чтобы начать прослушивать на сервере - вызываем 'Start' у 'Listener'
        listener.Start(10);
        // аналогично - 'Listen' у сокета
        lsock.Listen(10);

        // когда сервер получает соед-ие - мы можем вызвать у него 'Accept'
        // получаем сокет из 'Listener' с помощью 'AcceptSocketAsync'
        Socket asock = await listener.AcceptSocketAsync();
        // либо можем получить его из сокета с помощью 'Accept' или 'AcceptAsync'
        asock = await lsock.AcceptAsync();

        // принимаем данные с помощью след. класса
        NetworkStream stream;
    }
}