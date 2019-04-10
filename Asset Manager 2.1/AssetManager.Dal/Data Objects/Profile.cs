using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager.Common;
using AssetManager.Common.Enums;
using AssetManager.Dal.Models;

namespace AssetManager.Dal
{
    public partial class Profile
    {
        public static DbId Create(string name, string assetListSet)
        {
            using (var db = new AssetManagerEntities())
            {
                var profile = new Profile()
                {
                    Name = name,
                    AssetListSet = assetListSet
                };

                db.Profiles.Add(profile);

                if (db.SaveChanges() > 0)
                {
                    return profile.ProfileId;
                }

                return 0;
            }
        }

        public static List<Profile> GetAllProfiles()
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Profiles.ToList();
            }
        }

        public static DbId GetProfileId(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Profiles.Where(p => p.Name == profileName).Select(p => p.ProfileId).FirstOrDefault();
            }
        }

        public static Profile GetProfile(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Profiles.Where(p => p.Name == profileName).FirstOrDefault();
            }
        }

        public static List<AssetModel> GetAssets(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from al in db.AssetLists
                        join p in db.Profiles
                        on al.AssetListSet equals p.AssetListSet
                        join a in db.Assets
                        on al.AssetId equals a.AssetId
                        where p.Name == profileName
                        orderby a.AssetTypeId
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
        /// Returns a list of assets that do not have any children assigned to them.  Doesn't lookup materials, meshes, or fonts.
        /// </summary>
        /// <returns></returns>
        public static List<AssetModel> GetAssetsWithNoDependencies(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from al in db.AssetLists
                        join p in db.Profiles on al.AssetListSet equals p.AssetListSet
                        join a in db.Assets on al.AssetId equals a.AssetId
                        join ad in db.AssetDependencies on a.AssetId equals ad.ParentAssetId into tempTbl
                        from tbl in tempTbl.DefaultIfEmpty()
                        where p.Name == profileName &&
                              a.AssetTypeId != (int)AssetTypes.Material &&
                              a.AssetTypeId != (int)AssetTypes.Mesh &&
                              a.AssetTypeId != (int)AssetTypes.Font &&
                              tbl == null
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

        public static bool Delete(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                var query = (from p in db.Profiles where p.Name == profileName select p.ProfileId);
                if (query.Any())
                {
                    db.DeleteProfile(query.FirstOrDefault());
                    db.SaveChanges();
                }
                return true;
            }
        }

    }
}
