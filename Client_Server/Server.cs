using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuoteGenerator
{
    public static class Server
    {
        private const int Port = 5000;
        private const int MaxClients = 5;
        private const int MaxQuotesPerUser = 3;
        private static readonly List<string> Quotes = new()
        {
            "Жизнь — это то, что происходит с тобой, пока ты строишь другие планы. — Джон Леннон",
            "Единственный способ делать великие дела — любить то, что ты делаешь. — Стив Джобс",
            "Успех — это способность идти от неудачи к неудаче, не теряя энтузиазма. — Уинстон Черчилль",
            "Будь собой; все остальные роли уже заняты. — Оскар Уайльд",
            "Не бойся совершенства — тебе его никогда не достичь. — Сальвадор Дали",
            "Программирование — это искусство рассказывать другому человеку, что он хочет, чтобы компьютер сделал. — Дональд Кнут",
            "Простота — высшая степень сложности. — Леонардо да Винчи",
            "Лучшее время посадить дерево было 20 лет назад. Следующее лучшее время — сейчас. — Китайская пословица"
        };

        private static readonly Dictionary<string, string> Users = new()
        {
            { "user1", "pass1" },
            { "user2", "pass2" },
            { "admin", "admin123" }
        };

        private static int _currentClients = 0;
        private static readonly object _clientsLock = new();
        private static readonly ConcurrentDictionary<string, int> _userQuoteCounts = new();
        private static readonly string LogFile = "server_log.txt";
        private static readonly Random _random = new();

        public static void Start()
        {
            Console.WriteLine("=== Генератор цитат — Сервер ===");
            Console.WriteLine($"Порт: {Port}");
            Console.WriteLine($"Макс. клиентов: {MaxClients}");
            Console.WriteLine($"Макс. цитат на пользователя: {MaxQuotesPerUser}");
            Console.WriteLine("Ожидание подключений...\n");

            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var thread = new Thread(() => HandleClient(client));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        static void HandleClient(TcpClient tcpClient)
        {
            string username = null;
            var connectTime = DateTime.Now;
            var receivedQuotes = new List<string>();
            string clientEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "unknown";
            bool clientCountIncremented = false;

            try
            {
                using var stream = tcpClient.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                lock (_clientsLock)
                {
                    if (_currentClients >= MaxClients)
                    {
                        writer.WriteLine("ERROR: Сервер находится под максимальной нагрузкой. Попробуйте подключиться позже.");
                        return;
                    }
                    _currentClients++;
                    clientCountIncremented = true;
                }

                writer.WriteLine("AUTH");
                var authLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(authLine) || !authLine.Contains(':'))
                {
                    writer.WriteLine("ERROR: Неверный формат. Ожидается username:password");
                    return;
                }

                var parts = authLine.Split(':', 2);
                var user = parts[0].Trim();
                var pass = parts[1].Trim();

                if (!Users.TryGetValue(user, out var correctPass) || correctPass != pass)
                {
                    writer.WriteLine("ERROR: Неверное имя пользователя или пароль.");
                    return;
                }

                username = user;
                var logConnect = $"[{connectTime:yyyy-MM-dd HH:mm:ss}] Подключился: {username} ({clientEndPoint})";
                Console.WriteLine(logConnect);
                try { File.AppendAllText(LogFile, logConnect + Environment.NewLine); } catch { }

                writer.WriteLine("OK");

                while (true)
                {
                    var request = reader.ReadLine();
                    if (request == null || request.Trim().Equals("EXIT", StringComparison.OrdinalIgnoreCase))
                        break;

                    if (request.Trim().Equals("QUOTE", StringComparison.OrdinalIgnoreCase))
                    {
                        var currentCount = _userQuoteCounts.GetOrAdd(username, 0);
                        if (currentCount >= MaxQuotesPerUser)
                        {
                            writer.WriteLine("ERROR: Достигнуто максимальное количество цитат. Соединение будет разорвано.");
                            break;
                        }

                        string quote;
                        lock (_random)
                        {
                            quote = Quotes[_random.Next(Quotes.Count)];
                        }
                        writer.WriteLine(quote);
                        receivedQuotes.Add(quote);
                        _userQuoteCounts.AddOrUpdate(username, 1, (_, v) => v + 1);
                    }
                    else
                    {
                        writer.WriteLine("ERROR: Неизвестная команда. Используйте QUOTE или EXIT.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка с клиентом {clientEndPoint}: {ex.Message}");
            }
            finally
            {
                var disconnectTime = DateTime.Now;
                if (username != null)
                {
                    var quotesStr = receivedQuotes.Count > 0
                        ? string.Join(" | ", receivedQuotes)
                        : "(нет цитат)";
                    var logDisconnect = $"[{disconnectTime:yyyy-MM-dd HH:mm:ss}] Отключился: {username} ({clientEndPoint}). Цитаты: {quotesStr}";
                    Console.WriteLine(logDisconnect);
                    try { File.AppendAllText(LogFile, logDisconnect + Environment.NewLine); } catch { }
                }

                if (clientCountIncremented)
                {
                    lock (_clientsLock)
                    {
                        if (_currentClients > 0)
                            _currentClients--;
                    }
                }
                tcpClient.Close();
            }
        }
    }
}