using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using AssetManager.Common.Enums;
using AssetManager.Code_Generators;
using AssetManager.Common.FontParser;
using AssetManager.Common;
using AssetManager.Dal;
using AssetManager.Common.MaterialParser;

namespace AssetManager
{
    public class CodeGenerator
    {
        private GameObjectGenerator _gameObjectGenerator = new GameObjectGenerator();
        private CharacterGenerator _characterGenerator = new CharacterGenerator();
        private BuildingGenerator _buildingGenerator = new BuildingGenerator();

        #region Public

        public bool GenerateCode(string profileName, string outputDir, bool enableScripting, bool codeMaterials, bool codeFonts, TextBox txtReport)
        {
            // If the output directory doesn't exsit, create it
            var gameObjectDir = outputDir + "\\Game Objects";
            var characterDir = outputDir + "\\Characters";
            var buildingDir = outputDir + "\\Buildings";

            var outputInfo = new DirectoryInfo(gameObjectDir);
            if (!outputInfo.Exists)
                outputInfo.Create();
            outputInfo = new DirectoryInfo(characterDir);
            if (!outputInfo.Exists)
                outputInfo.Create();
            outputInfo = new DirectoryInfo(buildingDir);
            if (!outputInfo.Exists)
                outputInfo.Create();

            var allFilesList = new List<string>();
            var gameObjectList = new List<string>();
            var characterList = new List<string>();
            var buildingList = new List<string>();
            var successfulMaterialNames = new List<string>();

            // Create materials first
            if (codeMaterials)
            {
                CreateGOPMaterialCode(profileName, outputDir, txtReport, ref successfulMaterialNames);

                // Create a new set of material files from the materials that could not be coded successfully
                var resourceHelper = new ResourceHelper();
                var fileList = GetUnsuccessfulMaterials(profileName, successfulMaterialNames);
                var tempDir = outputDir + "\\Materials";

                outputInfo = new DirectoryInfo(tempDir);
                if (!outputInfo.Exists)
                    outputInfo.Create();

                // Merge the materials file into one
                if (fileList.Count > 0)
                {
                    dynamic imageAssetList = Image.GetImages(profileName);
                    CreateMaterialFile(tempDir, fileList, imageAssetList);
                }
            }

            // Create fonts
            if (codeFonts)
            {
                CreateGOPFontCode(profileName, outputDir, txtReport);
            }

            // Get the list of assets in the profile
            foreach (var asset in Mesh.GetMeshes(profileName))
            {
                allFilesList.Add(asset.FileName);

                // Material name lookup
                string materialName = null;
                if (codeMaterials)
                {
                    materialName = Mesh.GetMaterialName(asset.AssetId);

                    // If the material was not successfully coded, don't manually add it to the entity
                    if (!successfulMaterialNames.Contains(materialName))
                    {
                        materialName = null;
                    }
                }

                // Create the mesh code
                if (asset.MeshTypeId == (int)MeshTypes.GameObject)
                {
                    _gameObjectGenerator.CreateHeader(asset.FileName, gameObjectDir);
                    _gameObjectGenerator.CreateSource(asset.FileName, gameObjectDir, enableScripting, materialName);
                    gameObjectList.Add(asset.FileName);
                }
                else if (asset.MeshTypeId == (int)MeshTypes.Character)
                {
                    _characterGenerator.CreateHeader(asset.FileName, characterDir);
                    _characterGenerator.CreateSource(asset.FileName, characterDir, materialName);
                    characterList.Add(asset.FileName);
                }
                else
                {
                    _buildingGenerator.CreateHeader(asset.FileName, buildingDir);
                    _buildingGenerator.CreateSource(asset.FileName, buildingDir, materialName);
                    buildingList.Add(asset.FileName);
                }
            }

            // Exporter code
            _gameObjectGenerator.CreateFactoryExportHeader(gameObjectDir);
            _gameObjectGenerator.CreateFactoryExportSource(gameObjectList, gameObjectDir);

            _characterGenerator.CreateFactoryExportHeader(characterDir);
            _characterGenerator.CreateFactoryExportSource(characterList, characterDir);

            _buildingGenerator.CreateFactoryExportHeader(buildingDir);
            _buildingGenerator.CreateFactoryExportSource(buildingList, buildingDir);

            return true;
        }

