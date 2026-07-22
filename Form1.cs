using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeistStudio
{
    public partial class GeistStudioWin : Form
    {
        public GeistStudioWin()
        {
            InitializeComponent();
            addComponents();
        }
    }

    public class GeistStudioColorTable : ProfessionalColorTable
    {
        private static readonly Color Background = Color.FromArgb(30, 27, 58);
        private static readonly Color HoverBg = Color.FromArgb(60, 45, 110);
        private static readonly Color SelectedBg = Color.FromArgb(124, 58, 237);
        private static readonly Color BorderColor = Color.FromArgb(45, 40, 80);

        public override Color MenuStripGradientBegin => Background;
        public override Color MenuStripGradientEnd => Background;

        public override Color MenuItemSelectedGradientBegin => HoverBg;
        public override Color MenuItemSelectedGradientEnd => HoverBg;
        public override Color MenuItemSelected => HoverBg;

        public override Color MenuItemPressedGradientBegin => SelectedBg;
        public override Color MenuItemPressedGradientEnd => SelectedBg;

        public override Color MenuItemBorder => SelectedBg;
        public override Color MenuBorder => BorderColor;

        public override Color ToolStripDropDownBackground => Background;

        public override Color ImageMarginGradientBegin => Background;
        public override Color ImageMarginGradientMiddle => Background;
        public override Color ImageMarginGradientEnd => Background;

        public override Color SeparatorDark => Color.FromArgb(55, 50, 90);
        public override Color SeparatorLight => Color.FromArgb(55, 50, 90);
    }

    public class GeistStudioMenuRenderer : ToolStripProfessionalRenderer
    {
        public GeistStudioMenuRenderer() : base(new GeistStudioColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var item = e.Item;
            var g = e.Graphics;
            var rect = new Rectangle(Point.Empty, item.Size);

            if (item.Selected || item.Pressed)
            {
                Color fill = item.Pressed
                    ? Color.FromArgb(124, 58, 237)
                    : Color.FromArgb(60, 45, 110);

                using (var brush = new SolidBrush(fill))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Kein Rahmen um die Menüleiste selbst
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.FromArgb(225, 220, 245);
            base.OnRenderItemText(e);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("uxtheme.dll")]
        public static extern int SetWindowTheme(
            IntPtr hwnd,
            string pszSubAppName,
            string pszSubIdList);
    }

    public class DarkTabControl : TabControl
    {
        public Color BackgroundColorDark { get; set; } = Color.FromArgb(38, 35, 72);

        public DarkTabControl()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Appearance = TabAppearance.FlatButtons;
            this.ItemSize = new Size(150, 32);
            this.SizeMode = TabSizeMode.Fixed;
            this.Padding = new Point(0, 0);

            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackgroundColorDark);

            for (int i = 0; i < TabPages.Count; i++)
            {
                Rectangle rect = GetTabRect(i);
                bool selected = SelectedIndex == i;

                using (SolidBrush brush = new SolidBrush(
                    selected
                        ? Color.FromArgb(55, 50, 95)
                        : Color.FromArgb(26, 23, 55)))
                {
                    e.Graphics.FillRectangle(
                        brush,
                        rect);
                }

                if (selected)
                {
                    using (SolidBrush accent = new SolidBrush(Color.FromArgb(140, 110, 255)))
                    {
                        e.Graphics.FillRectangle(
                            accent,
                            rect.X,
                            (rect.Y + rect.Height) - 5,
                            rect.Width,
                            3);
                    }
                }

                using (Brush text = new SolidBrush(
                    selected
                    ? Color.White
                    : Color.FromArgb(160, 155, 190)))
                {
                    e.Graphics.DrawString(
                        TabPages[i].Text,
                        Font,
                        text,
                        rect.X + 8,
                        rect.Y + 5);
                }

                if (i > 0)
                {
                    Rectangle closeButton = new Rectangle(
                        rect.Right - 20,
                        rect.Y + 4,
                        16,
                        16);

                    using (Font closeFont = new Font("Segoe UI Symbol", 20F))
                    using (StringFormat format = new StringFormat())
                    using (Brush closeBrush = new SolidBrush(Color.Red))
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        e.Graphics.DrawString(
                            "×",
                            closeFont,
                            closeBrush,
                            closeButton,
                            format);
                    }
                }
            }

            Rectangle content = new Rectangle(
                0,
                ItemSize.Height,
                Width,
                Height - ItemSize.Height);

            using (SolidBrush brush = new SolidBrush(BackgroundColorDark))
            {
                e.Graphics.FillRectangle(
                    brush,
                    content);
            }
        }

        public void RecalculateTabWidth()
        {
            if (this.TabCount == 0)
                return;

            using (Graphics g = this.CreateGraphics())
            {
                TabPage lastTab = this.TabPages[this.TabCount - 1];

                Size textSize = TextRenderer.MeasureText(
                    g,
                    lastTab.Text,
                    this.Font);

                int width = Math.Max(textSize.Width + 40, 60);

                this.ItemSize = new Size(
                    width,
                    this.ItemSize.Height);
            }
        }
    }

    public class SyncedRichTextBox : RichTextBox
    {
        public new event EventHandler VScroll;
        public new event EventHandler HScroll;

        private const int WM_VSCROLL = 0x115;
        private const int WM_HSCROLL = 0x114;
        private const int WM_MOUSEWHEEL = 0x20A;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                VScroll?.Invoke(this, EventArgs.Empty);
            }
            else if (m.Msg == WM_HSCROLL)
            {
                HScroll?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
