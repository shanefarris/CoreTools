using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using AssetManager.Common.MaterialParser;

namespace AssetManager.Code_Generators
{
    class MaterialGenerator
    {
        public void CreateSource(string fileName, string outputDir, List<MaterialAttributes> materialAttributesList, bool enableScripting)
        {
            using (var streamWriter = new StreamWriter(outputDir + "\\" + fileName))
            {
                streamWriter.WriteLine("#include \"OgreMaterialManager.h\"");
                streamWriter.WriteLine("#include \"OgreTechnique.h\"");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using namespace Ogre;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("void LoadMaterials()");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("MaterialPtr mat;");
                streamWriter.WriteLine("Pass* pass = nullptr;");
                streamWriter.WriteLine("Technique* tech = nullptr;");
                streamWriter.WriteLine("TextureUnitState* tus = nullptr;");

                foreach (var ma in materialAttributesList)
                {
                    string receiveShadows = ma.IsReceiveShadows == true ? "true" : "false";
                    string transparencyCastsShadows = ma.TransparencyCastsShadows == true ? "true" : "false";

                    streamWriter.WriteLine("// ============================ " + ma.Name + " ============================================");
                    streamWriter.WriteLine("mat = MaterialManager::getSingleton().create(\"" + ma.Name + "\", ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);");
                    streamWriter.WriteLine("mat->setReceiveShadows(" + receiveShadows + ");");
                    streamWriter.WriteLine("mat->setTransparencyCastsShadows(" + transparencyCastsShadows + ");");

                    if (ma.Techniques != null)
                    {
                        AddTechnique(streamWriter, ma);
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("}");

                streamWriter.Close();
            }
        }

        private void AddTechnique(StreamWriter streamWriter, MaterialAttributes materialAttributes)
        {
            int counter = 0;
            string lightingEnabled = string.Empty;
            foreach (var t in materialAttributes.Techniques)
            {
                if (counter > 0)
                {
                    streamWriter.WriteLine("tech = mat->createTechnique();");
                }

                lightingEnabled = t.IsLightingEnabled == true ? "true" : "false";
                streamWriter.WriteLine("mat->getTechnique(" + counter.ToString() + ")->setLightingEnabled(" + lightingEnabled + ");");
                streamWriter.WriteLine("mat->getTechnique(" + counter.ToString() + ")->setSchemeName(\"" + t.Scheme + "\");");

                if (t.LoDLevel != null)
                {
                    streamWriter.WriteLine("mat->getTechnique(" + counter.ToString() + ")->setLodIndex(" + t.LoDLevel.Value.ToString() + ");");
                }

                if (t.IsLightingEnabled != null)
                {
                    streamWriter.WriteLine("mat->getTechnique(" + counter.ToString() + ")->setLightingEnabled(" + t.IsLightingEnabled.Value.ToString() + ");");
                }

                if (t.Passes != null)
                {
                    AddPass(streamWriter, t, counter);
                }

                counter++;
            }
        }

        private void AddPass(StreamWriter streamWriter, MaterialAttributes.Technique technique, int techniqueIndex)
        {
            int counter = 0;
            foreach (var p in technique.Passes)
            {
                string color = string.Empty;

                if (counter > 0)
                {
                    streamWriter.WriteLine("pass = mat->getTechnique(" + techniqueIndex.ToString() + ")->createPass();");
                }

                // Name
                if(p.Name != null)
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setName(ColourValue(" + p.Name + "));");

                // Diffuse
                if (p.Diffuse != null)
                {
                    color = p.Diffuse.x.ToString("F2") + "f, " + p.Diffuse.y.ToString("F2") + "f, " + p.Diffuse.z.ToString("F2") + "f, " + p.Diffuse.w.ToString("F2") + "f";
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setDiffuse(ColourValue(" + color + "));");
                }

                // Ambient
                if (p.Ambient != null)
                {
                    color = p.Ambient.x.ToString("F2") + "f, " + p.Ambient.y.ToString("F2") + "f, " + p.Ambient.z.ToString("F2") + "f, " + p.Ambient.w.ToString("F2") + "f";
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setAmbient(ColourValue(" + color + "));");
                }

                // Self Illumination
                if (p.SelfIllumination != null)
                {
                    color = p.SelfIllumination.x.ToString("F2") + "f, " + p.SelfIllumination.y.ToString("F2") + "f, " + p.SelfIllumination.z.ToString("F2") + "f, " + p.SelfIllumination.w.ToString("F2") + "f";
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setSelfIllumination(ColourValue(" + color + "));");
                }

                // Specular
                if (p.Specular != null)
                {
                    color = p.Specular.x.ToString("F2") + "f, " + p.Specular.y.ToString("F2") + "f, " + p.Specular.z.ToString("F2") + "f, " + p.Specular.w.ToString("F2") + "f";
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setSpecular(ColourValue(" + color + "));");
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setShininess(" + p.Specular.a.ToString("F4") + "f);");
                }

                // Is Depth Write Enabled
                if (p.IsDepthWriteEnabled != null)
                {
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setSpecular(ColourValue(" + p.IsDepthWriteEnabled.Value.ToString() + "));");
                }

                // Scene Blending
                if (p.SceneBlending != null)
                {
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + counter.ToString() + ")->setSceneBlending(" + p.SceneBlending.Value.ToString() + ");");
                }
                else if (p.SceneBlendSource != null && p.SceneBlendDest != null)
                {
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" +
                        counter.ToString() + ")->setSceneBlending(" + p.SceneBlendSource.Value.ToString() + ", " + p.SceneBlendDest.Value.ToString() + ");");
                }

                // Alpha Reject
                if (p.AlphaRej != null)
                {
                    streamWriter.WriteLine("mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" +
                        counter.ToString() + ")->setAlphaRejectSettings(CompareFunction::" + p.AlphaRej.Compare.ToString() + ", " + p.AlphaRej.Value + ");");
                }

                // Texture units
                foreach (var tu in p.TextureUnits)
                {
                    AddTextureUnit(streamWriter, tu, techniqueIndex, counter);
                }

                counter++;
            }
        }

