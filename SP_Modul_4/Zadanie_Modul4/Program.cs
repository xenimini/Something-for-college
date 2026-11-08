using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Program
{
    static Mutex appMutex = new Mutex(false, "MyAppMutex_3Copies");
    static int runningCopies = 0;
    static object lockObj = new object();

    static void Main()
    {
        Console.WriteLine("1 - Задание 3 (только 3 копии)");
        Console.WriteLine("2 - Задания 4-5 (файлы + отчёт)");
        Console.WriteLine("3 - Задание 6 (казино)");
        Console.Write("Выбор: ");
        string choice = Console.ReadLine();

        if (choice == "1") Zadanie3();
        else if (choice == "2") Zadanie4_5();
        else if (choice == "3") Zadanie6();

        Console.WriteLine("Нажми Enter...");
        Console.ReadLine();
    }

    static void Zadanie3()
    {
        bool created;
        Mutex m = new Mutex(true, "Limit3Copies", out created);
        if (!created)
        {
            lock (lockObj) runningCopies++;
            if (runningCopies >= 3)
            {
                Console.WriteLine("Уже запущено 3 копии. Закрытие.");
                return;
            }
        }
        Console.WriteLine("Приложение запущено. Закрой окно консоли чтобы освободить место.");
        Console.ReadLine();
        m.ReleaseMutex();
    }

    static void Zadanie4_5()
    {
        string file1 = "numbers.txt";
        string file2 = "primes.txt";
        string file3 = "primes7.txt";
        string report = "report.txt";

        ManualResetEvent e1 = new ManualResetEvent(false);
        ManualResetEvent e2 = new ManualResetEvent(false);
        ManualResetEvent e3 = new ManualResetEvent(false);

        Thread t1 = new Thread(() =>
        {
            Random rnd = new Random();
            using (StreamWriter sw = new StreamWriter(file1))
            {
                for (int i = 0; i < 100; i++)
                    sw.WriteLine(rnd.Next(1, 1000));
            }
            e1.Set();
        });

        Thread t2 = new Thread(() =>
        {
            e1.WaitOne();
            List<int> primes = new List<int>();
            foreach (string line in File.ReadAllLines(file1))
            {
                int n = int.Parse(line);
                if (IsPrime(n)) primes.Add(n);
            }
            File.WriteAllLines(file2, primes.ConvertAll(x => x.ToString()));
            e2.Set();
        });

        Thread t3 = new Thread(() =>
        {
            e2.WaitOne();
            List<int> primes7 = new List<int>();
            foreach (string line in File.ReadAllLines(file2))
            {
                int n = int.Parse(line);
                if (n % 10 == 7) primes7.Add(n);
            }
            File.WriteAllLines(file3, primes7.ConvertAll(x => x.ToString()));
            e3.Set();
        });

        Thread t4 = new Thread(() =>
        {
            e3.WaitOne();
            using (StreamWriter sw = new StreamWriter(report))
            {
                WriteFileInfo(sw, file1);
                WriteFileInfo(sw, file2);
                WriteFileInfo(sw, file3);
            }
            Console.WriteLine("Отчёт создан: report.txt");
        });

        t1.Start(); t2.Start(); t3.Start(); t4.Start();
        t4.Join();
    }

    static void WriteFileInfo(StreamWriter sw, string path)
    {
        string[] lines = File.ReadAllLines(path);
        FileInfo fi = new FileInfo(path);
        sw.WriteLine("Файл: " + path);
        sw.WriteLine("Количество чисел: " + lines.Length);
        sw.WriteLine("Размер: " + fi.Length + " байт");
        sw.WriteLine("Содержимое:");
        foreach (string l in lines) sw.WriteLine(l);
        sw.WriteLine();
    }

    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }

    static void Zadanie6()
    {
        int totalPlayers = new Random().Next(20, 101);
        Semaphore seats = new Semaphore(5, 5);
        List<string> results = new List<string>();
        object resLock = new object();

        for (int i = 1; i <= totalPlayers; i++)
        {
            int id = i;
            new Thread(() =>
            {
                seats.WaitOne();
                int start = new Random(id).Next(100, 1001);
                int money = start;
                int bet = new Random().Next(1, money + 1);
                int number = new Random().Next(0, 37);
                int roulette = new Random().Next(0, 37);
                if (number == roulette) money += bet;
                else money -= bet;
                if (money < 0) money = 0;

                lock (resLock)
                    results.Add("Игрок" + id + " [" + start + "] [" + money + "]");

                Thread.Sleep(100);
                seats.Release();
            }).Start();
        }

        Thread.Sleep(5000);
        File.WriteAllLines("casino_report.txt", results);
        Console.WriteLine("Отчёт казино: casino_report.txt");
    }
}