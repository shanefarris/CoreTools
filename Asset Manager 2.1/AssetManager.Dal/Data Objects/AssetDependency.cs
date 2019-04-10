using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager.Common;
using AssetManager.Dal.Models;

namespace AssetManager.Dal
{
    public partial class AssetDependency
    {
        /// <summary>
        /// Adds an entry for the given parent Id given a list of child dependencies.  Does not add dependencies recursively.
        /// </summary>
        /// <param name="parentAssetId"></param>
        /// <returns></returns>
        public static bool SaveDependencies(DbId parentAssetId, List<DbId> dependencies)
        {
            using (var db = new AssetManagerEntities())
            {
                foreach (var childId in dependencies)
                {
                    db.AssetDependencies.Add(new AssetDependency()
                    {
                        ChildAssetId = childId,
                        ParentAssetId = parentAssetId
                    });
                }
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Gets all dependencies of a given parent asset, but not the depencies of those assets that are returned.
        /// </summary>
        public static List<AssetModel> GetAllAssetDependencies(DbId parentAssetId)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from ad in db.AssetDependencies
                       join a in db.Assets
                       on ad.ChildAssetId equals a.AssetId
                       where ad.ParentAssetId == (int)parentAssetId
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
        /// Delete all entries in the asset dependency table with the given parent Id.  Does not delete the child assets for the asset table.
        /// </summary>
        /// <param name="parentAssetId"></param>
        /// <returns></returns>
        public static bool DeleteDependencies(int parentAssetId)
        {
            using (var db = new AssetManagerEntities())
            {
                foreach (var child in db.AssetDependencies.Where(ad => ad.ParentAssetId == parentAssetId))
                {
                    db.AssetDependencies.Remove(child);
                }
                db.SaveChanges();
                return true;
            }
        }
    }
}
