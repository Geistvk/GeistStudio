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
            for (int i = 0; i < form.FileList.TabPages.Count; i++)
            {
                Console.WriteLine("Index: " + i);
                if (!effectAll && i > 0)
                {
                    if (form.FileList.TabPages[i] == form.FileList.SelectedTab)
                    {
                        if (type == "save")
                            handleFileFromIndex(form, i, false);
                        else if (type == "close")
                            handleFileFromIndex(form, i, true);
                        break;
                    }
                }
                else if (effectAll && i > 0)
                {
                    if (type == "save")
                        handleFileFromIndex(form, i, false);
                    else if (type == "close")
                        handleFileFromIndex(form, i, true);
                }
            }
        }
    }
}
