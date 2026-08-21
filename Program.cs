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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeistStudio
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormUtils.SetDefaultIcon();

            LoadingScreen loading = new LoadingScreen();
            GeistStudioWin mainForm = null;
            Thread loadingThread = new Thread(() =>
            {
                Application.Run(loading);
            });

            loadingThread.SetApartmentState(ApartmentState.STA);
            loadingThread.Start();

            try
            {
                mainForm = new GeistStudioWin();

                while (!loading.IsHandleCreated)
                {
                    Thread.Sleep(1);
                }

                loading.BeginInvoke(new Action(() =>
                {
                    loading.Close();
                }));

                loadingThread.Join();

                Application.Run(mainForm);
            }
            catch
            {
                if (loading != null &&
                    !loading.IsDisposed &&
                    loading.IsHandleCreated)
                {
                    loading.BeginInvoke(new Action(() =>
                    {
                        loading.Close();
                    }));
                }

                throw;
            }
        }
    }
}