        private void AddTextureUnit(StreamWriter streamWriter, MaterialAttributes.Technique.Pass.PassTextureUnit textureUnit, int techniqueIndex, int passIndex)
        {
            streamWriter.WriteLine("tus = mat->getTechnique(" + techniqueIndex.ToString() + ")->getPass(" + passIndex.ToString() + ")->createTextureUnitState();");

            // Texture name
            if (textureUnit.Texture != null)
            {
                streamWriter.WriteLine("tus->setTextureName(\"" + textureUnit.Texture + "\");");

            }

            // Number of Mipmaps
            if (textureUnit.NumMipmaps != null)
            {
                streamWriter.WriteLine("tus->setNumMipmaps(" + textureUnit.NumMipmaps + ");");
            }

            // Filtering
            if (textureUnit.FilteringMin != null && textureUnit.FilteringMax != null && textureUnit.FilteringMip != null)
            {
                string min, max, mip;
                if (textureUnit.FilteringMin.ToLower() == "none")
                    min = "FilterOptions::FO_NONE";
                else if (textureUnit.FilteringMin.ToLower() == "point")
                    min = "FilterOptions::FO_POINT";
                else if (textureUnit.FilteringMin.ToLower() == "linear")
                    min = "FilterOptions::FO_LINEAR";
                else
                    min = "FilterOptions::FO_ANISOTROPIC";

                if (textureUnit.FilteringMax.ToLower() == "none")
                    max = "FilterOptions::FO_NONE";
                else if (textureUnit.FilteringMax.ToLower() == "point")
                    max = "FilterOptions::FO_POINT";
                else if (textureUnit.FilteringMax.ToLower() == "linear")
                    max = "FilterOptions::FO_LINEAR";
                else
                    max = "FilterOptions::FO_ANISOTROPIC";

                if (textureUnit.FilteringMip.ToLower() == "none")
                    mip = "FilterOptions::FO_NONE";
                else if (textureUnit.FilteringMip.ToLower() == "point")
                    mip = "FilterOptions::FO_POINT";
                else if (textureUnit.FilteringMip.ToLower() == "linear")
                    mip = "FilterOptions::FO_LINEAR";
                else
                    mip = "FilterOptions::FO_ANISOTROPIC";

                streamWriter.WriteLine("tus->setTextureFiltering(" + min + ", " + max + ", " + mip + ");");
            }
        }
    }
}
