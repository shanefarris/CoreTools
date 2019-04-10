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
    public class Font : Asset
    {
        protected string SourceName { get; set; }
        protected int SourceId { get; set; }

        public new static DbId Create(string fileName, int categoryId, ref List<string> errorLog)
        {
            using (var db = new AssetManagerEntities())
            {
                if (!ValidateFile(fileName, ref errorLog))
                    return null;

                // Is this in the list of asset types
                var fileInfo = new FileInfo(fileName);
                var lines = File.ReadAllLines(fileName);
                var fonts = SortFonts(lines);

                foreach (var font in fonts)
                {
                    if (ValidateFont(fileName, font, ref errorLog))
                    {
                        // Check duplicate names
                        var dupFile = db.Assets.Select(a => a.FileName).FirstOrDefault(a => a == fileInfo.Name);
                        if (dupFile != null)
                        {
                            errorLog.Add("File name already exist: " + fileInfo.FullName);
                        }
                        else
                        {
                            // Add font asset
                            var asset = new Asset()
                            {
                                FileName = fileInfo.Name,
                                Name = font.Name,
                                Data = font.Data,
                                AssetTypeId = (int)AssetTypes.Font,
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
                            }
                            catch (Exception e)
                            {
                                errorLog.Add(fileInfo.FullName + " exception: " + e.Message);
                                return null;
                            }

                            // Add dependency to asset dependecy table
                            var depend = new AssetDependency()
                            {
                                ParentAssetId = asset.AssetId,
                                ChildAssetId = font.SourceId
                            };

                            db.AssetDependencies.Add(depend);

                            if (db.SaveChanges() == 0)
                            {
                                errorLog.Add("Error saving dependency.");
                                return null;
                            }

                            return asset.AssetId;
                        }
                    }
                    else
                    {
                        errorLog.Add("Unable to validate font: " + font.Name);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns a collection of font assets that are assigned to a given profile.
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static List<AssetModel> GetFontdef(string profileName)
        {
            using (var db = new AssetManagerEntities())
            {
                return (from al in db.AssetLists
                        join p in db.Profiles
                            on al.AssetListSet equals p.AssetListSet
                        join a in db.Assets
                            on al.AssetId equals a.AssetId
                        where p.Name == profileName &&
                              a.AssetTypeId == (int)AssetTypes.Font
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

        private static List<Font> SortFonts(string[] lines)
        {
            var fonts = new List<Font>();
            var name = string.Empty;
            var source = string.Empty;
            var data = string.Empty;
            var id = 0;

            // Iterate through each line
            foreach (string s in lines)
            {
                data += s + "\r\n";
                if (s.Trim() == "{")
                {
                    // Skip line
                }
                else if (s.Trim().StartsWith("//"))
                {
                    // Skip line
                }
                else if (s.Trim() == "}")
                {
                    fonts.Add(new Font()
                    {
                        Name = name,
                        SourceName = source,
                        Data = Common.Utility.ConvertStringToBytes(data),
                        SourceId = id
                    });

                    name = string.Empty;
                    source = string.Empty;
                    data = string.Empty;
                    id = 0;
                }
                else if (s.Trim().ToLower().Contains("source"))
                {
                    string temp = s.Trim();
                    source = temp.Substring(s.IndexOf("source") + 6).Trim();
                    id = Asset.GetAssetId(source);
                }
                else if (s.Trim().Length > 0 && name.Length == 0)
                {
                    name = s.Trim();
                }
            }

            return fonts;
        }

        private static bool ValidateFont(string fileName, Font font, ref List<string> errorLog)
        {
            var ret = true;

            using (var db = new AssetManagerEntities())
            {
                // Check for the existance of the files in the external file
                var asset = (from a in db.Assets where a.AssetId == font.SourceId select new { a.AssetId }).FirstOrDefault();
                if (asset == null)
                {
                    errorLog.Add(fileName + " missing file: " + font.SourceName);
                    ret = false;
                }
            }
            return ret;
        }
    }
}
