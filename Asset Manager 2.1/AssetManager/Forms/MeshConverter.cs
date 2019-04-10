using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager
{
    public partial class MeshConverter : Form
    {
        public MeshConverter()
        {
            InitializeComponent();
        }

        private void MeshConverter_Load(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            cmbBatchFileType.SelectedItem = "LightWave ( .lwo )";
        }

        private void btnSingleBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { InitialDirectory = @".\" };
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtSingleFile.Text = ofd.FileName;
            }
        }

        private void btnSingleConvert_Click(object sender, EventArgs e)
        {
            if (txtSingleFile.Text.Length == 0)
            {
                MessageBox.Show("You need to pick a file first dummy.", "Why are you so stupid?");
                return;
            }

            // Execute the command 
            var fi = new FileInfo(Application.StartupPath + @"\Tools\OgreAssimp.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = "\"" + txtSingleFile.Text + "\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                txtReport.Text = string.Empty;
                using (Process process = Process.Start(info))
                {
                    StreamReader sr = process.StandardOutput;
                    string returnvalue = sr.ReadToEnd();
                    txtReport.Text += returnvalue + "\r\n\r\n";
                }
            }
            else
            {
                MessageBox.Show("Could not find Assimp.exe.");
            }
        }

        private void btnBatchDir_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtBatchDir.Text = fd.SelectedPath;
            }
        }

        private void btnBatchConvert_Click(object sender, EventArgs e)
        {
            if (txtBatchDir.Text.Length == 0)
            {
                MessageBox.Show("You need to pick a file first dummy.", "Why are you so stupid?");
                return;
            }

            // Get the extension selected
            var fileType = cmbBatchFileType.SelectedItem.ToString();
            fileType = fileType.Remove(0, fileType.IndexOf("(") + 1);
            fileType = fileType.Replace(")", string.Empty).Trim();

            txtReport.Text = "Converting files with extension: " + fileType + "\r\n";

            // Iterate through each file with that extension
            var fi = new FileInfo(Application.StartupPath + @"\Tools\OgreAssimp.exe");
            if (fi.Exists)
            {
                var di = new DirectoryInfo(txtBatchDir.Text);
                foreach (var file in di.GetFiles())
                {
                    if (file.Extension == fileType)
                    {
                        // Execute the command 
                        var info = new ProcessStartInfo(fi.FullName)
                        {
                            UseShellExecute = false,
                            Arguments = "\"" + file.FullName + "\"",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };

                        using (Process process = Process.Start(info))
                        {
                            StreamReader sr = process.StandardOutput;
                            string returnvalue = sr.ReadToEnd();
                            txtReport.Text += returnvalue + "\r\n";
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Could not find Assimp.exe.");
            }
        }
    }
}
