using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exam_chat_server
{
    internal class Server
    {
        // тк мы прослушиваем - то указываем любой адрес
        TcpListener tcp_listener = new TcpListener(IPAddress.Any, 15); 

        // список подключенных к серверу клиентов
        List<Client> clients = new List<Client>(); 

        // удаление клиента из списка всех клиентов
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            // метод 'FirstOrDefault' применяется к последовательностям
            // возвращает первый элемент или значение по умолчанию,
            // если ни одного элемента не найдено
            Client client = clients.FirstOrDefault(c => c.Id == id);

            // удаляем клиента из списка 
            if (client != null)
            {
                clients.Remove(client);
            }

            // закрываем подключение клиента
            client.Close();
        }

        // прослушивание входящих подключений
        protected internal async Task ListenAsync()
        {
            try
            {
                // запускаем 'listener', который будет слушать входящие соед-ия
                tcp_listener.Start();
                Console.WriteLine("Сервер запущен!");

                while (true)
                {
                    // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
                    // получаем его с помощью 'Accept' - вернем 'TcpClient'
                    TcpClient tcp_сlient = await tcp_listener.AcceptTcpClientAsync();

                    // создаем нового клиента
                    Client client = new Client(tcp_сlient, this);

                    // добавляем его в список всех клиентов
                    clients.Add(client);

                    // запускаем метод работы клиента в фоновом потоке
                    // освобождаем вызывающий поток для выполнения других задач
                    Task.Run(client.ProcessAsync);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // отключаем всех клиентов
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var client in clients)
            {
                // если id клиента не равно id отправителя
                if (client.Id != id) 
                {
                    // передаем данные
                    await client.Writer.WriteLineAsync(message); 
                    await client.Writer.FlushAsync();
                }
            }
        }

        // отключение всех подключений
        protected internal void Disconnect()
        {
            foreach (var client in clients)
            {
                //отключение клиента
                client.Close(); 
            }

            //остановка сервера
            tcp_listener.Stop(); 
        }
    }
}
