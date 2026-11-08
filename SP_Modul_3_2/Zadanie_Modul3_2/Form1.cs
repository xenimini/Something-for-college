using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zadanie_Modul3_2
{
    public partial class Form1 : Form
    {
        private List<ProgressBar> bars = new List<ProgressBar>();
        private ListBox lstResults;
        private TextBox txtCount, txtLimit, txtFile, txtWord, txtDir;
        private Button btnDance, btnRace, btnFib, btnSearchFile, btnSearchDir;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Модуль 3 Часть 2";
            this.Size = new System.Drawing.Size(800, 600);

            txtCount = new TextBox { Location = new System.Drawing.Point(10, 10), Width = 50, Text = "5" };
            btnDance = new Button { Text = "Танцующие бары", Location = new System.Drawing.Point(70, 8), Width = 130 };
            btnDance.Click += BtnDance_Click;

            btnRace = new Button { Text = "Скачки", Location = new System.Drawing.Point(210, 8), Width = 80 };
            btnRace.Click += BtnRace_Click;

            txtLimit = new TextBox { Location = new System.Drawing.Point(300, 10), Width = 60, Text = "20" };
            btnFib = new Button { Text = "Фибоначчи", Location = new System.Drawing.Point(370, 8), Width = 90 };
            btnFib.Click += BtnFib_Click;

            txtFile = new TextBox { Location = new System.Drawing.Point(10, 40), Width = 200, Text = @"C:\temp\test.txt" };
            txtWord = new TextBox { Location = new System.Drawing.Point(220, 40), Width = 80, Text = "word" };
            btnSearchFile = new Button { Text = "Поиск в файле", Location = new System.Drawing.Point(310, 38), Width = 110 };
            btnSearchFile.Click += BtnSearchFile_Click;

            txtDir = new TextBox { Location = new System.Drawing.Point(10, 70), Width = 200, Text = @"C:\temp" };
            btnSearchDir = new Button { Text = "Поиск в папке", Location = new System.Drawing.Point(220, 68), Width = 110 };
            btnSearchDir.Click += BtnSearchDir_Click;

            lstResults = new ListBox { Location = new System.Drawing.Point(10, 110), Size = new System.Drawing.Size(760, 430) };

            this.Controls.Add(txtCount);
            this.Controls.Add(btnDance);
            this.Controls.Add(btnRace);
            this.Controls.Add(txtLimit);
            this.Controls.Add(btnFib);
            this.Controls.Add(txtFile);
            this.Controls.Add(txtWord);
            this.Controls.Add(btnSearchFile);
            this.Controls.Add(txtDir);
            this.Controls.Add(btnSearchDir);
            this.Controls.Add(lstResults);
        }

        private void BtnDance_Click(object sender, EventArgs e)
        {
            foreach (var b in bars) this.Controls.Remove(b);
            bars.Clear();
            int count = int.Parse(txtCount.Text);
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                ProgressBar pb = new ProgressBar();
                pb.Location = new System.Drawing.Point(10, 110 + i * 30);
                pb.Size = new System.Drawing.Size(400, 25);
                pb.Maximum = 100;
                bars.Add(pb);
                this.Controls.Add(pb);
                int idx = i;
                Thread t = new Thread(delegate ()
                {
                    while (pb.Value < 100)
                    {
                        Thread.Sleep(rnd.Next(50, 200));
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (pb.Value < 100) pb.Value += rnd.Next(1, 8);
                        });
                    }
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        private void BtnRace_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
            ProgressBar[] horses = new ProgressBar[5];
            int finished = 0;
            for (int i = 0; i < 5; i++)
            {
                horses[i] = new ProgressBar();
                horses[i].Location = new System.Drawing.Point(10, 110 + i * 40);
                horses[i].Size = new System.Drawing.Size(500, 30);
                horses[i].Maximum = 100;
                this.Controls.Add(horses[i]);
                int idx = i;
                Thread t = new Thread(delegate ()
                {
                    Random rnd = new Random(idx + DateTime.Now.Millisecond);
                    while (horses[idx].Value < 100)
                    {
                        Thread.Sleep(rnd.Next(30, 150));
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (horses[idx].Value < 100) horses[idx].Value += rnd.Next(1, 6);
                        });
                    }
                    int place = Interlocked.Increment(ref finished);
                    this.Invoke((MethodInvoker)delegate
                    {
                        lstResults.Items.Add("Лошадь " + (idx + 1) + " финишировала " + place + "-й");
                    });
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        private void BtnFib_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
            int limit = int.Parse(txtLimit.Text);
            Task.Run(delegate ()
            {
                long a = 0;
                long b = 1;
                int n = 0;
                while (n <= limit)
                {
                    long current = a;
                    this.Invoke((MethodInvoker)delegate
                    {
                        lstResults.Items.Add(current.ToString());
                    });
                    long temp = a + b;
                    a = b;
                    b = temp;
                    n++;
                    Thread.Sleep(20);
                }
            });
        }

        private void BtnSearchFile_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
            string file = txtFile.Text;
            string word = txtWord.Text;
            Task.Run(delegate ()
            {
                int count = 0;
                if (File.Exists(file))
                {
                    string text = File.ReadAllText(file);
                    string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                    count = words.Count(w => w.Equals(word, StringComparison.OrdinalIgnoreCase));
                }
                this.Invoke((MethodInvoker)delegate
                {
                    lstResults.Items.Add("Слово \"" + word + "\" встречается " + count + " раз(а)");
                });
            });
        }

        private void BtnSearchDir_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
            string dir = txtDir.Text;
            string word = txtWord.Text;
            Task.Run(delegate ()
            {
                if (!Directory.Exists(dir)) return;
                string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        string text = File.ReadAllText(file);
                        string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                        int count = words.Count(w => w.Equals(word, StringComparison.OrdinalIgnoreCase));
                        if (count > 0)
                        {
                            string name = Path.GetFileName(file);
                            string path = file;
                            int c = count;
                            this.Invoke((MethodInvoker)delegate
                            {
                                lstResults.Items.Add("Название файла: " + name);
                                lstResults.Items.Add("Путь к файлу: " + path);
                                lstResults.Items.Add("Количество вхождений слова: " + c);
                                lstResults.Items.Add("");
                            });
                        }
                    }
                    catch { }
                }
            });
        }
    }
}