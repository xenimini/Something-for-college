using System;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - Задание 1 (время и дата тремя способами)");
            Console.WriteLine("2 - Задание 2 (простые числа 0-1000)");
            Console.WriteLine("3 - Задание 3 (простые числа с границами + количество)");
            Console.WriteLine("0 - Выход");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine() ?? "";

            if (choice == "1") Zadanie1();
            else if (choice == "2") Zadanie2();
            else if (choice == "3") Zadanie3();
            else if (choice == "0") break;
            else Console.WriteLine("Неверный выбор");

            Console.WriteLine("\nНажми Enter чтобы вернуться в меню...");
            Console.ReadLine();
        }
    }

    static void Zadanie1()
    {
        Task t1 = new Task(() =>
        {
            Console.WriteLine("Способ 1 (Start): " + DateTime.Now);
        });
        t1.Start();
        t1.Wait();

        Task t2 = Task.Factory.StartNew(() =>
        {
            Console.WriteLine("Способ 2 (Factory.StartNew): " + DateTime.Now);
        });
        t2.Wait();

        Task t3 = Task.Run(() =>
        {
            Console.WriteLine("Способ 3 (Task.Run): " + DateTime.Now);
        });
        t3.Wait();
    }

    static void Zadanie2()
    {
        Task task = Task.Run(() =>
        {
            for (int i = 0; i <= 1000; i++)
            {
                if (IsPrime(i))
                    Console.Write(i + " ");
            }
            Console.WriteLine();
        });
        task.Wait();
        Console.WriteLine("Задача завершена");
    }

    static void Zadanie3()
    {
        Console.Write("Нижняя граница: ");
        string inputFrom = Console.ReadLine() ?? "0";
        int from = int.TryParse(inputFrom, out int f) ? f : 0;

        Console.Write("Верхняя граница: ");
        string inputTo = Console.ReadLine() ?? "1000";
        int to = int.TryParse(inputTo, out int t) ? t : 1000;

        int count = 0;
        Task task = Task.Run(() =>
        {
            for (int i = from; i <= to; i++)
            {
                if (IsPrime(i))
                {
                    Console.Write(i + " ");
                    count++;
                }
            }
            Console.WriteLine();
        });
        task.Wait();
        Console.WriteLine("Количество простых чисел: " + count);
    }

    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;
        for (int i = 3; i * i <= n; i += 2)
            if (n % i == 0) return false;
        return true;
    }
}