using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    static void Main()
    {
        MessageBox(IntPtr.Zero, "Имя: Xenimini", "Информация 1", 0);
        MessageBox(IntPtr.Zero, "Город: Симф", "Информация 2", 0);
        MessageBox(IntPtr.Zero, "РПО 3 курс", "Информация 3", 0);
    }
}