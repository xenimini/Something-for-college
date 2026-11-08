#nullable disable
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Дочерний процесс запущен без аргументов");
            Thread.Sleep(3000);
            Environment.Exit(42);
        }
        else if (args.Length == 3)
        {
            int a = int.Parse(args[0]);
            int b = int.Parse(args[1]);
            string op = args[2];
            int result = 0;
            if (op == "+") result = a + b;
            else if (op == "-") result = a - b;
            else if (op == "*") result = a * b;
            else if (op == "/" && b != 0) result = a / b;

            Console.WriteLine("Аргументы: " + a + " " + op + " " + b);
            Console.WriteLine("Результат: " + result);
            Environment.Exit(0);
        }
        else if (args.Length == 2)
        {
            string path = args[0];
            string word = args[1];
            int count = 0;
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string w in words)
                {
                    if (w.Equals(word, StringComparison.OrdinalIgnoreCase))
                        count++;
                }
            }
            Console.WriteLine("Слово \"" + word + "\" встречается " + count + " раз(а) в файле " + path);
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("Неизвестные аргументы");
            Environment.Exit(1);
        }
    }
}