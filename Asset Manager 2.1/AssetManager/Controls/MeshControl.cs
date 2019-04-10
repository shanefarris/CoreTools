using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using AssetManager.Common.Enums;
using AssetManager.Common;
using AssetManager.Dal;

namespace AssetManager
{
    public partial class MeshControl : UserControl
    {
        private int _assetId = -1;
        private int _assetSkeletonId = -1;
        private string _baseDir = Application.StartupPath + "/sandbox/";
        private string _meshFileName = string.Empty;
        private string _skeletonFileName = string.Empty;
        private string _materialName = string.Empty;
        private string _fileName = string.Empty;
        private bool isChanged = false;
        private float _xm, _ym, _zm;

        public MeshControl()
        {
            InitializeComponent();
        }

        public void Show(bool isVisible, int assetId = -1)
        {
            Visible = isVisible;
            _assetId = assetId;

            if (Visible)
                PopulateControls();
        }

        private void PopulateControls()
        {
            var asset = Asset.GetAsset(_assetId);
            if (asset != null)
            {
                txtFileName.Text = asset.FileName;
                cmbMaterial.DataSource = Asset.GetAssets(AssetTypes.Material).Select(a => a.Name).ToList();
                cmbMaterial.DisplayMember = "Name";

                int matId = Mesh.GetMaterialDependency(asset.AssetId);
                if (matId >= 0)
                {
                    var matName = Asset.GetAsset(matId);
                    if (matName != null)
                    {
                        cmbMaterial.SelectedItem = matName.Name;
                    }
                }
                else
                {
                    cmbMaterial.SelectedItem = string.Empty;
                }

                // Set the mesh type
                rdoGameObject.Checked = true;
                if (asset.MeshTypeId != null)
                {
                    if (asset.MeshTypeId == (int)MeshTypes.Character)
                    {
                        rdoCharacter.Checked = true;
                    }
                    else if (asset.MeshTypeId == (int)MeshTypes.Building)
                    {
                        rdoBuilding.Checked = true;
                    }
                }

                // Reset some of the other controls
                _materialName = cmbMaterial.SelectedItem.ToString();
                _fileName = txtFileName.Text;
                txtReport.Text = string.Empty;
                txtAngle.Text = string.Empty;
                chkRotateX.Checked = false;
                chkRotateY.Checked = false;
                chkRotateZ.Checked = false;
                txtScaleX.Text = string.Empty;
                txtScaleY.Text = string.Empty;
                txtScaleZ.Text = string.Empty;

                // Get the size of the mesh in meters
                // Download the file to a temp dir first
                if (!DownloadFile())
                {
                    MessageBox.Show(@"Error downloading file.");
                    return;
                }

                var reportLog = new List<string>();
                MeshAsset.GetInfo(_meshFileName, out _xm, out _ym, out _zm, ref reportLog);
                txtXm.Text = _xm.ToString();
                txtYm.Text = _ym.ToString();
                txtZm.Text = _zm.ToString();
            }
            else
            {
                txtReport.Text = "Error loading mesh data.";
            }
        }

