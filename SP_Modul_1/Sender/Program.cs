using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

    const uint WM_SETTEXT = 0x000C;
    const uint WM_CLOSE = 0x0010;

    static void Main()
    {
        IntPtr hWnd = FindWindow(null, "МоёОкно");
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("Окно не найдено");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("1 - сменить заголовок");
        Console.WriteLine("2 - закрыть окно");
        Console.Write("Выбор: ");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Новый заголовок: ");
            string newTitle = Console.ReadLine();
            SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, newTitle);
        }
        else if (choice == "2")
        {
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, null);
        }

        Console.WriteLine("Готово. Нажми любую клавишу...");
        Console.ReadKey();
    }
}