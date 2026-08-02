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
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GeistStudio
{
    partial class About
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

        public void Open()
        {
            Show();
            Activate();
        }

        private void About_Resize(object sender, EventArgs e)
        {
            if (main == null)
                return;

            int padding = Math.Max(25, this.Width / 25);
            int spacing = 20;
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            main.Location = new Point(0, 0);
            main.Size = this.ClientSize;

            int headerHeight = Math.Max(100, height / 5);
            header.Location = new Point(padding, 40 + padding);
            header.Size = new Size(width - padding * 2, headerHeight);
            accent.Size = new Size(5, headerHeight);

            int contactHeight = 70;
            contact.Location = new Point(padding, height - padding - contactHeight);
            contact.Size = new Size(width - padding * 2, contactHeight);

            int middleTop = header.Bottom + spacing;
            int middleBottom = contact.Top - spacing;
            int middleHeight = middleBottom - middleTop;
            int cardWidth = (width - padding * 2 - spacing) / 2;
            info.Location = new Point(padding, middleTop);
            info.Size = new Size(cardWidth, Math.Max(150, middleHeight));

            license.Location = new Point(padding + cardWidth + spacing, middleTop);
            license.Size = new Size(cardWidth, Math.Max(150, middleHeight));
        }

        #region Windows Form Designer generated code

        private Button addContactButton(String title, String link)
        {
            Button contactButton = new Button();
            contactButton.Text = title;
            contactButton.Size = new Size(95, 35);
            contactButton.FlatStyle = FlatStyle.Flat;
            contactButton.FlatAppearance.BorderSize = 0;
            contactButton.BackColor = Util.Config.Colors.Background.Button;
            contactButton.ForeColor = Util.Config.Colors.Foreground.Text;
            contactButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            contactButton.Cursor = Cursors.Hand;

            contactButton.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = link,
                        UseShellExecute = true
                    });
            };

            return contactButton;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            loadAboutData();
            this.components = new System.ComponentModel.Container();

            this.SuspendLayout();

            this.BackColor = Util.Config.Colors.Background.About.Background;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(850, 520);
            this.Text = "About " + this.appName;
            this.FormBorderStyle = FormBorderStyle.None;

            Util.CreateCustomTitleBar(this, "About " + this.appName);


            // Main container
            main = new Panel();
            main.Dock = DockStyle.Fill;
            main.BackColor = Util.Config.Colors.Background.About.Background;
            main.Padding = new Padding(35);
            this.Controls.Add(main);


            // Header card
            header = new Panel();
            header.BackColor = Util.Config.Colors.Background.About.Card;

            main.Controls.Add(header);


            Label title = new Label();
            title.Text = this.appName;
            title.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            title.ForeColor = Util.Config.Colors.Foreground.Text;
            title.AutoSize = true;
            title.Location = new Point(25, 15);

            header.Controls.Add(title);


            Label subtitle = new Label();
            subtitle.Text = "Modern IDE for GeistScript";
            subtitle.Font = new Font("Segoe UI", 11F);
            subtitle.ForeColor = Util.Config.Colors.Foreground.About.SubText;
            subtitle.AutoSize = true;
            subtitle.Location = new Point(30, 65);

            header.Controls.Add(subtitle);


            accent = new Panel();
            accent.Location = new Point(0, 0);
            accent.BackColor = Util.Config.Colors.Background.Accent;

            header.Controls.Add(accent);



            // Information card
            info = new Panel();
            info.BackColor = Util.Config.Colors.Background.About.Card;

            main.Controls.Add(info);


            Label infoTitle = new Label();
            infoTitle.Text = "Information";
            infoTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            infoTitle.ForeColor = Util.Config.Colors.Foreground.Text;
            infoTitle.AutoSize = true;
            infoTitle.Location = new Point(20, 20);

            info.Controls.Add(infoTitle);


            Label version = new Label();
            version.Text = $"Version\n{this.curVersion}";
            version.Font = new Font("Segoe UI", 10F);
            version.ForeColor = Util.Config.Colors.Foreground.About.Data;
            version.AutoSize = true;
            version.Location = new Point(20, 60);

            info.Controls.Add(version);


            Label copyright = new Label();
            copyright.Text = "Copyright\n© 2026 " + this.author;
            copyright.Font = new Font("Segoe UI", 10F);
            copyright.ForeColor = Util.Config.Colors.Foreground.About.Data;
            copyright.AutoSize = true;
            copyright.Location = new Point(20, 120);

            info.Controls.Add(copyright);



            // License card
            license = new Panel();
            license.BackColor = Util.Config.Colors.Background.About.Card;

            main.Controls.Add(license);


            Label licenseTitle = new Label();
            licenseTitle.Text = "License";
            licenseTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            licenseTitle.ForeColor = Util.Config.Colors.Foreground.Text;
            licenseTitle.AutoSize = true;
            licenseTitle.Location = new Point(20, 20);

            license.Controls.Add(licenseTitle);


            Label licenseText = new Label();
            licenseText.Text = this.licenseData;
            licenseText.Font = new Font("Segoe UI", 10F);
            licenseText.ForeColor = Util.Config.Colors.Foreground.About.Data;
            licenseText.AutoSize = true;
            licenseText.Location = new Point(20, 60);

            license.Controls.Add(licenseText);



            // Bottom contact area
            contact = new Panel();
            contact.BackColor = Util.Config.Colors.Background.About.Contact;
            contact.Padding = new Padding(10);

            main.Controls.Add(contact);


            Label contactText = new Label();
            contactText.Text = $"Contact: {this.author}";
            contactText.Font = new Font("Segoe UI", 10F);
            contactText.ForeColor = Util.Config.Colors.Foreground.Text;
            contactText.AutoSize = true;
            contactText.Location = new Point(20, 22);

            contact.Controls.Add(contactText);


            Panel contactBtns = new Panel();
            contactBtns.Dock = DockStyle.Right;
            contactBtns.Width = 340;
            contactBtns.BackColor = Color.Transparent;


            Button source = addContactButton("Sourcecode", this.sourceCode);
            Button github = addContactButton("Github", this.authorGithub);
            Button email = addContactButton("Email", this.authorEmail);


            contactBtns.Resize += (s, e) =>
            {
                int y = (contactBtns.Height - github.Height) / 2;

                source.Location = new Point(10, y);
                github.Location = new Point(120, y);
                email.Location = new Point(230, y);
            };


            contactBtns.Controls.Add(source);
            contactBtns.Controls.Add(github);
            contactBtns.Controls.Add(email);

            contact.Controls.Add(contactBtns);


            this.ResumeLayout(false);

            this.Resize += About_Resize;

            About_Resize(null, null);
        }

        #endregion

        private Panel main;
        private Panel accent;
        private Panel header;
        private Panel info;
        private Panel license;
        private Panel contact;
    }
}