        public bool Save()
        {
            // Check scale changes
            bool isScaled = false;
            if (txtScaleX.Text.Trim().Length > 0 ||
                txtScaleY.Text.Trim().Length > 0 ||
                txtScaleZ.Text.Trim().Length > 0)
            {
                isScaled = true;
                isChanged = true;
                if (!SaveScale())
                {
                    MessageBox.Show(@"Invalid scale value.");
                    return false;
                }
            }

            // Check rotation changes
            if (txtAngle.Text.Trim().Length > 0)
            {
                isChanged = true;
                if (!SaveRotation())
                {
                    MessageBox.Show(@"Invalid rotate value.");
                    return false;
                }
            }

            // Check if the material has changed
            if (cmbMaterial.SelectedItem != null && cmbMaterial.SelectedItem.ToString() != _materialName)
            {
                isChanged = true;
                if (!SaveMaterial())
                    return false;
            }

            // Check file name
            if (_fileName != txtFileName.Text)
                isChanged = true;

            // Check for scaling based on meters
            if (!isScaled)
            {
                if (txtXm.Text != _xm.ToString() ||
                    txtYm.Text != _ym.ToString() ||
                    txtZm.Text != _zm.ToString())
                {
                    if (!SaveScaleMeters())
                    {
                        MessageBox.Show(@"Invalid scale(m) value.");
                        return false;
                    }
                    isChanged = true;
                }
            }

            bool result = true;
            if (isChanged)
            {
                // Update mesh
                var asset = Asset.GetAsset(_assetId);
                if (asset != null)
                {
                    asset.FileName = txtFileName.Text;
                    var data = File.ReadAllBytes(_meshFileName);
                    if (rdoBuilding.Checked)
                        asset.MeshTypeId = (int)MeshTypes.Building;
                    else if (rdoCharacter.Checked)
                        asset.MeshTypeId = (int)MeshTypes.Character;
                    else
                        asset.MeshTypeId = (int)MeshTypes.GameObject;

                    result = Mesh.Update(asset.AssetId, asset.CategoryId, data, asset.FileName, (MeshTypes)asset.MeshTypeId, asset.Name);
                    if (result)
                    {
                        // Save skeleton
                        if (_assetSkeletonId > 0)
                        {
                            var skeletonAsset = Asset.GetAsset(_assetSkeletonId);
                            if (skeletonAsset != null)
                            {
                                var skeletonData = File.ReadAllBytes(_skeletonFileName);
                                result = Skeleton.Update(skeletonAsset.AssetId, skeletonAsset.CategoryId, skeletonData, skeletonAsset.FileName, null, skeletonAsset.Name);
                            }
                        }
                        _assetSkeletonId = -1;
                    }
                }

                // Delete the file if it exists
                if (File.Exists(_meshFileName))
                    File.Delete(_meshFileName);
                if (File.Exists(_skeletonFileName))
                    File.Delete(_skeletonFileName);
            }

            return result;
        }

        private bool DownloadFile(bool downloadDependencies = true)
        {
            // Delete the file if it exists
            if (File.Exists(_meshFileName))
                File.Delete(_meshFileName);
            if (File.Exists(_skeletonFileName))
                File.Delete(_skeletonFileName);
            if (File.Exists(_baseDir + "zzzMaterial.material"))
                File.Delete(_baseDir + "zzzMaterial.material");

            if (_assetId == -1)
                return false;

            // Download the data
            var assetIds = new List<DbId>();

            // Download dependencies if required
            if (downloadDependencies)
            {
                assetIds = Asset.GetDependencyIds(_assetId);
            }

            assetIds.Add(_assetId);
            foreach (var id in assetIds)
            {
                var asset = Asset.GetAsset(id);
                if (asset == null)
                {
                    MessageBox.Show("Error downloading asset with Id: " + id);
                    return false;
                }
                else
                {
                    // Write the data to the HDD
                    var data = Asset.GetAssetData(asset.AssetId);
                    if (asset.AssetTypeId == (int)AssetTypes.Mesh)
                    {
                        _meshFileName = _baseDir + asset.FileName;
                        File.WriteAllBytes(_baseDir + asset.FileName, data);
                    }
                    else if (asset.AssetTypeId == (int)AssetTypes.Skeleton)
                    {
                        _skeletonFileName = _baseDir + asset.FileName;
                        _assetSkeletonId = asset.AssetId;
                        File.WriteAllBytes(_baseDir + asset.FileName, data);
                    }
                    else if (asset.AssetTypeId == (int)AssetTypes.Material)
                    {
                        // zzzMaterial needs to be made so all materials can go into this one file.  Otherwise if multiple
                        // materials come from the same material filename in the DB, it will override each other.
                        File.AppendAllText(_baseDir + "zzzMaterial.material", Common.Utility.ConvertBytesToString(data));
                    }
                    else
                    {
                        File.WriteAllBytes(_baseDir + asset.FileName, data);
                    }
                }
            }

            return File.Exists(_meshFileName);
        }

