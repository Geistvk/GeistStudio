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
    public partial class Form1 : Form
    {
        private int tabSize = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void addTab(object sender, EventArgs e) {
            System.Windows.Forms.TabPage tabPage;
            tabPage = new System.Windows.Forms.TabPage();

            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = "tabPageTmp";
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(1046, 495);
            tabPage.TabIndex = this.tabSize++;
            tabPage.Text = "tabPageTmp";
            tabPage.UseVisualStyleBackColor = true;

            tabControl1.Controls.Add(tabPage);
        }
    }
}
