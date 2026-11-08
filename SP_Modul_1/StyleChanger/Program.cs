using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    const int GWL_STYLE = -16;

    static void Main()
    {
        IntPtr form = FindWindow(null, "ФормаСЭлементами");
        if (form == IntPtr.Zero)
        {
            Console.WriteLine("Форма не найдена");
            Console.ReadKey();
            return;
        }

        IntPtr textBox = FindWindowEx(form, IntPtr.Zero, "Edit", null);
        IntPtr button = FindWindowEx(form, IntPtr.Zero, "Button", null);

        if (textBox != IntPtr.Zero)
        {
            int style = GetWindowLong(textBox, GWL_STYLE);
            SetWindowLong(textBox, GWL_STYLE, style | 0x0008);
            Console.WriteLine("TextBox изменён");
        }

        if (button != IntPtr.Zero)
        {
            int style = GetWindowLong(button, GWL_STYLE);
            SetWindowLong(button, GWL_STYLE, style | 0x0001);
            Console.WriteLine("Button изменён");
        }

        Console.WriteLine("Готово");
        Console.ReadKey();
    }
}