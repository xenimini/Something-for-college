#nullable disable
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine("1 - Запустить и ждать завершения (Задание 1)");
        Console.WriteLine("2 - Ждать или убить (Задание 2)");
        Console.WriteLine("3 - Передать 7 3 + (Задание 3)");
        Console.WriteLine("4 - Поиск слова в файле (Задание 4)");
        Console.Write("Выбор: ");
        string choice = Console.ReadLine();

        string childPath = @"C:\Users\antik\Downloads\SQL_Project_V\WebApi\Zadanie1\Child\bin\Debug\net8.0\Child.exe";
        // ↑ ПОМЕНЯЙ ЭТОТ ПУТЬ на реальный путь к Child.exe после сборки

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = childPath;
        psi.UseShellExecute = false;

        if (choice == "1")
        {
            Process p = Process.Start(psi);
            p.WaitForExit();
            Console.WriteLine("Код завершения: " + p.ExitCode);
        }
        else if (choice == "2")
        {
            Process p = Process.Start(psi);
            Console.WriteLine("1 - ждать, 2 - убить");
            string c = Console.ReadLine();
            if (c == "1")
            {
                p.WaitForExit();
                Console.WriteLine("Код завершения: " + p.ExitCode);
            }
            else
            {
                p.Kill();
                Console.WriteLine("Процесс принудительно завершён");
            }
        }
        else if (choice == "3")
        {
            psi.Arguments = "7 3 +";
            Process p = Process.Start(psi);
            p.WaitForExit();
        }
        else if (choice == "4")
        {
            Console.Write("Путь к файлу: ");
            string file = Console.ReadLine();
            Console.Write("Слово: ");
            string word = Console.ReadLine();
            psi.Arguments = "\"" + file + "\" " + word;
            Process p = Process.Start(psi);
            p.WaitForExit();
        }

        Console.WriteLine("Нажми любую клавишу...");
        Console.ReadKey();
    }
}