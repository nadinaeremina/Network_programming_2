using System.Net.Sockets;
using System.Threading.Tasks;


using TcpClient client = new TcpClient();

Console.Write("Введите ваше имя: ");

string user_Name = Console.ReadLine();

Console.WriteLine($"Добро пожаловать, {user_Name}");

StreamReader Reader = null;
StreamWriter Writer = null;

try
{
    // присоединяемся к серверу
    client.Connect("127.0.0.1", 15); 

    // создаем поток для получения данных
    Reader = new StreamReader(client.GetStream());

    // создаем поток для передачи данных
    Writer = new StreamWriter(client.GetStream());

    if (Writer is null || Reader is null)
    {
        return;
    }

    // запускаем новый поток для получения данных в фоновом потоке
    // освобождаем вызывающий поток для выполнения других задач
    Task.Run(() => ReceiveMessageAsync(Reader));

    // запускаем ввод сообщений
    await SendMessageAsync(Writer);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// закрываем все потоки
Writer.Close();
Reader.Close();

// отправка сообщений
async Task SendMessageAsync(StreamWriter writer)
{
    // сначала отправляем имя
    await writer.WriteLineAsync(user_Name);

    // очищаем все буферы для текущего потока
    await writer.FlushAsync();

    Console.WriteLine("Введите сообщение:");

    while (true)
    {
        string message = Console.ReadLine();

        // отправляем сообщение 
        await writer.WriteLineAsync(message);

        // очищаем все буферы для текущего потока
        await writer.FlushAsync();
    }
}

// получение сообщений
async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            // считываем ответ в виде строки
            string message = await reader.ReadLineAsync();

            // если пустой ответ - ничего не выводим на консоль
            if (string.IsNullOrEmpty(message))
            {
                // переходим к следующей итерации
                continue;
            }

            //выводим сообщение
            Console.WriteLine(message);
        }
        catch
        {
            // при возникновении ошибки - останавливаем цикл
            break;
        }
    }
}
