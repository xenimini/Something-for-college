using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Zadanie_PLINQ
{
    public class Form1 : Form
    {
        private TextBox txtFile, txtFolder;
        private Button btnUnique, btnIncreasing, btnPositive, btnLoadOne, btnLoadFolder, btnReports;
        private ListBox lstResult;
        private List<Resume> resumes = new List<Resume>();

        public Form1()
        {
            this.Text = "PLINQ и Резюме";
            this.Size = new System.Drawing.Size(700, 500);

            txtFile = new TextBox { Location = new System.Drawing.Point(10, 10), Width = 300, Text = @"C:\temp\numbers.txt" };
            btnUnique = new Button { Text = "Уникальные", Location = new System.Drawing.Point(320, 8), Width = 100 };
            btnIncreasing = new Button { Text = "Возрастающая", Location = new System.Drawing.Point(430, 8), Width = 110 };
            btnPositive = new Button { Text = "Положительные", Location = new System.Drawing.Point(550, 8), Width = 110 };

            btnUnique.Click += BtnUnique_Click;
            btnIncreasing.Click += BtnIncreasing_Click;
            btnPositive.Click += BtnPositive_Click;

            txtFolder = new TextBox { Location = new System.Drawing.Point(10, 50), Width = 300, Text = @"C:\temp\resumes" };
            btnLoadOne = new Button { Text = "Одно резюме", Location = new System.Drawing.Point(320, 48), Width = 100 };
            btnLoadFolder = new Button { Text = "Вся папка", Location = new System.Drawing.Point(430, 48), Width = 100 };
            btnReports = new Button { Text = "Отчёты", Location = new System.Drawing.Point(540, 48), Width = 80 };

            btnLoadOne.Click += BtnLoadOne_Click;
            btnLoadFolder.Click += BtnLoadFolder_Click;
            btnReports.Click += BtnReports_Click;

            lstResult = new ListBox { Location = new System.Drawing.Point(10, 90), Size = new System.Drawing.Size(660, 350) };

            this.Controls.Add(txtFile);
            this.Controls.Add(btnUnique);
            this.Controls.Add(btnIncreasing);
            this.Controls.Add(btnPositive);
            this.Controls.Add(txtFolder);
            this.Controls.Add(btnLoadOne);
            this.Controls.Add(btnLoadFolder);
            this.Controls.Add(btnReports);
            this.Controls.Add(lstResult);
        }

        private void BtnUnique_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();
            if (!File.Exists(txtFile.Text)) { lstResult.Items.Add("Файл не найден"); return; }
            var numbers = File.ReadAllLines(txtFile.Text).Select(int.Parse).ToList();
            int unique = numbers.AsParallel().Distinct().Count();
            lstResult.Items.Add("Уникальных значений: " + unique);
        }

        private void BtnIncreasing_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();
            if (!File.Exists(txtFile.Text)) { lstResult.Items.Add("Файл не найден"); return; }
            var numbers = File.ReadAllLines(txtFile.Text).Select(int.Parse).ToList();
            int maxLen = 1;
            for (int i = 0; i < numbers.Count; i++)
            {
                int len = 1;
                for (int j = i + 1; j < numbers.Count && numbers[j] > numbers[j - 1]; j++) len++;
                if (len > maxLen) maxLen = len;
            }
            lstResult.Items.Add("Макс. длина возрастающей: " + maxLen);
        }

        private void BtnPositive_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();
            if (!File.Exists(txtFile.Text)) { lstResult.Items.Add("Файл не найден"); return; }
            var numbers = File.ReadAllLines(txtFile.Text).Select(int.Parse).ToList();
            int maxLen = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i] <= 0) continue;
                int len = 1;
                for (int j = i + 1; j < numbers.Count && numbers[j] > 0; j++) len++;
                if (len > maxLen) maxLen = len;
            }
            lstResult.Items.Add("Макс. длина положительных: " + maxLen);
        }

        private void BtnLoadOne_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Текстовые|*.txt" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var r = ParseResume(ofd.FileName);
                if (r != null) { resumes.Add(r); lstResult.Items.Add("Загружено: " + r.Name); }
            }
        }

        private void BtnLoadFolder_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtFolder.Text)) { MessageBox.Show("Папка не найдена"); return; }
            var files = Directory.GetFiles(txtFolder.Text, "*.txt");
            resumes.AddRange(files.AsParallel().Select(ParseResume).Where(r => r != null));
            lstResult.Items.Add("Загружено резюме: " + resumes.Count);
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();
            if (resumes.Count == 0) { lstResult.Items.Add("Нет резюме"); return; }

            var mostExp = resumes.AsParallel().OrderByDescending(r => r.Experience).First();
            var leastExp = resumes.AsParallel().OrderBy(r => r.Experience).First();
            var lowestSal = resumes.AsParallel().OrderBy(r => r.Salary).First();
            var highestSal = resumes.AsParallel().OrderByDescending(r => r.Salary).First();

            lstResult.Items.Add("Самый опытный: " + mostExp.Name + " (" + mostExp.Experience + " лет)");
            lstResult.Items.Add("Самый неопытный: " + leastExp.Name + " (" + leastExp.Experience + " лет)");
            lstResult.Items.Add("Самые низкие требования: " + lowestSal.Name + " (" + lowestSal.Salary + ")");
            lstResult.Items.Add("Самые высокие требования: " + highestSal.Name + " (" + highestSal.Salary + ")");

            var byCity = resumes.AsParallel().GroupBy(r => r.City);
            foreach (var g in byCity)
                lstResult.Items.Add("Город " + g.Key + ": " + string.Join(", ", g.Select(r => r.Name)));
        }

        private Resume ParseResume(string path)
        {
            try
            {
                var lines = File.ReadAllLines(path);
                return new Resume
                {
                    Name = lines.FirstOrDefault(l => l.StartsWith("Имя:"))?.Split(':')[1].Trim() ?? "Неизвестно",
                    City = lines.FirstOrDefault(l => l.StartsWith("Город:"))?.Split(':')[1].Trim() ?? "Неизвестно",
                    Experience = int.Parse(lines.FirstOrDefault(l => l.StartsWith("Опыт:"))?.Split(':')[1].Trim() ?? "0"),
                    Salary = int.Parse(lines.FirstOrDefault(l => l.StartsWith("Зарплата:"))?.Split(':')[1].Trim() ?? "0")
                };
            }
            catch { return null; }
        }
    }

    class Resume
    {
        public string Name { get; set; }
        public string City { get; set; }
        public int Experience { get; set; }
        public int Salary { get; set; }
    }
}