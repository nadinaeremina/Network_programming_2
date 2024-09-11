using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net.Http; // подключаем
using System.Net;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;

namespace HTTP_Client
{
    // простые классы для банальных записей
    // поля класса будут совпадать с полями возвращаемого 'json'
    public record class Todo(
        int? UserId = null,
        int? Id = null,
        string? Title = null,
        bool? Completed = null
        );
    public partial class MainWindow : Window
    {
        // этот класс будет отвечать за вып-ие запросов
        private HttpClient client;
        private string jsonPlaceholder = "https://jsonplaceholder.typicode.com/";

        public MainWindow()
        {
            InitializeComponent();

            // для 'client' базовым адресом яв-ся текущий адрес, который готов к нему подключаться
            // создаем экземпляр, ему следует передать URI, к которому будем подключатья
            client = new HttpClient
            {
                // встроена сис-ма взаим-ия с адресами // кладем адрес
                BaseAddress = new Uri(jsonPlaceholder),

                // сколько будем ждать, прежде чем запрос будет считаться потерянным
                // по умолчанию - 100 секунд // лучше не ставить меньше 15 секунд
                Timeout = TimeSpan.FromSeconds(120)
            };
        }

        // все делается асинхронно, чтобы программа дожидалась ответа, не зависала
        private async void Do_request_Click(object sender, RoutedEventArgs e)
        {
            // используем этот класс, чтобы прочитать ответ
            // получение ответа // в 'GetAsync' можно предоставить доп инф-цию к адресу
            using HttpResponseMessage response = await client.GetAsync("todos/3");
            // "todos/3" - другая страница, кот. хранится на браузере
            // здесь get-запрос делается не к главной странице, а каким-то доп-ым адресам на сайте
            // когда в адресной строке мы пишем адрес сервера - мы делаем запрос
            // в 'get-запросе' мы узнаем содержимое страницы

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();
            // этот метод вызовет искл-ие, если код 'response' не равен 200

            // получаем ответ в виде 'json' 
            var jsonResponse = await response.Content.ReadAsStringAsync();
            // отвте вернулся от сервера и он имеет содержимое 
            // 'content' относится к этому содержимому
            // 'ReadAsStringAsync' - чтобы получить ответ в виде строки

            // выводим ответ
            OutPutTB.Text = jsonResponse;
        }

        // получаем данные в виде списка под наш класс 'Todo'
        private async void Get_fromJson_but_Click(object sender, RoutedEventArgs e)
        {
            // получим список дел // можем его фильтровать
            // так как у нас 'todos' - возвращается массив
            // нужно раскрыть шаблонный метод 'GetFromJsonAsync' в кол-цию
            // добавляем св-во фиьтрации
            var todos = await client.GetFromJsonAsync<List<Todo>>("todos?userId=1&completed=false");
            // получим json-файл - автоматически преобразует в список файлов типа 'Todo'
            // в этот метод можно передать то, чем я буду дополнять адрес (св-ва фильтрации)

            OutPutTB.Text = "";

            if (todos != null)
            {
                foreach (var todo in todos)
                {
                    OutPutTB.Text += todo.ToString() + "\n";
                }
            }
        }

        // для отправки сообщения
        private async void DoPOST_but_Click(object sender, RoutedEventArgs e)
        {
            // описывает http-content в виде строки
            // создаем строку из 'json'
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(
                    new
                    {
                        userId = 77,
                        id = 1,
                        title = InputTB.Text,
                        completed = false
                    }
                    ), Encoding.UTF8, "application/json" // тип запроса
                );

            // 'PostAsync()' - передает данные на сервер
            // нужно указать, на какую страницу мы его делаем
            using HttpResponseMessage response = await client.PostAsync("todos", jsonContent);
            // у post-запроса должно быть тело обязат-но // он передает данные на сервер
            // мы в любом случае получим ответ - поэтому создаем 'HttpResponseMessage'
            // "todos" - на какую страницу мы его делаем
            // 'jsonContent' - какие данные

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            // получили ответ
            response.EnsureSuccessStatusCode();

            // выводим
            var jsonResponse = await response.Content.ReadAsStringAsync();
            // сервер ответит нам тем же самым текстом - это тестовый сервер

            OutPutTB.Text = jsonResponse;
        }

        private async void DoPOSTasJson_but_Click(object sender, RoutedEventArgs e)
        {
            // "todos" - адрес запрашиваемой страницы и создаем новый 'Todo'
            using HttpResponseMessage response = await client.PostAsJsonAsync(
                "todos",
                new Todo(UserId: 9, Id: 99, Title: InputTB.Text, Completed: false)
                );
            // здесь автоматически происходит сериализация в json

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();

            // автоматически десериализуем
            var todo = await response.Content.ReadFromJsonAsync<Todo>();

            OutPutTB.Text = $"Response: {todo}";
        }

        // 'PUT' предназначен для обновления какого-то ресурса
        // либо заменяет, либо создает новый
        private async void DoPUT_but_Click(object sender, RoutedEventArgs e)
        {
            // сериализуем класс, отправляем в 'json'
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    userId = 1,
                    id = 1,
                    title = "hello world",
                    completed = false
                }),
                Encoding.UTF8, "application/json"
                );

            using HttpResponseMessage response = await client.PutAsync(
                "todos/1", // вернет 'todo' с индексом 1
                jsonContent
                );

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();

            // выводим
            var jsonResponse = await response.Content.ReadAsStringAsync();

            OutPutTB.Text = jsonResponse;
        }

        private async void DoPUTasJson_but_Click(object sender, RoutedEventArgs e)
        {
            using HttpResponseMessage response = await client.PutAsJsonAsync(
                "todos/5", // и отправляем на эту страницу
                new Todo(Title: "partial update", Completed: true) // файл зашифровываем в 'json'
                );

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();

            // записываем ответ // говорим, что из 'json' читаем экземпляр класса 'Todo'
            var todo = await response.Content.ReadFromJsonAsync<Todo>();

            OutPutTB.Text = todo.ToString();
        }

        // 'using' использовали для вызова 'Dispose()'
    }
}

