using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
//using System.Data.SQLite;

using AssetManager.Code_Generators;
using System.Threading;
using AssetManager.Common.Enums;
using AssetManager.Common;
using AssetManager.Dal;

namespace AssetManager
{
    public partial class Main : Form
    {
        #region Variables

        private readonly AssetTreeView _assetTreeView;
        private readonly CodeGenerator _codeGenerator = new CodeGenerator();
        private readonly ItemProfileHelper _itemProfileHelper = new ItemProfileHelper();
        private readonly ResourceHelper _resourceHelper = new ResourceHelper();
        private readonly MeshControl _meshControl = new MeshControl() { Visible = false };

        #endregion

        #region Private Methods

        #region Main

        public Main()
        {
            InitializeComponent();

            lblStatus.Text = string.Empty;
            gbAssetInfo.Controls.Add(_meshControl);
            _meshControl.Location = new Point(9, 16);
            _assetTreeView = new AssetTreeView();

            PoplateControls();

            // Setup tool tip
            toolTip.SetToolTip(btnArchiveRun, "Copies all assets from the DB and compresses the files.");
            toolTip.SetToolTip(rdoLineIncrements,
                               "When copying, it will take all material files, and merge them in a big " +
                               "material file that consists of no more then 2000 lines.");
            toolTip.SetToolTip(rdoLeaveMaterials,
                               "Does not modify the material files, and just copies all of them over.");
            toolTip.SetToolTip(rdoMergeMaterials,
                               "Merges all material files while copying into one big material file, used for " +
                               "validating all materials");
            toolTip.SetToolTip(btnGCRun,
                               "Takes the root directory and finds all mesh files and generates game object " +
                               "code and outputs that to the output directory.");
            toolTip.SetToolTip(btnGCUnitTest,
                               "Generates unit tests for all mesh files in the root directory and creates a " +
                               "singel \"GameObject.cpp\" file.");

            // Set the tab width of the asset overview textbox
            SetTabs(txtAssetOverview);

            // Main Asset treeview
            //_assetTreeView.PopulateTreeAll(tvMain);
            var thread = new Thread(() => _assetTreeView.PopulateParentMeshAssets(tvMain));
            thread.Start();
            //_assetTreeView.PopulateParentAssets(tvMain);
        }

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref int lParam);
        }

        static void SetTabs(TextBox box)
        {
            int lParam = 16;  //Set tab size to 4 spaces
            NativeMethods.SendMessage(box.Handle, 0x00CB, new IntPtr(1), ref lParam);
            box.Invalidate();
        }

        private void PopulateMainLabels()
        {
            // Main labels
            lblNumMesh.Text = "Meshes: " + Asset.NumberOfAssetType(AssetTypes.Mesh).ToString();
            lblNumMaterial.Text = "Materials: " + Asset.NumberOfAssetType(AssetTypes.Material).ToString();
            lblTotal.Text = "Total Assets: " + Asset.NumberOfTotalAssets().ToString();
        }

        private void PoplateControls()
        {
            // Main labels
            PopulateMainLabels();

            // Category controls
            txtCategoryName.Text = string.Empty;
            cboCategoryParent.DataSource = Category.GetAllCategories().Select(c => c.Name).ToList();
            cboCategoryParent.DisplayMember = "Name";

            // Asset type grid
            dgvAssetType.DataSource = AssetType.GetAllAssetTypes();
            var dataGridViewColumn = dgvAssetType.Columns["AssetTypeId"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.Visible = false;

            var gridViewColumn = dgvAssetType.Columns["Assets"];
            if (gridViewColumn != null)
                gridViewColumn.Visible = false;
            dgvAssetType.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Profiles
            var profileList = Profile.GetAllProfiles().Select(p => p.Name).ToList();
            profileList.Insert(0, "");
            cboProfileExisting.DataSource = profileList;
            cboProfileExisting.DisplayMember = "Name";

            // Asset controls
            cboAssetType.DataSource = AssetType.GetAllAssetTypes().Select(t => t.Description).ToList();
            cboAssetType.DisplayMember = "Description";

            cboAssetCategory.DataSource = Category.GetAllCategories().Select(c => c.Name).ToList();
            cboAssetCategory.DisplayMember = "Name";

            // Archive controls
            cboArchiveAssetProfiles.DataSource = Profile.GetAllProfiles().Select(p => p.Name).ToList();
            cboArchiveAssetProfiles.DisplayMember = "Name";

            txtArchiveOutDir.Text = Application.StartupPath + @"\zzzOut";

            // Generated Code
            cboGCAssetProfile.DataSource = Profile.GetAllProfiles().Select(p => p.Name).ToList();
            cboGCAssetProfile.DisplayMember = "Name";
        }

        private Size GenerateImageDimensions(int currW, int currH, int destW, int destH)
        {
            // Double to hold the final multiplier to use when scaling the image
            double multiplier = 0;

            //determine if it's Portrait or Landscape
            var layout = currH > currW ? "portrait" : "landscape";

            switch (layout.ToLower())
            {
                case "portrait":
                    //calculate multiplier on heights
                    if (destH > destW)
                        multiplier = (double)destW / (double)currW;
                    else
                        multiplier = (double)destH / (double)currH;
                    break;
                case "landscape":
                    //calculate multiplier on widths
                    if (destH > destW)
                        multiplier = (double)destW / (double)currW;
                    else
                        multiplier = (double)destH / (double)currH;
                    break;
            }

            //return the new image dimensions
            return new Size((int)(currW * multiplier), (int)(currH * multiplier));
        }

        private void SetImage(PictureBox pb)
        {
            try
            {
                //create a temp image
                var img = pb.Image;

                //calculate the size of the image
                var imgSize = GenerateImageDimensions(img.Width, img.Height, picMain.Width, picMain.Height);

                //create a new Bitmap with the proper dimensions
                var finalImg = new Bitmap(img, imgSize.Width, imgSize.Height);

                //create a new Graphics object from the image
                var gfx = Graphics.FromImage(img);

                //clean up the image (take care of any image loss from resizing)
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //empty the PictureBox
                pb.Image = null;

                //center the new image
                pb.SizeMode = PictureBoxSizeMode.CenterImage;

                //set the new image
                pb.Image = finalImg;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion

        #region Asset

        private void SeparateAssetDirList(IEnumerable<string> totalList, ref List<string> meshList, ref List<string> matList, ref List<string> otherList)
        {
            foreach (string file in totalList)
            {
                string extension = file.Substring(file.LastIndexOf("."));
                if (extension == ".mesh")
                {
                    meshList.Add(file);
                }
                else if (extension == ".material")
                {
                    matList.Add(file);
                }
                else
                {
                    otherList.Add(file);
                }
            }
        }

        #endregion

        #region Profiles

        private void GetTagIds(ref List<int> list, TreeNode node)
        {
            if (node == null)
            {
                foreach (TreeNode n in tvProfileAssets.Nodes)
                {
                    if (n.Nodes.Count > 0)
                        GetTagIds(ref list, n);

                    int id = int.Parse(n.Tag.ToString());
                    if (!list.Contains(id))
                        list.Add(id);
                }
            }
            else
            {
                foreach (TreeNode n in node.Nodes)
                {
                    if (n.Nodes.Count > 0)
                        GetTagIds(ref list, n);

                    int id = int.Parse(n.Tag.ToString());
                    if (!list.Contains(id))
                        list.Add(id);
                }
            }
        }

        private void SaveProfile(IEnumerable<int> assetIds)
        {
            if (assetIds == null)
                throw new ArgumentNullException("assetIds");

            // Save to the profile
            if (txtProfileName.Text.Length == 0)
            {
                MessageBox.Show(@"Enter a profile name.");
                return;
            }

            // Check if this is an update, meaning if the profile is selected in the combobox, and the profile name
            // is the same, then we are updating.
            if (cboProfileExisting.SelectedValue != null)
            {
                if (cboProfileExisting.SelectedValue.ToString() == txtProfileName.Text)
                {
                    if (!Profile.Delete(txtProfileName.Text))
                    {
                        MessageBox.Show("Error deleting profile.");
                        return;
                    }
                }
            }

            // Check that name is unique
            if (Profile.GetProfile(txtProfileName.Text) != null)
            {
                MessageBox.Show(@"A profile with is name is already created.");
                return;
            }

            // Create the asset list first
            var guid = Guid.NewGuid();
            foreach (int id in assetIds)
            {
                if (AssetList.Create(id, guid.ToString()) == 0)
                {
                    lblStatus.Text = @"Error saving asset list.";
                    return;
                }
            }

            // Create the profile
            if (Profile.Create(txtProfileName.Text.Trim(), guid.ToString()) == 0)
            {
                lblStatus.Text = @"Error saving profile.";
                return;
            }

            // Reset controls
            tvProfileAssets.Nodes.Clear();
            txtProfileName.Text = string.Empty;
            tabPage3_Enter(null, null);

            var profileNames = Profile.GetAllProfiles().Select(p => p.Name).ToList();
            cboArchiveAssetProfiles.DataSource = profileNames;
            cboGCAssetProfile.DataSource = profileNames;

            profileNames.Insert(0, "");
            cboProfileExisting.DataSource = profileNames;
            cboProfileExisting.DisplayMember = "Name";

            lblStatus.Text = "Saved";
        }

        private IEnumerable<string> GetFilesFromConfigXml(string fileName)
        {
            XmlTextReader reader = null;
            try
            {
                string xml = "<root>" + File.ReadAllText(fileName) + "</root>";
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                doc.Save("./tempXml.xml");
                reader = new XmlTextReader("./tempXml.xml");

                var fileList = new List<string>();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // GameObject
                        if (reader.Name.ToLower() == "gameobject")
                        {
                            if (reader.GetAttribute("meshfile").Length > 0)
                                fileList.Add(reader.GetAttribute("meshfile").ToLower());
                            else
                                throw new Exception("Invalid GameObject");
                        }
                        // Weapon, Magazine, Projectile
                        else if (reader.Name.ToLower() == "weapon" ||
                                 reader.Name.ToLower() == "magazine" ||
                                 reader.Name.ToLower() == "projectile")
                        {
                            if (reader.GetAttribute("mesh").Length > 0)
                                fileList.Add(reader.GetAttribute("mesh").ToLower());
                        }
                        // Water
                        else if (reader.Name.ToLower() == "water")
                        {
                            if (reader.GetAttribute("watermaterial").Length > 0)
                                fileList.Add(reader.GetAttribute("watermaterial").ToLower());
                            else
                                throw new Exception("Invalid Water");
                        }
                        // BgSound
                        else if (reader.Name.ToLower() == "bgsound")
                        {
                            if (reader.GetAttribute("filename").Length > 0)
                                fileList.Add(reader.GetAttribute("filename").ToLower());
                            else
                                throw new Exception("Invalid BgSound");
                        }
                        // billboard
                        else if (reader.Name.ToLower() == "billboard")
                        {
                            if (reader.GetAttribute("material").Length > 0)
                                fileList.Add(reader.GetAttribute("material").ToLower());
                            else
                                throw new Exception("Invalid Billboard");
                        }
                    }
                }
                return fileList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return null;
        }

        private IEnumerable<string> GetFilesFromConfigDb(string fileName)
        {
            //throw new NotImplementedException();
            // This needs to get all the mesh names and the types as well.  The types are used for game objects and players, and the mesh for those
            // types will need to be looked up somehow as well.

            var fileList = new List<string>();
            //var dt = new DataTable();

            //try
            //{

            //    using (var cnn = new SQLiteConnection("Data Source=" + @fileName))
            //    {
            //        cnn.Open();
            //        var cmd = new SQLiteCommand(cnn);

            //        // The all the mesh names from the go and player table
            //        cmd.CommandText = "select * from gameobjects";
            //        SQLiteDataReader reader = cmd.ExecuteReader();
            //        dt.Load(reader);
            //        reader.Close();
            //        cnn.Close();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            return fileList;
        }

        #endregion

        #endregion

        #region Events

        #region Asset Overview/General

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void tvMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tvMain.SelectedNode != null)
            {
                var p = new Point(e.X, e.Y);
                var node = tvMain.GetNodeAt(p);

                if (node != null)
                {
                    var selectedNode = tvMain.SelectedNode;
                    tvMain.SelectedNode = node;

                    if (node.Tag != null)
                        cmsAssetTree.Show(tvMain, p);

                    tvMain.SelectedNode = selectedNode;
                }
            }
        }

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show(@"Delete asset?", "Delete?", MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (tvMain.SelectedNode != null)
                {
                    var id = (int)tvMain.SelectedNode.Tag;
                    var errorLog = new List<string>();
                    if (!Asset.DeleteAsset(id, ref errorLog))
                    {
                        if (errorLog.Count > 0)
                        {
                            var msg = errorLog.Aggregate(string.Empty, (current, s) => current + (s + "\r\n"));
                            MessageBox.Show(msg);
                        }
                    }
                    else
                    {
                        tvMain.Nodes.Remove(tvMain.SelectedNode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMainRefresh_Click(object sender, EventArgs e)
        {
            Thread thread = null;
            if (rbAllAssets.Checked)
                thread = new Thread(() => _assetTreeView.PopulateTreeAll(tvMain));
            else if (rbCategory.Checked)
                thread = new Thread(() => _assetTreeView.PopulateTreeByCategory(tvMain));

            if (thread != null)
                thread.Start();

            // Main labels
            PopulateMainLabels();
        }

        private void btnUpdateText_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;

            // Modifying something in the textbox
            if (txtAssetOverview.Tag != null && txtAssetOverview.Visible)
            {
                var id = (int)txtAssetOverview.Tag;
                var asset = Asset.GetAsset(id);
                if (asset != null)
                {
                    // Special logic for saving materials
                    if (asset.AssetTypeId == (int)AssetTypes.Material)
                    {
                        string errorReport = string.Empty;
                        if (Material.Update(asset.AssetId, txtAssetOverview.Text, ref errorReport))
                        {
                            lblStatus.Text = "Saved";
                        }
                        else
                        {
                            MessageBox.Show(errorReport);
                            return;
                        }
                    }
                    // No special logic for any other text files for now
                    else
                    {
                        var assetType = AssetType.GetAssetType((AssetTypes)asset.AssetTypeId);
                        if (assetType.IsText)
                        {
                            var data = Common.Utility.ConvertStringToBytes(txtAssetOverview.Text);
                            if (Asset.Update(asset.AssetId, asset.CategoryId, data, asset.FileName, (MeshTypes)asset.MeshTypeId, asset.Name))
                            {
                                lblStatus.Text = "Saved";
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Not sure how to save this.");
                        }
                    }
                }
            }
            // Modifying something in the mesh control
            else if (_meshControl.Visible)
            {
                if (_meshControl.Save())
                    lblStatus.Text = "Saved";
                else
                    MessageBox.Show(@"Error saving updates to the mesh file.");
            }
            else
            {
                lblStatus.Text = @"Nothing to save.";
            }
        }

        private void tvMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                var pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                var destinationNode = ((TreeView)sender).GetNodeAt(pt);
                var newNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                if (destinationNode == null)
                {
                    tvMain.Nodes.Add((TreeNode)newNode.Clone());
                    tvMain.ExpandAll();
                    newNode.Remove();
                }
                else if (destinationNode.TreeView != newNode.TreeView)
                {
                    destinationNode.Nodes.Add((TreeNode)newNode.Clone());
                    destinationNode.Expand();
                    //Remove Original Node
                    newNode.Remove();
                }
            }
        }

        private void tvMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (tvMain.SelectedNode != null)
                {
                    if (tvMain.SelectedNode.Tag != null)
                    {
                        var id = (int)tvMain.SelectedNode.Tag;
                        var asset = Asset.GetAsset(id);
                        if (asset != null)
                        {
                            var assetType = AssetType.GetAssetType((AssetTypes)asset.AssetTypeId);
                            if (assetType.IsText)
                            {
                                var data = Asset.GetAssetData(asset.AssetId);
                                _meshControl.Show(false);
                                txtAssetOverview.Visible = true;
                                txtAssetOverview.Text = Common.Utility.ConvertBytesToString(data);
                                txtAssetOverview.Tag = asset.AssetId;
                                return;
                            }
                            else if (asset.AssetTypeId == (int)AssetTypes.Mesh)
                            {
                                txtAssetOverview.Visible = false;
                                _meshControl.Show(true, asset.AssetId);
                            }
                            else
                            {
                                var data = Asset.GetAssetData(asset.AssetId);
                                var ms = new MemoryStream(data);
                                picMain.Image = System.Drawing.Image.FromStream(ms);
                                SetImage(picMain);
                                return;
                            }
                        }
                    }
                }
            }
            catch
            {
                if (picMain != null)
                    picMain.Image = null;
            }
        }

        #endregion

        #region Categories

        private void btnCategoryAdd_Click(object sender, EventArgs e)
        {
            var name = txtCategoryName.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show(@"Name required.");
                return;
            }

            var category = Category.GetCategory(name);
            if (category == null)
            {
                category = new Category { Name = name, ParentCategoryId = null };

                if (cboCategoryParent.SelectedItem != null)
                {
                    string parentName = cboCategoryParent.SelectedItem.ToString();
                    category.ParentCategoryId = Category.GetCategoryId(parentName);
                }

                Category.Create(category.Name, category.ParentCategoryId);
                PoplateControls();
            }
            else
            {
                MessageBox.Show(@"Name already exist.");
                return;
            }
        }

        private void btnSaveAssetType_Click(object sender, EventArgs e)
        {
            // TODO
        }

        #endregion

        #region Assets

        private void btnAssetOpenFile_Click(object sender, EventArgs e)
        {
            // Get the script type
            var filter = string.Empty;
            if (cboAssetType.SelectedItem != null)
            {
                filter = cboAssetType.SelectedValue.ToString();
            }

            var ofd = new OpenFileDialog { InitialDirectory = @".\" };
            if (filter.Length > 0)
            {
                var assetType = AssetType.GetAssetType(filter);
                if (assetType != null)
                {
                    var typeName = assetType.Extension;
                    ofd.Filter = filter + " files (*." + typeName + ")|*." + typeName + "|All files (*.*)|*.*";
                }
            }
            else
            {
                filter = "All files (*.*)|*.*";
                foreach (string type in AssetType.GetAllAssetTypes().Select(t => t.Extension))
                {
                    filter += "| " + type + " files (*." + type + ")|*." + type;
                }
                ofd.Filter = filter;
            }
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileName.Contains("."))
                {
                    filter = ofd.FileName.Substring(ofd.FileName.LastIndexOf(".")).Replace(".", string.Empty);

                    var assetType = AssetType.GetAssetType(filter);
                    if (assetType != null)
                    {
                        cboAssetType.SelectedItem = assetType.Description;
                    }
                }
            }

            txtAssetName.Text = ofd.FileName;
        }

        private void btnAssetAdd_Click(object sender, EventArgs e)
        {
            txtAssetReport.Text = string.Empty;

            // Check name
            if (txtAssetName.Text.Length == 0)
            {
                MessageBox.Show(@"File name required.");
                return;
            }

            // Check category
            if (cboAssetCategory.SelectedItem == null)
            {
                MessageBox.Show(@"Select category.");
                return;
            }

            var catName = cboAssetCategory.SelectedValue.ToString();
            int categoryId = Category.GetCategoryId(catName);

            var fi = new FileInfo(txtAssetName.Text);
            var extension = fi.Extension.Replace(".", string.Empty);

            DbId result = null;
            var errorList = new List<string>();
            if (extension == "mesh")
                result = Mesh.Create(txtAssetName.Text, categoryId, ref errorList);
            else if (extension == "material")
                result = Material.Create(txtAssetName.Text, categoryId, ref errorList);
            else if (extension == "fontdef")
                result = Dal.Font.Create(txtAssetName.Text, categoryId, ref errorList);
            else
                result = Asset.Create(txtAssetName.Text, categoryId, ref errorList);

            if (result > 0)
            {
                lblStatus.Text = "Saved";
                cboAssetType.SelectedItem = null;
            }
            else
            {
                foreach (string s in errorList)
                    txtAssetReport.Text += s + "\r\n";

                lblStatus.Text = "Error";
            }
        }

        private void btnAssetOpenDir_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtAssetDirectory.Text = fd.SelectedPath;
            }
        }

        private void btnAssetAddDir_Click(object sender, EventArgs e)
        {
            // Check asset directory
            if (txtAssetDirectory.Text.Length == 0)
            {
                MessageBox.Show(@"Select directory first.");
                return;
            }

            // Check category
            int categoryId = 0;
            if (cboAssetCategory.SelectedItem == null)
            {
                MessageBox.Show(@"Select category.");
                return;
            }

            var catName = cboAssetCategory.SelectedValue.ToString();
            var invalidList = new List<string>();
            var fileList = new List<string>();

            categoryId = Category.GetCategoryId(catName);
            var extensions = AssetType.GetAllAssetTypes().Select(t => t.Extension).ToList();
            Common.Utility.FileSearch(txtAssetDirectory.Text, ref fileList, extensions);

            // Separate the complete list by file types
            var meshList = new List<string>();
            var matList = new List<string>();
            var otherList = new List<string>();
            SeparateAssetDirList(fileList, ref meshList, ref matList, ref otherList);

            // Save other files first
            foreach (string file in otherList)
            {
                Asset.Create(file, categoryId, ref invalidList);
            }

            // Save material files next
            foreach (string file in matList)
            {
                Material.Create(file, categoryId, ref invalidList);
            }

            // Save mesh files last
            foreach (string file in meshList)
            {
                Mesh.Create(file, categoryId, ref invalidList);
            }

            // Populate report and create error log file
            using (var sw = File.CreateText(".\\Error.log"))
            {
                foreach (string s in invalidList)
                {
                    txtAssetReport.Text += s + "\r\n";
                    if (!s.Contains("(DB)") && !s.Contains("(HDD") && !s.Contains("already exists"))
                        sw.WriteLine(s);
                }
            }
            MessageBox.Show("Done.");
        }

        #endregion

        #region Profiles

        private void tvProfileAsset_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                var pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                var destinationNode = ((TreeView)sender).GetNodeAt(pt);
                var newNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                if (destinationNode == null)
                {
                    tvProfileAssets.Nodes.Add((TreeNode)newNode.Clone());
                    tvProfileAssets.ExpandAll();
                    newNode.Remove();
                }
                else if (destinationNode.TreeView != newNode.TreeView)
                {
                    destinationNode.Nodes.Add((TreeNode)newNode.Clone());
                    destinationNode.Expand();
                    //Remove Original Node
                    newNode.Remove();
                }
            }
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            if (tvExistingAssetProfile.Nodes.Count != tvMain.Nodes.Count)
            {
                tvExistingAssetProfile.Nodes.Clear();
                foreach (TreeNode node in tvMain.Nodes)
                {
                    tvExistingAssetProfile.Nodes.Add((TreeNode)node.Clone());
                }
            }
        }

        private void btnAddProfile_Click(object sender, EventArgs e)
        {
            if (tvProfileAssets.Nodes.Count == 0)
            {
                MessageBox.Show(@"There are no assets selected dummy.");
                return;
            }

            // Create the asset list first
            var assetIds = new List<int>();
            GetTagIds(ref assetIds, null);

            // Save the profile
            SaveProfile(assetIds);
        }

        private void cboProfileExisting_SelectedIndexChanged(object sender, EventArgs e)
        {
            var profileName = cboProfileExisting.SelectedValue.ToString();
            if (profileName.Length > 0)
            {
                txtProfileName.Text = profileName;
                tvProfileAssets.Nodes.Clear();

                var assetTreeView = new AssetTreeView();

                // Populate meshes
                var meshes = Asset.GetAssetsInProfile(AssetTypes.Mesh, profileName);
                assetTreeView.PopulateWithMeshes(tvProfileAssets, meshes, null);

                // Populate fontdefs
                var fontdefs = Dal.Font.GetFontdef(profileName);
                assetTreeView.PopulateFontdefs(tvProfileAssets, fontdefs, null);

                // Populate materials
                var materials = Material.GetRootMaterialsWithNoChildren(profileName);
                assetTreeView.PopulateWithParentMaterials(tvProfileAssets, materials, null);

                // Populate other assets not assigned to anything
                var others = Profile.GetAssetsWithNoDependencies(profileName);
                assetTreeView.PopulateWithParentOtherAssets(tvProfileAssets, others, null);

                foreach (TreeNode node in tvProfileAssets.Nodes)
                {
                    tvExistingAssetProfile.Nodes.RemoveByKey(node.Name);
                }
            }
            else
            {
                tabPage3_Enter(null, null);
                txtProfileName.Text = string.Empty;
                tvProfileAssets.Nodes.Clear();
            }
        }

        private void tvProfileAssets_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tvProfileAssets.SelectedNode != null)
            {
                var p = new Point(e.X, e.Y);
                var node = tvProfileAssets.GetNodeAt(p);

                if (node != null)
                {
                    var selectedNode = tvProfileAssets.SelectedNode;
                    tvProfileAssets.SelectedNode = node;

                    if (node.Tag != null)
                        cmsProfileTree.Show(tvProfileAssets, p);

                    tvProfileAssets.SelectedNode = selectedNode;
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvProfileAssets.SelectedNode != null && cboProfileExisting.SelectedValue != null)
                {
                    var id = (int)tvProfileAssets.SelectedNode.Tag;
                    var profileName = cboProfileExisting.SelectedValue.ToString();

                    Asset.DeleteAssetFromProfile(id, profileName);

                    tvProfileAssets.Nodes.Remove(tvProfileAssets.SelectedNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConfigGo_Click(object sender, EventArgs e)
        {
            if (txtConfigFileName.Text.Length == 0)
            {
                MessageBox.Show(@"Select config file first dummy.");
                return;
            }

            IEnumerable<string> fileList;
            if (txtConfigFileName.Text.EndsWith("scx"))
            {
                fileList = GetFilesFromConfigXml(txtConfigFileName.Text);
            }
            else if (txtConfigFileName.Text.EndsWith("scd"))
            {
                fileList = GetFilesFromConfigDb(txtConfigFileName.Text);
            }
            else
            {
                MessageBox.Show("Unknown config file extension.");
                return;
            }


            // Get the total list of assets
            var assetIds = new List<int>();

            // List all dependencies
            foreach (string name in fileList)
            {
                var id = Asset.GetAssetId(name);
                if (id > 0)
                {
                    if (!assetIds.Contains(id))
                        assetIds.Add(id);
                }
                else
                {
                    MessageBox.Show("Asset " + name + " is missing.");
                    return;
                }

                Asset.GetDependencyIds(id).ForEach(i => assetIds.Add(i));
            }

            // Save the profile
            SaveProfile(assetIds);

        }

        private void btnConfigOpenFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
                          {
                              InitialDirectory = ".\\",
                              Filter = @"All files (*.*)|*.*",
                              FilterIndex = 1,
                              RestoreDirectory = false
                          };


            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtConfigFileName.Text = ofd.FileName;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvExistingAssetProfile.Nodes)
            {
                if (node != null)
                {
                    tvProfileAssets.Nodes.Add((TreeNode)node.Clone());
                    tvProfileAssets.ExpandAll();
                    node.Remove();
                    toolStripMenuItem1_Click(null, null);
                }
            }
        }

        #endregion

        #region Archive

        private void btnArchiveRun_Click(object sender, EventArgs e)
        {
            if (txtArchiveOutDir.Text.Length == 0)
            {
                txtArchiveOutDir.Text = @".\";
            }

            // Check to see if a profile is selected
            if (cboArchiveAssetProfiles.SelectedValue == null)
            {
                MessageBox.Show(@"Select a profile first.");
                return;
            }

            // Clear the report
            txtArchiveReport.Text = string.Empty;

            // Get the profile name.
            var profileName = cboArchiveAssetProfiles.SelectedValue.ToString();

            // Do we merge the materials file into one
            ResourceHelper.MaterialOutputType materialOutputType;
            if (rdoMergeMaterials.Checked)
            {
                materialOutputType = ResourceHelper.MaterialOutputType.SingleFile;
            }
            else if (rdoLineIncrements.Checked)
            {
                materialOutputType = ResourceHelper.MaterialOutputType.Merge2000;
            }
            else
            {
                materialOutputType = ResourceHelper.MaterialOutputType.OriginalFile;
            }

            string outText = string.Empty;
            _resourceHelper.CreateArchive(txtArchiveOutDir.Text, profileName, materialOutputType, lblStatus, this, ref outText);
            txtArchiveReport.Text = outText;
        }

        private void btnReportAssets_Click(object sender, EventArgs e)
        {
            // Check to see if a profile is selected
            if (cboArchiveAssetProfiles.SelectedValue == null)
            {
                MessageBox.Show(@"Select a profile first.");
                return;
            }

            // Get the profile name.
            var profileName = cboArchiveAssetProfiles.SelectedValue.ToString();

            txtArchiveReport.Text = string.Empty;

            // Get the list of assets
            foreach (var asset in Profile.GetAssets(profileName))
            {
                var assetType = AssetType.GetAssetType((AssetTypes)asset.AssetTypeId);
                txtArchiveReport.Text += "# (" + asset.AssetId + ") '''" + asset.Name + "''' (" + assetType.Description + ")\r\n";
            }
        }

        #endregion

        #region Generated Code

        private void btnGCOutDir_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtGCOutputDir.Text = fd.SelectedPath;
            }
        }

        private void btnGCRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboGCAssetProfile.SelectedValue == null)
                {
                    MessageBox.Show(@"Select a profile.");
                    return;
                }

                if (txtGCOutputDir.Text.Length == 0)
                {
                    MessageBox.Show(@"Select an output directory first.");
                    return;
                }

                var materials = chkGCMaterials.Checked;
                var fonts = chkGCFonts.Checked;
                var message = "Done";

                var profileName = cboGCAssetProfile.SelectedValue.ToString();
                _codeGenerator.GenerateCode(profileName, txtGCOutputDir.Text, chkGCScripting.Checked, materials, fonts, txtGCOutput);

                if (materials)
                    message += "\r\nNew material files are created.";

                if (fonts)
                    message += "\r\nNew font files are created.";

                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGCUnitTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboGCAssetProfile.SelectedValue == null)
                {
                    MessageBox.Show(@"Select a profile.");
                    return;
                }

                if (txtGCOutputDir.Text.Length == 0)
                {
                    MessageBox.Show(@"Select an output directory first.");
                    return;
                }

                var profileName = cboGCAssetProfile.SelectedValue.ToString();
                _codeGenerator.CreateUnitTests(profileName, txtGCOutputDir.Text);
                MessageBox.Show("Done.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGCScripting_Click(object sender, EventArgs e)
        {
            new Scripting().Show();
        }

        #endregion

        #region Inventory

        private void btnInventoryOutput_Click(object sender, EventArgs e)
        {
            txtGCOutput.Text = _itemProfileHelper.CreateToolProfileOutput();
        }

        private void btnAddTool_Click(object sender, EventArgs e)
        {
            var itemTool = new ItemTool(_itemProfileHelper);
            itemTool.Show();
        }

        private void btnAddMagazine_Click(object sender, EventArgs e)
        {
            var itemTool = new Magazine(_itemProfileHelper);
            itemTool.Show();
        }

        private void btnAddProjectile_Click(object sender, EventArgs e)
        {
            var itemTool = new Projectiles(_itemProfileHelper);
            itemTool.Show();
        }

        private void btnGOPSceneFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { InitialDirectory = @".\" };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var sceneFile = new SceneFile();
                txtGCOutput.Text = sceneFile.ConvertFile(ofd.FileName);
            }
        }

        #endregion

        #region Gui Generator

        private void btnGuiXmlOpenFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                InitialDirectory = ".\\",
                Filter = @"All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtGuiXmlFileName.Text = ofd.FileName;
            }
        }

        private void btnGuiGenerate_Click(object sender, EventArgs e)
        {
            if (txtGuiXmlFileName.Text.Length == 0)
            {
                MessageBox.Show(@"Select config file first dummy.");
                return;
            }

            var reader = new XmlTextReader(txtGuiXmlFileName.Text);
            GuiGeneratorBase guiGenerator;
            if (rdoMyGui.Checked)
                guiGenerator = new MyGuiGenerator();
            else
                guiGenerator = new Gui3dGuiGenerator();

            guiGenerator.Parse(reader, "CLASSNAME");

            string output = string.Empty;
            txtGuiHeaderOutput.Text = "//======================================Variable Declarations======================================\r\n";
            foreach (var line in guiGenerator.VariableDeclarations)
            {
                output += line;
            }
            txtGuiHeaderOutput.Text += output + "\r\n\r\n";

            output = string.Empty;
            txtGuiHeaderOutput.Text += "//======================================Event Declarations=====================================\r\n";
            foreach (var line in guiGenerator.EventDeclarations)
            {
                output += line;
            }
            txtGuiHeaderOutput.Text += output + "\r\n\r\n";

            output = string.Empty;
            txtGuiSourceOutput.Text = "//======================================Variable Assignments======================================\r\n";
            foreach (var line in guiGenerator.VariableAssignments)
            {
                output += line;
            }
            txtGuiSourceOutput.Text += output + "\r\n\r\n";

            output = string.Empty;
            txtGuiSourceOutput.Text += "//======================================Event Assignments======================================\r\n";
            foreach (var line in guiGenerator.EventAssignments)
            {
                output += line;
            }
            txtGuiSourceOutput.Text += output + "\r\n\r\n";

            output = string.Empty;
            txtGuiSourceOutput.Text += "//======================================Event Definitions======================================\r\n";
            foreach (var line in guiGenerator.EventDefinitions)
            {
                output += line;
            }
            txtGuiSourceOutput.Text += output + "\r\n\r\n";
        }

        #endregion

        #region Level Converter

        private void btnOpenFileOgitor_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                InitialDirectory = ".\\",
                Filter = @"All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtOgitorLevelConverter.Text = ofd.FileName;
            }
        }

        private void btnConvertOgitor_Click(object sender, EventArgs e)
        {
            var content = LevelConverter.Convert(txtOgitorLevelConverter.Text, LevelConverter.FileType.Ogitor);
            content.ForEach(c => txtLevelConverterOutput.Text += c + "\r\n");
        }

        #endregion

        #region Menu Strip

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Default Image Format: " + Common.Utility.GetDefaultImageId().ToString());
        }

        private void dBTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var fileList = new List<string>();
            //var dt = new DataTable();

            //try
            //{

            //    using (var cnn = new SQLiteConnection("Data Source=../../../Test_database"))
            //    {
            //        cnn.Open();
            //        var cmd = new SQLiteCommand(cnn);

            //        // The all the mesh names from the go and player table
            //        cmd.CommandText = "select * from gameobjects";
            //        SQLiteDataReader reader = cmd.ExecuteReader();
            //        dt.Load(reader);
            //        reader.Close();
            //        cnn.Close();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Settings.Default.Help);
        }

        private void exportOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new ExportFormat();
            frm.Show();
        }

        private void meshConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new MeshConverter();
            frm.Show();
        }

        private void batchMeshSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                InitialDirectory = ".\\",
                Filter = @"XML files (*.xml*)|*.xml*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var reportLog = new List<string>();
                MeshAsset.BatchScaleAndRotation(ofd.FileName, ref reportLog);
                string report = "Done";
                reportLog.ForEach(r => report += r + "\r\n");
                txtAssetOverview.Text = report;
            }
        }

        #endregion

        #endregion

    }
}