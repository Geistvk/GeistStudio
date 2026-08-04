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

using GeistStudio;
using GeistStudio.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GeistStudio.Util;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace GeistStudio
{
    partial class GeistStudioWin 
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Erforderliche Logikvariable.
        /// </summary>
        /// ChangeTitleBarColor(Color.FromArgb(26, 23, 55));
        private int tabSize = 0;
        private const int DWMWA_CAPTION_COLOR = 35;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(
            int left,
            int top,
            int right,
            int bottom,
            int width,
            int height);


        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(
            IntPtr hWnd,
            IntPtr hRgn,
            bool redraw);


        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(
            IntPtr hObject);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        private const int EM_SHOWSCROLLBAR = 0x0460;
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        private const int GWL_STYLE = -16;
        private const int WS_HSCROLL = 0x00100000;
        private const int WS_VSCROLL = 0x00200000;
        private const int EM_SETTARGETDEVICE = 0x0448;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
           IntPtr hwnd,
           int attr,
           ref int attrValue,
           int attrSize);
        private void InitializeStyledToolTipOld()
        {
            this.StyledToolTip = new System.Windows.Forms.ToolTip();

            this.StyledToolTip.OwnerDraw = true;
            this.StyledToolTip.ShowAlways = true;

            this.StyledToolTip.InitialDelay = 400;
            this.StyledToolTip.ReshowDelay = 100;
            this.StyledToolTip.AutoPopDelay = 6000;


            this.StyledToolTip.Popup += (s, e) =>
            {
                e.ToolTipSize = new Size(
                    e.ToolTipSize.Width + 24,
                    e.ToolTipSize.Height + 14);


                this.BeginInvoke(new Action(() =>
                {
                    IntPtr handle = GetToolTipHandle();

                    if (handle != IntPtr.Zero)
                    {
                        IntPtr region = CreateRoundRectRgn(
                            0,
                            0,
                            e.ToolTipSize.Width,
                            e.ToolTipSize.Height,
                            18,
                            18);

                        SetWindowRgn(
                            handle,
                            region,
                            true);
                    }
                }));
            };


            this.StyledToolTip.Draw += (s, e) =>
            {
                e.Graphics.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 18;

                    Rectangle rect = new Rectangle(
                        1,
                        1,
                        e.Bounds.Width - 3,
                        e.Bounds.Height - 3);


                    path.AddArc(
                        rect.X,
                        rect.Y,
                        radius,
                        radius,
                        180,
                        90);

                    path.AddArc(
                        rect.Right - radius,
                        rect.Y,
                        radius,
                        radius,
                        270,
                        90);

                    path.AddArc(
                        rect.Right - radius,
                        rect.Bottom - radius,
                        radius,
                        radius,
                        0,
                        90);

                    path.AddArc(
                        rect.X,
                        rect.Bottom - radius,
                        radius,
                        radius,
                        90,
                        90);

                    path.CloseFigure();

                    using (SolidBrush background = new SolidBrush(
                        Util.Config.Colors.Background.ToolTip.Background))
                    {
                        e.Graphics.FillPath(
                            background,
                            path);
                    }

                    using (Pen border = new Pen(
                        Util.Config.Colors.Foreground.ToolTip.Border,
                        1))
                    {
                        border.Alignment =
                            System.Drawing.Drawing2D.PenAlignment.Inset;

                        e.Graphics.DrawPath(
                            border,
                            path);
                    }
                }


                TextRenderer.DrawText(
                    e.Graphics,
                    e.ToolTipText,
                    new Font(
                        "Segoe UI",
                        9F),
                    new Rectangle(
                        12,
                        7,
                        e.Bounds.Width - 24,
                        e.Bounds.Height - 14),
                    Util.Config.Colors.Foreground.ToolTip.Text,
                    TextFormatFlags.Left
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.WordBreak);
            };
        }

        private void InitializeStyledToolTip()
        {
            this.StyledToolTip = new ToolTip();

            this.StyledToolTip.OwnerDraw = true;
            this.StyledToolTip.ShowAlways = true;

            this.StyledToolTip.InitialDelay = 400;
            this.StyledToolTip.ReshowDelay = 100;
            this.StyledToolTip.AutoPopDelay = 6000;

            Font tooltipFont = new Font(
                "Segoe UI",
                9F);

            Color backgroundColor = Util.Config.Colors.Background.ToolTip.Background;
            Color borderColor = Util.Config.Colors.Foreground.ToolTip.Border;
            Color textColor = Util.Config.Colors.Foreground.ToolTip.Text;
            
            this.StyledToolTip.Popup += (s, e) =>
            {
                e.ToolTipSize = new Size(
                    e.ToolTipSize.Width + 24,
                    e.ToolTipSize.Height + 14);
            };

            this.StyledToolTip.Draw += (s, e) =>
            {
                Rectangle rect = new Rectangle(
                    1,
                    1,
                    e.Bounds.Width - 2,
                    e.Bounds.Height - 2);

                using (SolidBrush background = new SolidBrush(backgroundColor))
                {
                    e.Graphics.FillRectangle(
                        background,
                        rect);
                }

                using (Pen border = new Pen(borderColor))
                {
                    e.Graphics.DrawRectangle(
                        border,
                        rect);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    e.ToolTipText,
                    tooltipFont,
                    new Rectangle(
                        12,
                        7,
                        e.Bounds.Width - 24,
                        e.Bounds.Height - 14),
                    textColor,
                    TextFormatFlags.Left
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.WordBreak);
            };
        }

        private IntPtr GetToolTipHandle()
        {
            foreach (System.Diagnostics.Process process in
                System.Diagnostics.Process.GetProcesses())
            {
                if (process.MainWindowTitle.Contains("ToolTip"))
                {
                    return process.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }

        private Point lastToolTipPos = Point.Empty;
        private Control lastToolTipControl = null;
        private string lastToolTipText = "";

        private ToolStripMenuItem CreateMenuItem(string name, string description, Action func)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(name);
            item.AutoToolTip = false;

            item.MouseEnter += (s, e) => ShowStyledToolTip(this, this.MainMenu, description);
            item.MouseMove += (s, e) => UpdateStyledToolTip(this, this.MainMenu, description);
            item.MouseLeave += (s, e) => HideStyledToolTip(this, this.MainMenu);
            item.Click += (s, e) => func();

            return item;
        }

        private void HideStyledToolTip(GeistStudioWin form, Control parent)
        {
            form.StyledToolTip.Hide(parent);

            this.lastToolTipControl = null;
            this.lastToolTipText = "";
            this.lastToolTipPos = Point.Empty;
        }

        private void UpdateStyledToolTip(
            GeistStudioWin form,
            Control parent,
            string desc
        )
        {
            Point pos = parent.PointToClient(Cursor.Position);
            pos.Offset(16, 24);

            if (this.lastToolTipControl == parent &&
                this.lastToolTipText == desc &&
                Math.Abs(pos.X - this.lastToolTipPos.X) < 4 &&
                Math.Abs(pos.Y - this.lastToolTipPos.Y) < 4)
                return;

            this.lastToolTipPos = pos;
            this.lastToolTipControl = parent;
            this.lastToolTipText = desc;

            form.StyledToolTip.Hide(parent);
            form.StyledToolTip.Show(desc, parent, pos);
        }

        private void ShowStyledToolTip(
            GeistStudioWin form,
            Control parent,
            string desc
        )
        {
            if (string.IsNullOrWhiteSpace(desc))
                return;

            Point pos = parent.PointToClient(Cursor.Position);
            pos.Offset(16, 24);

            this.lastToolTipPos = pos;
            this.lastToolTipControl = parent;
            this.lastToolTipText = desc;

            form.StyledToolTip.Hide(parent);
            form.StyledToolTip.Show(desc, parent, pos);
        }

        private void AddMenuItems(
            ToolStripMenuItem menu,
            params object[] items)
        {
            InitializeStyledToolTip();
            foreach (object item in items)
            {
                if (item is string text && text == "-")
                {
                    menu.DropDownItems.Add(new ToolStripSeparator());
                    continue;
                }

                if (item is Tuple<string, string, Action> entry)
                {
                    menu.DropDownItems.Add(
                        CreateMenuItem(
                            entry.Item1,
                            entry.Item2,
                            entry.Item3
                        )
                    );
                }
            }
        }

        private Tuple<string, string, Action> MenuItem(string name, string description, Action func)
        {
            return new Tuple<string, string, Action>(name, description, func);
        }

        private void addMenuComponents()
        {
            // 
            // File
            // 
            AddMenuItems(
                this.FileMenu,
                MenuItem("New", "Creates a new empty workspace.",                       () => { }),
                MenuItem("New File", "Creates a new source file.",                      () => WelcomeNewButton_Click(null, null)),
                MenuItem("New Project", "Creates a new GeistStudio project.",           () => { }),
                "-",
                MenuItem("Open...", "Opens an existing file.",                          () => WelcomeOpenButton_Click(null, null)),
                MenuItem("Open Folder...", "Opens a folder as a project.",              () => { }),
                MenuItem("Recent Files", "Shows recently opened files.",                () => { }),
                "-",
                MenuItem("Save", "Saves the current file.",                             () => Util.HandleFileAction(this, "save")),
                MenuItem("Save As...", "Saves the current file with a different name.", () => Util.SaveAsFile(this)),
                MenuItem("Save All", "Saves all opened files.",                         () => Util.HandleFileAction(this, "save", true)),
                "-",
                MenuItem("Close", "Closes the current file.",                           () => Util.HandleFileAction(this, "close")),
                MenuItem("Close All", "Closes all opened files.",                       () => Util.HandleFileAction(this, "close", true)),
                "-",
                MenuItem("Settings", "Opens GeistStudio settings.",                     () => { }),
                "-",
                MenuItem("Exit", "Closes GeistStudio.",                                 () => { })
            );
            // 
            // Edit
            // 
            AddMenuItems(
                this.EditMenu,
                MenuItem("Undo", "Reverts the last action.",                () => Util.simulateKeyPress(new string[] { "Ctrl", "Z" })),
                MenuItem("Redo", "Restores the last undone action.",        () => Util.simulateKeyPress(new string[] { "Ctrl", "Y" })),
                "-",
                MenuItem("Cut", "Cuts the selected text.",                  () => Util.simulateKeyPress(new string[] { "Ctrl", "X" })),
                MenuItem("Copy", "Copies the selected text.",               () => Util.simulateKeyPress(new string[] { "Ctrl", "C" })),
                MenuItem("Paste", "Pastes copied text.",                    () => Util.simulateKeyPress(new string[] { "Ctrl", "V" })),
                MenuItem("Delete", "Deletes the selected content.",         () => Util.simulateKeyPress(new string[] { "Ctrl", "DEL" })),
                "-",
                MenuItem("Find", "Searches text in the current document.",  () => Util.simulateKeyPress(new string[] { "Ctrl", "F" })),
                MenuItem("Replace", "Finds and replaces text.",             () => Util.simulateKeyPress(new string[] { "Ctrl", "H" })),
                MenuItem("Go to Line", "Jumps to a specific line number.",  () => Util.simulateKeyPress(new string[] { "Ctrl", "G" })),
                MenuItem("Select All", "Selects all content.",              () => Util.simulateKeyPress(new string[] { "Ctrl", "A" })),
                "-",
                MenuItem("Open Settings", "Opens the Settings Menu.",       () => Util.OpenSettings())
            );
            // 
            // Selection
            // 
            AddMenuItems(
                this.SelectionMenu,
                MenuItem("Select All", "Selects the complete document.", () => { }),
                MenuItem("Select Line", "Selects the current line.", () => { }),
                MenuItem("Add Cursor", "Adds another text cursor.", () => { }),
                MenuItem("Select All Occurrences", "Selects every matching occurrence.", () => { })
            );
            // 
            // View
            // 
            AddMenuItems(
                this.ViewMenu,
                MenuItem("Explorer", "Shows the project file explorer.", () => { }),
                MenuItem("Search", "Opens the global search panel.", () => { }),
                "-",
                MenuItem("Terminal", "Opens the integrated terminal.", () => Util.OpenTerminal(this)),
                MenuItem("Problems", "Shows detected errors and warnings.", () => { }),
                MenuItem("Output", "Displays build and application output.", () => { }),
                MenuItem("Debug Console", "Opens the debugging console.", () => { }),
                "-",
                MenuItem("Fullscreen", "Toggles fullscreen mode.", () => { }),
                MenuItem("Zen Mode", "Activates distraction-free editing.", () => { })
            );
            // 
            // Go
            // 
            AddMenuItems(
                this.GoMenu,
                MenuItem("Go to File", "Searches and opens a file.", () => { }),
                "-",
                MenuItem("Go to Line", "Moves the cursor to a line.", () => { }),
                MenuItem("Go to Symbol", "Searches symbols in the project.", () => { }),
                "-",
                MenuItem("Go to Definition", "Jumps to a symbol definition.", () => { }),
                MenuItem("Go to References", "Finds all references.", () => { }),
                "-",
                MenuItem("Go to Home", "Go back to the Welcome Screen.", () => Util.gotToHome(this))
            );
            // 
            // Project
            // 
            AddMenuItems(
                this.ProjectMenu,
                MenuItem("New Project", "Creates a new project.", () => { }),
                MenuItem("Open Project", "Opens an existing project.", () => { }),
                MenuItem("Close Project", "Closes the current project.", () => { }),
                MenuItem("Project Settings", "Changes project configuration.", () => { })
            );
            // 
            // Build
            // 
            AddMenuItems(
                this.BuildMenu,
                MenuItem("Build", "Compiles the current project.", () => { }),
                MenuItem("Rebuild", "Cleans and rebuilds the project.", () => { }),
                MenuItem("Clean", "Removes generated files.", () => { }),
                MenuItem("Publish", "Creates a distributable build.", () => { })
            );
            // 
            // Debug
            // 
            AddMenuItems(
                this.DebugMenu,
                MenuItem("Start", "Starts debugging.", () => { }),
                MenuItem("Start Without Debugging", "Runs the project without debugger.", () => { }),
                MenuItem("Stop", "Stops the current execution.", () => { }),
                MenuItem("Restart", "Restarts the application.", () => { }),
                "-",
                MenuItem("Toggle Breakpoint", "Adds or removes a breakpoint.", () => { }),
                "-",
                MenuItem("Step Over", "Executes the next line.", () => { }),
                MenuItem("Step Into", "Enters the current function.", () => { }),
                MenuItem("Step Out", "Leaves the current function.", () => { })
            );
            // 
            // Git
            // 
            AddMenuItems(
                this.GitMenu,
                MenuItem("Commit", "Creates a new Git commit.", () => { }),
                MenuItem("Push", "Uploads changes to the repository.", () => { }),
                "-",
                MenuItem("Pull", "Downloads repository changes.", () => { }),
                MenuItem("Fetch", "Fetches remote repository information.", () => { }),
                MenuItem("Merge", "Combines branches.", () => { }),
                "-",
                MenuItem("Branches", "Manages Git branches.", () => { }),
                MenuItem("Clone Repository", "Copies a remote repository.", () => { })
            );
            // 
            // Tools
            // 
            AddMenuItems(
                this.ToolsMenu,
                MenuItem("Terminal", "Opens the integrated terminal.", () => Util.OpenTerminal(this)),
                MenuItem("Extensions", "Manages installed extensions.", () => { }),
                MenuItem("Format Document", "Formats the current document.", () => { }),
                "-",
                MenuItem("Settings", "Opens the Settings Menu.", () => Util.OpenSettings()),
                MenuItem("Options", "Opens editor options.", () => { })
            );
            // 
            // Window
            // 
            AddMenuItems(
                this.WindowMenu,
                MenuItem("Split Editor", "Splits the editor view.", () => { }),
                MenuItem("Next Tab", "Moves to the next tab.", () => { }),
                MenuItem("Previous Tab", "Moves to the previous tab.", () => { })
            );
            // 
            // Help
            // 
            AddMenuItems(
                this.HelpMenu,
                MenuItem("Documentation", "Opens GeistStudio documentation.", () => { }),
                MenuItem("Keyboard Shortcuts", "Shows available shortcuts.", () => { }),
                "-",
                MenuItem("Check for Updates", "Checks for new versions.", () => { }),
                MenuItem("Show Versions", "Shows every Version with it's changes.", () => Util.openInformation()),
                "-",
                MenuItem("About GeistStudio", "Shows information about GeistStudio.", () => Util.openAbout(this)),
                "-",
                MenuItem("Cache Store", "Just a Debug Function for the Memory Caching.", () => Util.cacheData()),
                MenuItem("Cache Load", "Just a Debug Function for the Memory Caching.", () => Util.loadCacheData())
            );
        }

        public static string LoadEmbeddedJson()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("GeistStudio.GeistStudioData.json"))
            using (StreamReader reader = new StreamReader(stream))
            {

                if (stream == null)
                    throw new FileNotFoundException("Embedded Resource wurde nicht gefunden.");

                return reader.ReadToEnd();
            }
        }

        private void processJson() 
        {
            Dictionary<string, object> root = (Dictionary<string, object>)JsonParser.LoadEmbeddedJson("GeistStudio.GeistStudioData.json");
            Dictionary<string, object> versions = (Dictionary<string, object>)root["AllVersions"];

            VersionReader.PrintVersions(versions);

            String result = "";
            result += $"Name:           {(string)root["Name"]}\n";
            result += $"Author:         {(string)root["Author"]}\n";
            result += $"CurVersion:     {(string)root["CurVersion"]}";
            MessageBox.Show(result);
        }

        private void WelcomePanel_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                this.WelcomePanel.ClientRectangle,
                System.Drawing.Color.FromArgb(76, 29, 149),
                System.Drawing.Color.FromArgb(37, 99, 235),
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, this.WelcomePanel.ClientRectangle);
            }
        }

        private void UpdateWelcomeScreenVisibility()
        {
            bool anyFilesOpen = this.FileList.TabPages.Count > 0;
            this.WelcomePanel.Visible = !anyFilesOpen;
            if (this.WelcomePanel.Visible)
            {
                this.WelcomePanel.BringToFront();
            }
        }

        private void WelcomeNewButton_Click(object sender, EventArgs e)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "New File";
                dialog.Size = new Size(400, 200);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.BackColor = Util.Config.Colors.Background.Dialog.Background;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                Util.CreateCustomTitleBar(dialog, "Create New File", true);

                Label title = new Label();
                title.Text = "Create a new file";
                title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                title.Location = new Point(16, 40);
                title.AutoSize = true;
                title.ForeColor = dialog.ForeColor;

                Label label = new Label();
                label.Text = "File name (without .gsScript)";
                label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                label.Location = new Point(20, 90);
                label.AutoSize = true;
                label.ForeColor = dialog.ForeColor;

                TextBox textBox = new TextBox();
                textBox.Location = new Point(20, 120);
                textBox.Size = new Size(360, 25);
                textBox.BackColor = Util.Config.Colors.Background.Dialog.TextBox;
                textBox.ForeColor = dialog.ForeColor;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Text = "Untitled";
                textBox.SelectAll();

                Button create = new Button();
                create.Text = "Create";
                create.Location = new Point(210, 160);
                create.Size = new Size(80, 30);
                create.DialogResult = DialogResult.OK;

                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Location = new Point(300, 160);
                cancel.Size = new Size(80, 30);
                cancel.DialogResult = DialogResult.Cancel;

                dialog.Controls.Add(title);
                dialog.Controls.Add(label);
                dialog.Controls.Add(textBox);
                dialog.Controls.Add(create);
                dialog.Controls.Add(cancel);

                dialog.AcceptButton = create;
                dialog.CancelButton = cancel;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = textBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(fileName))
                        fileName = "Untitled";

                    OpenFileInNewTab(fileName + ".gsScript", "");
                }
            }
        }

        private void WelcomeOpenButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "GeistScript-Dateien (*.gsScript)|*.gsScript|Alle Dateien (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string content = System.IO.File.ReadAllText(dialog.FileName);
                    OpenFileInNewTab(System.IO.Path.GetFileName(dialog.FileName), content, true);
                }
            }
        }















        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

        private struct HighlightSpan
        {
            public int Start;
            public int Length;
            public Color Color;
            public HighlightSpan(int start, int length, Color color)
            {
                Start = start; Length = length; Color = color;
            }
        }




        private (HashSet<string> declaredNames, List<SyncedRichTextBox.HighlightSpan> spans) ComputeHighlighting(
            string text, int rangeStart, int rangeLength)
        {
            var declaredNames = CollectDeclaredNames(text);
            var spans = new List<SyncedRichTextBox.HighlightSpan>();
            string searchText = text.Substring(rangeStart, rangeLength);

            void AddLayer(Dictionary<string, Color> patterns, bool useGroupOne)
            {
                foreach (var kvp in patterns)
                {
                    foreach (Match m in Regex.Matches(searchText, kvp.Key, RegexOptions.IgnoreCase))
                    {
                        Group g = (useGroupOne && m.Groups.Count > 1 && m.Groups[1].Success)
                            ? m.Groups[1] : m.Groups[0];

                        spans.Add(new SyncedRichTextBox.HighlightSpan(rangeStart + g.Index, g.Length, kvp.Value));
                    }
                }
            }

            AddLayer(syntaxPatterns, useGroupOne: false);
            AddLayer(identifierPatterns, useGroupOne: true);

            if (declaredNames.Count > 0)
            {
                string combined = @"\b(" + string.Join("|", declaredNames.Select(Regex.Escape)) + @")\b";
                AddLayer(new Dictionary<string, Color> { { combined, Color.FromArgb(156, 220, 254) } }, useGroupOne: true);
            }

            AddLayer(stringCommentPatterns, useGroupOne: false);

            var groupedKeywords = new Dictionary<string, Color>();
            foreach (var group in keywordColors.GroupBy(kvp => kvp.Value))
            {
                string combined = @"\b(" + string.Join("|", group.Select(g => Regex.Escape(g.Key))) + @")\b";
                groupedKeywords[combined] = group.Key;
            }
            AddLayer(groupedKeywords, useGroupOne: false);

            return (declaredNames, spans);
        }



        

        private CancellationTokenSource _highlightCts;
        private long _highlightGeneration = 0;
        private volatile bool _highlightRunning = false;
        private bool _highlightPending = false;
        private HashSet<string> declaredNamesCache = new HashSet<string>();

        private async void HighlightAsync(SyncedRichTextBox editor, bool fullDocument = false)
        {
            if (_highlightRunning)
            {
                _highlightPending = true;
                return;
            }

            _highlightRunning = true;
            long myGeneration = Interlocked.Increment(ref _highlightGeneration);

            string text = editor.Text;
            if (text.Length == 0)
            {
                _highlightRunning = false;
                return;
            }

            Color defaultColor = editor.ForeColor;

            int rangeStart, rangeLength;
            if (fullDocument)
            {
                rangeStart = 0;
                rangeLength = text.Length;
            }
            else
            {
                var (s, l) = GetRangeAroundCaret(editor, bufferChars: 500);
                rangeStart = s;
                rangeLength = l;
            }

            try
            {
                var (declaredNames, spans) = await Task.Run(
                    () => ComputeHighlighting(text, rangeStart, rangeLength));

                if (myGeneration != Interlocked.Read(ref _highlightGeneration)) return;
                if (editor.Text != text) return;

                declaredNamesCache = declaredNames;
                editor.ApplyHighlightSpans(spans, defaultColor, rangeStart, rangeLength);
            }
            finally
            {
                _highlightRunning = false;

                if (_highlightPending)
                {
                    _highlightPending = false;
                    HighlightAsync(editor);
                }
            }
        }




        private Dictionary<string, Color> keywordColors = new Dictionary<string, Color> {};

        private Dictionary<string, Color> syntaxPatterns = new Dictionary<string, Color>
            {
                { @"\b\d+(\.\d+)?\b", Color.FromArgb(181, 206, 168) }
            };

        private Dictionary<string, Color> identifierPatterns = new Dictionary<string, Color>
            {
                { @"\bfunction\s+([A-Za-z_]\w*)", Color.FromArgb(220, 220, 170) }
            };

        private Dictionary<string, Color> stringCommentPatterns = new Dictionary<string, Color>
            {
                { "\"(?:[^\"\\\\]|\\\\.)*\"", Color.FromArgb(206, 145, 120) },
                { "//.*",                     Color.FromArgb(106, 153, 85) },
                { @"/\*[\s\S]*?\*/",          Color.FromArgb(106, 153, 85) },
                { "Copyright.*",              Color.FromArgb(255, 255, 80) }
            };

        private void loadAboutData()
        {
            Color getCol(String colStr) { 
                return Color.FromArgb(
                    int.Parse(colStr.Split(',')[0]),
                    int.Parse(colStr.Split(',')[1]),
                    int.Parse(colStr.Split(',')[2])
                );
            }

            Dictionary<string, object> root = (Dictionary<string, object>)JsonParser.LoadEmbeddedJson("GeistStudio.GeistStudioData.json");
            Dictionary<string, object> config = (Dictionary<string, object>)root["Config"];
            Dictionary<string, object> syntaxAll = (Dictionary<string, object>)config["Syntax"];
            
            
            List<object> Syntax = (List<object>)syntaxAll["Keywords"];
            for (int i = 0; i < Syntax.Count; i++)
            {
                var keyword = (Dictionary<string, object>)Syntax[i];
                string color = (string)keyword["color"];
                keywordColors[(string)keyword["name"]] = getCol(color);
            }

            String idCol = (string)syntaxAll["IdentifierColor"];
            String idPat = @"\b(?!";
            List<object> allIdPatterns = (List<object>)syntaxAll["Identifiers"];
            for (int i = 0; i < allIdPatterns.Count; i++)
            {
                var pattern = (string)allIdPatterns[i];
                idPat += pattern + @"\b";
                if (i != allIdPatterns.Count - 1)
                    idPat += @"|";
            }
            idPat += @")([A-Za-z_]\w*)\s*\(";
            identifierPatterns[idPat] = getCol(idCol);
        }


        private static void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        private void HighlightGrouped(SyncedRichTextBox editor, Dictionary<string, Color> words, bool resetColors, int rangeStart, int rangeLength)
        {
            var grouped = new Dictionary<string, Color>();
            foreach (var group in words.GroupBy(kvp => kvp.Value))
            {
                string combined = @"\b(" + string.Join("|", group.Select(g => Regex.Escape(g.Key))) + @")\b";
                grouped[combined] = group.Key;
            }
            editor.HighlightSyntax(grouped, useRegex: true, resetColors: resetColors, rangeStart: rangeStart, rangeLength: rangeLength);
        }

        private HashSet<string> CollectDeclaredNames(string text)
        {
            var names = new HashSet<string>();

            foreach (Match m in Regex.Matches(text, @"\b(?:let|const)\s+([A-Za-z_]\w*)"))
                names.Add(m.Groups[1].Value);

            foreach (Match m in Regex.Matches(text, @"\bfunction\s+[A-Za-z_]\w*\s*\(([^)]*)\)"))
            {
                foreach (var p in m.Groups[1].Value.Split(','))
                {
                    string name = p.Trim();
                    if (!string.IsNullOrEmpty(name) && Regex.IsMatch(name, @"^[A-Za-z_]\w*$"))
                        names.Add(name);
                }
            }
            return names;
        }


        private void ApplyHighlighting(SyncedRichTextBox editor, HashSet<string> declaredNames, int rangeStart = -1, int rangeLength = -1)
        {
            editor.HighlightSyntax(syntaxPatterns, useRegex: true, resetColors: false, rangeStart: rangeStart, rangeLength: rangeLength);
            editor.HighlightSyntax(identifierPatterns, useRegex: true, resetColors: false, useGroupOne: true, rangeStart: rangeStart, rangeLength: rangeLength);
            HighlightGrouped(editor, keywordColors, resetColors: true, rangeStart, rangeLength);

            if (declaredNames.Count > 0)
            {
                string combined = @"\b(" + string.Join("|", declaredNames.Select(Regex.Escape)) + @")\b";
                editor.HighlightSyntax(new Dictionary<string, Color> { { combined, Color.FromArgb(156, 220, 254) } },
                    useRegex: true, resetColors: false, rangeStart: rangeStart, rangeLength: rangeLength);
            }

            editor.HighlightSyntax(stringCommentPatterns, useRegex: true, resetColors: false, rangeStart: rangeStart, rangeLength: rangeLength);
        }

        
        private (int start, int length) GetRangeAroundCaret(SyncedRichTextBox editor, int bufferChars = 1000)
        {
            int caret = editor.SelectionStart;
            int start = Math.Max(0, caret - bufferChars);
            int end = Math.Min(editor.TextLength, caret + bufferChars);
            return (start, end - start);
        }

        public void OpenFileInNewTab(string title, string content, bool opened = false, bool hideMsg = false)
        {
            loadAboutData();
            TabPage page = new TabPage(title);

            Panel container = new Panel();
            container.Dock = DockStyle.Fill;
            container.BackColor = Util.Config.Colors.Background.Editor.Background;

            Panel linePanel = new Panel();
            linePanel.Dock = DockStyle.Left;
            linePanel.Width = 50;
            linePanel.BackColor = Util.Config.Colors.Background.Editor.LineNumbers;

            SyncedRichTextBox editor = new SyncedRichTextBox();
            editor.Multiline = true;
            editor.AcceptsTab = true;
            editor.WordWrap = false;
            editor.MaxLength = 0;
            editor.Dock = DockStyle.Fill;
            editor.Font = new Font("Consolas", 11F);
            editor.BackColor = Util.Config.Colors.Background.Editor.Background;
            editor.ForeColor = Util.Config.Colors.Foreground.Text;
            editor.BorderStyle = BorderStyle.None;
            editor.DetectUrls = false;
            editor.ScrollBars = RichTextBoxScrollBars.Both;

            EnableDoubleBuffering(container);
            EnableDoubleBuffering(linePanel);
            EnableDoubleBuffering(editor);

            int cachedLineHeight = -1;

            int GetLineHeight()
            {
                if (cachedLineHeight > 0)
                    return cachedLineHeight;

                using (Graphics g = editor.CreateGraphics())
                {
                    cachedLineHeight = TextRenderer.MeasureText(g, "Ay", editor.Font,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Height;
                }
                return cachedLineHeight;
            }

            void EnableUnboundedHorizontalScroll()
            {
                if (!editor.IsHandleCreated)
                    return;
                SendMessage(editor.Handle, EM_SETTARGETDEVICE, IntPtr.Zero, (IntPtr)1);
            }

            //Line Numbers
            linePanel.Paint += (s, e) =>
            {
                if (!editor.IsHandleCreated)
                    return;

                int lineHeight = GetLineHeight();
                if (lineHeight <= 0)
                    return;

                int firstCharIndex = editor.GetCharIndexFromPosition(new Point(1, 1));
                int firstVisibleLine = editor.GetLineFromCharIndex(firstCharIndex);
                int firstLineCharIndex = editor.GetFirstCharIndexFromLine(firstVisibleLine);
                Point charPos = editor.GetPositionFromCharIndex(firstLineCharIndex);

                int y = charPos.Y;
                int lineNumber = firstVisibleLine + 1;
                int totalLines = Math.Max(editor.Lines.Length, 1);

                using (var brush = new SolidBrush(Util.Config.Colors.Foreground.Editor.LineNumbers))
                {
                    while (y < linePanel.Height && lineNumber <= totalLines)
                    {
                        string text = lineNumber.ToString();
                        var size = e.Graphics.MeasureString(text, editor.Font);
                        e.Graphics.DrawString(text, editor.Font, brush, linePanel.Width - size.Width - 6, y);
                        y += lineHeight;
                        lineNumber++;
                    }
                }
            };

            System.Windows.Forms.Timer highlightTimer = new System.Windows.Forms.Timer();
            highlightTimer.Interval = 150;
            highlightTimer.Tick += (s, e) =>
            {
                highlightTimer.Stop();
                HighlightAsync(editor);
            };

            void RequestHighlight()
            {
                highlightTimer.Stop();
                highlightTimer.Start();
            }

            editor.TextChanged += (s, e) =>
            {
                linePanel.Invalidate();
                EnableUnboundedHorizontalScroll();
                RequestHighlight();

                /*
                 declaredNamesCache = CollectDeclaredNames(editor.Text);
                ApplyHighlighting(editor, declaredNamesCache);
                 */
            };

            editor.FontChanged += (s, e) =>
            {
                cachedLineHeight = -1;
                linePanel.Invalidate();
            };

            // Scroll Update
            editor.Resize += (s, e) => linePanel.Invalidate();
            editor.VScroll += (s, e) => linePanel.Invalidate();
            editor.HScroll += (s, e) => linePanel.Invalidate();
            editor.KeyUp += (s, e) => linePanel.Invalidate();
            editor.SelectionChanged += (s, e) => linePanel.Invalidate();

            editor.HandleCreated += (s, e) =>
            {
                EnableUnboundedHorizontalScroll();
                linePanel.Invalidate();
                SetWindowTheme(editor.Handle, "DarkMode_Explorer", null);

                HighlightAsync(editor, fullDocument: true);
            };

            container.Controls.Add(editor);
            container.Controls.Add(linePanel);
            page.Controls.Add(container);

            FileList.TabPages.Add(page);
            FileList.SelectedTab = page;
            tabEditors.Add(page, editor);

            editor.Text = content;
            HighlightAsync(editor, fullDocument: true);

            FileList.PerformLayout();
            FileList.Invalidate(true);

            String actionType = !opened ? "Created" : "Opened";
            if (!hideMsg)
                Util.Notify(this, "Success", actionType + " file successfully");
        }





        public DialogResult ShowSaveDialog(bool closeFile = true)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Save and Close File";
                dialog.Size = new Size(400, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.BackColor = Util.Config.Colors.Background.Dialog.Background;

                Label title = new Label();
                title.Text = "Close File";
                title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                title.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                title.Location = new Point(16, 10);
                title.AutoSize = true;

                Label text = new Label();
                text.Text = "Do you want to save this file before closing?";
                text.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                text.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                text.Location = new Point(20, 60);
                text.AutoSize = true;

                Button yes = new Button();
                yes.Text = "Save | Close";
                yes.Location = new Point(80, 100);
                yes.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                yes.DialogResult = DialogResult.Yes;

                if (!closeFile)
                {
                    dialog.Text = "Save File";
                    title.Text = "Save File";
                    text.Text = "Do you want to save this file?";
                    yes.Text = "Save";
                    yes.Location = new Point(210, 100);
                }

                Button no = new Button();
                no.Text = "Don't Save | Close";
                no.Location = new Point(170, 100);
                no.Size = new Size(110, 22);
                no.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                no.DialogResult = DialogResult.No;

                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Location = new Point(300, 100);
                cancel.ForeColor = Util.Config.Colors.Foreground.Dialog.Text;
                cancel.DialogResult = DialogResult.Cancel;

                dialog.Controls.Add(title);
                dialog.Controls.Add(text);
                dialog.Controls.Add(yes);
                if (closeFile)
                    dialog.Controls.Add(no);
                dialog.Controls.Add(cancel);


                return dialog.ShowDialog();
            }
        }

        public void SaveTabFile(TabPage page)
        {
            if (!tabEditors.ContainsKey(page))
                return;

            string fileName = page.Text;

            File.WriteAllText(fileName, tabEditors[page].Text);
            Util.Notify(this, "Success", "Saved file successfully.");
        }

        private void FileList_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 1; i < FileList.TabPages.Count; i++)
            {
                Rectangle tabRect = FileList.GetTabRect(i);

                Rectangle closeButton = new Rectangle(
                    tabRect.Right - 25,
                    tabRect.Y + 8,
                    16,
                    16);

                if (closeButton.Contains(e.Location))
                {
                    TabPage page = FileList.TabPages[i];
                    DialogResult result = ShowSaveDialog();

                    if (result == DialogResult.Yes)
                    {
                        SaveTabFile(page);
                    }


                    if (result != DialogResult.Cancel)
                    {
                        FileList.TabPages.Remove(page);
                        page.Dispose();
                    }


                    break;
                }
            }
        }

        private void FileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.FileList.Invalidate();
        }

        struct Attributes
        {
            public bool NeedsCtrl;
            public bool NeedsShift;
            public bool NeedsAlt;

            public Attributes(bool ctrl, bool shift, bool alt)
            {
                NeedsCtrl = ctrl;
                NeedsShift = shift;
                NeedsAlt = alt;
            }
        }

        struct ShortCut
        {
            public Attributes Attributes;
            public Keys Key;
            public Action<GeistStudioWin> Func;

            public ShortCut(Attributes attributes, Keys key, Action<GeistStudioWin> func)
            {
                Attributes = attributes;
                Key = key;
                Func = func;
            }
        }
        //new Attributes(needsCtrl, needsShift, needsAlt)
        ShortCut[] keyShortCuts =
        {
            new ShortCut(
                new Attributes(true, true, false),
                Keys.S,
                form => Util.HandleFileAction(form, "save", true)
            ),

            new ShortCut(
                new Attributes(true, false, true),
                Keys.S,
                form => Util.SaveAsFile(form)
            ),

            new ShortCut(
                new Attributes(true, false, false),
                Keys.S,
                form => Util.HandleFileAction(form, "save", false)
            ),

            new ShortCut(
                new Attributes(true, true, false),
                Keys.W,
                form => Util.HandleFileAction(form, "close", true)
            ),

            new ShortCut(
                new Attributes(true, false, false),
                Keys.W,
                form => Util.HandleFileAction(form, "close", false)
            ),

            new ShortCut(
                new Attributes(true, false, false),
                Keys.N,
                form => form.WelcomeNewButton_Click(null, null)
            ),

            new ShortCut(
                new Attributes(true, false, false),
                Keys.T,
                form => Util.OpenTerminal(form)
            ),

            new ShortCut(
                new Attributes(false, false, false),
                Keys.F5,
                form => Util.ExecuteCode(form)
            ),

            new ShortCut(
                new Attributes(false, false, false),
                Keys.F6,
                form => Util.OpenTerminal(form)
            ),

            new ShortCut(
                new Attributes(false, false, false),
                Keys.F7,
                form => Util.OpenSettings()
            )
        };

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (ShortCut shortcut in keyShortCuts)
            {
                if (e.KeyCode == shortcut.Key &&
                    (!shortcut.Attributes.NeedsCtrl || e.Control) &&
                    (!shortcut.Attributes.NeedsShift || e.Shift) &&
                    (!shortcut.Attributes.NeedsAlt || e.Alt))
                {
                    shortcut.Func(this);
                    e.SuppressKeyPress = true;
                    break;
                }
            }
        }


        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.Navbar = new System.Windows.Forms.Panel();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.EditMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.GoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.BuildMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DebugMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.GitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.Sidebar = new System.Windows.Forms.Panel();
            this.MainContent = new System.Windows.Forms.Panel();
            this.FileList = new DarkTabControl();
            this.home = new System.Windows.Forms.TabPage();
            this.WelcomePanel = new System.Windows.Forms.Panel();
            this.WelcomeOpenButton = new System.Windows.Forms.Button();
            this.WelcomeNewButton = new System.Windows.Forms.Button();
            this.WelcomeSubtitleLabel = new System.Windows.Forms.Label();
            this.WelcomeTitleLabel = new System.Windows.Forms.Label();
            this.Navbar.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.MainContent.SuspendLayout();
            this.FileList.SuspendLayout();
            this.WelcomePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Navbar
            // 
            this.Navbar.Controls.Add(this.MainMenu);
            this.Navbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.Navbar.Location = new System.Drawing.Point(0, 0);
            this.Navbar.Name = "Navbar";
            this.Navbar.Size = new System.Drawing.Size(1280, 25);
            this.Navbar.TabIndex = 0;
            this.Navbar.BackColor = Util.Config.Colors.Background.Navbar;
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.FileMenu,
                this.EditMenu,
                this.SelectionMenu,
                this.ViewMenu,
                this.GoMenu,
                this.ProjectMenu,
                this.BuildMenu,
                this.DebugMenu,
                this.GitMenu,
                this.ToolsMenu,
                this.WindowMenu,
                this.HelpMenu
            });
            this.MainMenu.BackColor = Util.Config.Colors.Background.Background;
            this.MainMenu.ForeColor = Util.Config.Colors.Foreground.MainMenu;
            this.MainMenu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenu.Renderer = new GeistStudioMenuRenderer();
            this.MainMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.MainMenu.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(1280, 25);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.ShowItemToolTips = true;
            // 
            // FileMenu
            // 
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(37, 20);
            this.FileMenu.Text = "File";
            // 
            // EditMenu
            // 
            this.EditMenu.Name = "EditMenu";
            this.EditMenu.Size = new System.Drawing.Size(39, 20);
            this.EditMenu.Text = "Edit";
            // 
            // SelectionMenu
            // 
            this.SelectionMenu.Name = "SelectionMenu";
            this.SelectionMenu.Size = new System.Drawing.Size(67, 20);
            this.SelectionMenu.Text = "Selection";
            // 
            // ViewMenu
            // 
            this.ViewMenu.Name = "ViewMenu";
            this.ViewMenu.Size = new System.Drawing.Size(44, 20);
            this.ViewMenu.Text = "View";
            // 
            // GoMenu
            // 
            this.GoMenu.Name = "GoMenu";
            this.GoMenu.Size = new System.Drawing.Size(34, 20);
            this.GoMenu.Text = "Go";
            // 
            // ProjectMenu
            // 
            this.ProjectMenu.Name = "ProjectMenu";
            this.ProjectMenu.Size = new System.Drawing.Size(56, 20);
            this.ProjectMenu.Text = "Project";
            // 
            // BuildMenu
            // 
            this.BuildMenu.Name = "BuildMenu";
            this.BuildMenu.Size = new System.Drawing.Size(46, 20);
            this.BuildMenu.Text = "Build";
            // 
            // DebugMenu
            // 
            this.DebugMenu.Name = "DebugMenu";
            this.DebugMenu.Size = new System.Drawing.Size(54, 20);
            this.DebugMenu.Text = "Debug";
            // 
            // GitMenu
            // 
            this.GitMenu.Name = "GitMenu";
            this.GitMenu.Size = new System.Drawing.Size(34, 20);
            this.GitMenu.Text = "Git";
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Size = new System.Drawing.Size(46, 20);
            this.ToolsMenu.Text = "Tools";
            // 
            // WindowMenu
            // 
            this.WindowMenu.Name = "WindowMenu";
            this.WindowMenu.Size = new System.Drawing.Size(63, 20);
            this.WindowMenu.Text = "Window";
            // 
            // HelpMenu
            // 
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(44, 20);
            this.HelpMenu.Text = "Help";
            // 
            // Sidebar
            // 
            this.Sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.Sidebar.Location = new System.Drawing.Point(0, 35);
            this.Sidebar.Name = "Sidebar";
            this.Sidebar.Size = new System.Drawing.Size(226, 556);
            this.Sidebar.TabIndex = 1;
            this.Sidebar.BackColor = Util.Config.Colors.Background.Sidebar;
            // 
            // MainContent
            // 
            this.MainContent.Controls.Add(this.FileList);
            this.MainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContent.Location = new System.Drawing.Point(226, 35);
            this.MainContent.Name = "MainContent";
            this.MainContent.Size = new System.Drawing.Size(1054, 556);
            this.MainContent.TabIndex = 2;
            this.MainContent.BackColor = Util.Config.Colors.Background.Background;
            // 
            // FileList
            // 
            this.FileList.Controls.Add(this.home);
            this.FileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileList.Location = new System.Drawing.Point(0, 0);
            this.FileList.Name = "FileList";
            this.FileList.SelectedIndex = 0;
            this.FileList.Size = new System.Drawing.Size(1054, 256);
            this.FileList.TabIndex = 0;
            this.FileList.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.FileList.Padding = new Point(0, 0);
            this.FileList.ItemSize = new Size(150, 32);
            this.FileList.SizeMode = TabSizeMode.Normal;
            this.FileList.Appearance = TabAppearance.Normal;
            this.FileList.HotTrack = false;
            this.FileList.TabStop = false;
            this.FileList.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.FileList.SelectedIndexChanged += new System.EventHandler(this.FileList_SelectedIndexChanged);
            this.FileList.MouseDown += FileList_MouseDown;
            this.FileList.BackColor = Util.Config.Colors.Background.FileList;
            this.FileList.BackgroundColorDark = Util.Config.Colors.Background.FileList;
            // 
            // home
            // 
            this.home.Controls.Add(this.WelcomePanel);
            this.home.Location = new System.Drawing.Point(4, 22);
            this.home.Name = "home";
            this.home.Size = new System.Drawing.Size(1046, 530);
            this.home.BackColor = Util.Config.Colors.Background.Background;
            this.home.TabIndex = 0;
            this.home.Text = "Home";
            this.home.UseVisualStyleBackColor = false;
            this.home.BorderStyle = BorderStyle.None;
            // 
            // WelcomePanel
            // 
            this.WelcomePanel.BackColor = Util.Config.Colors.Background.Background;
            this.WelcomePanel.Controls.Add(this.WelcomeOpenButton);
            this.WelcomePanel.Controls.Add(this.WelcomeNewButton);
            this.WelcomePanel.Controls.Add(this.WelcomeSubtitleLabel);
            this.WelcomePanel.Controls.Add(this.WelcomeTitleLabel);
            this.WelcomePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WelcomePanel.Location = new System.Drawing.Point(0, 0);
            this.WelcomePanel.Name = "WelcomePanel";
            this.WelcomePanel.Size = new System.Drawing.Size(1054, 556);
            this.WelcomePanel.TabIndex = 0;
            this.WelcomePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.WelcomePanel_Paint);
            // 
            // WelcomeOpenButton
            // 
            this.WelcomeOpenButton.BackColor = Util.Config.Colors.Background.Button;
            this.WelcomeOpenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WelcomeOpenButton.FlatAppearance.BorderColor = Util.Config.Colors.Background.Button;
            this.WelcomeOpenButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WelcomeOpenButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.WelcomeOpenButton.ForeColor = Util.Config.Colors.Foreground.Text;
            this.WelcomeOpenButton.Location = new System.Drawing.Point(280, 270);
            this.WelcomeOpenButton.Name = "WelcomeOpenButton";
            this.WelcomeOpenButton.Size = new System.Drawing.Size(180, 42);
            this.WelcomeOpenButton.TabIndex = 0;
            this.WelcomeOpenButton.Text = "Open File";
            this.WelcomeOpenButton.UseVisualStyleBackColor = false;
            this.WelcomeOpenButton.Click += new System.EventHandler(this.WelcomeOpenButton_Click);
            // 
            // WelcomeNewButton
            // 
            this.WelcomeNewButton.BackColor = Util.Config.Colors.Background.Button;
            this.WelcomeNewButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WelcomeNewButton.FlatAppearance.BorderSize = 0;
            this.WelcomeNewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WelcomeNewButton.FlatAppearance.BorderColor = Util.Config.Colors.Background.Button;
            this.WelcomeNewButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.WelcomeNewButton.ForeColor = Util.Config.Colors.Foreground.Text;
            this.WelcomeNewButton.Location = new System.Drawing.Point(84, 270);
            this.WelcomeNewButton.Name = "WelcomeNewButton";
            this.WelcomeNewButton.Size = new System.Drawing.Size(180, 42);
            this.WelcomeNewButton.TabIndex = 1;
            this.WelcomeNewButton.Text = "+   New File";
            this.WelcomeNewButton.UseVisualStyleBackColor = false;
            this.WelcomeNewButton.Click += new System.EventHandler(this.WelcomeNewButton_Click);
            // 
            // WelcomeSubtitleLabel
            // 
            this.WelcomeSubtitleLabel.AutoSize = true;
            this.WelcomeSubtitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.WelcomeSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.WelcomeSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(205)))), ((int)(((byte)(235)))));
            this.WelcomeSubtitleLabel.Location = new System.Drawing.Point(84, 205);
            this.WelcomeSubtitleLabel.Name = "WelcomeSubtitleLabel";
            this.WelcomeSubtitleLabel.Size = new System.Drawing.Size(251, 21);
            this.WelcomeSubtitleLabel.TabIndex = 2;
            this.WelcomeSubtitleLabel.Text = "Your lightweight GeistScript-Code-Editor";
            // 
            // WelcomeTitleLabel
            // 
            this.WelcomeTitleLabel.AutoSize = true;
            this.WelcomeTitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.WelcomeTitleLabel.Font = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Bold);
            this.WelcomeTitleLabel.ForeColor = System.Drawing.Color.White;
            this.WelcomeTitleLabel.Location = new System.Drawing.Point(80, 140);
            this.WelcomeTitleLabel.Name = "WelcomeTitleLabel";
            this.WelcomeTitleLabel.Size = new System.Drawing.Size(245, 59);
            this.WelcomeTitleLabel.TabIndex = 3;
            this.WelcomeTitleLabel.Text = "GeistStudio";
            // 
            // GeistStudioWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 591);
            this.BackColor = Util.Config.Colors.Background.Background;
            this.Controls.Add(this.MainContent);
            this.Controls.Add(this.Sidebar);
            this.Controls.Add(this.Navbar);
            this.Name = "GeistStudioWin";
            this.Text = "GeistStudio";
            this.KeyPreview = true;

            Util.CreateCustomTitleBar(this, "GeistStudio", false, true);

            this.DoubleBuffered = true;
            this.KeyDown += Form1_KeyDown;
            this.Navbar.ResumeLayout(false);
            this.Navbar.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.MainContent.ResumeLayout(false);
            this.FileList.ResumeLayout(false);
            this.WelcomePanel.ResumeLayout(false);
            this.WelcomePanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        public Dictionary<TabPage, RichTextBox> tabEditors = new Dictionary<TabPage, RichTextBox>();
        public Font tabFont = new Font("Segoe UI", 9F);
        public Color tabBackground = Color.FromArgb(42, 38, 78);
        //public Color closeColor = Color.FromArgb(220, 100, 100);

        public System.Windows.Forms.Panel Navbar;
        public System.Windows.Forms.Panel Sidebar;
        public System.Windows.Forms.Panel MainContent;
        public DarkTabControl FileList;
        public System.Windows.Forms.TabPage home;

        public System.Windows.Forms.ToolTip StyledToolTip;
        public System.Windows.Forms.MenuStrip MainMenu;
        public ToolStripMenuItem FileMenu;
        public ToolStripMenuItem EditMenu;
        public ToolStripMenuItem SelectionMenu;
        public ToolStripMenuItem ViewMenu;
        public ToolStripMenuItem GoMenu;
        public ToolStripMenuItem ProjectMenu;
        public ToolStripMenuItem BuildMenu;
        public ToolStripMenuItem DebugMenu;
        public ToolStripMenuItem GitMenu;
        public ToolStripMenuItem ToolsMenu;
        public ToolStripMenuItem WindowMenu;
        public ToolStripMenuItem HelpMenu;

        public System.Windows.Forms.Panel WelcomePanel;
        public System.Windows.Forms.Label WelcomeTitleLabel;
        public System.Windows.Forms.Label WelcomeSubtitleLabel;
        public System.Windows.Forms.Button WelcomeNewButton;
        public System.Windows.Forms.Button WelcomeOpenButton;


        public void addTab(String name)
        {
            System.Windows.Forms.TabPage tabPage;
            tabPage = new System.Windows.Forms.TabPage();

            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = name.ToLower();
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(1046, 495);
            tabPage.TabIndex = this.tabSize++;
            tabPage.Text = name;
            tabPage.UseVisualStyleBackColor = true;

            FileList.Controls.Add(tabPage);
        }

        public void addComponents() 
        {
            addMenuComponents();
        }
    }
}

