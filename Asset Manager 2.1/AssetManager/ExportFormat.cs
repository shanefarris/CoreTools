using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager
{
    public partial class ExportFormat : Form
    {
        public ExportFormat()
        {
            InitializeComponent();
        }

        private void ExportFormat_Load(object sender, EventArgs e)
        {
            var format = Common.Utility.GetExportImage();
            if (format == "none")
                rdoNone.Checked = true;
            else if (format == "png")
                rdoPNG.Checked = true;
            else if (format == "jpg")
                rdoJPG.Checked = true;
            else if (format == "bmp")
                rdoBMP.Checked = true;
            else if (format == "dds")
                rdoDDS.Checked = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var format = Common.Utility.GetExportImage();
            if (rdoNone.Checked)
                Common.Utility.SetExportImage("none");
            else if (rdoPNG.Checked)
                Common.Utility.SetExportImage("png");
            else if (rdoJPG.Checked)
                Common.Utility.SetExportImage("jpg");
            else if (rdoBMP.Checked)
                Common.Utility.SetExportImage("bmp");
            else if (rdoDDS.Checked)
                Common.Utility.SetExportImage("dds");

            Properties.Settings.Default.Save();
            Close();
        }
    }
}
