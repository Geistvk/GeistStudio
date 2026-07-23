using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
    }
}
