using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager.Common.Enums;
using AssetManager.Dal.Models;

namespace AssetManager.Dal
{
    public class Image : Asset
    {
        public static List<AssetModel> GetImages(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from al in db.AssetLists
                        join p in db.Profiles
                        on al.AssetListSet equals p.AssetListSet
                        join a in db.Assets
                        on al.AssetId equals a.AssetId
                        where p.Name == profileName &&
                        (a.AssetTypeId == (int)AssetTypes.BitMap ||
                        a.AssetTypeId == (int)AssetTypes.DDS ||
                        a.AssetTypeId == (int)AssetTypes.JPG ||
                        a.AssetTypeId == (int)AssetTypes.PNG ||
                        a.AssetTypeId == (int)AssetTypes.PSD ||
                        a.AssetTypeId == (int)AssetTypes.TGA ||
                        a.AssetTypeId == (int)AssetTypes.TIF)
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
    }
}
