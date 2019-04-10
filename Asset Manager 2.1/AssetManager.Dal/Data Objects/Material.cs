using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using AssetManager.Common;
using AssetManager.Common.Enums;
using AssetManager.Dal.Models;
using AssetManager.Common.MaterialParser;

namespace AssetManager.Dal
{
    public class Material : Asset
    {
        #region Public

        /// <summary>
        /// This will create materials from a given file.  It is important to allow this method to look up the file because it will parse it, and extract the data automatically.
        /// This method could saved multiple materials, so on success, it will just return a DbId of 0 (zero).
        /// </summary>
        public new static DbId Create(string fileName, int categoryId, ref List<string> errorLog)
        {
            using (var db = new AssetManagerEntities())
            {
                if (!ValidateFile(fileName, ref errorLog))
                    return null;

#if DEBUG
                // Check for material asset type in database
                if (db.AssetTypes.Select(t => t.Extension).FirstOrDefault(t => t == "material") == null)
                {
                    errorLog.Add("Material file type is not in the database.");
                    return null;
                }
#endif

                // Validate material file
                var fileInfo = new FileInfo(fileName);
                var materialContent = File.ReadAllLines(fileInfo.FullName);
                if (!ValidateMaterialForNewEntry(fileName, materialContent, ref errorLog))
                {
                    return null;
                }

                // Parse material file
                var parser = new AssetManager.Common.MaterialParser.Parser(isForExportOperation: false);
                var materialAttributeList = parser.ParseFile(materialContent, ref errorLog);

                try
                {
                    // Save material files
                    foreach (var mat in materialAttributeList)
                    {
                        // Check to see if this is a program
                        if (mat.Program != null)
                        {
                            Program.Create(mat.Program, categoryId, ref errorLog);
                        }
                        else
                        {
                            // Get the dependency list (if any)
                            var missingAssets = new List<string>();
                            var dependencies = GetDependenciesFromMaterialAttribute(mat, ref missingAssets);

                            // Check for missing assets
                            if (missingAssets.Count > 0)
                            {
                                errorLog.AddRange(missingAssets.Select(str => fileName + " missing: " + str));
                                return null;
                            }

                            var asset = new Asset();
                            asset.FileName = fileInfo.Name;
                            asset.Name = mat.Name;
                            asset.Data = Utility.ConvertStringToBytes(mat.Data);
                            asset.AssetTypeId = (int)AssetTypes.Material;
                            asset.CategoryId = categoryId;

                            db.Assets.Add(asset);

                            if (db.SaveChanges() == 0)
                            {
                                errorLog.Add("Error saving file: " + fileInfo.Name);
                                return null;
                            }

                            // Save dependencies
                            AssetDependency.SaveDependencies(asset.AssetId, dependencies);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (errorLog != null)
                    {
                        errorLog.Add(fileInfo.FullName + " exception: " + ex.Message);
                    }
                    return null;
                }

                // Could be multiple materials saved, so just return a Id of 0.
                return new DbId(0);
            }
        }

        /// <summary>
        /// List all materials that are not assigned to a mesh or anything else, in other words it is a root asset.
        /// </summary>
        /// <returns></returns>
        public static List<AssetModel> GetRootMaterials(int categoryId)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from a in db.Assets
                        join ad in db.AssetDependencies on a.AssetId equals ad.ChildAssetId into tempTbl
                        from tbl in tempTbl.DefaultIfEmpty()
                        where a.AssetTypeId == (int)AssetTypes.Material && tbl.ChildAssetId == null && a.CategoryId == categoryId
                        orderby a.Name
                        select new AssetModel()
                        {
                            AssetId = a.AssetId,
                            AssetTypeId = a.AssetTypeId,
                            CategoryId = a.CategoryId,
                            FileName = a.FileName,
                            MeshTypeId = a.MeshTypeId,
                            Name = a.Name
                        }).ToList();
            }
        }

        public static List<AssetModel> GetRootMaterials()
        {
            using (var db = new AssetManagerEntities())
            {
                return (from a in db.Assets
                        join ad in db.AssetDependencies on a.AssetId equals ad.ChildAssetId into tempTbl
                        from tbl in tempTbl.DefaultIfEmpty()
                        where a.AssetTypeId == (int)AssetTypes.Material && tbl.ChildAssetId == null
                        orderby a.Name
                        select new AssetModel()
                        {
                            AssetId = a.AssetId,
                            AssetTypeId = a.AssetTypeId,
                            CategoryId = a.CategoryId,
                            FileName = a.FileName,
                            MeshTypeId = a.MeshTypeId,
                            Name = a.Name
                        }).ToList();
            }
        }

