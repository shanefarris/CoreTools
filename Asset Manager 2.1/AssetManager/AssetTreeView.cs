using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using AssetManager.Common.Enums;
using AssetManager.Dal;
using AssetManager.Dal.Models;

namespace AssetManager
{
    public class AssetTreeView
    {
        private delegate void AddSub(TreeNode parent, TreeNode child);
        private delegate void Add(TreeView treeView, TreeNode node);
        private delegate void Clear(TreeView treeView);
        private delegate void Expand(TreeView treeView);

        private void AddSubNode(TreeNode parent, TreeNode child)
        {
            parent.Nodes.Add(child);
        }

        private void AddNode(TreeView treeView, TreeNode node)
        {
            treeView.Nodes.Add(node);
        }

        private void ClearNode(TreeView treeView)
        {
            treeView.Nodes.Clear();
        }

        private void ExpandAll(TreeView treeView)
        {
            treeView.ExpandAll();
        }

        private TreeNode PopulateParentNode(TreeView treeView, AssetModel asset, bool includeFileName = false)
        {
            TreeNode node = null;
            var assetType = AssetType.GetAssetType((AssetTypes)asset.AssetTypeId);
            if (includeFileName)
            {
                node = new TreeNode(asset.Name + " (" + asset.FileName + ")");
            }
            else
            {
                node = new TreeNode(asset.Name);
            }
            node.Tag = asset.AssetId;
            node.ToolTipText = "File: " + asset.FileName +
                "\r\nType: " + assetType.Description +
                "\r\nCategory: " + Category.GetCategoryName(asset.CategoryId);
            node.ForeColor = GetNameColor(assetType.Extension);
            node.Name = asset.AssetId.ToString();
            treeView.Invoke(new Add(AddNode), new object[] { treeView, node });
            return node;
        }

        private TreeNode PopulateChildNode(TreeView treeView, TreeNode parentNode, AssetModel asset)
        {
            var assetType = AssetType.GetAssetType((AssetTypes)asset.AssetTypeId);
            var node = new TreeNode(asset.Name);
            node.Tag = asset.AssetId;
            node.ToolTipText = "File: " + asset.FileName +
                "\r\nType: " + assetType.Description +
                "\r\nCategory: " + Category.GetCategoryName(asset.CategoryId);
            node.ForeColor = GetNameColor(assetType.Extension);
            node.Name = asset.AssetId.ToString();
            treeView.Invoke(new AddSub(AddSubNode), new object[] { parentNode, node });
            return node;
        }

        #region Public Methods

        private Color GetNameColor(string extension)
        {
            if (extension == "mesh")
                return Color.Brown;
            if (extension == "material")
                return Color.Blue;
            if (extension == "ogg")
                return Color.DarkGreen;
            if (extension == "zip")
                return Color.Purple;
            if (extension == "ttf")
                return Color.DarkBlue;
            if (extension == "particle")
                return Color.Black;
            if (Common.Utility.IsImageExtension(extension))
                return Color.Crimson;
            if (extension == "vert" ||
                extension == "cg" ||
                extension == "program" ||
                extension == "source" ||
                extension == "ogreode" ||
                extension == "fontdef" ||
                extension == "overlay" ||
                extension == "compositor")
                return Color.Black;

            return Color.Orange;
        }

        public void PopulateWithMeshes(TreeView treeView, List<AssetModel> meshes, int? categoryId)
        {
            // List all mesh first
            if (meshes == null && categoryId == null)
            {
                meshes = Asset.GetRootAssets(AssetTypes.Mesh);
            }
            else if (meshes == null && categoryId != null)
            {
                meshes = Asset.GetRootAssets(AssetTypes.Mesh, categoryId.Value);
            }

            // DB could be empty
            if (meshes == null)
            {
                return;
            }

            foreach (var mesh in meshes)
            {
                var parentNode = PopulateParentNode(treeView, mesh);

                // List all dependencies that are not materials
                var dependencies = AssetDependency.GetAllAssetDependencies(mesh.AssetId);
                if (dependencies != null)
                {
                    foreach (var depend in dependencies.Where(d => d.AssetTypeId != (int)AssetTypes.Material))
                    {
                        PopulateChildNode(treeView, parentNode, depend);
                    }
                }

                // List all materials and their dependencies
                var materialDependencies = AssetDependency.GetAllAssetDependencies(mesh.AssetId).Where(m => m.AssetTypeId == (int)AssetTypes.Material);
                foreach (var mat in materialDependencies)
                {
                    parentNode = PopulateChildNode(treeView, parentNode, mat);

                    // List all material dependencies
                    var assetDependencies = AssetDependency.GetAllAssetDependencies(mat.AssetId);
                    foreach (var a in assetDependencies)
                    {
                        PopulateChildNode(treeView, parentNode, a);
                    }
                }
            }
        }

