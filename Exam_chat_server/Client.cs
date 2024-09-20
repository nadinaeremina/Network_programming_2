using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Exam_chat_server
{
    internal class Client
    {
        // создаем уникальный идентификатор для клиента
        protected internal string Id { get; } = Guid.NewGuid().ToString();

        // отправка данных
        protected internal StreamWriter Writer { get; }

        // получение данных
        protected internal StreamReader Reader { get; }

        // создание экземпляра 'TcpClient' аналогично созданию экземпляра класса 'Socket'
        TcpClient client;

        // для дальнейшего взаимодействия понадобится также и экземпляр класса 'Server'
        Server server; 

        public Client(TcpClient tcp_client, Server in_server)
        {
            client = tcp_client;
            server = in_server;

            // получаем 'NetworkStream' для взаимодействия с сервером
            NetworkStream stream = client.GetStream();

            // создаем 'StreamReader' для чтения данных
            Reader = new StreamReader(stream);

            // создаем 'StreamWriter' для отправки данных
            Writer = new StreamWriter(stream);
        }

        // работа с клиентом
        public async Task ProcessAsync()
        {
            try
            {
                // получаем имя пользователя
                string user_name = await Reader.ReadLineAsync();

                string message = $"{user_name} зашел в чат";

                // посылаем сообщение о входе в чат всем подключенным пользователям
                await server.BroadcastMessageAsync(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        // получаем сообщения от клиента
                        message = await Reader.ReadLineAsync();

                        if (message == null)
                        {
                            // если сообщение пустое - нечего выводить // переходим к следующей итерации
                            continue;
                        }

                        // инициализируем сообщение
                        message = $"{user_name}: {message}";

                        // выводим сообщение
                        Console.WriteLine(message);

                        // посылаем сообщение о входе в чат всем подключенным пользователям
                        await server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        // если приложение с пользователем закрылось - выводим следующее сообщение
                        message = $"{user_name} вышел из чата";
                        Console.WriteLine(message);

                        // посылаем сообщение о входе в чат всем подключенным пользователям
                        await server.BroadcastMessageAsync(message, Id);
                        
                        // останавливаем цикл
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
            }
        }

        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
