using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager.Common.Enums;

namespace AssetManager.Dal
{
    public partial class AssetType
    {
        public static AssetType GetAssetType(AssetTypes assetType)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.AssetTypes.Where(t => t.AssetTypeId == (int)assetType).Select(t => t).FirstOrDefault();
            }
        }

        public static AssetType GetAssetType(string description)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.AssetTypes.Where(t => t.Description == description).FirstOrDefault();
            }
        }

        public static string GetAssetTypeExtension(AssetTypes assetType)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.AssetTypes.Where(t => t.AssetTypeId == (int)assetType).Select(t => t.Extension).FirstOrDefault();
            }
        }

        public static List<AssetType> GetAllAssetTypes()
        {
            using (var db = new AssetManagerEntities())
            {
                return db.AssetTypes.ToList();
            }
        }
    }
}
