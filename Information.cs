using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeistStudio
{
    public partial class Information : Form
    {
        public Information()
        {
            InitializeComponent();
            InitializeVersionGUI();
            LoadVersionTree();
        }

        public void Open()
        {
            Show();
            Activate();
        }

        private TreeNode CreateVersionNode(VersionInfo version)
        {
            TreeNode node = new TreeNode();
            node.Text = version.Name;
            node.Tag = version;

            foreach (VersionInfo child in version.Children)
                node.Nodes.Add(CreateVersionNode(child));

            return node;
        }

        private TreeNode CreateNode(VersionInfo version)
        {
            TreeNode node = new TreeNode(version.Name);

            if (!string.IsNullOrEmpty(version.Title))
                node.Text += "  " + version.Title;

            foreach (string change in version.Changes)
                node.Nodes.Add(new TreeNode("• " + change));

            foreach (VersionInfo child in version.Children)
                node.Nodes.Add(CreateNode(child));

            return node;
        }

        private void LoadVersionTree()
        {
            VersionTree.Nodes.Clear();

            Dictionary<string, object> root = (Dictionary<string, object>)JsonParser.LoadEmbeddedJson("GeistStudio.GeistStudioData.json");
            Dictionary<string, object> allVersions = (Dictionary<string, object>)root["AllVersions"];

            VersionManager manager = new VersionManager(allVersions);
            List<VersionInfo> versions = manager.GetVersionTree();

            foreach (VersionInfo version in versions)
            {
                VersionTree.Nodes.Add(CreateVersionNode(version));
            }
        }

        private void StyleChangeCard(Panel card)
        {
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 12;

                    Rectangle rect =
                        new Rectangle(
                            0,
                            0,
                            card.Width - 1,
                            card.Height - 1
                        );

                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);

                    path.CloseFigure();

                    using (Pen pen =
                        new Pen(Color.FromArgb(80, 70, 130), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };
        }

        private Panel CreateChangeCard(string text)
        {
            Panel card = new Panel();

            int width = Math.Max(
                300,
                ChangesContainer.ClientSize.Width - 45
            );

            card.Width = width;
            card.BackColor = Color.FromArgb(40, 36, 75);
            card.Margin = new Padding(5, 5, 5, 8);
            card.Padding = new Padding(10);
            card.AutoSize = true;

            Label label = new Label();
            label.Text = "✓  " + text;
            label.ForeColor = Color.White;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.AutoSize = true;
            label.MaximumSize = new Size(width - 35, 0);
            label.TextAlign = ContentAlignment.MiddleLeft;

            Size textSize = TextRenderer.MeasureText(
                label.Text,
                label.Font,
                new Size(width - 35, 0),
                TextFormatFlags.WordBreak
            );

            int height = label.PreferredHeight + card.Padding.Top + card.Padding.Bottom;
            card.Height = Math.Max(10, height);

            label.Size = new Size(width - 35, textSize.Height);
            label.Location =
                new Point(
                    ((card.Width - label.Width) / 2) - 5,
                    (card.Height - label.Height) / 2
                );

            card.Controls.Add(label);
            StyleChangeCard(card);

            label.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(55, 50, 100);
            label.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(40, 36, 75);
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(55, 50, 100);
            card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(40, 36, 75);

            return card;
        }

        private void VersionTree_AfterSelect(
            object sender,
            TreeViewEventArgs e)
        {
            VersionInfo version = e.Node.Tag as VersionInfo;

            if (version == null)
                return;

            SelectedVersionName.Text = version.Name;
            SelectedVersionTitle.Text = version.Title ?? "";

            ChangesContainer.Controls.Clear();

            foreach (string change in version.Changes)
            {
                Panel card = CreateChangeCard(change);
                ChangesContainer.Controls.Add(card);
            }
        }
    }

    public static class VersionReader
    {
        public static void PrintVersions(Dictionary<string, object> data)
        {
            foreach (var item in data)
            {
                if (item.Value is Dictionary<string, object> versionData)
                {
                    if (versionData.ContainsKey("Title"))
                    {
                        Console.WriteLine("Version: " + item.Key);
                        Console.WriteLine("Title: " + versionData["Title"]);

                        Console.WriteLine("Changes:");

                        if (versionData["Changes"] is Dictionary<string, object> changes)
                        {
                            foreach (var change in changes)
                            {
                                Console.WriteLine(
                                    "   " + change.Key + ": " + change.Value
                                );
                            }
                        }

                        Console.WriteLine();
                    }

                    PrintVersions(versionData);
                }
            }
        }
    }
}
