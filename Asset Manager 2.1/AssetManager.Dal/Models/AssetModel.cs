using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Dal.Models
{
    public class AssetModel
    {
        public int AssetId { get; set; }
        public int AssetTypeId { get; set; }
        public string FileName { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public Nullable<int> MeshTypeId { get; set; }
    }
}
