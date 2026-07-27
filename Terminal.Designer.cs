using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace GeistStudio
{
    partial class Terminal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Panel titleBar;
        private Button closeButton;
        private Button maximizeButton;
        private Button minimizeButton;

        private Color closeColor = Color.FromArgb(220, 50, 50);
        private Color normalButtonColor = Color.FromArgb(35, 32, 70);

        [DllImport("user32.dll")]
        private static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam);

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private Button CreateWindowButton(string text, bool isClose = false)
        {
            Button btn = new Button();

            btn.Text = text;
            btn.Width = 45;
            btn.Cursor = Cursors.Hand;
            btn.Dock = DockStyle.Right;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = normalButtonColor;
            btn.Font = new Font("Segoe UI Symbol", 12);
            btn.ForeColor = Color.FromArgb(210, 210, 230);

            btn.MouseEnter += (s, e) => btn.BackColor = !isClose ? Color.FromArgb(70, 65, 110) : closeColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalButtonColor;

            return btn;
        }

        private void CreateCustomTitleBar()
        {
            titleBar = new Panel();
            titleBar.Dock = DockStyle.Top;
            titleBar.Height = 35;
            titleBar.BackColor = Color.FromArgb(26, 23, 55);

            this.Controls.Add(titleBar);

            Label title = new Label();
            title.Text = "GeistStudio Terminal";
            title.ForeColor = Color.FromArgb(230, 225, 245);
            title.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            title.Location = new Point(15, 8);
            title.AutoSize = true;

            titleBar.Controls.Add(title);

            minimizeButton = CreateWindowButton("─");
            minimizeButton.Dock = DockStyle.Right;

            maximizeButton = CreateWindowButton("□");
            maximizeButton.Dock = DockStyle.Right;

            closeButton = CreateWindowButton("×", true);
            closeButton.Dock = DockStyle.Right;

            minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            maximizeButton.Click += (s, e) => {
                this.WindowState =
                    this.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            };
            closeButton.Click += (s, e) => this.Close();

            titleBar.Controls.Add(minimizeButton);
            titleBar.Controls.Add(maximizeButton);
            titleBar.Controls.Add(closeButton);

            titleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(
                        Handle,
                        0xA1,
                        0x2,
                        0);
                }
            };
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(27)))), ((int)(((byte)(58)))));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Terminal";
            CreateCustomTitleBar();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        #endregion
    }
}