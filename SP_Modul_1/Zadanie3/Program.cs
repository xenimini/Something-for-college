using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    [DllImport("kernel32.dll")]
    static extern bool Beep(uint dwFreq, uint dwDuration);

    [DllImport("user32.dll")]
    static extern bool MessageBeep(uint uType);

    static void Main()
    {
        Beep(800, 300);
        Thread.Sleep(500);
        Beep(1000, 300);
        Thread.Sleep(500);
        Beep(1200, 300);
        Thread.Sleep(700);
        MessageBeep(0x00000040);
        Thread.Sleep(500);
        MessageBeep(0x00000030);

        Console.WriteLine("Готово");
        Console.ReadKey();
    }
}