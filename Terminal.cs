using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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

        private String[] initializationData;
        private String[] initHeader;

        public Terminal(Boolean runCode)
        {
            initHeader = new string[] { 
                "InitializeComponent", 
                "CreateConsole",
                "AddConsoleFunctions",
                "DisplayInitializationHelp",
                "WritePrompt"
            };

            initializationData = new string[] {
                addTrackingData(() => InitializeComponent()),
                addTrackingData(() => CreateConsole()),
                addTrackingData(() => AddConsFuncs()),
                addTrackingData(() => Send("Type 'data' to see the initialization data", false)),
                addTrackingData(() => {
                    if (!runCode)
                        WritePrompt();
                })
            };

            Shown += Terminal_Shown;
        }

        private String addTrackingData(Action func)
        {
            var start = DateTime.Now;
            func();
            return (String)((DateTime.Now - start).TotalMilliseconds + " ms");
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
                ShortcutsEnabled = true
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
                    int maxWidth = this.initHeader.Max(h => h.Length) + 2;
                    String InitData = "";
                    InitData += "Initialization Data: \n";
                    for (int i = 0; i < initializationData.Length; i++)
                    {
                        InitData += $"    {this.initHeader[i].PadRight(maxWidth)} : {this.initializationData[i]}";
                        if (i != initializationData.Length - 1)
                            InitData += "\n";
                    }
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

    public class MaskedInputDialog : Form
    {
        public string Result { get; private set; }

        public MaskedInputDialog(string prompt)
        {
            Text = "Password";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(320, 160);
            BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(27)))), ((int)(((byte)(58)))));
            ForeColor = Color.White;
            Util.CreateCustomTitleBar(this, "Enter Password", true);

            var label = new Label
            {
                Text = prompt,
                AutoSize = true,
                Font = new Font("Consolas", 11f),
                Location = new Point(10, 60)
            };

            TextBox textBox = new TextBox
            {
                UseSystemPasswordChar = true,
                Location = new Point(12, 86),
                Font = new Font("Consolas", 11f),
                TabStop = true,
                Width = 296
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(152, 120)
            };

            var cancelButton = new Button
            {
                Text = "Abbrechen",
                DialogResult = DialogResult.Cancel,
                Location = new Point(233, 120)
            };

            AcceptButton = okButton;
            CancelButton = cancelButton;

            Controls.Add(label);
            Controls.Add(textBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            this.Shown += (s, e) =>
            {
                textBox.Focus();
            };

            this.FormClosing += (s, e) =>
            {
                Result = (DialogResult == DialogResult.OK) ? textBox.Text : null;
            };
        }
    }
}
