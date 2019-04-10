using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AssetManager.Common;
using AssetManager.Common.Enums;
using AssetManager.Dal.Models;

namespace AssetManager.Dal
{
    public partial class Asset
    {
        public static DbId Create(string fileName, int categoryId, ref List<string> errorLog)
        {
            using (var db = new AssetManagerEntities())
            {
                if (!ValidateFile(fileName, ref errorLog))
                    return null;

                // Is this in the list of asset types
                var fileInfo = new FileInfo(fileName);
                var extension = fileInfo.Extension.Replace(".", string.Empty);
                if ((from a in db.AssetTypes where a.Extension == extension select a.AssetTypeId).Count() == 0)
                {
                    errorLog.Add("File type not valid: " + fileInfo.FullName);
                    return null;
                }
                else
                {
                    var assetTypeId = db.AssetTypes.Where(f => f.Extension == extension).Select(f => f.AssetTypeId).DefaultIfEmpty(0).First();

                    // data to save to the db
                    byte[] data = File.ReadAllBytes(fileInfo.FullName);

                    var asset = new Asset()
                    {
                        FileName = fileInfo.Name,
                        Name = fileInfo.Name,
                        Data = data,
                        AssetTypeId = assetTypeId,
                        CategoryId = categoryId
                    };

                    try
                    {
                        db.Assets.Add(asset);
                        if (db.SaveChanges() == 0)
                        {
                            errorLog.Add("Error saving file: " + fileInfo.FullName);
                            return null;
                        }

                        return asset.AssetId;
                    }
                    catch (Exception ex)
                    {
                        errorLog.Add(fileInfo.FullName + " exception: " + ex.Message);
                        return null;
                    }

                }
            }
        }

        public static DbId Create(AssetTypes assetType, int categoryId, byte[] data, string fileName, MeshTypes? meshType, string name)
        {
            using (var db = new AssetManagerEntities())
            {
                var asset = new Asset()
                {
                    AssetTypeId = (int)assetType,
                    CategoryId = categoryId,
                    Data = data,
                    FileName = fileName,
                    MeshTypeId = (int)meshType,
                    Name = name
                };

                db.Assets.Add(asset);

                if (db.SaveChanges() > 0)
                {
                    return asset.AssetId;
                }

                return null;
            }
        }

        public static bool Update(int assetId, int categoryId, byte[] data, string fileName, MeshTypes? meshType, string name)
        {
            using (var db = new AssetManagerEntities())
            {
                var asset = db.Assets.Where(a => a.AssetId == assetId).FirstOrDefault();
                asset.CategoryId = categoryId;
                asset.Data = data;
                asset.FileName = fileName;
                asset.Name = name;
                asset.MeshTypeId = meshType == null ? null : (int?)meshType.Value;

                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Returns the count of a asset type in the DB
        /// </summary>
        public static int NumberOfAssetType(AssetTypes assetType)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets.Where(a => a.AssetTypeId == (int)assetType).Select(a => a.AssetId).Count();
            }
        }