        public void CreateUnitTests(string profileName, string outputDir)
        {
            _gameObjectGenerator.CreateUnitTests(profileName, outputDir);
        }

        #endregion // Public

        #region Private

        private bool CreateGOPMaterialCode(string profileName, string outputDir, TextBox txtReport, ref List<string> successfulMaterialNames)
        {
            var materialGenerator = new MaterialGenerator();
            var materialAttributesList = new List<MaterialAttributes>();

            var errorLog = new List<string>();
            var parser = new AssetManager.Common.MaterialParser.Parser(isForExportOperation: true);

            foreach (var m in Material.GetAssetsInProfile(AssetTypes.Material, profileName))
            {
                txtReport.Text += "Status: Starting on " + m.Name + "\r\n";

                var data = Asset.GetAssetData(m.AssetId);
                var content = Common.Utility.ConvertBytesToString(data);
                var materialAttributes = parser.Parse(content, ref errorLog);
                if (errorLog.Count == 0)
                {
                    txtReport.Text += "Status: " + materialAttributes.Name + " Done.\r\n";

                    // Check for duplicate material names before adding it to the collection
                    bool found = false;

                    // If this is not a program then make sure its not a duplicate
                    if (materialAttributes.Program != null)
                    {
                        foreach (var ma in materialAttributesList)
                        {
                            if (ma.Name == materialAttributes.Name)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        materialAttributesList.Add(materialAttributes);
                        successfulMaterialNames.Add(materialAttributes.Name.ToLower());     // Lower case because it looks up the DB name which is lower case.
                    }
                }
                else
                {
                    errorLog.ForEach(s => txtReport.Text += "Error: " + s + "\r\n");
                    errorLog.Clear();
                }
            }

            materialGenerator.CreateSource("zzzMaterials.cpp", outputDir, materialAttributesList, false);
            return true;
        }

        private bool CreateGOPFontCode(string profileName, string outputDir, TextBox txtReport)
        {
            var fontGenerator = new FontGenerator();
            var fontAttributesList = new List<FontAttributes>();

            var errorLog = new List<string>();
            var parser = new AssetManager.Common.FontParser.Parser();

            foreach (var f in Font.GetFontdef(profileName))
            {
                txtReport.Text += "Status: Starting on " + f.Name + "\r\n";

                var data = Asset.GetAssetData(f.AssetId);
                var content = Common.Utility.ConvertBytesToString(data);
                var fontAttributes = parser.Parse(content, ref errorLog);
                if (errorLog.Count == 0)
                {
                    txtReport.Text += "Status: " + fontAttributes.Name + " Done.\r\n";
                    fontAttributesList.Add(fontAttributes);
                }
                else
                {
                    errorLog.ForEach(s => txtReport.Text += "Error: " + s + "\r\n");
                    errorLog.Clear();
                }
            }

            fontGenerator.CreateSource("zzzFonts.cpp", outputDir, fontAttributesList);
            return true;
        }

        private List<int> GetUnsuccessfulMaterials(string profile, List<string> successfulMaterials)
        {
            var materialList = new List<int>();
            foreach (var asset in Material.GetAssetsInProfile(AssetTypes.Material, profile))
            {
                if (!successfulMaterials.Contains(asset.Name))
                {
                    materialList.Add(asset.AssetId);
                }
            }

            return materialList;
        }

        private bool CreateMaterialFile(string outDir, List<int> fileList, dynamic profileAssetList)
        {
            string fileName = outDir + "\\Materials.material";
            if (File.Exists(fileName))
                File.Delete(fileName);

            var replacedFileList = new List<string>();
            foreach (var asset in profileAssetList)
            {
                replacedFileList.Add(asset.FileName);
            }

            using (var fs = File.Create(fileName))
            {
                foreach (var id in fileList)
                {
                    var asset = Asset.GetAsset(id);
                    if (asset != null)
                    {
                        // Check to see if we need to change the image file name
                        byte[] data = null;
                        if (Common.Utility.GetExportImageExtension() != ".none")
                        {
                            var matData = Asset.GetAssetData(asset.AssetId);
                            data = MaterialAsset.ReplaceImageNameForMaterial(matData, replacedFileList);
                        }

                        // Copy the content of this file
                        fs.Write(data, 0, data.Length);
                    }
                }
            }

            return true;
        }

        #endregion // Private
    }
}
