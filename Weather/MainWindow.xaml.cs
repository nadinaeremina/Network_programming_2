﻿using System.Net.Http;
using System.Net.Http.Json;
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

namespace Weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Current
        {
            string? Time; 
            int? Interval; 
            double? temperature_2m; 
            double? wind_speed_10m; 
            public Current()
            {
                Time = "";
                Interval = 0;
                temperature_2m = 0.0;
                wind_speed_10m = 0.0;
            }
        };

        public class Current_units
        {
            string? Time;
            string? Interval;
            string? temperature_2m;
            string? wind_speed_10m;
            public Current_units()
            {
                Time = "";
                Interval = "";
                temperature_2m = "";
                wind_speed_10m = "";  
            }
        }

        public record class Todo(
        double? latitude = null,
        double? longitude = null,
        decimal? generationtime_ms = null,
        int? utc_offset_seconds = null,
        string? timezone = null,
        string? timezone_abbreviation = null,
        decimal? elevation = null,
        Current_units? current_units = null,
        Current? current = null
        );

        private HttpClient client;
        private string jsonPlaceholder = "https://api.open-meteo.com/";
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // используем этот класс, чтобы прочитать ответ
            // получение ответа // в 'GetAsync' можно предоставить доп инф-цию к адресу
            using HttpResponseMessage response = await client.GetAsync("v1/forecast?latitude=56&longitude=60&current=temperature_2m,wind_speed_10m");
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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            // получим список дел // можем его фильтровать
            // так как у нас 'todos' - возвращается массив
            // нужно раскрыть шаблонный метод 'GetFromJsonAsync' в кол-цию
            // добавляем св-во фиьтрации
            var todos = await client.GetFromJsonAsync<List<Todo>>("v1/forecast?latitude=56&longitude=60&current=temperature_2m,wind_speed_10m");
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
    }
}