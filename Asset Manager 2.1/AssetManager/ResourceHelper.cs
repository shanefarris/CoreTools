using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using ImageMagick;
using System.IO.Compression;
using AssetManager.Dal;
using AssetManager.Common.Enums;

namespace AssetManager
{
    public class ResourceHelper
    {
        /// <summary>
        /// How are the material files going to be merged/archived in general.
        /// </summary>
        public enum MaterialOutputType
        {
            OriginalFile,
            SingleFile,
            Merge2000,
        };

        #region Public Methods

        /// <summary>
        /// Used to create an archive of a profile.
        /// </summary>
        /// <param name="outDir">Where the achieve will be created.</param>
        /// <param name="profileName">What profile is the archive created against.</param>
        /// <param name="materialOutputType">How the materials will be outputted, merged, not merged, etc.</param>
        /// <param name="lblStatus">Status label on the main form.</param>
        /// <param name="parent">The main form.</param>
        /// <param name="outText">Used to report how the archive went.</param>
        /// <returns></returns>
        public bool CreateArchive(string outDir, string profileName, MaterialOutputType materialOutputType, ToolStripStatusLabel lblStatus, Form parent, ref string outText)
        {
            // Create temp directory
            var tempDir = outDir + "\\temp";

            Directory.CreateDirectory(tempDir);

            lblStatus.Text = "Extracting files...";
            parent.Refresh();

            var fileList = new List<string>();
            var replacedFileList = new List<string>();
            if (ExtractFilesFromDb(profileName, tempDir, outDir, ref fileList))
            {
                lblStatus.Text = "Converting Images...";
                parent.Refresh();

                if (!ImageConversion(tempDir, ref fileList, ref replacedFileList))
                {
                    lblStatus.Text = string.Empty;
                    MessageBox.Show(@"Error converting images.");
                    return false;
                }

                foreach (string s in fileList)
                {
                    outText += s + "\r\n";
                }

                lblStatus.Text = "Materials...";
                parent.Refresh();

                // Do we merge the materials file into one
                if (materialOutputType == MaterialOutputType.SingleFile)
                {
                    if (!MergeMaterials(tempDir, replacedFileList, ref fileList, ref outText))
                    {
                        lblStatus.Text = string.Empty;
                        MessageBox.Show(@"Error merging material files.");
                        return false;
                    }
                }
                else if (materialOutputType == MaterialOutputType.Merge2000)
                {
                    if (!MergeMaterialsLineLimit2000(tempDir, replacedFileList, ref fileList))
                    {
                        lblStatus.Text = string.Empty;
                        MessageBox.Show(@"Error merging material files.");
                        return false;
                    }
                }

                // Compress the assets
                try
                {
                    lblStatus.Text = "Compressing files...";
                    parent.Refresh();

                    if (!CompressToZip(tempDir, outDir, fileList, profileName))
                    {
                        lblStatus.Text = @"Error compressing";
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = string.Empty;
                    MessageBox.Show(ex.Message);
                }

                // Create archive config XML
                outText += "\r\n\r\n********** Archive config *************\r\n";
                outText += "<zip>\r\n";
                outText += "\t<ZipFileArchive zip=\".\\[DIR]\\" + profileName + ".zip\" group=\"[Group]\" />\r\n";
                outText += "</zip>\r\n";

                lblStatus.Text = "Done";
                parent.Refresh();
            }
            else
            {
                MessageBox.Show(@"Error extracting files from the DB.");
                lblStatus.Text = string.Empty;
                return false;
            }
            return true;
        }

        #endregion // Public Methods

        #region Private

        /// <summary>
        /// Based on the profile, it will create a list of files in the profile, and save them all out to the temp directory.
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="archiveOutDir"></param>
        /// <param name="fileList">List of files in the profile.</param>
        /// <returns></returns>
        private bool ExtractFilesFromDb(string profileName, string tempDir, string archiveOutDir, ref List<string> fileList)
        {
            // Check if the director exist first
            if (Directory.Exists(archiveOutDir))
            {
                DialogResult res = MessageBox.Show(@"This directory already exist, remove it?", "Delete?", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                    return false;

                Directory.Delete(archiveOutDir, true);
                //Directory.CreateDirectory(archiveOutDir);
                Directory.CreateDirectory(tempDir);
            }

            // The data context can't handle all the assets in the database so this query has to be chopped up
            // instead of getting all the assets at once
            foreach (var asset in Profile.GetAssets(profileName))
            {
                // Copy the asset to the temp directory
                var assetType = AssetType.GetAssetType((AssetTypes)asset.AssetTypeId);
                var data = Asset.GetAssetData(asset.AssetId);
                if (assetType.IsText)
                {
                    if (File.Exists(tempDir + "\\" + asset.FileName))
                    {
                        File.AppendAllText(tempDir + "\\" + asset.FileName, Common.Utility.ConvertBytesToString(data));
                    }
                    else
                    {
                        File.WriteAllText(tempDir + "\\" + asset.FileName, Common.Utility.ConvertBytesToString(data));
                        fileList.Add(tempDir + "\\" + asset.FileName);
                    }
                }
                else
                {
                    File.WriteAllBytes(tempDir + "\\" + asset.FileName, data);
                    fileList.Add(tempDir + "\\" + asset.FileName);
                }
            }

            return true;
        }

        /// <summary>
        /// If a universal image format is specified, it will convert all images to that format.
        /// </summary>
        /// <param name="archiveOutDir"></param>
        /// <param name="fileList">Newly updated file list with the new names of the images.</param>
        /// <param name="replacedFiles">List of old image names before they were converted.</param>
        /// <returns></returns>
        private bool ImageConversion(string archiveOutDir, ref List<string> fileList, ref List<string> replacedFiles)
        {
            // After all the assets are saved in the temp directory, check to see if anythings needs to be converted
            var format = Common.Utility.GetExportImageExtension();
            if (format != ".none")
            {
                // Create a collection of <old, new> file names.
                var replaceFileList = new Dictionary<string, string>();
                foreach (var file in fileList)
                {
                    var fi = new FileInfo(file);
                    if (fi.Exists &&
                        Common.Utility.IsImageExtension(fi) &&
                        format != fi.Extension)
                    {
                        // Execute the command
                        var newFilename = "\\" + Common.Utility.ReplaceExtension(fi.Name, format);
                        using (MagickImage image = new MagickImage(archiveOutDir + "\\" + fi.Name))
                        {
                            image.Write(archiveOutDir + newFilename);

                            // Check to see if the image was converted.
                            var fiTarget = new FileInfo(archiveOutDir + newFilename);
                            if (fiTarget.Exists)
                            {
                                File.Delete(archiveOutDir + "\\" + fi.Name);
                                replaceFileList[file] = archiveOutDir + newFilename;
                            }
                            else
                            {
                                DialogResult res = MessageBox.Show(@"Could not convert image to export format (" + fi.Name + ")", "Resume?", MessageBoxButtons.YesNo);
                                if (res == DialogResult.No)
                                    return false;
                            }
                        }
                    }
                }

                // Replace the old file names with the new ones
                foreach (var fileSet in replaceFileList)
                {
                    // Remove the old file
                    fileList.Remove(fileSet.Key);

                    // Add the new file
                    fileList.Add(fileSet.Value);

                    // Add the old file to the replaced file list
                    replacedFiles.Add(fileSet.Key);
                }
            }
            return true;
        }

        private bool CompressToZip(string tempDir, string outDir, List<string> fileList, string outFile)
        {
            try
            {
                // Check output directory
                var di = new DirectoryInfo(outDir);
                if (!di.Exists)
                    di.Create();

                // Create temp directory
                var diTemp = new DirectoryInfo(tempDir);
                if (!diTemp.Exists)
                    diTemp.Create();

                foreach (var asset in fileList)
                {
                    var fileInfo = new FileInfo(asset);

                    // File could have been removed from a merge process
                    if (fileInfo.Exists)
                    {
                        if (fileInfo.Extension == ".zip")
                        {
                            File.Copy(fileInfo.FullName, outDir + "\\" + fileInfo.Name);
                            File.Delete(fileInfo.FullName);
                        }
                    }
                }
                ZipFile.CreateFromDirectory(tempDir, outDir + "\\" + outFile + ".zip");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        private string ValidateParticles(string[] content, string dir)
        {
            var ret = string.Empty;
            var partNames = new List<string>();

            // Iterate through each line
            foreach (string s in content)
            {
                // Check for material name
                if (s.Trim().ToLower() == "particle_system")
                {
                    // No material name, invalid
                    ret += "No partical name\r\n";
                }
                // Check for duplicate names
                else if (s.Contains("particle_system"))
                {
                    if (partNames.Contains(s))
                        ret += "Partical name already declared: " + s + "\r\n";
                    else
                        partNames.Add(s);
                }
            }

            ret += "Done.";

            return ret;
        }

        private bool MergeMaterials(string dir, List<string> replacedFileList, ref List<string> fileList, ref string outText)
        {
            string masterMatFile = dir + "\\zzzMaterials.material";
            if (File.Exists(masterMatFile))
                File.Delete(masterMatFile);

            //Create the file.
            using (FileStream fs = File.Create(masterMatFile))
            {
                foreach (string f in fileList)
                {
                    var fi = new FileInfo(f);
                    if (fi.Extension == ".material")
                    {
                        // Copy the content of this file
                        var content = File.ReadAllBytes(f);

                        // Check to see if we need to change the image file name
                        if (Common.Utility.GetExportImageExtension() != ".none")
                            content = MaterialAsset.ReplaceImageNameForMaterial(content, replacedFileList);

                        fs.Write(content, 0, content.Length);
                    }
                }
            }

            // Delete all material files except our merged file
            foreach (string f in fileList)
            {
                var fi = new FileInfo(f);
                string newFileName = dir + "\\" + fi.Name;
                if (fi.Extension == ".material" && fi.FullName != masterMatFile)
                {
                    if (File.Exists(newFileName))
                        File.Delete(newFileName);
                }
            }

            fileList.Add(masterMatFile);
            outText += "*** Material merge done.***\r\n";
            return true;
        }

        private bool MergeMaterialsLineLimit2000(string dir, List<string> replacedFileList, ref List<string> fileList)
        {
            var totalMaterialLines = new List<string>();
            foreach (string f in fileList)
            {
                var fi = new FileInfo(f);
                if (fi.Extension == ".material")
                {
                    var content = File.ReadAllBytes(f);
                    // Check to see if we need to change the image file name
                    if (Common.Utility.GetExportImageExtension() != ".none")
                        content = MaterialAsset.ReplaceImageNameForMaterial(content, replacedFileList);

                    // Copy the content of this file
                    totalMaterialLines.AddRange(Common.Utility.ConvertBytesToStringArray(content));
                }
            }

            // Split it in about 2000 lines
            var lineCount = 0;
            var setString = string.Empty;
            var splitMaterials = new List<string>();
            foreach (string s in totalMaterialLines)
            {
                if (s.Trim() != string.Empty)
                {
                    if (lineCount >= 2000 && s.StartsWith("material"))
                    {
                        // Don't include it in this set of strings
                        splitMaterials.Add(setString);
                        setString = s;
                        lineCount = 0;
                    }
                    else
                    {
                        setString += s + "\r\n";
                        lineCount++;
                    }
                }
            }

            if (setString.Length > 0)
            {
                splitMaterials.Add(setString);
            }

            // Delete all material files except our merged file
            foreach (string f in Directory.GetFiles(dir))
            {
                var fi = new FileInfo(f);
                if (fi.Extension == ".material")
                    fi.Delete();
            }

            //Create the file.
            var fileCount = 0;
            foreach (string content in splitMaterials)
            {
                string fileName = dir + "\\zzzMaterial" + ++fileCount + ".material";
                using (var fs = File.Create(fileName))
                {
                    fs.Write(Common.Utility.ConvertStringToBytes(content), 0, content.Length);
                }
                fileList.Add(fileName);
            }

            return true;
        }

        #endregion

    }
}
