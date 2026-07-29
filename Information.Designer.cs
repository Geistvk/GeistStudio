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
using System.Xml.Linq;

namespace GeistStudio
{
    partial class Information
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

        private GraphicsPath CreateRoundedRectangle(
            Rectangle rect,
            int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

            path.CloseFigure();

            return path;
        }

        private void FillRoundedRectangle(
            Graphics g,
            Brush brush,
            Rectangle rect,
            int radius)
        {
            using (GraphicsPath path = CreateRoundedRectangle(rect, radius))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawRoundedRectangle(
            Graphics g,
            Pen pen,
            Rectangle rect,
            int radius)
        {
            using (GraphicsPath path = CreateRoundedRectangle(rect, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private Rectangle GetArrowBounds(TreeNode node)
        {
            const int arrowSize = 12;

            return new Rectangle(
                node.Level * VersionTree.Indent + 8,
                node.Bounds.Top + (VersionTree.ItemHeight - arrowSize) / 2,
                arrowSize,
                arrowSize);
        }

        private void VersionTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            VersionInfo info = e.Node.Tag as VersionInfo;
            int indent = e.Node.Level * 24;

            Rectangle arrowRect = GetArrowBounds(e.Node);

            int arrowX = arrowRect.X;
            int arrowY = arrowRect.Y;

            int cardX = indent + 24;
            int cardWidth = Math.Min(240, VersionTree.ClientSize.Width - cardX - 10);
            int cardHeight = 30;

            Rectangle card = new Rectangle(
                cardX,
                e.Bounds.Top + 2,
                cardWidth,
                cardHeight);

            Color back = Color.FromArgb(38, 36, 68);
            Color border = Color.FromArgb(55, 52, 90);

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                back = Color.FromArgb(124, 58, 237);
                border = Color.FromArgb(150, 90, 255);
            }

            using (SolidBrush b = new SolidBrush(back))
            using (Pen p = new Pen(border))
            {
                FillRoundedRectangle(g, b, card, 8);
                DrawRoundedRectangle(g, p, card, 8);
            }

            if (e.Node.Nodes.Count > 0)
            {
                Point[] points;

                if (e.Node.IsExpanded)
                {
                    points = new[]
                    {
                        new Point(arrowRect.Left, arrowRect.Top + 2),
                        new Point(arrowRect.Right, arrowRect.Top + 2),
                        new Point(arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom - 2)
                    };
                }
                else
                {
                    points = new[]
                    {
                        new Point(arrowRect.Left + 2, arrowRect.Top),
                        new Point(arrowRect.Right - 2, arrowRect.Top + arrowRect.Height / 2),
                        new Point(arrowRect.Left + 2, arrowRect.Bottom)
                    };
                }

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
                {
                    g.FillPolygon(brush, points);
                }
            }

            string icon = "📄";

            if (info != null && info.Children.Count > 0)
                icon = "📁";

            using (Font iconFont = new Font("Segoe UI Emoji", 10))
            using (Font textFont = new Font("Segoe UI Semibold", 10))
            {
                g.DrawString(
                    icon,
                    iconFont,
                    Brushes.White,
                    card.Left + 10,
                    card.Top + 5);

                g.DrawString(
                    e.Node.Text,
                    textFont,
                    Brushes.White,
                    card.Left + 34,
                    card.Top + 5);
            }

            e.DrawDefault = false;
        }

        private void VersionTree_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = VersionTree.GetNodeAt(e.Location);

            if (node == null)
                return;

            Rectangle arrowRect = GetArrowBounds(node);

            if (arrowRect.Contains(e.Location) && node.Nodes.Count > 0)
            {
                if (node.IsExpanded)
                    node.Collapse();
                else
                    node.Expand();

                return;
            }

            VersionTree.SelectedNode = node;
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

            this.components = new System.ComponentModel.Container();
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(27)))), ((int)(((byte)(58)))));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainContent);
            this.Controls.Add(this.Sidebar);
            this.Text = "Information";
            Util.CreateCustomTitleBar(this, "GeistStudio Versions");
            this.ResumeLayout(false);
        }

        private void InitializeVersionGUI()
        {
            //
            // Versions Tree
            //
            VersionTree = new TreeView();
            VersionTree.Dock = DockStyle.Left;
            VersionTree.Width = 280;
            VersionTree.BorderStyle = BorderStyle.None;
            VersionTree.Font = new Font("Segoe UI", 10);
            VersionTree.AfterSelect += VersionTree_AfterSelect;
            VersionTree.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            VersionTree.BackColor = Color.FromArgb(22, 20, 45);
            VersionTree.ForeColor = Color.White;
            VersionTree.ItemHeight = 36;
            VersionTree.Indent = 24;
            VersionTree.Scrollable = true;
            VersionTree.FullRowSelect = false;
            VersionTree.ShowLines = false;
            VersionTree.ShowPlusMinus = false;
            VersionTree.ShowRootLines = true;
            VersionTree.HideSelection = false;
            VersionTree.HotTracking = true;
            VersionTree.DrawNode += VersionTree_DrawNode;
            VersionTree.MouseDown += VersionTree_MouseDown;
            //
            // Detail Panel
            //
            Panel detailPanel = new Panel();
            detailPanel.Dock = DockStyle.Fill;
            detailPanel.Padding = new Padding(25);
            detailPanel.BackColor = Color.FromArgb(30, 27, 58);
            //
            // Versionsname
            //
            SelectedVersionName = new Label();
            SelectedVersionName.Dock = DockStyle.Top;
            SelectedVersionName.Height = 35;
            SelectedVersionName.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            SelectedVersionName.ForeColor = Color.FromArgb(160, 160, 180);
            SelectedVersionName.Text = "Keine Version ausgewählt";
            //
            // Title
            //
            SelectedVersionTitle = new Label();
            SelectedVersionTitle.Dock = DockStyle.Top;
            SelectedVersionTitle.Height = 60;
            SelectedVersionTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            SelectedVersionTitle.ForeColor = Color.White;
            SelectedVersionTitle.Text = "";
            //
            // Changes Bereich
            //
            ChangesContainer = new FlowLayoutPanel();
            ChangesContainer.Dock = DockStyle.Fill;
            ChangesContainer.FlowDirection = FlowDirection.TopDown;
            ChangesContainer.WrapContents = false;
            ChangesContainer.AutoScroll = true;
            ChangesContainer.Padding = new Padding(5);

            detailPanel.Controls.Add(ChangesContainer);
            detailPanel.Controls.Add(SelectedVersionTitle);
            detailPanel.Controls.Add(SelectedVersionName);
            MainContent.Controls.Add(detailPanel);
            Sidebar.Controls.Add(VersionTree);
        }

        #endregion

        private Panel Sidebar;
        private Panel MainContent;

        private TreeView VersionTree;

        private Label SelectedVersionTitle;
        private Label SelectedVersionName;

        private FlowLayoutPanel ChangesContainer;
    }


    public class VersionInfo
    {
        public string Name;
        public string Title;

        public List<string> Changes = new List<string>();
        public List<VersionInfo> Children = new List<VersionInfo>();
    }


    public class VersionManager
    {
        private Dictionary<string, object> versions;

        public VersionManager(Dictionary<string, object> versions)
        {
            this.versions = versions;
        }

        public List<string> GetAllVersions()
        {
            List<string> result = new List<string>();

            IterateVersions(versions, result);

            return result;
        }

        private VersionInfo ParseVersion(
            string name,
            object value)
        {
            VersionInfo info = new VersionInfo();
            info.Name = name;

            if (value is Dictionary<string, object> data)
            {
                if (data.ContainsKey("Title"))
                {
                    info.Title = data["Title"].ToString();
                }

                if (data.ContainsKey("Changes"))
                {
                    //if (data["Changes"] is Dictionary<string, object> changes)
                    if (data["Changes"] is List<object> changes)
                    {
                        foreach (var change in changes)
                        {
                            /*info.Changes.Add(
                                change.Key + ": " +
                                change.Value
                            );*/
                            info.Changes.Add((string)change);
                        }
                    }
                }

                foreach (var child in data)
                {
                    if (child.Key != "Title" && child.Key != "Changes")
                    {
                        info.Children.Add(
                            ParseVersion(
                                child.Key,
                                child.Value
                            )
                        );
                    }
                }
            }


            return info;
        }

        public List<VersionInfo> GetVersionTree()
        {
            List<VersionInfo> result =
                new List<VersionInfo>();

            foreach (var item in versions)
            {
                result.Add(
                    ParseVersion(
                        item.Key,
                        item.Value
                    )
                );
            }

            return result;
        }

        private void IterateVersions(
            Dictionary<string, object> current,
            List<string> result)
        {
            foreach (var item in current)
            {
                if (item.Value is Dictionary<string, object> dict)
                {
                    if (dict.ContainsKey("Title"))
                    {
                        result.Add(item.Key);
                    }

                    IterateVersions(dict, result);
                }
            }
        }

        public Dictionary<string, object> GetVersion(string versionName)
        {
            return FindVersion(versions, versionName);
        }

        private Dictionary<string, object> FindVersion(
            Dictionary<string, object> current,
            string name)
        {
            foreach (var item in current)
            {
                if (item.Key == name)
                    return item.Value as Dictionary<string, object>;


                if (item.Value is Dictionary<string, object> dict)
                {
                    var result = FindVersion(dict, name);

                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        public Dictionary<string, object> GetChanges(
            string version)
        {
            var target = FindVersion(versions, version);

            if (target == null)
                return null;

            return target["Changes"]
                as Dictionary<string, object>;
        }
    }
}