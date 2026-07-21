using System.Drawing;
using System.Windows.Forms;

namespace GeistStudio   // muss 1:1 übereinstimmen
{
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
}