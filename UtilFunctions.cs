/*
 * Copyright (C) 2026 Geistvk
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Runtime.Caching;

namespace GeistStudio
{
    public class Util : GeistStudioWin {
        public Util() { 
        
        }

        [DllImport("user32.dll")]
        private static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

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
                dialog.BackColor = Util.Config.Colors.Background.Dialog.Background;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                Util.CreateCustomTitleBar(dialog, "Save File As", true);

                Label title = new Label();
                title.Text = "Save this File as";
                title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                title.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                title.Location = new Point(16, 40);
                title.AutoSize = true;

                Label text = new Label();
                text.Text = "What should be the file name? (without .gsScript)";
                text.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                text.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                text.Location = new Point(20, 90);
                text.AutoSize = true;

                TextBox fileNameBox = new TextBox();
                fileNameBox.Location = new Point(20, 120);
                fileNameBox.Size = new Size(350, 25);
                fileNameBox.Font = new Font("Segoe UI", 10F);

                Button save = new Button();
                save.Text = "Save";
                save.Location = new Point(210, 160);
                save.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                save.DialogResult = DialogResult.OK;

                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Location = new Point(300, 160);
                cancel.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
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
            Color backColor = Util.Config.Colors.Background.Notify.Standard;

            switch (type.ToLower())
            {
                case "information":
                    backColor = Util.Config.Colors.Background.Notify.Information;
                    break;

                case "warning":
                    backColor = Util.Config.Colors.Background.Notify.Warning;
                    break;

                case "error":
                    backColor = Util.Config.Colors.Background.Notify.Error;
                    break;

                case "success":
                    backColor = Util.Config.Colors.Background.Notify.Success;
                    break;
            }

            Panel panel = new Panel();
            panel.Size = new Size(300, 60);
            panel.BackColor = backColor;
            panel.Tag = true;

            Label label = new Label();
            label.Dock = DockStyle.Fill;
            label.Text = msg;
            label.ForeColor = Util.Config.Colors.Foreground.Text;
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

        private static Point lastToolTipPos = Point.Empty;
        private static Control lastToolTipControl = null;
        private static string lastToolTipText = "";

        public static void addBtnToTitleBar(
            GeistStudioWin form, 
            Panel parent, 
            String btn, 
            String desc, 
            Action func
        ) {
            Button customBtn = new Button();
            customBtn.Text = btn;
            customBtn.Width = 45;
            customBtn.TabStop = false;
            customBtn.Cursor = Cursors.Hand;
            customBtn.Dock = DockStyle.Right;
            customBtn.FlatStyle = FlatStyle.Flat;
            customBtn.FlatAppearance.BorderSize = 0;
            customBtn.Font = new Font("Segoe UI Symbol", 12);
            customBtn.ForeColor = Util.Config.Colors.Foreground.TitleBarButton;

            customBtn.MouseEnter += (s, e) => ShowStyledToolTip(form, parent, desc);
            customBtn.MouseMove += (s, e) => UpdateStyledToolTip(form, parent, desc);
            customBtn.MouseLeave += (s, e) => HideStyledToolTip(form, parent);
            customBtn.Click += (s, e) => func();

            parent.Controls.Add(customBtn);
        }

        private static void HideStyledToolTip(GeistStudioWin form, Control parent)
        {
            form.StyledToolTip.Hide(parent);

            lastToolTipControl = null;
            lastToolTipText = "";
            lastToolTipPos = Point.Empty;
        }

        private static void UpdateStyledToolTip(
            GeistStudioWin form,
            Control parent,
            string desc
        ) {
            Point pos = parent.PointToClient(Cursor.Position);
            pos.Offset(16, 24);

            if (lastToolTipControl == parent &&
                lastToolTipText == desc &&
                Math.Abs(pos.X - lastToolTipPos.X) < 4 &&
                Math.Abs(pos.Y - lastToolTipPos.Y) < 4)
                return;

            lastToolTipPos = pos;
            lastToolTipControl = parent;
            lastToolTipText = desc;

            form.StyledToolTip.Hide(parent);
            form.StyledToolTip.Show(desc, parent, pos);
        }

        private static void ShowStyledToolTip(
            GeistStudioWin form,
            Control parent,
            string desc
        ) {
            if (string.IsNullOrWhiteSpace(desc))
                return;

            Point pos = parent.PointToClient(Cursor.Position);
            pos.Offset(16, 24);

            lastToolTipPos = pos;
            lastToolTipControl = parent;
            lastToolTipText = desc;

            form.StyledToolTip.Hide(parent);
            form.StyledToolTip.Show(desc, parent, pos);
        }

        private static Button CreateWindowButton(string text, bool isClose = false)
        {
            Color closeColor = Util.Config.Colors.Background.TitleBarCloseButton;
            Color normalButtonHover = Util.Config.Colors.Background.TitleBarButtonHover;
            Color normalButtonColor = Util.Config.Colors.Background.TitleBarButton;

            Button btn = new Button();

            btn.Text = text;
            btn.Width = 45;
            btn.Cursor = Cursors.Hand;
            btn.Dock = DockStyle.Right;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = normalButtonColor;
            btn.Font = new Font("Segoe UI Symbol", 12);
            btn.ForeColor = Util.Config.Colors.Foreground.TitleBarButton;

            btn.MouseEnter += (s, e) => btn.BackColor = !isClose ? normalButtonHover : closeColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalButtonColor;

            return btn;
        }

        public static void CreateCustomTitleBar(
            Form form, 
            String text, 
            Boolean isDialog = false, 
            Boolean isMain = false
        ) {
            form.FormBorderStyle = FormBorderStyle.None;

            Panel titleBar = new Panel();
            titleBar.Dock = DockStyle.Top;
            titleBar.Height = 35;
            titleBar.BackColor = Util.Config.Colors.Background.Titlebar;

            form.Controls.Add(titleBar);

            PictureBox appIcon = new PictureBox();
            appIcon.Size = new Size(25, 25);
            appIcon.Location = new Point(10, (titleBar.Height - appIcon.Height) / 2);
            appIcon.SizeMode = PictureBoxSizeMode.StretchImage;

            if (form.Icon != null)
            {
                appIcon.Image = form.Icon.ToBitmap();
            }

            titleBar.Controls.Add(appIcon);

            Label title = new Label();
            title.Text = text;
            title.ForeColor = Util.Config.Colors.Foreground.TitleBarText;
            title.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            title.Location = new Point(appIcon.Right + 8, 8);
            title.AutoSize = true;

            titleBar.Controls.Add(title);

            if (isMain)
            {
                addBtnToTitleBar(
                    (GeistStudioWin)form, 
                    titleBar, 
                    "▶", 
                    "Run the GeistScript Code", 
                    () => ExecuteCode((GeistStudioWin)form)
                );
                addBtnToTitleBar(
                    (GeistStudioWin)form, 
                    titleBar, 
                    "💻", 
                    "Open a new Terminal", 
                    () => OpenTerminal((GeistStudioWin)form)
                );
                addBtnToTitleBar(
                    (GeistStudioWin)form, 
                    titleBar, 
                    "⛭", 
                    "Open the Settings", 
                    () => OpenSettings()
                );
            }

            if (!isDialog)
            {
                Button minimizeButton = CreateWindowButton("─");
                minimizeButton.Dock = DockStyle.Right;

                Button maximizeButton = CreateWindowButton("□");
                maximizeButton.Dock = DockStyle.Right;

                titleBar.Controls.Add(minimizeButton);
                titleBar.Controls.Add(maximizeButton);

                minimizeButton.Click += (s, e) => form.WindowState = FormWindowState.Minimized;
                maximizeButton.Click += (s, e) => {
                    form.WindowState =
                        form.WindowState == FormWindowState.Maximized
                        ? FormWindowState.Normal
                        : FormWindowState.Maximized;
                };
            }

            Button closeButton = CreateWindowButton("×", true);
            closeButton.Dock = DockStyle.Right;

            closeButton.Click += (s, e) => form.Close();

            titleBar.Controls.Add(closeButton);

            titleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(
                        form.Handle,
                        0xA1,
                        0x2,
                        0);
                }
            };
        }

        public static void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            //Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                // Console.WriteLine("stop wait timer");
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }


        public static void gotToHome(GeistStudioWin form)
        {
            form.FileList.SelectedTab = form.home;
        }

        public static Settings set;
        public static Information info;
        public static About about;
        public static Terminal terminal;
        public static CppProcess cpp = new CppProcess();

        public static void OpenSettings() 
        {
            if (set == null || set.IsDisposed)
                set = new Settings();
            else
                set.BringToFront();
            set.Open();
        }

        public static void openInformation()
        {
            if (info == null || info.IsDisposed)
                info = new Information();
            else
                info.BringToFront();
            info.Open();
        }

        public static void openAbout(GeistStudioWin form)
        {
            if (about == null || about.IsDisposed)
                about = new About();
            else
                about.BringToFront();
            about.Open(form);
        }

        public static string RequestHiddenInput(string prompt)
        {
            using (var done = new SemaphoreSlim(0, 1))
            {
                string result = null;

                terminal.Invoke((Action)(() =>
                {
                    using (var dlg = new MaskedInputDialog(prompt))
                    {
                        dlg.ShowDialog(terminal.FindForm());
                        result = dlg.Result;
                    }
                    done.Release();
                }));

                done.Wait();
                return result ?? "";
            }
        }

        public static void OpenTerminal(GeistStudioWin form, Boolean runCode = false, String fileName = "")
        {
            if (terminal != null && !terminal.IsDisposed)
            {
                terminal.Activate();
                return;
            }

            if (terminal == null || terminal.IsDisposed)
                terminal = new Terminal(runCode);
            else
                terminal.BringToFront();

            terminal.CommandEntered += (input) =>
            {
                Task.Run(() =>
                {
                    string result = cpp.RunInteractive(input, prompt => RequestHiddenInput(prompt));
                    terminal.Send(result);
                });
            };

            /*terminal.FormClosed += (s, e) =>
            {
                terminal = null;
            };*/

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


        public static void simulateKeyPress(String[] Keys)
        {
            String keyPress = "";
            String opKey = "";

            for (int i = 0; i < Keys.Length; i++) {
                String Key = Keys[i]; 

                if (Key == "Ctrl") keyPress += "^";
                else if (Key == "Shift") keyPress += "+";
                else if (Key == "Alt") keyPress += "%";

                if (Key == "Ctrl" || Key == "Shift" || Key == "Alt")
                {
                    opKey = Key;
                    keyPress += "(";
                }
                else
                    keyPress += Key;

                if ((opKey == "Ctrl" || opKey == "Shift" || opKey == "Alt") && i == Keys.Length - 1)
                    keyPress += ")";

            }

            SendKeys.Send(keyPress);
        }








        public class AllColors
        {
            public dynamic Background { get; set; }
            public dynamic Foreground { get; set; }

            public AllColors()
            {
                Background = new DynamicColorGroup();
                Foreground = new DynamicColorGroup();
            }
        }

        public class ConfigThis
        {
            public AllColors Colors { get; private set; }

            public ConfigThis()
            {
                Colors = new AllColors();
            }
        }

        public static ConfigThis Config = LoadColors();

        public static ConfigThis LoadColors()
        {
            ConfigThis configThis = new ConfigThis();

            Dictionary<string, object> root = (Dictionary<string, object>)JsonParser.LoadEmbeddedJson("GeistStudio.GeistStudioData.json");

            Dictionary<string, object> config = (Dictionary<string, object>)root["Config"];
            Dictionary<string, object> colors = (Dictionary<string, object>)config["Colors"];

            configThis.Colors.Background = LoadGroup((Dictionary<string, object>)colors["Background"]);
            configThis.Colors.Foreground = LoadGroup((Dictionary<string, object>)colors["Foreground"]);

            return configThis;
        }

        private static DynamicColorGroup LoadGroup(Dictionary<string, object> data)
        {
            DynamicColorGroup group = new DynamicColorGroup();

            foreach (KeyValuePair<string, object> entry in data)
            {
                if (entry.Value is string)
                {
                    string[] rgb = entry.Value.ToString().Split(',');

                    group.AddColor(
                        entry.Key,
                        Color.FromArgb(
                            int.Parse(rgb[0].Trim()),
                            int.Parse(rgb[1].Trim()),
                            int.Parse(rgb[2].Trim())
                        )
                    );
                }
                else
                {
                    group.AddGroup(
                        entry.Key,
                        LoadGroup((Dictionary<string, object>)entry.Value)
                    );
                }
            }

            return group;
        }

        public static void cacheData(bool Data = true)
        {
            // Create a MemoryCache instance
            var cache = MemoryCache.Default;

            // Define cache key and data
            string cacheKey = "MyCachedData";
            string cachedData = Data ? "True" : "False";

            // Add data to the cache with an expiration time of 5 minutes
            CacheItemPolicy cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
            };

            cache.Add(cacheKey, cachedData, cachePolicy);
        }

        public static void loadCacheData()
        {
            // Create a MemoryCache instance
            var cache = MemoryCache.Default;

            // Define cache key and data
            string cacheKey = "MyCachedData";

            // Add data to the cache with an expiration time of 5 minutes
            CacheItemPolicy cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
            };

            // Retrieve data from the cache
            string retrievedData = cache.Get(cacheKey) as string;

            if (retrievedData != null)
            {
                bool storedData = retrievedData == "True";

                MessageBox.Show("Data retrieved from cache: " + (storedData ? "Yes" : "No"));
            }
            else
            {
                MessageBox.Show("Data not found in cache.");
            }
        }
    }

    public class DynamicColorGroup : DynamicObject
    {
        private readonly Dictionary<string, object> _values;

        public DynamicColorGroup()
        {
            _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddColor(string name, Color color)
        {
            _values[name] = color;
        }

        public void AddGroup(string name, DynamicColorGroup group)
        {
            _values[name] = group;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _values.TryGetValue(binder.Name, out result);
        }

        public object this[string name]
        {
            get
            {
                object value;

                if (!_values.TryGetValue(name, out value))
                    throw new Exception("Element \"" + name + "\" does not exist.");

                return value;
            }
        }
    }

    public class CppProcess : IDisposable
    {
        private readonly Process process;

        // Zeichenweise statt zeilenweise gepuffert: BeginOutputReadLine liefert
        // erst dann etwas, wenn ein '\n' im Stream ankommt. Prompts wie
        // "Passwort: ", die GeistOS OHNE folgendes std::endl ausgibt, wuerden
        // damit nie (oder viel zu spaet) bei uns ankommen. Deshalb pumpen wir
        // den Stream selbst, Zeichen fuer Zeichen, in eine BlockingCollection.
        private readonly BlockingCollection<char> outputChars = new BlockingCollection<char>();
        private readonly Thread stdoutPump;
        private readonly Thread stderrPump;

        private readonly StringBuilder errorLog = new StringBuilder();
        private readonly object errorLock = new object();
        private readonly string tempExePath;

        // Nach dem Start ausgegebener Begruessungstext ("Type 'help' for some Commands...")
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

            process.Start();

            // WICHTIG: Kein BeginOutputReadLine mehr. Stattdessen lesen wir in
            // eigenen Hintergrund-Threads roh Zeichen fuer Zeichen, damit auch
            // ungeflushte / nicht mit '\n' abgeschlossene Prompts (z.B. eine
            // Passwortabfrage) bei uns landen, sobald sie im Pipe-Puffer stehen.
            stdoutPump = new Thread(() => PumpStdOut(process.StandardOutput))
            {
                IsBackground = true,
                Name = "CppProcess-stdout-pump"
            };
            stdoutPump.Start();

            stderrPump = new Thread(() => PumpStdErr(process.StandardError))
            {
                IsBackground = true,
                Name = "CppProcess-stderr-pump"
            };
            stderrPump.Start();

            process.StandardInput.AutoFlush = true;

            // GeistOS schreibt beim Start direkt "Type 'help' for some Commands.\n\n"
            // bevor irgendein Befehl gesendet wurde - das fangen wir separat ab.
            Banner = CollectUntilIdle(idleMs: 300, maxTotalMs: 2000, requireAny: false);
        }

        private void PumpStdOut(StreamReader reader)
        {
            try
            {
                int ch;
                while ((ch = reader.Read()) != -1)
                {
                    outputChars.Add((char)ch);
                }
            }
            catch (ObjectDisposedException) { /* Prozess/Reader wurde geschlossen */ }
            catch (IOException) { /* Pipe wurde geschlossen (Prozessende) */ }
            finally
            {
                try { outputChars.CompleteAdding(); } catch (InvalidOperationException) { }
            }
        }

        private void PumpStdErr(StreamReader reader)
        {
            try
            {
                int ch;
                while ((ch = reader.Read()) != -1)
                {
                    lock (errorLock) { errorLog.Append((char)ch); }
                }
            }
            catch (ObjectDisposedException) { }
            catch (IOException) { }
        }

        /// <summary>
        /// Kopiert die eingebettete .exe Resource in eine temporaere Datei und gibt deren Pfad zurueck.
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
        /// Fuehrt einen TOP-LEVEL Befehl im GeistOS-Terminal aus und gibt die komplette Ausgabe zurueck.
        ///
        /// WICHTIG: GeistOS' run()-Schleife liest pro Durchlauf ZWEI Zeilen von stdin
        /// (siehe GeistOS.cpp, Terminal::run(), ca. Zeile 1394 und 1410):
        ///   1. Zeile -> wird nur fuer den Echo-/Exit-Check verwendet
        ///   2. Zeile -> wird tatsaechlich als Befehl ausgefuehrt
        /// Deshalb senden wir den Befehl absichtlich zweimal.
        ///
        /// Loest der Befehl selbst einen Folge-Prompt aus (z.B. eine Passwortabfrage
        /// via getHiddenInput()), NICHT Run() erneut aufrufen, sondern SendLine()
        /// benutzen - diese Prompts lesen naemlich nur EINE Zeile pro Aufruf.
        /// </summary>
        public string Run(string command, int idleMs = 200, int maxTotalMs = 15000)
        {
            if (process.HasExited)
                throw new InvalidOperationException(
                    $"Der C++ Prozess laeuft nicht mehr (ExitCode {process.ExitCode}).");

            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine(command);

            return CollectUntilIdle(idleMs, maxTotalMs, requireAny: true);
        }

        /// <summary>
        /// Schluesselwoerter, bei deren Vorkommen im EINGEGEBENEN BEFEHL (nicht im
        /// Output!) davon ausgegangen wird, dass der Befehl eine maskierte
        /// Eingabe (z.B. ein Passwort) ausloesen wird. Bei Bedarf erweitern,
        /// z.B. um "passwd", "login", etc.
        /// </summary>
        public static readonly string[] DefaultHiddenInputCommands =
        {
            "sudo", "passwd"
        };

        /// <summary>
        /// Prueft, ob der eingegebene Befehl selbst (nicht dessen Ausgabe) darauf
        /// hindeutet, dass eine maskierte Eingabe folgen wird - z.B. weil "sudo"
        /// im Befehl vorkommt.
        /// </summary>
        public static bool CommandTriggersHiddenInput(string command)
        {
            if (string.IsNullOrEmpty(command))
                return false;

            foreach (var keyword in DefaultHiddenInputCommands)
            {
                if (command.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Fuehrt einen Befehl aus wie Run(), erkennt aber schon VOR der Ausfuehrung
        /// anhand des eingegebenen Befehls selbst (nicht anhand des Outputs), ob
        /// danach eine maskierte Eingabe folgen wird - standardmaessig, wenn "sudo"
        /// im Befehl vorkommt (siehe DefaultHiddenInputCommands) - und beantwortet
        /// sie ueber die uebergebene Callback-Funktion.
        ///
        /// <paramref name="onHiddenInputRequested"/> bekommt den bis dahin gesammelten
        /// Output (z.B. den "[sudo] Passwort fuer ..." Prompt) und MUSS synchron
        /// (blockierend) den vom Nutzer eingegebenen, unmaskierten Wert zurueckgeben -
        /// typischerweise indem euer Terminal-UI auf ein Passwort-Eingabefeld
        /// umschaltet und wartet, bis Enter gedrueckt wird.
        ///
        /// Im zurueckgegebenen Gesamt-Output wird die Eingabe selbst NICHT im Klartext
        /// angezeigt (nur "****"), damit nichts versehentlich im Terminal auftaucht.
        ///
        /// Befehle, die keines der DefaultHiddenInputCommands-Schluesselwoerter
        /// enthalten, verhalten sich exakt wie Run().
        /// </summary>
        public string RunInteractive(
            string command,
            Func<string, string> onHiddenInputRequested,
            Func<string, bool> commandTriggersHiddenInput = null,
            int idleMs = 200,
            int maxTotalMs = 15000,
            int maxPrompts = 1)
        {
            if (commandTriggersHiddenInput == null)
                commandTriggersHiddenInput = CommandTriggersHiddenInput;

            if (process.HasExited)
                throw new InvalidOperationException($"Der C++ Prozess laeuft nicht mehr (ExitCode {process.ExitCode}).");

            var combined = new StringBuilder();

            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine(command);

            string chunk = CollectUntilIdle(idleMs, maxTotalMs, requireAny: true);
            combined.Append(chunk);

            if (commandTriggersHiddenInput(command))
            {
                for (int i = 0; i < maxPrompts; i++)
                {
                    if (process.HasExited)
                        break;

                    string secret = onHiddenInputRequested != null ? onHiddenInputRequested(chunk) : "";

                    process.StandardInput.WriteLine(secret);

                    String stars = "";
                    foreach (char c in secret)
                        stars += "*";
                    stars += "\n";

                    combined.Append(stars);

                    chunk = CollectUntilIdle(idleMs, maxTotalMs, requireAny: false);
                    combined.Append(chunk);
                }
            }

            return combined.ToString();
        }

        /// <summary>
        /// Sendet EINE einzelne Zeile an den Prozess (KEIN Verdoppeln wie bei Run()).
        ///
        /// Zu benutzen fuer Antworten auf Prompts, die ein Befehl selbst waehrend
        /// seiner Ausfuehrung stellt, z.B. "Benutzername: " / "Passwort: " ueber
        /// getHiddenInput(). Diese lesen (nach dem C++-Fix auf std::getline)
        /// jeweils nur eine Zeile - ein zweites WriteLine wuerde als naechster
        /// Befehl fehlinterpretiert werden.
        /// </summary>
        public string SendLine(string text, int idleMs = 200, int maxTotalMs = 15000)
        {
            if (process.HasExited)
                throw new InvalidOperationException(
                    $"Der C++ Prozess laeuft nicht mehr (ExitCode {process.ExitCode}).");

            process.StandardInput.WriteLine(text);

            return CollectUntilIdle(idleMs, maxTotalMs, requireAny: false);
        }

        /// <summary>
        /// Wartet, bis der bisher angekommene (aber noch nicht abgeholte) Output
        /// einen bestimmten Teilstring enthaelt, z.B. um sicherzugehen, dass der
        /// "Passwort:"-Prompt wirklich angekommen ist, bevor man antwortet.
        /// Gibt den bis dahin gesammelten (ANSI-bereinigten) Output zurueck.
        /// </summary>
        public string WaitForPrompt(string expectedSubstring, int pollMs = 20, int maxTotalMs = 5000)
        {
            var sb = new StringBuilder();
            var timer = Stopwatch.StartNew();

            while (timer.ElapsedMilliseconds < maxTotalMs)
            {
                while (outputChars.TryTake(out char c, 0))
                    sb.Append(c);

                string current = StripAnsi(sb.ToString());
                if (current.Contains(expectedSubstring))
                    return current;

                Thread.Sleep(pollMs);
            }

            return StripAnsi(sb.ToString());
        }

        /// <summary>
        /// Sammelt Zeichen aus der Output-Queue, bis fuer <paramref name="idleMs"/> ms
        /// keine neuen Zeichen mehr ankommen (= Prozess wartet vermutlich wieder auf Input),
        /// oder bis <paramref name="maxTotalMs"/> insgesamt erreicht ist. Faengt dank
        /// zeichenweisem Lesen auch Prompts ohne abschliessendes '\n' ab.
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

                if (outputChars.TryTake(out char c, waitMs))
                {
                    sb.Append(c);
                    gotAny = true;
                }
                else
                {
                    break;
                }
            }

            if (requireAny && !gotAny)
                return "";

            return StripAnsi(sb.ToString()).TrimEnd('\r', '\n');
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

            // Pumps sollten sich nach Prozessende selbst beenden (Read() liefert -1
            // bzw. wirft eine IOException). Kurz warten, dann Ressourcen freigeben.
            stdoutPump.Join(1000);
            stderrPump.Join(1000);

            outputChars.Dispose();
            process.Dispose();

            // Temporaere exe wieder aufraeumen. Direkt nach Prozessende kann die Datei
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
