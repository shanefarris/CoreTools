using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using AssetManager.Dal;
using AssetManager.Common;
using AssetManager.Common.MaterialParser;

namespace AssetManager
{
    internal class MaterialAsset
    {
        /// <summary>
        /// If an image format is specificed for export, this will search for all the images that were formatted, and change the file name in the
        /// material text files.
        /// </summary>
        /// <param name="materialContent"></param>
        /// <param name="replacedFileList">Old file name list created by the ImageConversion method.</param>
        /// <returns></returns>
        public static byte[] ReplaceImageNameForMaterial(byte[] materialContent, List<string> replacedFileList)
        {
            // Convert byte[] to string
            var textContent = Common.Utility.ConvertBytesToString(materialContent);

            // Search to see if the material is referencing a replaced file
            foreach (var file in replacedFileList)
            {
                var index = file.LastIndexOf("\\") + 1;
                var fileName = file.Substring(index, file.Length - index);
                if (textContent.Contains(fileName))
                {
                    var newFilename = Common.Utility.ReplaceExtension(fileName, Common.Utility.GetExportImageExtension());
                    textContent = textContent.Replace(fileName, newFilename);
                    break;
                }
            }

            return Common.Utility.ConvertStringToBytes(textContent);
        }

        /// <summary>
        /// This will iterate through each material and report any that might be duplicates
        /// </summary>
        /// <param name="reportLog"></param>
        public static void FindDuplicates(ref List<string> reportLog)
        {
            var parser = new AssetManager.Common.MaterialParser.Parser(isForExportOperation: false);
            var materials = Asset.GetAssets(Common.Enums.AssetTypes.Material);

            foreach (var material in materials)
            {
                var baseMaterialData = Asset.GetAssetData(material.AssetId);
                if (baseMaterialData == null)
                {
                    reportLog.Add("Could not find base material to compare with.");
                    return;
                }

                // Parse base material into a MaterialAttributes class.
                var baseMaterialAttributes = parser.Parse(Utility.ConvertBytesToString(baseMaterialData), ref reportLog);
                if (baseMaterialAttributes == null)
                {
                    reportLog.Add("Error parsing material data into material attributes.");
                    return;
                }

                // Not going to run this if the material is a program
                if (baseMaterialAttributes.Program == null)
                {
                    foreach (var m2 in materials)
                    {
                        if (m2.AssetId != material.AssetId)
                        {
                            _FindDuplicate(baseMaterialAttributes, material.AssetId, ref reportLog);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Looks for any duplicates of the one material passed to check against.
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="reportLog"></param>
        public static void FindDuplicate(DbId materialId, ref List<string> reportLog)
        {
            var baseMaterialData = Asset.GetAssetData(materialId);
            if (baseMaterialData == null)
            {
                reportLog.Add("Could not find base material to compare with.");
                return;
            }

            // Parse base material into a MaterialAttributes class.
            var parser = new AssetManager.Common.MaterialParser.Parser(isForExportOperation: false);
            var baseMaterialAttributes = parser.Parse(Utility.ConvertBytesToString(baseMaterialData), ref reportLog);
            if (baseMaterialAttributes == null)
            {
                reportLog.Add("Error parsing material data into material attributes.");
                return;
            }
            else if (baseMaterialAttributes.Program != null)
            {
                reportLog.Add("This is not a material, its a program.");
                return;
            }

            _FindDuplicate(baseMaterialAttributes, materialId, ref reportLog);
        }

        private static void _FindDuplicate(MaterialAttributes baseMaterialAttributes, DbId baseMaterialId, ref List<string> reportLog)
        {
            var parser = new AssetManager.Common.MaterialParser.Parser(isForExportOperation: false);
            var materials = Asset.GetAssets(Common.Enums.AssetTypes.Material);
            foreach (var material in materials)
            {
                if (material.AssetId != baseMaterialId)
                {
                    var materialData = Asset.GetAssetData(material.AssetId);
                    if (materialData == null)
                    {
                        reportLog.Add("Unable to get material data for material: " + material.AssetId);
                    }
                    else
                    {
                        var materialAttributes = parser.Parse(Utility.ConvertBytesToString(materialData), ref reportLog);
                        if (materialAttributes == null)
                        {
                            reportLog.Add("Unable to parse material data into a MaterialAttributes class, Id: " + material.AssetId);
                        }
                        else if (materialAttributes.Program != null)
                        {
                            // This is a program and we will not compare it.
                        }
                        else
                        {
                            if (baseMaterialAttributes == materialAttributes)
                            {
                                reportLog.Add("Material Id: " + material.AssetId + "(" + material.FileName + ") could be a possible duplicate with Id: " + baseMaterialId.Id);
                            }
                        }
                    }
                }
            }
        }
    }
}
