using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeistStudio
{
    public class Util : GeistStudioWin {
        public Util() { 
        
        }

        private static void handleFileFromIndex(GeistStudioWin form, int i, bool closeFile) 
        {
            TabPage page = form.FileList.TabPages[i];
            DialogResult result = closeFile 
                ? form.ShowSaveDialog(closeFile) 
                : DialogResult.Yes;

            if (result == DialogResult.Yes || !closeFile)
                form.SaveTabFile(page);

            if (result != DialogResult.Cancel && closeFile)
            {
                form.FileList.TabPages.Remove(page);
                page.Dispose();
            }
        }
        public static void HandleFileAction(GeistStudioWin form, String type, bool effectAll = false)
        {
            if (type != "close" && type != "save")
                return;

            for (int i = form.FileList.TabPages.Count - 1; i > 0; i--)
            {
                TabPage page = form.FileList.TabPages[i];
                bool closeFile = type == "close";

                if (i != 0 && (page == form.FileList.SelectedTab || effectAll))
                    handleFileFromIndex(form, i, closeFile);

                if (!effectAll && i != 0)
                    break;
            }
        }

        public static int getTabIndex(GeistStudioWin form, TabPage cur) 
        {
            for (int i = form.FileList.TabPages.Count - 1; i > 0; i--)
            {
                if (form.FileList.TabPages[i] == cur)
                    return i;
            }
            return -1;
        }

        public static String getTabContent(GeistStudioWin form, TabPage cur) 
        {
            String content = "";

            if (!File.Exists(cur.Text))
            {
                Util.Notify(form, "Error", "This File doesn't exist");
                return content;
            }

            int fileIndex = Util.getTabIndex(form, form.FileList.SelectedTab);
            if (fileIndex < 0)
            {
                Util.Notify(form, "Error", "This File isn't opened");
                return content;
            }

            content = form.tabEditors[cur].Text;

            return content;
        }

        public static void SaveAsFile(GeistStudioWin form)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Save File";
                dialog.Size = new Size(400, 200);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.BackColor = Color.FromArgb(42, 38, 78);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label title = new Label();
                title.Text = "Save this File as";
                title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(225, 220, 245);
                title.Location = new Point(16, 10);
                title.AutoSize = true;

                Label text = new Label();
                text.Text = "What should be the file name? (without .gsScript)";
                text.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                text.ForeColor = Color.FromArgb(225, 220, 245);
                text.Location = new Point(20, 60);
                text.AutoSize = true;

                TextBox fileNameBox = new TextBox();
                fileNameBox.Location = new Point(20, 90);
                fileNameBox.Size = new Size(350, 25);
                fileNameBox.Font = new Font("Segoe UI", 10F);

                Button save = new Button();
                save.Text = "Save";
                save.Location = new Point(210, 130);
                save.ForeColor = Color.FromArgb(225, 220, 245);
                save.DialogResult = DialogResult.OK;

                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Location = new Point(300, 130);
                cancel.ForeColor = Color.FromArgb(225, 220, 245);
                cancel.DialogResult = DialogResult.Cancel;

                dialog.AcceptButton = save;
                dialog.CancelButton = cancel;

                dialog.Controls.Add(title);
                dialog.Controls.Add(text);
                dialog.Controls.Add(fileNameBox);
                dialog.Controls.Add(save);
                dialog.Controls.Add(cancel);

                if (dialog.ShowDialog(form) == DialogResult.OK)
                {
                    string fileName = fileNameBox.Text.Trim();
                    fileName += ".gsScript";

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        for (int i = form.FileList.TabPages.Count - 1; i > 0; i--)
                        {
                            TabPage page = form.FileList.TabPages[i];

                            if (i != 0 && page == form.FileList.SelectedTab) 
                            {
                                page.Text = fileName;
                                form.SaveTabFile(page);
                                form.FileList.TabPages.Remove(page);
                                form.OpenFileInNewTab(fileName, form.tabEditors[page].Text, true, true);
                                page.Dispose();
                                return;
                            }
                        }
                    }

                    Notify(form, "Error", "Please provide a valid FileName");
                }
            }
        }

        /*
        Util.Notify(this, "Information", "Projekt wurde geöffnet.");
        Util.Notify(this, "Success", "Datei erfolgreich gespeichert.");
        Util.Notify(this, "Warning", "Ungespeicherte Änderungen.");
        Util.Notify(this, "Error", "Datei konnte nicht geladen werden.");
        MessageBox.Show("Some Message");
        */
        public static async void Notify(GeistStudioWin form, string type, string msg)
        {
            Color backColor = Color.FromArgb(45, 45, 45);

            switch (type.ToLower())
            {
                case "information":
                    backColor = Color.FromArgb(0, 122, 204);
                    break;

                case "warning":
                    backColor = Color.FromArgb(255, 170, 0);
                    break;

                case "error":
                    backColor = Color.FromArgb(220, 53, 69);
                    break;

                case "success":
                    backColor = Color.FromArgb(40, 167, 69);
                    break;
            }

            Panel panel = new Panel();
            panel.Size = new Size(300, 60);
            panel.BackColor = backColor;
            panel.Tag = true;

            Label label = new Label();
            label.Dock = DockStyle.Fill;
            label.Text = msg;
            label.ForeColor = Color.White;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Padding = new Padding(15, 0, 15, 0);

            panel.Controls.Add(label);

            int margin = 15;

            panel.Left = margin;
            panel.Top = form.ClientSize.Height;

            form.Controls.Add(panel);
            panel.BringToFront();

            int index = 0;

            foreach (Control c in form.Controls)
            {
                if (c is Panel && c != panel && c.Tag is bool)
                    index++;
            }

            int targetTop = form.ClientSize.Height - ((index + 1) * (panel.Height + margin));

            while (panel.Top > targetTop)
            {
                panel.Top -= 5;
                await Task.Delay(5);
            }

            await Task.Delay(2500);

            while (panel.Top < form.ClientSize.Height)
            {
                panel.Top += 5;
                await Task.Delay(5);
            }

            form.Controls.Remove(panel);
            panel.Dispose();

            foreach (Control c in form.Controls)
            {
                if (c is Panel p && p.Tag is bool)
                {
                    p.Top += panel.Height + margin;
                }
            }
        }

        public static void gotToHome(GeistStudioWin form)
        {
            form.FileList.SelectedTab = form.home;
        }

        public static void OpenSettings() 
        {
            Settings set = new Settings();
            set.Open();
        }

        public static Terminal terminal;
        private static CppProcess cpp = new CppProcess();

        public static void OpenTerminal(GeistStudioWin form, Boolean runCode = false, String fileName = "")
        {
            if (terminal != null && !terminal.IsDisposed)
            {
                terminal.Activate();
                return;
            }

            if (terminal == null || terminal.IsDisposed)
            {
                terminal = new Terminal(runCode);
            }

            terminal.CommandEntered += (input) =>
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    string result;
                    result = cpp.Run(input);
                    terminal.Send(result);
                });
            };

            terminal.FormClosed += (s, e) =>
            {
                terminal = null;
            };

            if (runCode)
                terminal.Send(terminal.Prompt + "script " + fileName, false);

            terminal.Open();
            Util.Notify(form, "Success", "New Terminal opened successfully");
        }

        public static void ExecuteCode(GeistStudioWin form)
        {
            if (form.FileList.SelectedTab == form.home)
            {
                Util.Notify(form, "Error", "This Tab isn't a File");
                return;
            }

            TabPage file = form.FileList.SelectedTab;

            OpenTerminal(form, true, file.Text);

            String result = cpp.Run("script " + file.Text);
            terminal.Send(result);
        }
    }

    public class CppProcess : IDisposable
    {
        private readonly Process process;
        private readonly BlockingCollection<string> outputLines = new BlockingCollection<string>();
        private readonly StringBuilder errorLog = new StringBuilder();
        private readonly object errorLock = new object();
        private readonly string tempExePath;

        // Nach dem Start ausgegebener Begrüßungstext ("Type 'help' for some Commands...")
        public string Banner { get; private set; } = "";

        /// <param name="embeddedResourceName">
        /// Voller Name der eingebetteten Resource, z.B. "MeineApp.Resources.GeistOS.exe".
        /// Wenn null, wird automatisch die einzige eingebettete .exe Resource im Assembly gesucht.
        /// </param>
        public CppProcess(string embeddedResourceName = null)
        {
            tempExePath = ExtractEmbeddedExe(embeddedResourceName);

            process = new Process();

            process.StartInfo.FileName = tempExePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    outputLines.Add(StripAnsi(e.Data));
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    lock (errorLock) { errorLog.AppendLine(e.Data); }
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.StandardInput.AutoFlush = true;

            // GeistOS schreibt beim Start direkt "Type 'help' for some Commands.\n\n"
            // bevor irgendein Befehl gesendet wurde - das fangen wir separat ab.
            Banner = CollectUntilIdle(idleMs: 300, maxTotalMs: 2000, requireAny: false);
        }

        /// <summary>
        /// Kopiert die eingebettete .exe Resource in eine temporäre Datei und gibt deren Pfad zurück.
        /// </summary>
        private static string ExtractEmbeddedExe(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            if (resourceName == null)
            {
                var candidates = assembly.GetManifestResourceNames()
                    .Where(n => n.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (candidates.Length == 0)
                    throw new InvalidOperationException(
                        "Keine eingebettete .exe Resource gefunden. Vorhandene Resourcen: " +
                        string.Join(", ", assembly.GetManifestResourceNames()));

                if (candidates.Length > 1)
                    throw new InvalidOperationException(
                        "Mehrere eingebettete .exe Resourcen gefunden, bitte embeddedResourceName explizit angeben: " +
                        string.Join(", ", candidates));

                resourceName = candidates[0];
            }

            string tempPath = Path.Combine(Path.GetTempPath(), $"GeistOS_{Guid.NewGuid():N}.exe");

            using (Stream resStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resStream == null)
                    throw new InvalidOperationException(
                        $"Resource '{resourceName}' nicht gefunden. Vorhandene Resourcen: " +
                        string.Join(", ", assembly.GetManifestResourceNames()));

                using (FileStream fileStream = File.Create(tempPath))
                {
                    resStream.CopyTo(fileStream);
                }
            }

            return tempPath;
        }

        /// <summary>
        /// Führt einen Befehl im GeistOS-Terminal aus und gibt die komplette Ausgabe zurück.
        ///
        /// WICHTIG: GeistOS' run()-Schleife liest pro Durchlauf ZWEI Zeilen von stdin
        /// (siehe GeistOS.cpp, Terminal::run(), ca. Zeile 1394 und 1410):
        ///   1. Zeile -> wird nur für den Echo-/Exit-Check verwendet
        ///   2. Zeile -> wird tatsächlich als Befehl ausgeführt
        /// Deshalb senden wir den Befehl absichtlich zweimal.
        /// </summary>
        public string Run(string command, int idleMs = 200, int maxTotalMs = 15000)
        {
            if (process.HasExited)
                throw new InvalidOperationException(
                    $"Der C++ Prozess läuft nicht mehr (ExitCode {process.ExitCode}).");

            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine(command);

            string result = CollectUntilIdle(idleMs, maxTotalMs, requireAny: true);

            if (result.Length == 0)
                result = "";

            return result;
        }

        /// <summary>
        /// Sammelt Zeilen aus der Output-Queue, bis für <paramref name="idleMs"/> ms
        /// keine neue Zeile mehr ankommt (= Prozess wartet vermutlich wieder auf Input),
        /// oder bis <paramref name="maxTotalMs"/> insgesamt erreicht ist.
        /// </summary>
        private string CollectUntilIdle(int idleMs, int maxTotalMs, bool requireAny)
        {
            var sb = new StringBuilder();
            bool gotAny = false;
            var totalTimer = Stopwatch.StartNew();

            while (totalTimer.ElapsedMilliseconds < maxTotalMs)
            {
                int waitMs = gotAny
                    ? idleMs
                    : (int)Math.Max(1, maxTotalMs - totalTimer.ElapsedMilliseconds);

                if (outputLines.TryTake(out string line, waitMs))
                {
                    sb.AppendLine(line);
                    gotAny = true;
                }
                else
                {
                    break;
                }
            }

            if (requireAny && !gotAny)
                return "";

            return sb.ToString().TrimEnd('\r', '\n');
        }

        private static string StripAnsi(string input)
        {
            var sb = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\x1B')
                {
                    while (i < input.Length && input[i] != 'm')
                        i++;
                }
                else
                {
                    sb.Append(input[i]);
                }
            }
            return sb.ToString();
        }

        public void Close()
        {
            if (process.HasExited)
                return;

            try
            {
                process.StandardInput.WriteLine("exit");
                process.StandardInput.WriteLine("exit");
                process.StandardInput.Flush();
            }
            catch (InvalidOperationException)
            {
                // Prozess ist bereits weg
            }

            if (!process.WaitForExit(5000))
            {
                process.Kill();
            }
        }

        public void Dispose()
        {
            Close();
            outputLines.Dispose();
            process.Dispose();

            // Temporäre exe wieder aufräumen. Direkt nach Prozessende kann die Datei
            // kurzzeitig noch vom OS gesperrt sein, daher ein paar Versuche mit Wartezeit.
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    if (File.Exists(tempExePath))
                        File.Delete(tempExePath);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(200);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(200);
                }
            }
        }
    }
}
