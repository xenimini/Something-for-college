using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zadanie_Modul5
{
    public partial class Form1 : Form
    {
        private TextBox txtText, txtSource, txtDest;
        private CheckBox chkSentences, chkSymbols, chkWords, chkQuestions, chkExclamations;
        private RadioButton rbScreen, rbFile;
        private Button btnAnalyze, btnStop, btnRestart, btnMoveDuplicates, btnReport;
        private ListBox lstReport;
        private CancellationTokenSource cts;
        private string lastReport = "";

        public Form1()
        {
            this.Text = "Модуль 5 Часть 1";
            this.Size = new System.Drawing.Size(750, 550);

            txtText = new TextBox { Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(500, 80), Multiline = true, Text = "Это первое предложение. Это второе? Это третье!" };

            chkSentences = new CheckBox { Text = "Предложения", Location = new System.Drawing.Point(10, 100), Checked = true };
            chkSymbols = new CheckBox { Text = "Символы", Location = new System.Drawing.Point(120, 100), Checked = true };
            chkWords = new CheckBox { Text = "Слова", Location = new System.Drawing.Point(210, 100), Checked = true };
            chkQuestions = new CheckBox { Text = "Вопросительные", Location = new System.Drawing.Point(280, 100), Checked = true };
            chkExclamations = new CheckBox { Text = "Восклицательные", Location = new System.Drawing.Point(410, 100), Checked = true };

            rbScreen = new RadioButton { Text = "На экран", Location = new System.Drawing.Point(10, 130), Checked = true };
            rbFile = new RadioButton { Text = "В файл", Location = new System.Drawing.Point(100, 130) };

            btnAnalyze = new Button { Text = "Анализ", Location = new System.Drawing.Point(10, 160), Width = 80 };
            btnStop = new Button { Text = "Стоп", Location = new System.Drawing.Point(100, 160), Width = 80 };
            btnRestart = new Button { Text = "Рестарт", Location = new System.Drawing.Point(190, 160), Width = 80 };

            btnAnalyze.Click += BtnAnalyze_Click;
            btnStop.Click += (s, e) => cts?.Cancel();
            btnRestart.Click += (s, e) => { cts?.Cancel(); BtnAnalyze_Click(null, null); };

            txtSource = new TextBox { Location = new System.Drawing.Point(10, 200), Width = 250, Text = @"C:\temp\source" };
            txtDest = new TextBox { Location = new System.Drawing.Point(270, 200), Width = 250, Text = @"C:\temp\dest" };
            btnMoveDuplicates = new Button { Text = "Перенести дубликаты", Location = new System.Drawing.Point(530, 198), Width = 150 };
            btnMoveDuplicates.Click += BtnMoveDuplicates_Click;

            btnReport = new Button { Text = "Отчёт о действиях", Location = new System.Drawing.Point(10, 230), Width = 150 };
            btnReport.Click += (s, e) => MessageBox.Show(lastReport == "" ? "Отчёта ещё нет" : lastReport);

            lstReport = new ListBox { Location = new System.Drawing.Point(10, 270), Size = new System.Drawing.Size(700, 220) };

            this.Controls.Add(txtText);
            this.Controls.Add(chkSentences);
            this.Controls.Add(chkSymbols);
            this.Controls.Add(chkWords);
            this.Controls.Add(chkQuestions);
            this.Controls.Add(chkExclamations);
            this.Controls.Add(rbScreen);
            this.Controls.Add(rbFile);
            this.Controls.Add(btnAnalyze);
            this.Controls.Add(btnStop);
            this.Controls.Add(btnRestart);
            this.Controls.Add(txtSource);
            this.Controls.Add(txtDest);
            this.Controls.Add(btnMoveDuplicates);
            this.Controls.Add(btnReport);
            this.Controls.Add(lstReport);
        }

        private async void BtnAnalyze_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();
            lstReport.Items.Clear();
            string text = txtText.Text;
            var token = cts.Token;

            try
            {
                string report = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    var lines = new List<string>();

                    if (chkSentences.Checked)
                    {
                        int sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        lines.Add("Количество предложений: " + sentences);
                    }
                    if (chkSymbols.Checked)
                        lines.Add("Количество символов: " + text.Length);
                    if (chkWords.Checked)
                    {
                        int words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        lines.Add("Количество слов: " + words);
                    }
                    if (chkQuestions.Checked)
                        lines.Add("Вопросительных предложений: " + text.Count(c => c == '?'));
                    if (chkExclamations.Checked)
                        lines.Add("Восклицательных предложений: " + text.Count(c => c == '!'));

                    return string.Join(Environment.NewLine, lines);
                }, token);

                lastReport = report;
                if (rbScreen.Checked)
                    lstReport.Items.AddRange(report.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                else
                {
                    File.WriteAllText("report.txt", report);
                    lstReport.Items.Add("Отчёт сохранён в report.txt");
                }
            }
            catch (OperationCanceledException)
            {
                lstReport.Items.Add("Анализ остановлен");
            }
        }

        private async void BtnMoveDuplicates_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text;
            string dest = txtDest.Text;
            if (!Directory.Exists(source)) { MessageBox.Show("Источник не найден"); return; }
            Directory.CreateDirectory(dest);

            var report = new List<string>();
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                var hashes = new Dictionary<string, string>();

                foreach (var file in files)
                {
                    string hash = File.ReadAllBytes(file).GetHashCode().ToString();
                    if (hashes.ContainsKey(hash))
                    {
                        report.Add("Дубликат: " + file);
                    }
                    else
                    {
                        hashes[hash] = file;
                        string destFile = Path.Combine(dest, Path.GetFileName(file));
                        if (File.Exists(destFile)) File.Delete(destFile);
                        File.Move(file, destFile);
                        report.Add("Перенесён: " + Path.GetFileName(file));
                    }
                }
            });

            lastReport = string.Join(Environment.NewLine, report);
            lstReport.Items.Clear();
            lstReport.Items.AddRange(report.ToArray());
        }
    }
}