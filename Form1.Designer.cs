using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
        private int tabSize = 0;

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


        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const uint LWA_COLORKEY = 0x1;

        private void InitializeStyledToolTip()
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
                        Color.FromArgb(42, 38, 72)))
                    {
                        e.Graphics.FillPath(
                            background,
                            path);
                    }

                    using (Pen border = new Pen(
                        Color.FromArgb(124, 58, 237),
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
                    Color.FromArgb(225, 220, 245),
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

        private ToolStripMenuItem CreateMenuItem(string name, string description)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(name);
            item.AutoToolTip = false;

            item.MouseEnter += (s, e) => ShowStyledToolTip(description);
            item.MouseLeave += (s, e) => this.StyledToolTip.Hide(this.MainMenu);

            return item;
        }

        private void ShowStyledToolTip(string description)
        {
            InitializeStyledToolTip();
            if (string.IsNullOrEmpty(description))
                return;

            System.Drawing.Point clientPos = this.MainMenu.PointToClient(System.Windows.Forms.Cursor.Position);
            this.StyledToolTip.Show(description, this.MainMenu, clientPos.X + 12, clientPos.Y + 24);
        }

        private void AddMenuItems(
            ToolStripMenuItem menu,
            params object[] items)
        {
            foreach (object item in items)
            {
                if (item is string text && text == "-")
                {
                    menu.DropDownItems.Add(new ToolStripSeparator());
                    continue;
                }

                if (item is Tuple<string, string> entry)
                {
                    menu.DropDownItems.Add(
                        CreateMenuItem(
                            entry.Item1,
                            entry.Item2
                        )
                    );
                }
            }
        }

        private Tuple<string, string> MenuItem(string name, string description)
        {
            return new Tuple<string, string>(name, description);
        }

        private void addMenuComponents()
        {
            // 
            // File
            // 
            AddMenuItems(
                this.FileMenu,
                MenuItem("New", "Creates a new empty workspace."),
                MenuItem("New File", "Creates a new source file."),
                MenuItem("New Project", "Creates a new GeistStudio project."),
                "-",
                MenuItem("Open...", "Opens an existing file."),
                MenuItem("Open Folder...", "Opens a folder as a project."),
                MenuItem("Recent Files", "Shows recently opened files."),
                "-",
                MenuItem("Save", "Saves the current file."),
                MenuItem("Save As...", "Saves the current file with a different name."),
                MenuItem("Save All", "Saves all opened files."),
                "-",
                MenuItem("Close", "Closes the current file."),
                MenuItem("Close All", "Closes all opened files."),
                "-",
                MenuItem("Settings", "Opens GeistStudio settings."),
                "-",
                MenuItem("Exit", "Closes GeistStudio.")
            );
            // 
            // Edit
            // 
            AddMenuItems(
                this.EditMenu,
                MenuItem("Undo", "Reverts the last action."),
                MenuItem("Redo", "Restores the last undone action."),
                "-",
                MenuItem("Cut", "Cuts the selected text."),
                MenuItem("Copy", "Copies the selected text."),
                MenuItem("Paste", "Pastes copied text."),
                MenuItem("Delete", "Deletes the selected content."),
                "-",
                MenuItem("Find", "Searches text in the current document."),
                MenuItem("Replace", "Finds and replaces text."),
                MenuItem("Go to Line", "Jumps to a specific line number."),
                MenuItem("Select All", "Selects all content.")
            );
            // 
            // Selection
            // 
            AddMenuItems(
                this.SelectionMenu,
                MenuItem("Select All", "Selects the complete document."),
                MenuItem("Select Line", "Selects the current line."),
                MenuItem("Add Cursor", "Adds another text cursor."),
                MenuItem("Select All Occurrences", "Selects every matching occurrence.")
            );
            // 
            // View
            // 
            AddMenuItems(
                this.ViewMenu,
                MenuItem("Explorer", "Shows the project file explorer."),
                MenuItem("Search", "Opens the global search panel."),
                MenuItem("Terminal", "Opens the integrated terminal."),
                MenuItem("Problems", "Shows detected errors and warnings."),
                MenuItem("Output", "Displays build and application output."),
                MenuItem("Debug Console", "Opens the debugging console."),
                "-",
                MenuItem("Fullscreen", "Toggles fullscreen mode."),
                MenuItem("Zen Mode", "Activates distraction-free editing.")
            );
            // 
            // Go
            // 
            AddMenuItems(
                this.GoMenu,
                MenuItem("Go to File", "Searches and opens a file."),
                MenuItem("Go to Line", "Moves the cursor to a line."),
                MenuItem("Go to Symbol", "Searches symbols in the project."),
                MenuItem("Go to Definition", "Jumps to a symbol definition."),
                MenuItem("Go to References", "Finds all references.")
            );
            // 
            // Project
            // 
            AddMenuItems(
                this.ProjectMenu,
                MenuItem("New Project", "Creates a new project."),
                MenuItem("Open Project", "Opens an existing project."),
                MenuItem("Close Project", "Closes the current project."),
                MenuItem("Project Settings", "Changes project configuration.")
            );
            // 
            // Build
            // 
            AddMenuItems(
                this.BuildMenu,
                MenuItem("Build", "Compiles the current project."),
                MenuItem("Rebuild", "Cleans and rebuilds the project."),
                MenuItem("Clean", "Removes generated files."),
                MenuItem("Publish", "Creates a distributable build.")
            );
            // 
            // Debug
            // 
            AddMenuItems(
                this.DebugMenu,
                MenuItem("Start", "Starts debugging."),
                MenuItem("Start Without Debugging", "Runs the project without debugger."),
                MenuItem("Stop", "Stops the current execution."),
                MenuItem("Restart", "Restarts the application."),
                MenuItem("Toggle Breakpoint", "Adds or removes a breakpoint."),
                MenuItem("Step Over", "Executes the next line."),
                MenuItem("Step Into", "Enters the current function."),
                MenuItem("Step Out", "Leaves the current function.")
            );
            // 
            // Git
            // 
            AddMenuItems(
                this.GitMenu,
                MenuItem("Commit", "Creates a new Git commit."),
                MenuItem("Push", "Uploads changes to the repository."),
                MenuItem("Pull", "Downloads repository changes."),
                MenuItem("Fetch", "Fetches remote repository information."),
                MenuItem("Merge", "Combines branches."),
                MenuItem("Branches", "Manages Git branches."),
                MenuItem("Clone Repository", "Copies a remote repository.")
            );
            // 
            // Tools
            // 
            AddMenuItems(
                this.ToolsMenu,
                MenuItem("Terminal", "Opens the integrated terminal."),
                MenuItem("Extensions", "Manages installed extensions."),
                MenuItem("Format Document", "Formats the current document."),
                MenuItem("Options", "Opens editor options.")
            );
            // 
            // Window
            // 
            AddMenuItems(
                this.WindowMenu,
                MenuItem("Split Editor", "Splits the editor view."),
                MenuItem("Next Tab", "Moves to the next tab."),
                MenuItem("Previous Tab", "Moves to the previous tab.")
            );
            // 
            // Help
            // 
            AddMenuItems(
                this.HelpMenu,
                MenuItem("Documentation", "Opens GeistStudio documentation."),
                MenuItem("Keyboard Shortcuts", "Shows available shortcuts."),
                MenuItem("Check for Updates", "Checks for new versions."),
                MenuItem("About GeistStudio", "Shows information about GeistStudio.")
            );
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
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ShowInTaskbar = false;
                dialog.ClientSize = new Size(400, 180);
                dialog.BackColor = Color.FromArgb(42, 38, 78);
                dialog.ForeColor = Color.FromArgb(225, 220, 245);

                Label title = new Label();
                title.Text = "Create a new file";
                title.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                title.Location = new Point(20, 15);
                title.AutoSize = true;
                title.ForeColor = dialog.ForeColor;

                Label label = new Label();
                label.Text = "File name";
                label.Location = new Point(20, 55);
                label.AutoSize = true;
                label.ForeColor = dialog.ForeColor;

                TextBox textBox = new TextBox();
                textBox.Location = new Point(20, 80);
                textBox.Size = new Size(360, 25);
                textBox.BackColor = Color.FromArgb(55, 50, 95);
                textBox.ForeColor = dialog.ForeColor;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Text = "Untitled";
                textBox.SelectAll();

                Button create = new Button();
                create.Text = "Create";
                create.Location = new Point(210, 130);
                create.Size = new Size(80, 30);
                create.DialogResult = DialogResult.OK;

                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Location = new Point(300, 130);
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
                    OpenFileInNewTab(System.IO.Path.GetFileName(dialog.FileName), content);
                }
            }
        }



        private void OpenFileInNewTab(string title, string content)
        {
            TabPage page = new TabPage(title);

            Panel container = new Panel();
            container.Dock = DockStyle.Fill;
            container.BackColor = Color.FromArgb(30, 27, 58);


            Panel linePanel = new Panel();
            linePanel.Dock = DockStyle.Left;
            linePanel.Width = 50;
            linePanel.BackColor = Color.FromArgb(24, 21, 48);


            Label lineNumbers = new Label();
            lineNumbers.Dock = DockStyle.Fill;
            lineNumbers.Font = new Font("Consolas", 11F);
            lineNumbers.ForeColor = Color.FromArgb(130, 130, 160);
            lineNumbers.TextAlign = ContentAlignment.TopRight;
            lineNumbers.Padding = new Padding(0, 2, 6, 0);


            TextBox editor = new TextBox();
            editor.Multiline = true;
            editor.AcceptsTab = true;
            editor.WordWrap = false;
            editor.Dock = DockStyle.Fill;
            editor.Text = content;
            editor.Font = new Font("Consolas", 11F);
            editor.BackColor = Color.FromArgb(30, 27, 58);
            editor.ForeColor = Color.White;
            editor.BorderStyle = BorderStyle.None;



            void UpdateLineNumbers()
            {
                int lineCount = Math.Max(editor.Lines.Length, 1);
                StringBuilder sb = new StringBuilder();

                for (int i = 1; i <= lineCount; i++)
                    sb.AppendLine(i.ToString());

                lineNumbers.Text = sb.ToString();
            }

            editor.TextChanged += (s, e) =>
            {
                UpdateLineNumbers();
            };

            editor.FontChanged += (s, e) =>
            {
                lineNumbers.Font = editor.Font;
            };

            UpdateLineNumbers();


            linePanel.Controls.Add(lineNumbers);

            container.Controls.Add(editor);
            container.Controls.Add(linePanel);

            page.Controls.Add(container);

            FileList.TabPages.Add(page);
            FileList.SelectedTab = page;
            tabEditors.Add(page, editor);
        }

        private DialogResult ShowSaveDialog()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Save File";
                dialog.Size = new Size(400, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.BackColor = Color.FromArgb(42, 38, 78);

                Label title = new Label();
                title.Text = "Close File";
                title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(225, 220, 245);
                title.Location = new Point(16, 10);
                title.AutoSize = true;

                Label text = new Label();
                text.Text = "Do you want to save this file before closing?";
                text.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                text.ForeColor = Color.FromArgb(225, 220, 245);
                text.Location = new Point(20, 60);
                text.AutoSize = true;

                Button yes = new Button();
                yes.Text = "Save | Close";
                yes.Location = new Point(80, 100);
                yes.ForeColor = Color.FromArgb(225, 220, 245);
                yes.DialogResult = DialogResult.Yes;

                Button no = new Button();
                no.Text = "Don't Save | Close";
                no.Location = new Point(170, 100);
                no.Size = new Size(110, 22);
                no.ForeColor = Color.FromArgb(225, 220, 245);
                no.DialogResult = DialogResult.No;

                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Location = new Point(300, 100);
                cancel.ForeColor = Color.FromArgb(225, 220, 245);
                cancel.DialogResult = DialogResult.Cancel;

                dialog.Controls.Add(title);
                dialog.Controls.Add(text);
                dialog.Controls.Add(yes);
                dialog.Controls.Add(no);
                dialog.Controls.Add(cancel);


                return dialog.ShowDialog();
            }
        }

        private void SaveTabFile(TabPage page)
        {
            if (!tabEditors.ContainsKey(page))
                return;

            string fileName = page.Text;

            File.WriteAllText(fileName, tabEditors[page].Text);
        }



        private void FileList_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = FileList.TabPages[e.Index];
            Rectangle rect = e.Bounds;


            using (Brush bg = new SolidBrush(
                FileList.SelectedIndex == e.Index
                ? Color.FromArgb(55, 50, 95)
                : Color.FromArgb(30, 27, 58)))
            {
                e.Graphics.FillRectangle(bg, rect);
            }


            e.Graphics.DrawString(
                page.Text,
                tabFont,
                Brushes.White,
                rect.X + 10,
                rect.Y + 8);


            Rectangle closeButton = new Rectangle(
                rect.Right - 25,
                rect.Y + 8,
                16,
                16);

            if (e.Index > 0)
            {
                using (Pen pen = new Pen(closeColor, 2))
                {
                    using (Font closeFont = new Font(
                        "Segoe UI Symbol",
                        20F,
                        FontStyle.Regular))
                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        e.Graphics.DrawString(
                            "×",
                            closeFont,
                            Brushes.Red,
                            closeButton,
                            format);
                    }
                    /*e.Graphics.DrawLine(
                        pen,
                        closeButton.Left,
                        closeButton.Top,
                        closeButton.Right,
                        closeButton.Bottom);


                    e.Graphics.DrawLine(
                        pen,
                        closeButton.Right,
                        closeButton.Top,
                        closeButton.Left,
                        closeButton.Bottom);*/
                }
            }
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

        private void Navbar_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(45, 40, 80)))
            {
                e.Graphics.DrawLine(pen, 0, this.Navbar.Height - 1, this.Navbar.Width, this.Navbar.Height - 1);
            }
        }

        private void Sidebar_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(45, 40, 80)))
            {
                e.Graphics.DrawLine(pen, this.Sidebar.Width - 1, 0, this.Sidebar.Width - 1, this.Sidebar.Height);
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
            this.FileList = new System.Windows.Forms.TabControl();
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
            this.Navbar.BackColor = System.Drawing.Color.FromArgb(16, 14, 32);
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
            this.MainMenu.BackColor = System.Drawing.Color.FromArgb(16, 14, 32);
            this.MainMenu.ForeColor = System.Drawing.Color.FromArgb(225, 220, 245);
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
            this.Sidebar.BackColor = System.Drawing.Color.FromArgb(24, 21, 48);
            // 
            // MainContent
            // 
            this.MainContent.Controls.Add(this.FileList);
            this.MainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContent.Location = new System.Drawing.Point(226, 35);
            this.MainContent.Name = "MainContent";
            this.MainContent.Size = new System.Drawing.Size(1054, 556);
            this.MainContent.TabIndex = 2;
            this.MainContent.BackColor = System.Drawing.Color.FromArgb(30, 27, 58);
            this.MainContent.BackColor = Color.FromArgb(30, 27, 58);
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
            this.FileList.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FileList.ItemSize = new System.Drawing.Size(150, 34);
            //this.FileList.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.FileList.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            //this.FileList.Padding = new System.Drawing.Point(12, 6);
            this.FileList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.FileList_DrawItem);
            this.FileList.SelectedIndexChanged += new System.EventHandler(this.FileList_SelectedIndexChanged);
            this.FileList.BackColor = System.Drawing.Color.FromArgb(30, 27, 58);
            this.FileList.SizeMode = TabSizeMode.Fixed;
            this.FileList.ItemSize = new Size(150, 32);
            this.FileList.DrawItem += FileList_DrawItem;
            this.FileList.MouseDown += FileList_MouseDown;
            // 
            // home
            // 
            this.home.Controls.Add(this.WelcomePanel);
            this.home.Location = new System.Drawing.Point(4, 22);
            this.home.Name = "home";
            //this.home.Padding = new System.Windows.Forms.Padding(3);
            this.home.Size = new System.Drawing.Size(1046, 530);
            this.home.BackColor = System.Drawing.Color.FromArgb(30, 27, 58);
            this.home.TabIndex = 0;
            this.home.Text = "Home";
            this.home.UseVisualStyleBackColor = true;
            this.home.Paint += new System.Windows.Forms.PaintEventHandler(this.Sidebar_Paint);
            // 
            // WelcomePanel
            // 
            this.WelcomePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(27)))), ((int)(((byte)(58)))));
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
            this.WelcomeOpenButton.BackColor = System.Drawing.Color.FromArgb(124, 58, 237);
            this.WelcomeOpenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WelcomeOpenButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(124, 58, 237);
            this.WelcomeOpenButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WelcomeOpenButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.WelcomeOpenButton.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
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
            this.WelcomeNewButton.BackColor = System.Drawing.Color.FromArgb(124, 58, 237);
            this.WelcomeNewButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WelcomeNewButton.FlatAppearance.BorderSize = 0;
            this.WelcomeNewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WelcomeNewButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.WelcomeNewButton.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
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
            this.BackColor = System.Drawing.Color.FromArgb(30, 27, 58);
            this.Controls.Add(this.MainContent);
            this.Controls.Add(this.Sidebar);
            this.Controls.Add(this.Navbar);
            this.Name = "GeistStudioWin";
            this.Text = "GeistStudio";
            this.Load += new System.EventHandler(this.GeistStudioWin_Load);
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

        private Dictionary<TabPage, TextBox> tabEditors = new Dictionary<TabPage, TextBox>();
        private Font tabFont = new Font("Segoe UI", 9F);
        private Color tabBackground = Color.FromArgb(42, 38, 78);
        private Color closeColor = Color.FromArgb(220, 100, 100);

        private System.Windows.Forms.Panel Navbar;
        private System.Windows.Forms.Panel Sidebar;
        private System.Windows.Forms.Panel MainContent;
        private System.Windows.Forms.TabControl FileList;
        private System.Windows.Forms.TabPage home;

        private System.Windows.Forms.ToolTip StyledToolTip;
        private System.Windows.Forms.MenuStrip MainMenu;
        private ToolStripMenuItem FileMenu;
        private ToolStripMenuItem EditMenu;
        private ToolStripMenuItem SelectionMenu;
        private ToolStripMenuItem ViewMenu;
        private ToolStripMenuItem GoMenu;
        private ToolStripMenuItem ProjectMenu;
        private ToolStripMenuItem BuildMenu;
        private ToolStripMenuItem DebugMenu;
        private ToolStripMenuItem GitMenu;
        private ToolStripMenuItem ToolsMenu;
        private ToolStripMenuItem WindowMenu;
        private ToolStripMenuItem HelpMenu;

        private System.Windows.Forms.Panel WelcomePanel;
        private System.Windows.Forms.Label WelcomeTitleLabel;
        private System.Windows.Forms.Label WelcomeSubtitleLabel;
        private System.Windows.Forms.Button WelcomeNewButton;
        private System.Windows.Forms.Button WelcomeOpenButton;

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

