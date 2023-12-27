// Server
using System;
using System.Text;
using System.IO.Pipes;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Server
{
    public partial class Form1 : Form
    {
        // Объявлем лист серверов (подключённых клиентов к серверу)
        static readonly List<NamedPipeServerStream> servers = new List<NamedPipeServerStream>();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Зпуск задачи-сервера
            Task.Run(() => StartServer());
        }
        private async Task StartServer()
        {
            // Цикл
            while (true)
            {
                // Объявляем именованный канал сервера server
                NamedPipeServerStream server = new NamedPipeServerStream("myPipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                // Ждём подключение клиентов
                await server.WaitForConnectionAsync();
                // Добавляем подключённого клента в лист
                servers.Add(server);
                // Запуск прослушивания для подключённого клиента
                _ = Task.Run(() => HandleClient(server));
            }
        }
        private async Task HandleClient(NamedPipeServerStream server)
        {
            // Кол-во считанный байт сообщения от клиента
            int bytesRead;
            // Буффер сообщения от клиента
            byte[] buffer = new byte[1024];
            // Цикл while
            while (true)
            {
                // Чиатем сообщение от клиента
                bytesRead = await server.ReadAsync(buffer, 0, buffer.Length);
                // Если сообщение от клиента пустое выходи из цикла
                if (bytesRead == 0)
                {
                    break;
                }
                // Переводи соообщение от клиента в строку
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Отображаем полученное сообщение от клиента
                LogMessage(message);
                // Отправляем сообщение всем другим клиентам
                foreach (NamedPipeServerStream other_client in servers)
                {
                    if (other_client != server && other_client.IsConnected)
                    {
                        await other_client.WriteAsync(buffer, 0, bytesRead);
                        await other_client.FlushAsync();
                    }
                }
            }
            // Закрываем канал
            server.Close();
            // Удаляем сервер из листа серверов
            servers.Remove(server);
        }
        private void LogMessage(string message)
        {
            // Отобразить сообщение в textBox1 на Form1
            textBox1.Invoke((MethodInvoker)delegate { textBox1.AppendText(message); });
        }
    }
}