        /// <summary>
        /// Returns a collection of material assets that have no children and part of a profile.
        /// </summary>
        public static List<AssetModel> GetRootMaterialsWithNoChildren(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from al in db.AssetLists
                        join p in db.Profiles on al.AssetListSet equals p.AssetListSet
                        join a in db.Assets on al.AssetId equals a.AssetId
                        join ad in db.AssetDependencies on a.AssetId equals ad.ChildAssetId into tempTbl
                        from tbl in tempTbl.DefaultIfEmpty()
                        where p.Name == profileName && a.AssetTypeId == (int)AssetTypes.Material && tbl.ChildAssetId == null
                        orderby a.Name
                        select new AssetModel
                        {
                            AssetId = a.AssetId,
                            AssetTypeId = a.AssetTypeId,
                            CategoryId = a.CategoryId,
                            FileName = a.FileName,
                            MeshTypeId = a.MeshTypeId,
                            Name = a.Name
                        }).ToList();
            }
        }

        public static bool Update(DbId assetId, string content, ref string errorReport)
        {
            using (var db = new AssetManagerEntities())
            {
                var material = db.Assets.FirstOrDefault(a => a.AssetId == assetId);

                if (material == null)
                {
                    errorReport = @"Unable to find material.";
                    return false;
                }

                // Valide material file
                var lines = Regex.Split(content, "\r\n");
                var errorLog = new List<string>();
                if (!ValidateMaterialForUpdate(material.FileName, lines, ref errorLog))
                {
                    var errors = string.Empty;
                    errorLog.ForEach(delegate(string e) { errors += e + "\r\n"; });

                    errorReport = @"Material is not valid:\r\n" + errors;
                    return false;
                }

                // Parse material file
                var parser = new AssetManager.Common.MaterialParser.Parser(isForExportOperation: false);
                var materialAttributeList = parser.ParseFile(lines, ref errorLog);
                if (materialAttributeList.Count() != 1)
                {
                    errorReport = @"Invalid material, must have one material in the text.";
                    return false;
                }

                // Make sure this isn't a program just in case.  It should never be at this point, but just in case
                if (materialAttributeList[0].Program != null)
                {
                    return Program.Update(assetId, materialAttributeList[0].Program, ref errorReport);
                }

                // Get the dependency list (if any)
                var missingAssets = new List<string>();
                var dependencies = GetDependenciesFromMaterialAttribute(materialAttributeList[0], ref missingAssets);

                // Check for missing assets
                if (missingAssets.Count > 0)
                {
                    errorReport = @"Missing dependecy assets.";
                    return false;
                }

                try
                {
                    material.Name = materialAttributeList[0].Name;
                    material.Data = Utility.ConvertStringToBytes(materialAttributeList[0].Data);

                    // Update the dependencies if needed.
                    if (!AssetDependency.DeleteDependencies(material.AssetId))
                    {
                        errorReport = @"Unable to remove old/existing dependencies.";
                        return false;
                    }
                    if (!AssetDependency.SaveDependencies(material.AssetId, dependencies))
                    {
                        errorReport = @"Unable to save dependency list.";
                        return false;
                    }

                    if (db.SaveChanges() == 0)
                    {
                        errorReport = @"Error updating material.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorReport = @"Error updating material: " + ex.Message;
                    return false;
                }

                return true;
            }
        }

        #endregion // Public

        #region Private

        /// <summary>
        /// Will find all the dependencies in the MaterialAttributes class, and look them up in the DB.  If they are not found in the
        /// DB, it will add it to the missingAssets list.
        /// </summary>
        private static List<DbId> GetDependenciesFromMaterialAttribute(MaterialAttributes MaterialAttributes, ref List<string> missingAssets)
        {
            // Look up the materail dependencies first and ensure they are already saved in the DB.
            // TODO: For now we are only tracking images, but we will need scripts and base materials as well.
            var dependencies = new List<DbId>();
            foreach (var technique in MaterialAttributes.Techniques)
            {
                foreach (var pass in technique.Passes)
                {
                    foreach (var unit in pass.TextureUnits)
                    {
                        var id = Asset.GetAssetId(unit.Texture);
                        if (id > 0)
                        {
                            dependencies.Add(id);
                        }
                        else
                        {
                            missingAssets.Add(unit.Texture);
                        }
                    }
                }
            }
            return dependencies;
        }

        private static string CheckMaterialLineForAsset(string line, ref List<string> externalFiles)
        {
            var ret = string.Empty;
            if ((line.Contains("texture ") || line.Contains("source ") || line.Contains("cubic_texture ")) &&
                !(line.Contains("//") || line.Contains("src_texture")))
            {
                ret = line.Replace("\t", string.Empty).Trim();
                if (ret.Contains("cubic_texture "))
                {
                    ret = ret.Replace("cubic_texture ", string.Empty).Trim();
                    if (ret.Contains(" "))
                    {
                        ret = ret.Substring(0, ret.IndexOf(" "));
                        string extension = ret.Substring(ret.LastIndexOf("."));
                        string temp = ret.Replace(extension, "_fr") + extension;
                        if (!externalFiles.Contains(temp))
                            externalFiles.Add(temp);

                        temp = ret.Replace(extension, "_bk") + extension;
                        if (!externalFiles.Contains(temp))
                            externalFiles.Add(temp);

                        temp = ret.Replace(extension, "_up") + extension;
                        if (!externalFiles.Contains(temp))
                            externalFiles.Add(temp);

                        temp = ret.Replace(extension, "_dn") + extension;
                        if (!externalFiles.Contains(temp))
                            externalFiles.Add(temp);

                        temp = ret.Replace(extension, "_rt") + extension;
                        if (!externalFiles.Contains(temp))
                            externalFiles.Add(temp);

                        temp = ret.Replace(extension, "_lf") + extension;
                        if (!externalFiles.Contains(temp))
                            externalFiles.Add(temp);

                        // Clear the ret string because we don't want to add the "base file name"
                        ret = string.Empty;
                    }
                }
                else if (ret.Contains("texture "))
                {
                    ret = ret.Replace("texture ", string.Empty).Trim();
                    if (ret.Contains(" "))
                        ret = ret.Substring(0, ret.IndexOf(" "));
                }
                else if (ret.Contains("source "))
                {
                    ret = ret.Replace("source ", string.Empty).Trim();
                }
            }
            return ret;
        }

        private static bool ValidateMaterialForNewEntry(string fileName, IEnumerable<string> content, ref List<string> errorLog)
        {
            var materialNames = new List<string>();
            var returnValue = ValidateMaterialCommon(fileName, content, ref materialNames, ref errorLog);
            if (returnValue)
                returnValue = IsMaterialNew(materialNames, ref errorLog);
            return returnValue;
        }

        private static bool ValidateMaterialForUpdate(string fileName, IEnumerable<string> content, ref List<string> errorLog)
        {
            var materialNames = new List<string>();
            return ValidateMaterialCommon(fileName, content, ref materialNames, ref errorLog);
        }

        private static bool ValidateMaterialCommon(string fileName, IEnumerable<string> content, ref List<string> materialNames, ref List<string> errorLog)
        {
            using (var db = new AssetManagerEntities())
            {
                bool ret = true;
                var matNames = new List<string>();
                var externalFiles = new List<string>();

                // Iterate through each line
                foreach (string s in content)
                {
                    // Check for material name
                    if (s.Trim().ToLower() == "material")
                    {
                        // No material name, invalid
                        errorLog.Add("No material name");
                    }
                    // Check for duplicate names
                    else if (s.Contains("material"))
                    {
                        if (matNames.Contains(s))
                        {
                            errorLog.Add("Material name already declared: " + s);
                            ret = false;
                        }
                        else if (matNames.Contains("}material"))
                        {
                            errorLog.Add("Materal started with no return." + s);
                            ret = false;
                        }
                        else
                            matNames.Add(s);
                    }
                    // Gather external filename and put it in a list
                    else
                    {
                        var assetName = CheckMaterialLineForAsset(s, ref externalFiles);
                        if (assetName.Length > 0)
                        {
                            // Add the file to the external file list
                            if (!externalFiles.Contains(assetName))
                                externalFiles.Add(assetName);
                        }
                    }
                }

                // Check for the existance of the files in the external files list
                foreach (var file in externalFiles)
                {
                    if ((from a in db.Assets where a.Name == file select a.AssetId).Count() == 0)
                    {
                        errorLog.Add(fileName + " missing file: " + file);
                        ret = false;
                    }
                }

                return ret;
            }
        }

        private static bool IsMaterialNew(List<string> materialNames, ref List<string> errorLog)
        {
            // Check for the existance of the same material names in the database
            using (var db = new AssetManagerEntities())
            {
                foreach (var n in materialNames)
                {
                    string name = n.Replace("material ", string.Empty);
                    if ((from a in db.Assets where a.Name == name && a.AssetTypeId == 1 select a.AssetId).Count() > 0)
                    {
                        errorLog.Add("Material name already exists in database: " + name);
                        return false;
                    }
                }
                return true;
            }
        }

        #endregion // Private
    }
}