        public void PopulateWithParentMaterials(TreeView treeView, List<AssetModel> materials, int? categoryId)
        {
            // List all materials and their dependencies that are not tied to meshes
            if (materials == null)
            {
                if (categoryId != null)
                {
                    materials = Material.GetRootMaterials(categoryId.Value);
                }
                else
                {
                    materials = Material.GetRootMaterials();
                }
            }

            foreach (var mat in materials)
            {
                var parentNode = PopulateParentNode(treeView, mat, includeFileName: true);

                // List all material dependencies
                var dependencies = Asset.GetDependencies(mat.AssetId);
                foreach (var a in dependencies)
                {
                    PopulateChildNode(treeView, parentNode, a);
                }
            }
        }

        public void PopulateFontdefs(TreeView treeView, List<AssetModel> fontdefs, int? categoryId)
        {
            // List all materials and their dependencies that are not tied to meshes
            if (fontdefs == null)
            {
                if (categoryId == null)
                {
                    fontdefs = Asset.GetAssets(AssetTypes.Font);
                }
                else
                {
                    fontdefs = Asset.GetAssets(AssetTypes.Font, categoryId.Value);
                }
            }

            foreach (var font in fontdefs)
            {
                var parentNode = PopulateParentNode(treeView, font);

                // List all font dependencies
                foreach (var a in Asset.GetDependencies(font.AssetId))
                {
                    PopulateChildNode(treeView, parentNode, a);
                }
            }
        }

        public void PopulateWithParentOtherAssets(TreeView treeView, List<AssetModel> others, int? categoryId)
        {
            // List all assets that are not dependencies
            if (others == null)
            {
                others = Asset.GetAssetsWithNoDependencies(categoryId);
            }

            foreach (var a in others.Where(a => a.AssetTypeId != (int)AssetTypes.Mesh && a.AssetTypeId != (int)AssetTypes.Material))
            {
                PopulateParentNode(treeView, a);
            }
        }

        public void PopulateTreeAll(TreeView treeView)
        {
            treeView.Invoke(new Clear(ClearNode), new object[] { treeView });

            PopulateWithMeshes(treeView, null, null);
            PopulateFontdefs(treeView, null, null);
            PopulateWithParentMaterials(treeView, null, null);
            PopulateWithParentOtherAssets(treeView, null, null);

            // Expand the tree view
            if (treeView.Nodes.Count < 20)
                treeView.Invoke(new Expand(ExpandAll), new object[] { treeView });
        }

        public void PopulateTreeByCategory(TreeView treeView)
        {
            treeView.Invoke(new Clear(ClearNode), new object[] { treeView });

            foreach (var id in Category.GetAllCategories().Select(c => c.CategoryId))
            {
                PopulateWithMeshes(treeView, null, id);
                PopulateFontdefs(treeView, null, id);
                PopulateWithParentMaterials(treeView, null, id);
                PopulateWithParentOtherAssets(treeView, null, id);
            }

            // Expand the tree view
            if (treeView.Nodes.Count < 20)
                treeView.Invoke(new Expand(ExpandAll), new object[] { treeView });
        }

        public void PopulateParentMeshAssets(TreeView treeView)
        {
            foreach (var mesh in Asset.GetAssets(AssetTypes.Mesh))
            {
                PopulateParentNode(treeView, mesh);
            }
        }

        #endregion

    }
}
