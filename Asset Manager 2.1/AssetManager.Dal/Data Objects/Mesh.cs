using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using AssetManager.Common;
using AssetManager.Common.Enums;
using AssetManager.Dal.Models;

namespace AssetManager.Dal
{
    public class Mesh : Asset
    {
        /// <summary>
        /// Saves a mesh file to the DB.
        /// </summary>
        public new static DbId Create(string fileName, int categoryId, ref List<string> errorLog)
        {
            using (var db = new AssetManagerEntities())
            {
                if (!ValidateFile(fileName, ref errorLog))
                    return null;

                string returnValue = string.Empty;
                var fileInfo = new FileInfo(fileName);
                if (!RunXmlConverter(fileInfo.FullName, ref returnValue))
                {
                    errorLog.Add("Error converting mesh to Xml: " + fileInfo.FullName + ", " + returnValue);
                    return null;
                }

                var reader = new XmlTextReader(fileInfo.FullName + ".xml");

                var matList = new List<DbId>();
                var skelList = new List<DbId>();
                if (!FindMeshDependencies(reader, fileInfo.FullName, ref matList, ref skelList, ref errorLog))
                {
                    errorLog.Add("Mesh: " + fileInfo.FullName + " missing dependencies");
                    return null;
                }

#if DEBUG
                // Check for material asset type in database
                if (db.AssetTypes.Select(t => t.Extension).FirstOrDefault(f => f == "mesh") == null)
                {
                    errorLog.Add("Mesh file type is not in the database.");
                    return null;
                }
#endif

                // Save Mesh file
                var asset = new Asset()
                {
                    FileName = fileInfo.Name,
                    Name = fileInfo.Name,
                    Data = File.ReadAllBytes(fileInfo.FullName),
                    AssetTypeId = (int)AssetTypes.Mesh,
                    CategoryId = categoryId,
                    MeshTypeId = (int)MeshTypes.GameObject
                };

                db.Assets.Add(asset);
                if (db.SaveChanges() == 0)
                {
                    errorLog.Add("Error saving: " + fileInfo.FullName);
                }

                // Save material dependencies
                foreach (var id in matList)
                {
                    var depend = new AssetDependency { ParentAssetId = asset.AssetId, ChildAssetId = id };
                    db.AssetDependencies.Add(depend);

                    if (db.SaveChanges() == 0)
                    {
                        errorLog.Add("Error saving material dependency link: " + fileInfo.FullName);
                        return null;
                    }
                }

                // Save skeleton dependencies
                foreach (var id in skelList)
                {
                    var depend = new AssetDependency { ParentAssetId = asset.AssetId, ChildAssetId = id };
                    db.AssetDependencies.Add(depend);

                    if (db.SaveChanges() == 0)
                    {
                        errorLog.Add("Error saving skeleton: " + fileInfo.FullName);
                        return null;
                    }
                }

                return asset.AssetId;
            }
        }

        public static List<AssetModel> GetMeshes(MeshTypes? meshType = null)
        {
            using (var db = new AssetManagerEntities())
            {
                var query = from al in db.AssetLists
                            join a in db.Assets
                            on al.AssetId equals a.AssetId
                            where a.AssetTypeId == (int)AssetTypes.Mesh
                            select new AssetModel
                            {
                                AssetId = a.AssetId,
                                AssetTypeId = a.AssetTypeId,
                                CategoryId = a.CategoryId,
                                FileName = a.FileName,
                                MeshTypeId = a.MeshTypeId,
                                Name = a.Name
                            };

                if (meshType != null)
                {
                    query = query.Where(m => m.MeshTypeId == (int)meshType);
                }

                return query.ToList();
            }
        }

        public static List<AssetModel> GetMeshes(string profileName, MeshTypes? meshType = null)
        {
            using (var db = new AssetManagerEntities())
            {
                var query = from al in db.AssetLists
                            join p in db.Profiles
                            on al.AssetListSet equals p.AssetListSet
                            join a in db.Assets
                            on al.AssetId equals a.AssetId
                            where p.Name == profileName && a.AssetTypeId == (int)AssetTypes.Mesh
                            select new AssetModel
                            {
                                AssetId = a.AssetId,
                                AssetTypeId = a.AssetTypeId,
                                CategoryId = a.CategoryId,
                                FileName = a.FileName,
                                MeshTypeId = a.MeshTypeId,
                                Name = a.Name
                            };

                if (meshType != null)
                {
                    query = query.Where(m => m.MeshTypeId == (int)meshType);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the material assigned to the mesh.  Assumes only one material.
        /// </summary>
        public static DbId GetMaterialDependency(int assetId)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from ad in db.AssetDependencies
                        join a in db.Assets on ad.ChildAssetId equals a.AssetId
                        where ad.ParentAssetId == assetId && a.AssetTypeId == (int)AssetTypes.Material
                        select a.AssetId).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the name of the material assigned to the mesh.  Assumes only one material.
        /// </summary>
        public static string GetMaterialName(int assetId)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from ad in db.AssetDependencies
                        join a in db.Assets on ad.ChildAssetId equals a.AssetId
                        where ad.ParentAssetId == assetId && a.AssetTypeId == (int)AssetTypes.Material
                        select a.Name).DefaultIfEmpty(null).FirstOrDefault();
            }
        }

        private static bool RunXmlConverter(string fileName, ref string reportLog)
        {
            // Add quotes to the filename
            string arg = "\"" + fileName + "\"";

            var fi = new FileInfo(Application.StartupPath + "\\Tools\\OgreXmlConverter.exe");
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
                    reportLog = sr.ReadToEnd();
                }
            }

            return File.Exists(fileName + ".xml");
        }

        private static bool FindMeshDependencies(XmlTextReader reader, string fileName, ref List<DbId> matList, ref List<DbId> skelList, ref List<string> errorLog)
        {
            bool ret = true;

            // Find all materials and skeletons the mesh uses
            var materials = new List<string>();
            var skeletons = new List<string>();
            while (reader.Read())
            {
                // Submesh contains the material name
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "submesh")
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "material" && reader.Value.Length > 0)
                        {
                            if (!materials.Contains(reader.Value))
                                materials.Add(reader.Value);
                        }
                    }
                }
                // skeletonlink contains the skeleton name
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "skeletonlink")
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "name")
                        {
                            if (!skeletons.Contains(reader.Value))
                                skeletons.Add(reader.Value);
                        }
                    }
                }
            }

            // Check to see if the material name exist
            foreach (string name in materials)
            {
                var material = Asset.GetAsset(name, AssetTypes.Material);
                if (material == null)
                {
                    errorLog.Add(fileName + " material missing: " + name);
                    ret = false;
                }
                else
                {
                    matList.Add(material.AssetId);
                }
            }

            // Check to see if the skeleton name exist
            foreach (string name in skeletons)
            {
                var skeleton = Asset.GetAsset(name, AssetTypes.Skeleton);
                if (skeleton == null)
                {
                    errorLog.Add("Skeleton missing: " + name);
                    ret = false;
                }
                else
                {
                    skelList.Add(skeleton.AssetId);
                }
            }

            return ret;
        }
    }
}
