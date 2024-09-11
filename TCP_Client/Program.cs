using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Run();
    }
    static async Task Run()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15);

        using TcpClient client = new();

        // подсоединяемся к серверу
        await client.ConnectAsync(ipEndPoint);

        // создаем наш поток - получая от клиента поток
        await using NetworkStream stream = client.GetStream();

        // далее будем считывать сообщения от сервера
        // создаем буфер
        var buffer = new byte[1024];
        // вернется размер того, что считали
        int recLength = await stream.ReadAsync(buffer);

        // расшифровываем сообщение
        var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

        // выводим сообщение
        Console.WriteLine($"Message: {msg}");

        // тк использовали 'using' - у нас вызовется метод 'Dispose' и закроются сокеты 
    }
}