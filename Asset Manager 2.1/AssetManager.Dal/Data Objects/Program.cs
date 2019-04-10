using AssetManager.Common;
using AssetManager.Common.MaterialParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Dal
{
    public class Program : Asset
    {
        public static DbId CreateFromFile(string fileName, int categoryId, ref List<string> errorLog)
        {
            return new DbId(0);
        }

        public new static DbId Create(string content, int categoryId, ref List<string> errorLog)
        {
            return new DbId(0);
        }

        public static DbId Create(MaterialAttributes.ProgramAttributes programAttributes, int categoryId, ref List<string> errorLog)
        {
            return new DbId(0);
        }

        public static bool Update(DbId assetId, string content, ref string errorReport)
        {
            return true;
        }

        public static bool Update(DbId assetId, MaterialAttributes.ProgramAttributes programAttributes, ref string errorReport)
        {
            return true;
        }
    }
}
