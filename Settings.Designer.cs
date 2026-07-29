using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace GeistStudio
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        private void StyleScrollbars(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Panel panel)
                {
                    panel.HandleCreated += (s, e) =>
                    {
                        NativeMethods.SetWindowTheme(
                            panel.Handle,
                            "Explorer",
                            null);
                    };

                    StyleScrollbars(panel);
                }
            }
        }

        private void CreatePage(string name)
        {
            SmoothScrollPanel page = new SmoothScrollPanel();

            page.Name = name + "Page";
            page.Dock = DockStyle.Fill;
            page.Visible = false;

            Pages.Add(name, page);

            MainContent.Controls.Add(page);
        }

        private void CreateSidebarButton(string text, int y)
        {
            Button button = new Button();

            button.Text = text;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(22, 20, 45);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 10F);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Size = new Size(200, 42);
            button.Cursor = Cursors.Hand;
            button.Location = new Point(10, y);

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(40, 36, 78);
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(22, 20, 45);
            button.Click += (s, e) => ShowPage(text);

            Sidebar.Controls.Add(button);
        }

        struct Setting {
            public String Name;
            public String Description;
            public String Type;

            public Setting(String name, String description, String type) {
                Name = name;
                Description = description;
                Type = type;
            }
        }

        private void CreateSettingCard(
            SmoothScrollPanel parent, 
            Setting setting,
            int y)
        {
            Panel card = new Panel();

            card.Size = new Size(520, 85);
            card.Location = new Point(40, y);
            card.BackColor = Color.FromArgb(37, 34, 72);

            Label title = new Label();
            title.Text = setting.Name;
            title.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.AutoSize = true;
            title.Location = new Point(20, 15);

            Label info = new Label();
            info.Text = setting.Description;
            info.Font = new Font("Segoe UI", 9F);
            info.ForeColor = Color.FromArgb(160, 160, 180);
            info.AutoSize = true;
            info.Location = new Point(20, 43);

            Button action = new Button();
            action.Text = "Change";
            action.Size = new Size(90, 32);
            action.Location = new Point(400, 26);
            action.FlatStyle = FlatStyle.Flat;
            action.FlatAppearance.BorderSize = 0;
            action.BackColor = Color.FromArgb(124, 58, 237);
            action.ForeColor = Color.White;
            action.Cursor = Cursors.Hand;
            action.Font = new Font("Segoe UI", 9F);

            if (setting.Type == "Reset")
            {
                action.Text = setting.Type;
                action.BackColor = Color.FromArgb(220, 50, 50);
            }

            if (setting.Type != "Reset")
            {
                action.MouseEnter += (s, e) => action.BackColor = Color.FromArgb(145, 80, 255);
                action.MouseLeave += (s, e) => action.BackColor = Color.FromArgb(124, 58, 237);
            }
            else
            {
                action.MouseEnter += (s, e) => action.BackColor = Color.FromArgb(255, 90, 90);
                action.MouseLeave += (s, e) => action.BackColor = Color.FromArgb(220, 50, 50);
            }

            card.Controls.Add(title);
            card.Controls.Add(info);
            card.Controls.Add(action);

            parent.AddContent(card);
        }

        private void CreateSettingsLayout(
            string pageName,
            string description,
            Setting[] settings)
        {
            CreatePage(pageName);
            CreateSidebarButton(pageName, -30 + (Pages.Count * 50));
            
            SmoothScrollPanel page = (SmoothScrollPanel)Pages[pageName];

            Label title = new Label();
            title.Text = pageName;
            title.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.AutoSize = true;
            title.Location = new Point(40, 30);

            page.AddContent(title);

            Label desc = new Label();
            desc.Text = description;
            desc.Font = new Font("Segoe UI", 10F);
            desc.ForeColor = Color.FromArgb(160, 160, 180);
            desc.AutoSize = true;
            desc.Location = new Point(43, 75);

            page.AddContent(desc);

            int y = 130;

            foreach (Setting setting in settings)
            {
                CreateSettingCard(page, setting, y);
                y += 110;
            }
        }

        private void CreateAllPages()
        {
            CreateSettingsLayout(
                "Home",
                "General GeistStudio settings and information.",
                new Setting[]
                {
                    new Setting (
                        "Enable startup animation",
                        "Change if the startup animation should be shown",
                        "Boolean"
                    ),
                    new Setting (
                        "Show welcome screen",
                        "Enable or Disable the Welcome Screen",
                        "Boolean"
                    ),
                    new Setting (
                        "Check for updates",
                        "Enable or Disable the Welcome Screen",
                        "Boolean"
                    ),
                    new Setting (
                        "Rest Settings",
                        "Reset ALL Settings to the Standard Value",
                        "Reset"
                    )
                });

            
            CreateSettingsLayout(
                "Appearance",
                "Customize the look and colors of GeistStudio.",
                new Setting[]
                {
                    new Setting (
                        "Dark theme", 
                        "Enable or Disable the darkmode",
                        "Boolean"
                    ),
                    new Setting (
                        "Accent color",
                        "Change the Accent Color",
                        "Color"
                    ),
                    new Setting (
                        "Editor transparency",
                        "Change the Edtior transparency",
                        "Transparency"
                    ),
                    new Setting (
                        "Window animations",
                        "Enable or Disable the Window animations",
                        "Boolean"
                    )
                });


            CreateSettingsLayout(
                "Editor",
                "Configure your coding environment.",
                new Setting[]
                {
                    new Setting (
                        "Font size",
                        "Change the Font size",
                        "Number"
                    ),
                    new Setting (
                        "Line numbers",
                        "Enable or Disable the Line numbers",
                        "Boolean"
                    ),
                    new Setting (
                        "Syntax highlighting",
                        "Enable or Disable the Syntax highlighting",
                        "Boolean"
                    ),
                    new Setting (
                        "Auto save",
                        "Enable or Disable the Auto saves",
                        "Boolean"
                    )
                });


            CreateSettingsLayout(
                "Terminal",
                "Manage the integrated terminal.",
                new Setting[]
                {
                    new Setting (
                        "Terminal Font",
                        "Change the Terminal Font",
                        "Font"
                    ),
                    new Setting (
                        "Command history",
                        "Enable or Disable the Command history",
                        "boolean"
                    ),
                    new Setting (
                        "Default shell",
                        "enable or Disable the Default shell",
                        "Boolean"
                    )
                });


            CreateSettingsLayout(
                "Extensions",
                "Manage GeistStudio extensions.",
                new Setting[]
                {
                    new Setting (
                        "Extension updates",
                        "Enable or Disable the automatic Extension updates",
                        "Boolean"
                    ),
                    new Setting (
                        "Installed extensions",
                        "Enable or Disable the installes Extensions",
                        "Boolean"
                    ),
                    new Setting (
                        "Extension Marketplace",
                        "Enable or Disable the Extension Marketplace",
                        "Boolean"
                    )
                });


            CreateSettingsLayout(
                "About",
                "Information about GeistStudio.",
                new Setting[]
                {
                    new Setting (
                        "Version",
                        "Set the current Edtior Version",
                        "Number"
                    ),
                    new Setting (
                        "License",
                        "Choose the current License",
                        "Dropdown"
                    ),
                    new Setting (
                        "Credits",
                        "Enable or Disable the Credits",
                        "Boolean"
                    )
                });
        }

        private void ShowPage(string name)
        {
            if (!Pages.ContainsKey(name))
                return;

            foreach (var kvp in Pages)
                kvp.Value.Visible = false;

            var page = Pages[name];
            page.Visible = true;
            page.BringToFront();

            if (page is SmoothScrollPanel scrollPanel)
                scrollPanel.RefreshLayout();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Sidebar = new System.Windows.Forms.Panel();
            this.MainContent = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            //
            // Sidebar
            //
            this.Sidebar.BackColor = System.Drawing.Color.FromArgb(22, 20, 45);
            this.Sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.Sidebar.Location = new System.Drawing.Point(0, 0);
            this.Sidebar.Name = "Sidebar";
            this.Sidebar.Size = new System.Drawing.Size(220, 450);
            this.Sidebar.TabIndex = 0;

            //
            // MainContent
            //
            this.MainContent.BackColor = System.Drawing.Color.FromArgb(30, 27, 58);
            this.MainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContent.Location = new System.Drawing.Point(220, 0);
            this.MainContent.Name = "MainContent";
            this.MainContent.TabStop = true;
            this.MainContent.Size = new System.Drawing.Size(580, 450);
            this.MainContent.TabIndex = 1;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(27)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainContent);
            this.Controls.Add(this.Sidebar);
            this.Name = "Settings";
            this.Text = "Settings";
            Util.CreateCustomTitleBar(this, "GeistStudio Settings");
            this.ResumeLayout(false);

        }

        #endregion


        private void InitializeSettingsPage()
        {
            CreateAllPages();

            ShowPage("Home");
        }

        private Dictionary<string, SmoothScrollPanel> Pages = new Dictionary<string, SmoothScrollPanel>();

        private Panel Sidebar;
        private Panel MainContent;
    }
}