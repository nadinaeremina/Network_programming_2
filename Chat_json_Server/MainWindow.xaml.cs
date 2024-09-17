using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
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
using System.Xml.Serialization;

namespace Chat_json_Server
{
    public partial class MainWindow : Window
    {
        Socket s;
        Student student = new Student
        {
            Name = "Ivan",
            Lastname = "Ivanov",
            Group = "PV555",
            Grades = [3, 4, 5, 4, 5]
        };

        // передаем в конструктор 'XmlSerializer' тип класса Student
        XmlSerializer xmlser = new XmlSerializer(typeof(Student));

        public MainWindow()
        {
            InitializeComponent();

            WriteToFile();
        }

        private void WriteToFile()
        {
            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream("student.txt", FileMode.OpenOrCreate))
            {
                xmlser.Serialize(fs, student);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // десериализуем объект
            using (FileStream fs = new FileStream("student.txt", FileMode.OpenOrCreate))
            {
                student = xmlser.Deserialize(fs) as Student;
            }

            TB.Text = student.ToString();

            if (s == null)
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IPEndPoint ep = new IPEndPoint(ip, 1024);
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                s.Bind(ep);
                s.Listen(10);
            }

            Socket ns = s.Accept();

            byte[] buff = new byte[1024];

            // посылаем сообщение
            ns.Send(Encoding.ASCII.GetBytes(student.ToString()));

            ns.Close();
        }
    }
}
