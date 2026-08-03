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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GeistStudio.Util;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

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
        private static readonly Color HoverBg = Util.Config.Colors.Background.TabControl.BackgroundHover;
        private static readonly Color SelectedBg = Util.Config.Colors.Background.TabControl.BackgroundSelected;
        private static readonly Color BorderColor = Util.Config.Colors.Background.TabControl.BackgroundBorder;

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
                    ? Util.Config.Colors.Background.TabControl.ItemPressed
                    : Util.Config.Colors.Background.TabControl.ItemNotPressed;

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
            e.TextColor = Util.Config.Colors.Foreground.MainMenu;
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
        public Color BackgroundColorDark { get; set; } = Util.Config.Colors.Background.TabControl.Background;
        private Color closeNormalColor = Util.Config.Colors.Background.TabControl.Close;
        private Color closeHoverColor = Util.Config.Colors.Background.TabControl.CloseHover;
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
                        ? Util.Config.Colors.Background.TabControl.Selected
                        : Util.Config.Colors.Background.TabControl.NotSelected))
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
                    ? Util.Config.Colors.Foreground.Text
                    : Util.Config.Colors.Foreground.TextNotSelected))
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
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETSCROLLPOS = WM_USER + 221;
        private const int EM_SETSCROLLPOS = WM_USER + 222;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref POINT lParam);

        private bool _isHighlighting = false;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
                VScroll?.Invoke(this, EventArgs.Empty);
            else if (m.Msg == WM_HSCROLL)
                HScroll?.Invoke(this, EventArgs.Empty);
        }

        private POINT GetScrollPos()
        {
            POINT p = new POINT();
            SendMessage(Handle, EM_GETSCROLLPOS, 0, ref p);
            return p;
        }

        private void SetScrollPos(POINT p)
        {
            SendMessage(Handle, EM_SETSCROLLPOS, 0, ref p);
        }

        public void HighlightSyntax(Dictionary<string, Color> patterns, bool useRegex = false,
            bool caseSensitive = false, bool resetColors = true, bool useGroupOne = false,
            int rangeStart = -1, int rangeLength = -1)
        {
            if (!IsHandleCreated || _isHighlighting)
                return;

            _isHighlighting = true;

            int selectionStart = SelectionStart;
            int selectionLength = SelectionLength;
            Color defaultColor = ForeColor;
            POINT scrollPos = GetScrollPos();

            SuspendDrawing();

            try
            {
                string fullText = Text;
                if (string.IsNullOrEmpty(fullText))
                    return;

                int start = rangeStart >= 0 ? Math.Max(0, Math.Min(rangeStart, fullText.Length)) : 0;
                int length = rangeLength >= 0
                    ? Math.Min(rangeLength, fullText.Length - start)
                    : fullText.Length - start;

                if (length <= 0)
                    return;

                string searchText = fullText.Substring(start, length);

                if (resetColors)
                {
                    Select(start, length);
                    SelectionColor = defaultColor;
                }

                var regexOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

                foreach (var kvp in patterns)
                {
                    string pattern = useRegex ? kvp.Key : $@"\b{Regex.Escape(kvp.Key)}\b";
                    Color color = kvp.Value;

                    foreach (Match match in Regex.Matches(searchText, pattern, regexOptions))
                    {
                        if (useGroupOne && match.Groups.Count > 1 && match.Groups[1].Success)
                            Select(start + match.Groups[1].Index, match.Groups[1].Length);
                        else
                            Select(start + match.Index, match.Length);

                        SelectionColor = color;
                    }
                }
            }
            finally
            {
                SelectionStart = selectionStart;
                SelectionLength = selectionLength;
                SelectionColor = defaultColor;

                ResumeDrawing();

                SetScrollPos(scrollPos);

                _isHighlighting = false;
            }
        }

        private void SuspendDrawing() => SendMessage(Handle, WM_SETREDRAW, false, IntPtr.Zero);

        private void ResumeDrawing()
        {
            SendMessage(Handle, WM_SETREDRAW, true, IntPtr.Zero);
            Invalidate();
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
