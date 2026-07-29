using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
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

    public static class EntryAssemblyInfo
    {
        private static string _executablePath;

        public static string ExecutablePath
        {
            get
            {
                if (_executablePath == null)
                {
                    PermissionSet permissionSets = new PermissionSet(PermissionState.None);
                    permissionSets.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
                    permissionSets.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
                    permissionSets.Assert();

                    string uriString = null;
                    var entryAssembly = Assembly.GetEntryAssembly();

                    if (entryAssembly == null)
                        uriString = Process.GetCurrentProcess().MainModule.FileName;
                    else
                        uriString = entryAssembly.CodeBase;

                    PermissionSet.RevertAssert();

                    if (string.IsNullOrWhiteSpace(uriString))
                        throw new Exception("Can not Get EntryAssembly or Process MainModule FileName");
                    else
                    {
                        var uri = new Uri(uriString);
                        if (uri.IsFile)
                            _executablePath = string.Concat(uri.LocalPath, Uri.UnescapeDataString(uri.Fragment));
                        else
                            _executablePath = uri.ToString();
                    }
                }

                return _executablePath;
            }
        }
    }

    public static class FormUtils
    {
        public static void SetDefaultIcon()
        {
            var icon = Icon.ExtractAssociatedIcon(EntryAssemblyInfo.ExecutablePath);
            typeof(Form)
                .GetField("defaultIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .SetValue(null, icon);
        }
    }

    public class GeistStudioColorTable : ProfessionalColorTable
    {
        private static readonly Color Background = Util.Config.Colors.Background.Background;
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
        private Color closeNormalColor = Color.FromArgb(140, 140, 140);
        private Color closeHoverColor = Color.FromArgb(220, 50, 50);
        public Timer closeHoverTimer;
        public float closeHoverProgress = 0f;
        public bool closeHovered = false;

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

            this.closeHoverTimer = new Timer();
            this.closeHoverTimer.Interval = 15;

            this.closeHoverTimer.Tick += (s, e) =>
            {
                float speed = 0.08f;

                if (this.closeHovered)
                    this.closeHoverProgress = Math.Min(1f, this.closeHoverProgress + speed);
                else
                    this.closeHoverProgress = Math.Max(0f, this.closeHoverProgress - speed);

                Invalidate();
            };

            this.closeHoverTimer.Start();
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
                    using (SolidBrush accent = new SolidBrush(Util.Config.Colors.Foreground.Accent))
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

                    this.closeHovered = closeButton.Contains(PointToClient(MousePosition));

                    Color currentColor = Color.FromArgb(
                        (int)(closeNormalColor.R + (closeHoverColor.R - closeNormalColor.R) * closeHoverProgress),
                        (int)(closeNormalColor.G + (closeHoverColor.G - closeNormalColor.G) * closeHoverProgress),
                        (int)(closeNormalColor.B + (closeHoverColor.B - closeNormalColor.B) * closeHoverProgress)
                    );


                    using (Font closeFont = new Font("Segoe UI Symbol", 20F))
                    using (StringFormat format = new StringFormat())
                    using (Brush closeBrush = new SolidBrush(currentColor))
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




    public class JsonParser
    {
        private readonly string json;
        private int index;

        public JsonParser(string json)
        {
            this.json = json;
        }

        public static object LoadEmbeddedJson(String name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {

                if (stream == null)
                    throw new FileNotFoundException("Embedded Resource wurde nicht gefunden.");

                return Parse(reader.ReadToEnd());
            }
        }

        public static object Parse(string json)
        {
            return new JsonParser(json).ParseValue();
        }

        private object ParseValue()
        {
            SkipWhitespace();

            if (index >= json.Length)
                throw new Exception("Unexpected end of JSON.");

            switch (json[index])
            {
                case '{':
                    return ParseObject();

                case '[':
                    return ParseArray();

                case '"':
                    return ParseString();

                case 't':
                    Expect("true");
                    return true;

                case 'f':
                    Expect("false");
                    return false;

                case 'n':
                    Expect("null");
                    return null;

                default:
                    return ParseNumber();
            }
        }

        private Dictionary<string, object> ParseObject()
        {
            var obj = new Dictionary<string, object>();

            index++;

            SkipWhitespace();

            if (json[index] == '}')
            {
                index++;
                return obj;
            }

            while (true)
            {
                SkipWhitespace();

                string key = ParseString();

                SkipWhitespace();

                if (json[index] != ':')
                    throw new Exception("Expected ':'");

                index++;

                object value = ParseValue();

                obj[key] = value;

                SkipWhitespace();

                if (json[index] == '}')
                {
                    index++;
                    break;
                }

                if (json[index] != ',')
                    throw new Exception("Expected ','");

                index++;
            }

            return obj;
        }

        private List<object> ParseArray()
        {
            var list = new List<object>();

            index++;

            SkipWhitespace();

            if (json[index] == ']')
            {
                index++;
                return list;
            }

            while (true)
            {
                list.Add(ParseValue());

                SkipWhitespace();

                if (json[index] == ']')
                {
                    index++;
                    break;
                }

                if (json[index] != ',')
                    throw new Exception("Expected ','");

                index++;
            }

            return list;
        }

        private string ParseString()
        {
            if (json[index] != '"')
                throw new Exception("Expected string.");

            index++;

            var sb = new StringBuilder();

            while (index < json.Length)
            {
                char c = json[index++];

                if (c == '"')
                    break;

                if (c == '\\')
                {
                    c = json[index++];

                    switch (c)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;

                        case 'u':
                            sb.Append((char)Convert.ToInt32(json.Substring(index, 4), 16));
                            index += 4;
                            break;

                        default:
                            throw new Exception("Invalid escape sequence.");
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private object ParseNumber()
        {
            int start = index;

            while (index < json.Length)
            {
                char c = json[index];

                if ("0123456789+-.eE".IndexOf(c) == -1)
                    break;

                index++;
            }

            string number = json.Substring(start, index - start);

            if (number.Contains(".") || number.Contains("e") || number.Contains("E"))
                return double.Parse(number, CultureInfo.InvariantCulture);

            long l;

            if (!long.TryParse(number, out l))
                throw new Exception("Invalid number.");

            if (l >= int.MinValue && l <= int.MaxValue)
                return (int)l;

            return l;
        }

        private void SkipWhitespace()
        {
            while (index < json.Length && char.IsWhiteSpace(json[index]))
                index++;
        }

        private void Expect(string value)
        {
            if (json.Substring(index, value.Length) != value)
                throw new Exception("Expected " + value);

            index += value.Length;
        }
    }
}