        /// <summary>
        /// Returns the count of all assets in the DB
        /// </summary>
        public static int NumberOfTotalAssets()
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets.Select(a => a.AssetId).Count();
            }
        }

        /// <summary>
        /// Returns a collection of Asset that are "root", meaning the asset is not a child of any other asset.  This
        /// is also order by name by default.
        /// </summary>
        public static List<AssetModel> GetRootAssets(AssetTypes assetType)
        {
            using (var db = new AssetManagerEntities())
            {
                IEnumerable<Asset> assets = db.GetRootAssets();
                assets = assets.Where(a => a.AssetTypeId == (int)assetType);

                return assets.OrderBy(a => a.Name).Select(a => new AssetModel()
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

        public static List<AssetModel> GetRootAssets(AssetTypes assetType, int categoryId)
        {
            using (var db = new AssetManagerEntities())
            {
                IEnumerable<Asset> assets = db.GetRootAssets();
                assets = assets.Where(a => a.AssetTypeId == (int)assetType);
                assets = assets.Where(a => a.CategoryId == categoryId);

                return assets.OrderBy(a => a.Name).Select(a => new AssetModel()
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
        /// Returns a list of assets that do not have any children assigned to them.
        /// </summary>
        /// <returns></returns>
        public static List<AssetModel> GetAssetsWithNoDependencies(int? categoryId = null)
        {
            using (var db = new AssetManagerEntities())
            {
                if (categoryId != null)
                {
                    return (from a in db.Assets
                            join ad in db.AssetDependencies on a.AssetId equals ad.ChildAssetId into tempTbl
                            join at in db.AssetTypes on a.AssetTypeId equals at.AssetTypeId
                            from tbl in tempTbl.DefaultIfEmpty()
                            where tbl.ChildAssetId == null &&
                            a.CategoryId == categoryId.Value
                            orderby at.Extension, a.Name
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
                return (from a in db.Assets
                        join ad in db.AssetDependencies on a.AssetId equals ad.ChildAssetId into tempTbl
                        join at in db.AssetTypes on a.AssetTypeId equals at.AssetTypeId
                        from tbl in tempTbl.DefaultIfEmpty()
                        where tbl.ChildAssetId == null
                        orderby at.Extension, a.Name
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
        /// Returns a collection of assets based on the type passed.
        /// </summary>
        public static List<AssetModel> GetAssets(AssetTypes assetType)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets.Where(a => a.AssetTypeId == (int)assetType).OrderBy(a => a.Name).Select(a => new AssetModel()
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

        public static List<AssetModel> GetAssets(AssetTypes assetType, int categoryId)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets
                        .Where(a => a.AssetTypeId == (int)assetType && a.CategoryId == categoryId)
                        .OrderBy(a => a.Name)
                        .Select(a => new AssetModel()
                        {
                            AssetId = a.AssetId,
                            AssetTypeId = a.AssetTypeId,
                            CategoryId = a.CategoryId,
                            FileName = a.FileName,
                            MeshTypeId = a.MeshTypeId,
                            Name = a.Name
                        })
                        .ToList();
            }
        }

        /// <summary>
        /// Get a single asset given an asset Id
        /// </summary>
        public static AssetModel GetAsset(int assetId)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets.Where(a => a.AssetId == assetId).Select(a => new AssetModel()
                {
                    AssetId = a.AssetId,
                    AssetTypeId = a.AssetTypeId,
                    CategoryId = a.CategoryId,
                    FileName = a.FileName,
                    MeshTypeId = a.MeshTypeId,
                    Name = a.Name
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get a single asset given an asset name and type Id.
        /// </summary>
        public static AssetModel GetAsset(string name, AssetTypes assetType)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets.Where(a => a.Name == name && a.AssetTypeId == (int)assetType).Select(a => new AssetModel()
                {
                    AssetId = a.AssetId,
                    AssetTypeId = a.AssetTypeId,
                    CategoryId = a.CategoryId,
                    FileName = a.FileName,
                    MeshTypeId = a.MeshTypeId,
                    Name = a.Name
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Returns just the data of the asset because every other method will not automatically download that data.
        /// </summary>
        public static byte[] GetAssetData(int assetId)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Assets.Where(a => a.AssetId == assetId).Select(a => a.Data).FirstOrDefault();
            }
        }

        /// <summary>
        /// Returns a collection of assets by asset type and within a profile.
        /// </summary>
        public static List<AssetModel> GetAssetsInProfile(AssetTypes assetType, string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from al in db.AssetLists
                        join p in db.Profiles
                            on al.AssetListSet equals p.AssetListSet
                        join a in db.Assets
                            on al.AssetId equals a.AssetId
                        where p.Name == profileName &&
                              a.AssetTypeId == (int)assetType
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
        /// Return the asset id of a given asset file name.
        /// </summary>
        /// <param name="assetFileName"></param>
        /// <returns></returns>
        public static DbId GetAssetId(string assetFileName)
        {
            using (var db = new AssetManagerEntities())
            {
                int id = (from a in db.Assets
                          where a.FileName == assetFileName
                          select a.AssetId).FirstOrDefault();

                // Check if it is a material, materials are looked up by "Name"
                if (id == 0)
                {
                    id = (from a in db.Assets
                          where a.Name == assetFileName
                          select a.AssetId).FirstOrDefault();
                }

                return id;
            }
        }

        /// <summary>
        /// Return the file name of a given asset Id.
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public static string GetAssetFileName(int assetId)
        {
            using (var db = new AssetManagerEntities())
            {
                string ret = null;
                var asset = (from a in db.Assets
                             where a.AssetId == assetId
                             select a).FirstOrDefault();

                // Materials go by "Name" instead of "FileName"
                if (asset != null)
                {
                    if (asset.AssetTypeId == (int)AssetTypes.Material)
                        ret = asset.Name;
                    else
                        ret = asset.FileName;
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets the dependency Ids plus any sub dependencies of an asset.
        /// </summary>
        /// <param name="parentAssetFileName"></param>
        /// <param name="dependencyList"></param>
        /// <returns></returns>
        public static List<DbId> GetDependencyIds(string parentAssetFileName)
        {
            using (var db = new AssetManagerEntities())
            {
                var dependencies = new List<DbId>();
                foreach (var asset in (from ad in db.AssetDependencies
                                       join a in db.Assets
                                       on ad.ParentAssetId equals a.AssetId
                                       where a.FileName.ToLower() == parentAssetFileName.ToLower() ||
                                       a.Name.ToLower() == parentAssetFileName.ToLower()
                                       select new { AssetId = ad.ChildAssetId, FileName = a.FileName }))
                {
                    if (!dependencies.Contains(asset.AssetId))
                    {
                        dependencies.Add(asset.AssetId);
                        GetDependencyIds(asset.AssetId).ForEach(i =>
                        {
                            if (!dependencies.Contains(i))
                            {
                                dependencies.Add(i);
                            }
                        });
                    }
                }
                return dependencies;
            }
        }

        /// <summary>
        /// Gets the dependencies plus any sub dependencies of an asset.
        /// </summary>
        /// <param name="parentAssetId"></param>
        /// <param name="dependencyList"></param>
        public static List<DbId> GetDependencyIds(int parentAssetId)
        {
            using (var db = new AssetManagerEntities())
            {
                var dependencies = new List<DbId>();
                foreach (var id in (from ad in db.AssetDependencies
                                    join a in db.Assets
                                    on ad.ParentAssetId equals a.AssetId
                                    where ad.ParentAssetId == parentAssetId
                                    select ad.ChildAssetId))
                {
                    if (!dependencies.Contains(id))
                    {
                        dependencies.Add(id);
                        GetDependencyIds(id).ForEach(i =>
                        {
                            if (!dependencies.Contains(i))
                            {
                                dependencies.Add(i);
                            }
                        });
                    }
                }
                return dependencies;
            }
        }

        /// <summary>
        /// Returns dependency Asset objects based on the parent Id
        /// </summary>
        public static List<AssetModel> GetDependencies(int parentAssetId)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from ad in db.AssetDependencies
                        join a in db.Assets
                        on ad.ChildAssetId equals a.AssetId
                        where ad.ParentAssetId == parentAssetId
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
        /// Deletes an asset from the profile list
        /// </summary>
        public static void DeleteAssetFromProfile(int assetId, string profileName)
        {
            var profileId = Profile.GetProfileId(profileName);
            DeleteAssetFromProfile(assetId, profileId);
        }

        /// <summary>
        /// Deletes an asset from the profile list
        /// </summary>
        private static void DeleteAssetFromProfile(int assetId, int profileId)
        {
            using (var db = new AssetManagerEntities())
            {
                db.DeleteAssetFromProfile(assetId, profileId);
            }
        }

        /// <summary>
        /// Delete an asset and its dependencies from the DB.
        /// </summary>
        public static bool DeleteAsset(int assetId, ref List<string> errorLog)
        {
            try
            {
                using (var db = new AssetManagerEntities())
                {
                    // Get dependencies
                    var assetIds = db.AssetDependencies.Where(ad => ad.ParentAssetId == assetId).Select(ad => ad.ChildAssetId).ToList();

                    if (assetIds.Count() == 0)
                    {
                        DeleteAssetList(assetId, db, ref errorLog);
                        DeleteAssetDependencies(assetId, db, ref errorLog);
                        _DeleteAsset(assetId, db, ref errorLog);
                    }
                    else
                    {
                        foreach (var id in assetIds)
                        {
                            DeleteAsset(id, ref errorLog);
                        }

                        DeleteAssetList(assetId, db, ref errorLog);
                        DeleteAssetDependencies(assetId, db, ref errorLog);
                        _DeleteAsset(assetId, db, ref errorLog);
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                errorLog.Add(ex.Message);
                return false;
            }
        }

        private static void DeleteAssetDependencies(int assetId, AssetManagerEntities db, ref List<string> errorLog)
        {
            // Delete from asset dependency list
            var assetDepnds = db.AssetDependencies.Where(ad => ad.ChildAssetId == assetId);
            if (assetDepnds != null)
            {
                foreach (var depend in assetDepnds)
                {
                    db.AssetDependencies.Remove(depend);
                }
            }

            // Commit
            db.SaveChanges();
        }

        private static void DeleteAssetList(int assetId, AssetManagerEntities db, ref List<string> errorLog)
        {
            // Delete from asset list
            var assetLists = db.AssetLists.Where(al => al.AssetId == assetId);
            if (assetLists != null)
            {
                foreach (var a in assetLists)
                {
                    db.AssetLists.Remove(a);
                }
            }

            // Commit
            db.SaveChanges();
        }

        private static void _DeleteAsset(int assetId, AssetManagerEntities db, ref List<string> errorLog)
        {
            // Delete from asset table
            var asset = db.Assets.Where(a => a.AssetId == assetId).FirstOrDefault();
            if (asset != null)
            {
                db.Assets.Remove(asset);
            }

            // Commit
            db.SaveChanges();
        }

        /// <summary>
        /// Ensures the file is found on the HDD, and is not a duplicate in the DB.
        /// </summary>
        protected static bool ValidateFile(string fileName, ref List<string> errorLog)
        {
            using (var db = new AssetManagerEntities())
            {
                // Check if file exist
                var fileInfo = new FileInfo(fileName);
                if (!fileInfo.Exists)
                {
                    errorLog.Add(fileName + ": does not exist (HDD)");
                    return false;
                }

                // Check duplicate names
                var dupFile = db.Assets.FirstOrDefault(a => a.FileName == fileInfo.Name);
                if (dupFile != null)
                {
                    errorLog.Add(fileInfo.FullName + ": File name already exist (DB)");
                    return false;
                }
                return true;
            }
        }
    }
}
