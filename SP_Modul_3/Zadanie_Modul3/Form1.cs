using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace Zadanie_Modul3
{
    public partial class Form1 : Form
    {
        private Thread primeThread;
        private Thread fibThread;
        private volatile bool primeRunning = false;
        private volatile bool fibRunning = false;
        private volatile bool primePaused = false;
        private volatile bool fibPaused = false;

        private TextBox txtPrimeFrom, txtPrimeTo;
        private ListBox lstPrimes, lstFib;
        private Button btnStartPrime, btnStopPrime, btnPausePrime, btnResumePrime, btnRestartPrime;
        private Button btnStartFib, btnStopFib, btnPauseFib, btnResumeFib, btnRestartFib;

        public Form1()
        {
            InitializeComponent();
            CreateControls();
        }

        private void CreateControls()
        {
            this.Text = "Многопоточность";
            this.Size = new Size(700, 500);

            Label lbl1 = new Label { Text = "От:", Location = new Point(10, 15), AutoSize = true };
            txtPrimeFrom = new TextBox { Location = new Point(40, 12), Width = 60 };
            Label lbl2 = new Label { Text = "До:", Location = new Point(110, 15), AutoSize = true };
            txtPrimeTo = new TextBox { Location = new Point(140, 12), Width = 60 };

            btnStartPrime = new Button { Text = "Старт простые", Location = new Point(220, 10), Width = 110 };
            btnStopPrime = new Button { Text = "Стоп простые", Location = new Point(340, 10), Width = 110 };
            btnPausePrime = new Button { Text = "Пауза", Location = new Point(460, 10), Width = 70 };
            btnResumePrime = new Button { Text = "Продолжить", Location = new Point(540, 10), Width = 90 };
            btnRestartPrime = new Button { Text = "Рестарт простые", Location = new Point(220, 40), Width = 120 };

            btnStartPrime.Click += (s, e) => StartPrime();
            btnStopPrime.Click += (s, e) => primeRunning = false;
            btnPausePrime.Click += (s, e) => primePaused = true;
            btnResumePrime.Click += (s, e) => primePaused = false;
            btnRestartPrime.Click += (s, e) => { primeRunning = false; Thread.Sleep(100); StartPrime(); };

            lstPrimes = new ListBox { Location = new Point(10, 80), Size = new Size(320, 350) };

            btnStartFib = new Button { Text = "Старт Фибоначчи", Location = new Point(350, 80), Width = 130 };
            btnStopFib = new Button { Text = "Стоп Фибоначчи", Location = new Point(490, 80), Width = 130 };
            btnPauseFib = new Button { Text = "Пауза", Location = new Point(350, 110), Width = 70 };
            btnResumeFib = new Button { Text = "Продолжить", Location = new Point(430, 110), Width = 90 };
            btnRestartFib = new Button { Text = "Рестарт Фибоначчи", Location = new Point(350, 140), Width = 150 };

            btnStartFib.Click += (s, e) => StartFib();
            btnStopFib.Click += (s, e) => fibRunning = false;
            btnPauseFib.Click += (s, e) => fibPaused = true;
            btnResumeFib.Click += (s, e) => fibPaused = false;
            btnRestartFib.Click += (s, e) => { fibRunning = false; Thread.Sleep(100); StartFib(); };

            lstFib = new ListBox { Location = new Point(350, 180), Size = new Size(320, 250) };

            this.Controls.AddRange(new Control[] {
                lbl1, txtPrimeFrom, lbl2, txtPrimeTo,
                btnStartPrime, btnStopPrime, btnPausePrime, btnResumePrime, btnRestartPrime,
                lstPrimes,
                btnStartFib, btnStopFib, btnPauseFib, btnResumeFib, btnRestartFib,
                lstFib
            });
        }

        private void StartPrime()
        {
            if (primeRunning) return;
            primeRunning = true;
            primePaused = false;
            lstPrimes.Items.Clear();

            int from = 2;
            int to = int.MaxValue;
            if (int.TryParse(txtPrimeFrom.Text, out int f)) from = Math.Max(2, f);
            if (int.TryParse(txtPrimeTo.Text, out int t)) to = t;

            primeThread = new Thread(() =>
            {
                for (int i = from; i <= to && primeRunning; i++)
                {
                    while (primePaused && primeRunning) Thread.Sleep(50);
                    if (!primeRunning) break;
                    if (IsPrime(i))
                    {
                        this.Invoke(() => lstPrimes.Items.Add(i));
                        Thread.Sleep(30);
                    }
                }
                primeRunning = false;
            });
            primeThread.IsBackground = true;
            primeThread.Start();
        }

        private void StartFib()
        {
            if (fibRunning) return;
            fibRunning = true;
            fibPaused = false;
            lstFib.Items.Clear();

            fibThread = new Thread(() =>
            {
                BigInteger a = 0, b = 1;
                while (fibRunning)
                {
                    while (fibPaused && fibRunning) Thread.Sleep(50);
                    if (!fibRunning) break;
                    this.Invoke(() => lstFib.Items.Add(a.ToString()));
                    BigInteger temp = a + b;
                    a = b;
                    b = temp;
                    Thread.Sleep(50);
                }
                fibRunning = false;
            });
            fibThread.IsBackground = true;
            fibThread.Start();
        }

        private bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            for (int i = 3; i * i <= n; i += 2)
                if (n % i == 0) return false;
            return true;
        }
    }
}