        private bool SaveScale()
        {
            double x, y, z;
            if (txtScaleX.Text.Trim().Length == 0)
            {
                x = 1.0;
            }
            else if (!double.TryParse(txtScaleX.Text, out x))
            {
                return false;
            }

            if (txtScaleY.Text.Trim().Length == 0)
            {
                y = 1.0;
            }
            else if (!double.TryParse(txtScaleY.Text, out y))
            {
                return false;
            }

            if (txtScaleZ.Text.Trim().Length == 0)
            {
                z = 1.0;
            }
            else if (!double.TryParse(txtScaleZ.Text, out z))
            {
                return false;
            }

            var reportLog = new List<string>();
            var result = MeshAsset.SaveScale(x, y, z, _meshFileName, ref reportLog);
            reportLog.ForEach(r => txtReport.Text += r);

            return result;
        }

        private bool SaveScaleMeters()
        {
            double x, y, z;
            if (txtXm.Text.Trim().Length == 0 || !double.TryParse(txtXm.Text, out x))
            {
                return false;
            }

            if (txtYm.Text.Trim().Length == 0 || !double.TryParse(txtYm.Text, out y))
            {
                return false;
            }

            if (txtZm.Text.Trim().Length == 0 || !double.TryParse(txtZm.Text, out z))
            {
                return false;
            }

            var standardScale = Properties.Settings.Default.StandardSize;
            x *= standardScale;
            y *= standardScale;
            z *= standardScale;

            var reportLog = new List<string>();
            var result = MeshAsset.SaveSize(x, y, z, _meshFileName, ref reportLog);
            reportLog.ForEach(r => txtReport.Text += r);

            return result;
        }

        private bool SaveRotation()
        {
            double angle = 0.0;
            if (!double.TryParse(txtAngle.Text, out angle))
            {
                return false;
            }

            var reportLog = new List<string>();
            var result = MeshAsset.SaveRotation(angle, chkRotateX.Checked, chkRotateY.Checked, chkRotateZ.Checked, _meshFileName, ref reportLog);
            reportLog.ForEach(r => txtReport.Text += r);

            return result;
        }

        private bool SaveMaterial()
        {
            var reportLog = new List<string>();
            var result = MeshAsset.SaveMaterial(_materialName, (string)cmbMaterial.SelectedItem, _meshFileName, ref reportLog);
            reportLog.ForEach(r => txtReport.Text += r);
            return result;
        }

        private void ResizeBasedOnY()
        {
            if (txtYm.Text != _ym.ToString())
            {
                float newY;
                if (float.TryParse(txtYm.Text, out newY))
                {
                    var scale = decimal.Round((decimal)(newY / _ym), 5);
                    _xm *= (float)scale;
                    _zm *= (float)scale;

                    txtXm.Text = _xm.ToString();
                    txtZm.Text = _zm.ToString();
                }
            }
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            // Convert back to a mesh file
            string returnValue = string.Empty;
            if (!MeshAsset.RunUpgrader(_meshFileName, ref returnValue))
            {
                MessageBox.Show(@"Error upgrading mesh, so sad: " + returnValue);
                return;
            }

            txtReport.Text += "Upgraded";

            // Update the mesh.
            var asset = Asset.GetAsset(_assetId);
            if (asset != null)
            {
                asset.FileName = txtFileName.Text;
                var data = File.ReadAllBytes(_meshFileName);
                Asset.Update(asset.AssetId, asset.CategoryId, data, asset.FileName, (MeshTypes)asset.MeshTypeId, asset.Name);
            }

            // Delete the file if it exists
            if (File.Exists(_meshFileName))
                File.Delete(_meshFileName);
        }

        private void btnOptimize_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This isn't done yet stupid.");
        }

        private void txtScaleX_Leave(object sender, EventArgs e)
        {
            txtScaleY.Text = txtScaleX.Text;
            txtScaleZ.Text = txtScaleX.Text;
        }

        private void rdoGameObject_CheckedChanged(object sender, EventArgs e)
        {
            isChanged = true;
        }

        private void rdoCharacter_CheckedChanged(object sender, EventArgs e)
        {
            isChanged = true;
        }

        private void rdoBuilding_CheckedChanged(object sender, EventArgs e)
        {
            isChanged = true;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            // Execute the command
            var fi = new FileInfo(@"C:\Program Files (x86)\OgreMeshy\OgreMeshy.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    Arguments = "\"" + _meshFileName + "\"",
                };

                Process.Start(info);
            }
        }

        private void txtYm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ResizeBasedOnY();
            }
        }
    }
}
