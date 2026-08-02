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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GeistStudio.Util;

namespace GeistStudio
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            InitializeSettingsPage();
            StyleScrollbars(this);
        }

        public void Open()
        {
            this.Show();
        }
    }

    public class SmoothScrollPanel : Panel
    {
        private Panel content;
        private Panel scrollbar;
        private Panel thumb;

        private int scrollOffset = 0;

        private bool isDraggingThumb = false;
        private int dragStartMouseY;
        private int dragStartScrollOffset;

        public SmoothScrollPanel()
        {
            DoubleBuffered = true;
            BackColor = Util.Config.Colors.Background.Background;

            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;

            content = new Panel();
            content.Location = new Point(0, 0);
            content.BackColor = Util.Config.Colors.Background.Background;

            scrollbar = new Panel();
            scrollbar.Width = 10;
            scrollbar.Dock = DockStyle.Right;
            scrollbar.BackColor = Util.Config.Colors.Background.Scrollbar;

            thumb = new Panel();
            thumb.Width = 10;
            thumb.Height = 40;
            thumb.Left = 0;
            thumb.Top = 0;
            thumb.BackColor = Util.Config.Colors.Background.Thumb;

            Controls.Add(content);
            Controls.Add(scrollbar);
            scrollbar.Controls.Add(thumb);

            scrollbar.BringToFront();
            thumb.BringToFront();

            MouseEnter += (s, e) => Focus();
            content.MouseEnter += (s, e) => Focus();

            MouseWheel += OnMouseWheel;
            content.MouseWheel += OnMouseWheel;

            thumb.MouseDown += Thumb_MouseDown;
            thumb.MouseMove += Thumb_MouseMove;
            thumb.MouseUp += Thumb_MouseUp;

            scrollbar.MouseDown += Scrollbar_MouseDown;

            Resize += (s, e) => UpdateContentSize();
            HandleCreated += (s, e) => UpdateContentSize();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x20A;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
                HandleWheel(delta);
                return;
            }

            base.WndProc(ref m);
        }

        public void AddContent(Control control)
        {
            content.Controls.Add(control);
            HookMouseWheel(control);

            UpdateContentSize();
        }

        private void HookMouseWheel(Control control)
        {
            control.MouseWheel += OnMouseWheel;
            control.MouseEnter += (s, e) => Focus();

            foreach (Control child in control.Controls)
                HookMouseWheel(child);

            control.ControlAdded += (s, e) => HookMouseWheel(e.Control);
        }

        private void UpdateContentSize()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            content.Width = ClientSize.Width - scrollbar.Width;

            int max = 0;

            foreach (Control c in content.Controls)
            {
                if (c.Bottom > max)
                    max = c.Bottom;
            }

            content.Height = max + 40;

            ClampScrollOffset();
            ApplyScrollOffset();
            UpdateScrollbar();
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            HandleWheel(e.Delta);
        }

        private void HandleWheel(int delta)
        {
            int maxScroll = content.Height - ClientSize.Height;

            if (maxScroll <= 0)
                return;

            scrollOffset -= delta / 3;

            ClampScrollOffset();
            ApplyScrollOffset();
            UpdateScrollbar();
        }

        private void Thumb_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggingThumb = true;
            dragStartMouseY = Cursor.Position.Y;
            dragStartScrollOffset = scrollOffset;
        }

        private void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggingThumb)
                return;

            int maxScroll = content.Height - ClientSize.Height;
            if (maxScroll <= 0)
                return;

            int thumbTravel = ClientSize.Height - thumb.Height;
            if (thumbTravel <= 0)
                return;

            int deltaY = Cursor.Position.Y - dragStartMouseY;
            int deltaScroll = deltaY * maxScroll / thumbTravel;

            scrollOffset = dragStartScrollOffset + deltaScroll;

            ClampScrollOffset();
            ApplyScrollOffset();
            UpdateScrollbar();
        }

        private void Thumb_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggingThumb = false;
        }

        private void Scrollbar_MouseDown(object sender, MouseEventArgs e)
        {
            int maxScroll = content.Height - ClientSize.Height;
            if (maxScroll <= 0)
                return;

            int page = Math.Max(ClientSize.Height - 40, 40);

            if (e.Y < thumb.Top)
                scrollOffset -= page;
            else if (e.Y > thumb.Bottom)
                scrollOffset += page;

            ClampScrollOffset();
            ApplyScrollOffset();
            UpdateScrollbar();
        }

        private void ClampScrollOffset()
        {
            int maxScroll = content.Height - ClientSize.Height;

            if (maxScroll < 0)
                maxScroll = 0;

            if (scrollOffset < 0)
                scrollOffset = 0;

            if (scrollOffset > maxScroll)
                scrollOffset = maxScroll;
        }

        private void ApplyScrollOffset()
        {
            content.Top = -scrollOffset;
        }

        private void UpdateScrollbar()
        {
            int maxScroll = content.Height - ClientSize.Height;

            if (maxScroll <= 0)
            {
                thumb.Visible = false;
                return;
            }

            thumb.Visible = true;

            int thumbHeight = ClientSize.Height * ClientSize.Height / content.Height;

            if (thumbHeight < 40)
                thumbHeight = 40;

            if (thumbHeight > ClientSize.Height)
                thumbHeight = ClientSize.Height;

            thumb.Height = thumbHeight;

            thumb.Top = scrollOffset * (ClientSize.Height - thumbHeight) / maxScroll;
        }

        public void RefreshLayout()
        {
            UpdateContentSize();
        }
    }
}
