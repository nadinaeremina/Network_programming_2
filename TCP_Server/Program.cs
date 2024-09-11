using System.Net.Sockets;
using System.Net;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Run();
    }

    // можно писать клиента с помощью 'Client', а клиента с помощью сокетов например
    static async Task Run()
    {
        // тк мы прослушиваем - то указываем любой адрес
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 15);

        TcpListener listener = new(ipEndPoint);

        try
        {
            // запускаем 'listener', который будет слушать входящие соед-ия
            listener.Start();

            // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
            // получаем его с помощью 'Accept' - вернем 'TcpClient'
            // (также как с помощью него можно вернуть и сокет)
            using TcpClient handler = await listener.AcceptTcpClientAsync();
            // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

            // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
            await using NetworkStream stream = handler.GetStream();

            // когда произойдет 'рукопожатие' - соединение установится

            // формируем сообщение
            var msg = $"Current Time: 📅{DateTime.Now}";

            // преобразовываем в массив байт
            var byteMsg = Encoding.UTF8.GetBytes(msg);

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream.WriteAsync(byteMsg);

            // выводим
            Console.WriteLine($"Sent message: {msg}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            listener.Stop();
        }
    }
}