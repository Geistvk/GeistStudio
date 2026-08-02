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

namespace GeistStudio
{
    public partial class About : Form
    {
        private String appName;
        private String licenseData;
        private String author;
        private String authorEmail;
        private String authorGithub;
        private String sourceCode;
        private String curVersion;

        public About()
        {
            InitializeComponent();
        }

        private void loadAboutData()
        {
            Dictionary<string, object> root = (Dictionary<string, object>)JsonParser.LoadEmbeddedJson("GeistStudio.GeistStudioData.json");
            Dictionary<string, object> config = (Dictionary<string, object>)root["Config"];
            Dictionary<string, object> about = (Dictionary<string, object>)config["About"];
            List<object> license = (List<object>)about["License"];

            for (int i = 0; i < license.Count; i++)
                this.licenseData += (String)license[i] + "\n";

            this.appName =      (string)about["Name"];
            this.author =       (string)about["Author"];
            this.authorEmail =  (string)about["Email"];
            this.authorGithub = (string)about["Github"];
            this.sourceCode =   (string)about["SourceCode"];
            this.curVersion =   (string)about["CurVersion"];
        }
    }
}
