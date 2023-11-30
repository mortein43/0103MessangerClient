using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace _0103MessangerClient
{
    internal class Program
    {
        static string? userName;

       
        static async Task SendMessageAsync(StreamWriter writer, string? userName)
        {
            
            await writer.WriteLineAsync(userName);
            await writer.FlushAsync();
            Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");

            while (true)
            {
                string? message = Console.ReadLine();
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }
        
        static async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    
                    string? message = await reader.ReadLineAsync();
                    
                    if (string.IsNullOrEmpty(message)) continue;
                    Print(message);
                }
                catch
                {
                    break;
                }
            }
        }
        
        static void Print(string message)
        {
            if (OperatingSystem.IsWindows())    
            {
                var position = Console.GetCursorPosition(); 
                int left = position.Left;   
                int top = position.Top;     
                
                Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
                
                Console.SetCursorPosition(0, top);
                
                Console.WriteLine(message);
                
                Console.SetCursorPosition(left, top + 1);
            }
            else Console.WriteLine(message);
        }

        static async Task Main(string[] args)
        {
            string host = "192.168.31.76";
            int port = 13003;
            using TcpClient client = new TcpClient();
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            Console.WriteLine($"Добро пожаловать, {userName}");
            StreamReader? Reader = null;
            StreamWriter? Writer = null;

            try
            {
                await client.ConnectAsync(host, port); 
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                if (Writer is null || Reader is null) return;
                
                _ = Task.Run(() => ReceiveMessageAsync(Reader));
                
                await SendMessageAsync(Writer, userName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Writer?.Close();
            Reader?.Close();
        }
    }
}
