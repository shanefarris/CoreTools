using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using AssetManager.Common.Enums;
using AssetManager.Common;
using AssetManager.Dal;
using AssetManager.Dal.Models;
using System;

namespace AssetManager
{
    class MeshAsset
    {
        private static string BASE_DIR = Application.StartupPath + "/sandbox/";

        #region Public Methods

        /// <summary>
        /// Runs the Ogre mesh upgrader tool
        /// </summary>
        public static bool RunUpgrader(string fileName, ref string reportLog)
        {
            // Add quotes to the filename
            string arg = "\"" + fileName + "\"";

            var fi = new FileInfo(Application.StartupPath + "\\Tools\\OgreMeshUpgrader.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = arg,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(info))
                {
                    using (StreamReader sr = process.StandardOutput)
                    {
                        while (sr.Peek() > 0)
                        {
                            reportLog = sr.ReadToEnd();
                        }
                    }

                    return process.ExitCode == 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Change the scale of the mesh.
        /// </summary>
        public static bool SaveScale(double x, double y, double z, string meshFileName, ref List<string> reportLog)
        {
            x = Math.Round((double)x, 5, MidpointRounding.AwayFromZero);
            y = Math.Round((double)y, 5, MidpointRounding.AwayFromZero);
            z = Math.Round((double)z, 5, MidpointRounding.AwayFromZero);

            // Execute the command
            string args = string.Format(" transform -scale={0}/{1}/{2} {3}", x, y, z, "\"" + meshFileName + "\"");
            var fi = new FileInfo(Application.StartupPath + @"\Tools\OgreMeshMagick\OgreMeshMagick.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = args,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(info))
                {
                    StreamReader sr = process.StandardOutput;
                    string returnvalue = sr.ReadToEnd();
                    reportLog.Add(returnvalue + "\r\n\r\n");
                }
            }

            return true;
        }

        /// <summary>
        /// Change the size of the mesh.
        /// </summary>
        public static bool SaveSize(double x, double y, double z, string meshFileName, ref List<string> reportLog)
        {
            x = Math.Round((double)x, 5, MidpointRounding.AwayFromZero);
            y = Math.Round((double)y, 5, MidpointRounding.AwayFromZero);
            z = Math.Round((double)z, 5, MidpointRounding.AwayFromZero);

            // Execute the command
            string args = string.Format(" transform -resize={0}/{1}/{2} {3}", x, y, z, "\"" + meshFileName + "\"");
            var fi = new FileInfo(Application.StartupPath + @"\Tools\OgreMeshMagick\OgreMeshMagick.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = args,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(info))
                {
                    StreamReader sr = process.StandardOutput;
                    string returnvalue = sr.ReadToEnd();
                    reportLog.Add(returnvalue + "\r\n\r\n");
                }
            }

            return true;
        }

        /// <summary>
        /// Change the rotation of the model.
        /// </summary>
        public static bool SaveRotation(double angle, bool rotX, bool rotY, bool rotZ, string meshFileName, ref List<string> reportLog)
        {
            // Execute the command 
            var args = string.Format(" transform -rotate={0}/{1}/{2}/{3} {4}", angle, rotX ? "1" : "0", rotY ? "1" : "0", rotZ ? "1" : "0", "\"" + meshFileName + "\"");
            var fi = new FileInfo(Application.StartupPath + @"\Tools\OgreMeshMagick\OgreMeshMagick.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = args,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(info))
                {
                    StreamReader sr = process.StandardOutput;
                    string returnvalue = sr.ReadToEnd();
                    reportLog.Add(returnvalue + "\r\n\r\n");
                }
            }

            return true;
        }

        /// <summary>
        /// Change the material that is assigned to one model and assign it a new one.
        /// </summary>
        public static bool SaveMaterial(string oldMaterialName, string newMaterialName, string meshFileName, ref List<string> reportLog)
        {
            // Create the argument
            string arg = string.Format("rename -material=/{0}/{1}/ {2}", "\"" + oldMaterialName + "\"", newMaterialName, "\"" + meshFileName + "\"");

            var fi = new FileInfo(Application.StartupPath + "\\Tools\\OgreMeshMagick\\OgreMeshMagick.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = arg,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(info))
                {
                    StreamReader sr = process.StandardOutput;
                    reportLog.Add(sr.ReadToEnd());
                }
            }

            return true;
        }

        public static bool GetInfo(string meshFileName, out float x, out float y, out float z, ref List<string> reportLog)
        {
            // Negetive numbers means an error.
            x = -1;
            y = -1;
            z = -1;

            // Execute the command
            string args = string.Format(" info \"" + meshFileName + "\"");
            var fi = new FileInfo(Application.StartupPath + @"\Tools\OgreMeshMagick\OgreMeshMagick.exe");
            if (fi.Exists)
            {
                var info = new ProcessStartInfo(fi.FullName)
                {
                    UseShellExecute = false,
                    Arguments = args,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                string returnvalue = string.Empty;
                using (Process process = Process.Start(info))
                {
                    var sr = process.StandardOutput;
                    returnvalue = sr.ReadToEnd();
                    reportLog.Add(returnvalue + "\r\n\r\n");
                }

                // Find the size in meters
                var standardScale = Properties.Settings.Default.StandardSize;
                var startingPoint = "Bounding box:";
                if (returnvalue.Contains(startingPoint))
                {
                    var start = returnvalue.IndexOf(startingPoint) + startingPoint.Length;
                    var bound = returnvalue.Substring(start, returnvalue.Length - start);
                    var end = bound.IndexOf("]]");
                    bound = returnvalue.Substring(start, end).Replace("[", "").Replace("]", "").Trim();
                    var points = bound.Split(',');

                    float rawX1, rawX2, rawY1, rawY2, rawZ1, rawZ2;

                    // Get x.
                    if (!float.TryParse(points[0], out rawX1) || !float.TryParse(points[3], out rawX2))
                        return false;

                    x = (Math.Abs(rawX1 - rawX2)) / standardScale;

                    // Get y.
                    if (!float.TryParse(points[1], out rawY1) || !float.TryParse(points[4], out rawY2))
                        return false;

                    y = (Math.Abs(rawY1 - rawY2)) / standardScale;

                    // Get z.
                    if (!float.TryParse(points[2], out rawZ1) || !float.TryParse(points[5], out rawZ2))
                        return false;

                    z = (Math.Abs(rawZ1 - rawZ2)) / standardScale;
                }
            }

            return true;
        }

        /// <summary>
        /// Use an XML file to modify meshes in a batch instead of individually.
        /// </summary>
        public static void BatchScaleAndRotation(string xmlSettingsFile, ref List<string> reportLog)
        {
            FileInfo fi = new FileInfo(xmlSettingsFile);
            if (!fi.Exists)
            {
                reportLog.Add(xmlSettingsFile + ": Does not exist");
                return;
            }

            var reader = new XmlTextReader(xmlSettingsFile);
            if (reader == null)
            {
                reportLog.Add("Unable to open XML file.");
                return;
            }

            int modelCount = 0;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "model")
                {
                    modelCount++;
                    ParseModelXmlFile(reader, modelCount, ref reportLog);
                }
            }
        }

        #endregion // Public Methods

        #region Private Methods

        private static void ParseModelXmlFile(XmlTextReader reader, int modelCount, ref List<string> reportLog)
        {
            double s = 0.0, r = 0.0;
            bool rotX = false, rotY = false, rotZ = false;
            string name = reader.GetAttribute("name");
            string scale = reader.GetAttribute("scale");
            string rotation = reader.GetAttribute("rotate");

            if (name.Length == 0)
            {
                reportLog.Add("Unable to find model name on count: " + modelCount);
                return;
            }

            if (!DownloadFile(name))
            {
                reportLog.Add("Unable to download model: " + name);
                return;
            }

            if (scale.Length > 0)
            {
                if (!double.TryParse(scale, out s))
                {
                    reportLog.Add("Unable to parse scale: " + name);
                    return;
                }

                SaveScale(s, s, s, BASE_DIR + name + ".mesh", ref reportLog);
            }

            if (rotation.Length > 0)
            {
                var str = rotation.Substring(0, 1);
                rotation = rotation.Remove(0, 1);

                if (str.ToLower() == "x")
                    rotX = true;
                else if (str.ToLower() == "y")
                    rotY = true;
                else if (str.ToLower() == "z")
                    rotZ = true;
                else
                {
                    reportLog.Add("Unable to parse rotation: " + name);
                    return;
                }

                if (!double.TryParse(rotation, out r))
                {
                    reportLog.Add("Unable to parse rotation: " + name);
                    return;
                }

                SaveRotation(r, rotX, rotY, rotZ, BASE_DIR + name + ".mesh", ref reportLog);
            }
        }

        private static bool DownloadFile(string meshFileName)
        {
            var parentAsset = Asset.GetAsset(meshFileName + ".mesh", AssetTypes.Mesh);

            // Delete the file if it exists
            if (File.Exists(BASE_DIR + meshFileName))
                File.Delete(BASE_DIR + meshFileName);

            // Download dependencies if required
            var assets = new List<AssetModel>() { parentAsset };
            Asset.GetDependencies(parentAsset.AssetId).ForEach(a => assets.Add(a));

            foreach(var asset in assets)
            {
                // Look for the mesh and skeleton
                if(asset.AssetTypeId == (int)AssetTypes.Skeleton || asset.AssetTypeId == (int)AssetTypes.Mesh)
                {
                    // Download the data
                    var data = Asset.GetAssetData(asset.AssetId);
                    if (data != null)
                    {
                        File.WriteAllBytes(BASE_DIR + asset.FileName, data);
                    }
                }
            }

            return File.Exists(BASE_DIR + meshFileName + ".mesh");
        }

        #endregion // Private Methods
    }
}
