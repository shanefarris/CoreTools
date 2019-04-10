using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager.Common;

namespace AssetManager.Dal
{
    public partial class AssetList
    {
        public static DbId Create(int assetId, string assetListSet)
        {
            using (var db = new AssetManagerEntities())
            {
                var assetList = new AssetList()
                {
                    AssetId = assetId,
                    AssetListSet = assetListSet
                };

                db.AssetLists.Add(assetList);

                if (db.SaveChanges() > 0)
                {
                    return assetList.AssetListId;
                }

                return 0;
            }
        }
    }
}
