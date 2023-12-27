// Client2
using System;
using System.IO;
using System.IO.Pipes;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        // Устанавливаем начальный счётчик сообщений
        private int ctr = 1;
        // Id клиента
        private static string guid = Guid.NewGuid().ToString();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Клиент запущен. Подключение к серверу...");
            try
            {
                // Объявляем именованный канал клиента client
                NamedPipeClientStream client = new NamedPipeClientStream(".", "myPipe", PipeDirection.InOut);
                // Подключения к серверу
                client.Connect();
                // Поток считывния
                StreamReader sr = new StreamReader(client);
                // Поток записи
                StreamWriter sw = new StreamWriter(client);
                // Сообщение от клиента с guid
                string message = $"{guid}: сообщение {ctr.ToString()}";
                // Отображаем сообщение клиента
                LogMessage(message);
                // Отсылаем полученное сообщение обратно клиенту
                sw.WriteLine(message);
                // Очищаем буферы для потока sw и вызываем запись всех буферизованных данных
                sw.Flush();
                // Читаем ответ от сервера
                string response = sr.ReadLine();
                // Отображаем сообщение сервера
                LogMessage(response);
                // Закрываем канал
                client.Close();
                // Увеличиваем счётчик сообщений
                ctr++;
            }
            catch (Exception err)
            {
                // Обработка ошибки err, покажи окно сообщения об ошибке err
                MessageBox.Show(err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogMessage(string message)
        {
            // Отобразить сообщение в textBox1 на Form1
            this.textBox1.AppendText(message + Environment.NewLine);
        }
    }
}
