using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeistStudio
{
    public class Util : GeistStudioWin {
        public Util() { 
        
        }


        private static void handleFileFromIndex(GeistStudioWin form, int i, bool closeFile) 
        {
            TabPage page = form.FileList.TabPages[i];
            DialogResult result = form.ShowSaveDialog(closeFile);

            if (result == DialogResult.Yes)
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

                if (i != 0)
                    handleFileFromIndex(form, i, closeFile);

                if (page == form.FileList.SelectedTab && !effectAll && i != 0)
                    break;
            }
        }
    }
}
