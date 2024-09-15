using System.Security.Cryptography;
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

namespace Encryption
{
    public partial class MainWindow : Window
    {
        // Наличие 'соли' значительно затрудняет взлом, тк есть дополнительная зашифрованная и уникальная информация
        // какой-то кусок данных, который известен серверу и клиенту, но он не передается по сети и перехватить его нельзя
        // может быть сгенерированным для каждого клиента уникально - например, в рез-те хэширования логина клиента
        public string salt;

        public MainWindow()
        {
            InitializeComponent();

            // имя клиента+время регистрации
            salt = "UserName" + DateTime.Now.ToString();
            salt = CreateMD5(salt, false);
        }

        // 'MD5' - алгоритм неполноценного шифрования,а алгоритм хэширования, тк он не исп-ет ключи
        private void EncryptMD5Button_Click(object sender, RoutedEventArgs e)
        { 
            string input = InitialTB.Text;
            EncryptedTB.Text = CreateMD5(input);
        }
        private string CreateMD5(string input, bool salted = true)
        {
            // в MD5 всегда получится строка одинакового размера
            // файл хэшируется, мы знаем хэш-код целостного файла
            // если после проверки сумма ранее хэшированного файла и сейчас совпадает - значит файл не поврежден
            // если строки не совпадают - значит файл был поврежден
            // Это назывется - 'Контрольная сумма' - исп-ся для контроля того, хорошо ли сейчас файлам

            if (salted) { input += salt; }

            // создаем экземпляр класса MD5
            using (MD5 md5 = MD5.Create())
            {
                // то, что пришло от пользователя - мы зашифровываем сначала в байты
                byte[] inputBytes = Encoding.Unicode.GetBytes(input);

                // нам нужны байты в соответствующем хэше - шмфруем байты
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // преобразуем обратно в строку
                return Convert.ToHexString(hashBytes);
            }
        }

        // 'SHA' - чуть более надежный вид шифрования,строка получится подлиннее, работать будет чуть медленнее
        private void EncryptSHA1Button_Click(object sender, RoutedEventArgs e)
        {
            string input = InitialTB.Text;
            EncryptedTB.Text = CreateSHA1(input);
        }
        private string CreateSHA1(string input, bool salted = true)
        {
            if (salted) { input += salt; }

            // принимаем строку, создаем экземпляр 'SHA1'
            using (SHA1 sha1 = SHA1.Create())
            {
                // то, что пришло от пользователя - мы зашифровываем сначала в байты
                byte[] inputBytes = Encoding.Unicode.GetBytes(input);

                // нам нужны байты в соответствующем хэше - шмфруем байты
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                // преобразуем обратно в строку
                return Convert.ToHexString(hashBytes);
            }
        }


        // 'SHA384' - еще чуть медленнее, длина увеличивается, надежность тоже
        // один из самых надежных
        private void EncryptSHA384Button_Click(object sender, RoutedEventArgs e)
        {
            string input = InitialTB.Text;
            EncryptedTB.Text = CreateSHA384(input);
        }

        private string CreateSHA384(string input, bool salted = true)
        {
            if (salted) { input += salt; }

            // принимаем строку, создаем экземпляр 'SHA384'
            using (SHA384 sha384 = SHA384.Create())
            {
                // то, что пришло от пользователя - мы зашифровываем сначала в байты
                byte[] inputBytes = Encoding.Unicode.GetBytes(input);

                // нам нужны байты в соответствующем хэше - шмфруем байты
                byte[] hashBytes = sha384.ComputeHash(inputBytes);

                // преобразуем обратно в строку
                return Convert.ToHexString(hashBytes);
            }
        }

        // 'SHA512'
        private void EncryptSHA512Button_Click(object sender, RoutedEventArgs e)
        {
            string input = InitialTB.Text;
            EncryptedTB.Text = CreateSHA512(input);
        }
        private string CreateSHA512(string input, bool salted = true)
        {
            if (salted) { input += salt; }

            // принимаем строку, создаем экземпляр 'SHA384'
            using (SHA512 sha512 = SHA512.Create())
            {
                // то, что пришло от пользователя - мы зашифровываем сначала в байты
                byte[] inputBytes = Encoding.Unicode.GetBytes(input);

                // нам нужны байты в соответствующем хэше - шмфруем байты
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // преобразуем обратно в строку
                return Convert.ToHexString(hashBytes);
            }
        }
    }
}