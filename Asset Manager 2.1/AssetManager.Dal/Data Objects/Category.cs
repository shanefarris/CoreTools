using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager.Common;

namespace AssetManager.Dal
{
    public partial class Category
    {
        public static DbId Create(string name, int? parentCategoryId)
        {
            using (var db = new AssetManagerEntities())
            {
                var category = new Category()
                {
                    Name = name,
                    ParentCategoryId = parentCategoryId
                };

                db.Categories.Add(category);

                if (db.SaveChanges() > 0)
                {
                    return category.CategoryId;
                }

                return 0;
            }
        }

        public static Category GetCategory(string categoryName)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
            }
        }

        public static string GetCategoryName(int categoryId)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Categories.Where(c => c.CategoryId == categoryId).Select(c => c.Name).FirstOrDefault();
            }
        }

        public static DbId GetCategoryId(string categoryName)
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Categories.First(c => c.Name == categoryName).CategoryId;
            }
        }

        public static List<Category> GetAllCategories()
        {
            using (var db = new AssetManagerEntities())
            {
                return db.Categories.ToList();
            }
        }
    }
}
