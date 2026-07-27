using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeistStudio
{
    public partial class Terminal : Form
    {
        private RichTextBox console;
        private int inputStart;

        public string Prompt { get; set; } = "root@GeistOS:/$ ";
        public event Action<string> CommandEntered;

        private List<string> commandHistory = new List<string>();
        private int historyIndex = -1;

        private String initComp;
        private String CreateCons;
        private String AddFuncs;
        private String SendInitHelp;
        private String WriteProm;

        public Terminal(Boolean runCode)
        {
            var start = DateTime.Now;
            InitializeComponent();
            this.initComp = (String)((DateTime.Now - start).TotalMilliseconds + " ms");

            start = DateTime.Now;
            CreateConsole();
            this.CreateCons = (DateTime.Now - start).TotalMilliseconds + " ms";

            start = DateTime.Now;
            AddConsFuncs();
            this.AddFuncs = (DateTime.Now - start).TotalMilliseconds + " ms";

            start = DateTime.Now;
            Send("Type 'data' to see the initialization data", false);
            this.SendInitHelp = (DateTime.Now - start).TotalMilliseconds + " ms";

            start = DateTime.Now;
            if (!runCode)
                WritePrompt();
            this.WriteProm = (DateTime.Now - start).TotalMilliseconds + " ms";

            Shown += Terminal_Shown;
        }

        private async void Terminal_Shown(object sender, EventArgs e)
        {
            console.Focus();
        }

        private void CreateConsole()
        {
            console = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ScrollBars = RichTextBoxScrollBars.Both,
                BackColor = Color.FromArgb(18, 16, 38),
                ForeColor = Color.FromArgb(180, 220, 255),
                Font = new Font("Consolas", 11f),
                BorderStyle = BorderStyle.None,
                WordWrap = false,
                HideSelection = false,
                DetectUrls = false,
                Multiline = true,
                AcceptsTab = true,
                ShortcutsEnabled = false
            };

            Controls.Add(console);
            console.BringToFront();
        }

        private void AddConsFuncs()
        {
            console.PreviewKeyDown += Console_PreviewKeyDown;
            console.KeyDown += Console_KeyDown;
            console.KeyPress += Console_KeyPress;
        }

        public void Open()
        {
            Show();
            Activate();

            if (console != null)
                console.Focus();
        }

        public void WritePrompt()
        {
            console.ReadOnly = false;

            console.AppendText(Prompt);

            inputStart = console.TextLength;
            console.SelectionStart = inputStart;
            console.ScrollToCaret();
            console.Focus();
        }

        private void Console_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.IsInputKey = true;
            }
        }

        private void Console_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back || e.KeyCode == Keys.Left) && console.SelectionStart <= inputStart)
            {
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Home)
            {
                e.SuppressKeyPress = true;
                console.SelectionStart = inputStart;
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (commandHistory.Count > 0)
                {
                    if (historyIndex < commandHistory.Count - 1)
                        historyIndex++;

                    SetCurrentCommand(
                        commandHistory[commandHistory.Count - 1 - historyIndex]
                    );
                }

                return;
            }


            if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (commandHistory.Count > 0)
                {
                    if (historyIndex > 0)
                    {
                        historyIndex--;

                        SetCurrentCommand(
                            commandHistory[commandHistory.Count - 1 - historyIndex]
                        );
                    }
                    else
                    {
                        historyIndex = -1;
                        SetCurrentCommand("");
                    }
                }

                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string line = console.Text.Substring(inputStart).Trim();
                console.AppendText(Environment.NewLine);

                if (string.IsNullOrEmpty(line))
                {
                    WritePrompt();
                    return;
                }

                if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != line)
                {
                    commandHistory.Add(line);
                }

                historyIndex = -1;

                if (line == "clear")
                {
                    console.Text = "";
                    WritePrompt();
                    return;
                }
                else if (line == "exit")
                {
                    this.Close();
                    return;
                }
                else if (line == "data")
                {
                    String InitData = "";
                    InitData += "Initialization Data: \n";
                    InitData += $"    InitializeComponents:   {this.initComp}\n";
                    InitData += $"    CreateConsole:          {this.CreateCons}\n";
                    InitData += $"    AddConsoleFuncs:        {this.AddFuncs}\n";
                    InitData += $"    SendInitHelp:           {this.SendInitHelp}\n";
                    InitData += $"    WritePrompt:            {this.WriteProm}";
                    Send(InitData);

                    return;
                }

                console.ReadOnly = true;
                CommandEntered?.Invoke(line);
            }
        }

        private void Console_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (console.SelectionStart < inputStart)
                console.SelectionStart = console.TextLength;
        }

        private void SetCurrentCommand(string command)
        {
            console.SelectionStart = inputStart;
            console.SelectionLength = console.TextLength - inputStart;

            console.SelectedText = command;

            console.SelectionStart = console.TextLength;
            console.ScrollToCaret();
        }

        public void Send(string text, Boolean nextPrompt = true)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Boolean>(Send), text, nextPrompt);
                return;
            }

            if (!string.IsNullOrEmpty(text))
                console.AppendText(text);

            console.AppendText(Environment.NewLine);

            if (nextPrompt)
                WritePrompt();
        }
    }
}
