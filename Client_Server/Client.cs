using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace QuoteGenerator
{
    public static class Client
    {
        private const int Port = 5000;

        public static void Start()
        {
            Console.WriteLine("=== Генератор цитат — Клиент ===\n");

            Console.Write("IP-адрес сервера (Enter = 127.0.0.1): ");
            var ip = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(ip))
                ip = "127.0.0.1";

            Console.Write("Имя пользователя: ");
            var username = Console.ReadLine()?.Trim();

            Console.Write("Пароль: ");
            var password = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Имя пользователя и пароль обязательны.");
                return;
            }

            try
            {
                using var client = new TcpClient();
                Console.WriteLine($"\nПодключение к {ip}:{Port}...");
                client.Connect(ip, Port);

                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                var response = reader.ReadLine();
                if (response != "AUTH")
                {
                    Console.WriteLine($"Неожиданный ответ сервера: {response}");
                    return;
                }

                writer.WriteLine($"{username}:{password}");
                response = reader.ReadLine();

                if (response != "OK")
                {
                    Console.WriteLine($"Ошибка подключения: {response}");
                    return;
                }

                Console.WriteLine("Успешно подключено!\n");
                Console.WriteLine("Команды: QUOTE — получить цитату, EXIT — выйти\n");

                while (true)
                {
                    Console.Write("> ");
                    var command = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(command))
                        continue;

                    writer.WriteLine(command);

                    if (command.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Отключение...");
                        break;
                    }

                    response = reader.ReadLine();
                    if (response == null)
                    {
                        Console.WriteLine("Соединение разорвано сервером.");
                        break;
                    }

                    Console.WriteLine($"Сервер: {response}");

                    if (response.StartsWith("ERROR"))
                        break;
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Не удалось подключиться к серверу. Проверьте IP и что сервер запущен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите режим:");
            Console.WriteLine("1 — Сервер");
            Console.WriteLine("2 — Клиент");
            Console.Write("Ваш выбор (1/2): ");

            var choice = Console.ReadLine()?.Trim();

            if (choice == "1")
                Server.Start();
            else if (choice == "2")
                Client.Start();
            else
                Console.WriteLine("Неверный выбор.");
        }
    }
}