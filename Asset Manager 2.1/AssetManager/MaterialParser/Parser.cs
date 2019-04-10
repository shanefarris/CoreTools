using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MaterialParser
{
    public class Parser
    {
        public MaterialAttributes Parse(string material, ref List<string> errorLog)
        {
            var materialAttributes = new MaterialAttributes();

            bool isFoundName = false;
            int nestedIndex = 0;

            var lines = Regex.Split(material, "\r\n");
            int i = 0;
            while (i < lines.Count())
            {
                string s = lines[i].Trim();

                // Check for comments
                if (s.StartsWith("//"))
                {
                    // Skip this line
                }
                // Check for material name
                else if (s.StartsWith("material ") && !isFoundName)
                {
                    materialAttributes.Name = s.Replace("material ", "").Trim();
                    isFoundName = true;
                }
                // Check for {
                else if (s == "{")
                {
                    nestedIndex++;
                }
                // Check for }
                else if (s == "}")
                {
                    nestedIndex--;
                }
                // Check for technique
                else if (s.StartsWith("technique") && nestedIndex > 0)
                {
                    var currentTechnique = ParseTechnique(lines, ref i, ref errorLog);
                    if (currentTechnique != null)
                        materialAttributes.Techniques.Add(currentTechnique);
                }
                // Gather external filename and put it in a list
                else
                {
                    var assetName = CheckMaterialLineForAsset(s);
                }
                i++;
            }
            return materialAttributes;
        }

        private Technique ParseTechnique(string[] lines, ref int index, ref List<string> errorLog)
        {
            var technique = new Technique();
            int nestedIndex = 0;
            for (; index < lines.Count(); index++)
            {
                string s = lines[index].Trim().ToLower();

                // Check for {
                if (s == "{")
                {
                    nestedIndex++;
                }
                // Check for }
                else if (s == "}")
                {
                    nestedIndex--;
                    if (nestedIndex == 0)
                        break;
                }
                // Check for pass
                else if (s.StartsWith("pass") && nestedIndex > 0)
                {
                    var pass = ParsePass(lines, ref index, ref errorLog);
                    if (pass != null)
                    {
                        technique.Passes.Add(pass);
                        nestedIndex--;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return technique;
        }

        private Pass ParsePass(string[] lines, ref int index, ref List<string> errorLog)
        {
            var pass = new Pass();
            int nestedIndex = 0;
            while (index < lines.Count())
            {
                string s = lines[index].Trim().ToLower();

                // Check for {
                if (s == "{")
                {
                    nestedIndex++;
                }
                // Check for }
                else if (s == "}")
                {
                    nestedIndex--;
                    if (nestedIndex == 0)
                        break;
                }
                // Check for ambient
                else if (s.StartsWith("ambient"))
                {
                    var ambient = Regex.Split(s, " ");
                    if (ambient.Count() == 5)
                    {
                        pass.Ambient = new Vector4D(float.Parse(ambient[1]), float.Parse(ambient[2]), float.Parse(ambient[3]), float.Parse(ambient[4]));
                    }
                    else if (ambient.Count() == 4)
                    {
                        pass.Ambient = new Vector4D(float.Parse(ambient[1]), float.Parse(ambient[2]), float.Parse(ambient[3]), 1.0f);
                    }
                    else
                    {
                        errorLog.Add("Invalid number of parameters for ambient.");
                        return null;
                    }
                }
                // Check for diffuse
                else if (s.StartsWith("diffuse"))
                {
                    var diffuse = Regex.Split(s, " ");
                    if (diffuse.Count() == 5)
                    {
                        pass.Diffuse = new Vector4D(float.Parse(diffuse[1]), float.Parse(diffuse[2]), float.Parse(diffuse[3]), float.Parse(diffuse[4]));
                    }
                    else if (diffuse.Count() == 4)
                    {
                        pass.Diffuse = new Vector4D(float.Parse(diffuse[1]), float.Parse(diffuse[2]), float.Parse(diffuse[3]), 1.0f);
                    }
                    else
                    {
                        errorLog.Add("Invalid number of parameters for diffused.");
                        return null;
                    }
                }
                // Check for specular
                else if (s.StartsWith("specular"))
                {
                    var specular = Regex.Split(s, " ");
                    if (specular.Count() == 6)
                    {
                        pass.Specular = new Vector5D(float.Parse(specular[1]), float.Parse(specular[2]), float.Parse(specular[3]), float.Parse(specular[4]), float.Parse(specular[5]));
                    }
                    else if (specular.Count() == 5)
                    {
                        pass.Specular = new Vector5D(float.Parse(specular[1]), float.Parse(specular[2]), float.Parse(specular[3]), float.Parse(specular[4]), 0.0f);
                    }
                    else
                    {
                        errorLog.Add("Invalid number of parameters for specular.");
                        return null;
                    }
                }
                // Check for scene_blend
                // Format1: scene_blend <add|modulate|alpha_blend|colour_blend>
                // Format2: scene_blend <src_factor> <dest_factor>
                else if (s.StartsWith("scene_blend"))
                {
                    var scene_blend = Regex.Split(s, " ");
                    if (scene_blend.Count() == 2)
                    {
                        if (scene_blend[1] == "alpha_blend")
                            pass.SceneBlending = SceneBlendType.SBT_TRANSPARENT_ALPHA;
                        else if (scene_blend[1] == "add")
                            pass.SceneBlending = SceneBlendType.SBT_ADD;
                        else if (scene_blend[1] == "modulate")
                            pass.SceneBlending = SceneBlendType.SBT_MODULATE;
                        else if (scene_blend[1] == "colour_blend")
                            pass.SceneBlending = SceneBlendType.SBT_TRANSPARENT_COLOUR;
                        else
                        {
                            errorLog.Add("Unsupported scene_blend parameter format 1.");
                            return null;
                        }
                    }
                    else if (scene_blend.Count() == 3)
                    {
                        if (scene_blend[1] == "one")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_ONE;
                        else if (scene_blend[1] == "zero")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_ZERO;
                        else if (scene_blend[1] == "dest_colour")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_DEST_COLOUR;
                        else if (scene_blend[1] == "src_colour")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_SOURCE_COLOUR;
                        else if (scene_blend[1] == "one_minus_dest_colour")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_ONE_MINUS_DEST_COLOUR;
                        else if (scene_blend[1] == "one_minus_src_colour")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_ONE_MINUS_SOURCE_COLOUR;
                        else if (scene_blend[1] == "dest_alpha")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_DEST_ALPHA;
                        else if (scene_blend[1] == "src_alpha")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_SOURCE_ALPHA;
                        else if (scene_blend[1] == "one_minus_dest_alpha")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_ONE_MINUS_DEST_ALPHA;
                        else if (scene_blend[1] == "one_minus_src_alpha")
                            pass.SceneBlendSource = SceneBlendFactor.SBF_ONE_MINUS_SOURCE_ALPHA;
                        else
                        {
                            errorLog.Add("Unsupported scene_blend parameter format 2.");
                            return null;
                        }

                        if (scene_blend[2] == "one")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_ONE;
                        else if (scene_blend[2] == "zero")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_ZERO;
                        else if (scene_blend[2] == "dest_colour")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_DEST_COLOUR;
                        else if (scene_blend[2] == "src_colour")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_SOURCE_COLOUR;
                        else if (scene_blend[2] == "one_minus_dest_colour")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_ONE_MINUS_DEST_COLOUR;
                        else if (scene_blend[2] == "one_minus_src_colour")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_ONE_MINUS_SOURCE_COLOUR;
                        else if (scene_blend[2] == "dest_alpha")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_DEST_ALPHA;
                        else if (scene_blend[2] == "src_alpha")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_SOURCE_ALPHA;
                        else if (scene_blend[2] == "one_minus_dest_alpha")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_ONE_MINUS_DEST_ALPHA;
                        else if (scene_blend[2] == "one_minus_src_alpha")
                            pass.SceneBlendDest = SceneBlendFactor.SBF_ONE_MINUS_SOURCE_ALPHA;
                        else
                        {
                            errorLog.Add("Unsupported scene_blend parameter format 2.");
                            return null;
                        }
                    }
                    else
                    {
                        errorLog.Add("Unsupported scene_blend parameter.");
                        return null;
                    }
                }
                // Check for depth_write
                else if (s.StartsWith("depth_write"))
                {
                    if (s.Replace("depth_write", "").Trim() == "on")
                        pass.IsDepthWriteEnabled = true;
                }
                // Check for alpha_rejection
                else if (s.StartsWith("alpha_rejection"))
                {
                    pass.AlphaRejection = new AlphaRejection();
                    var alphaRejection = Regex.Split(s, " ");
                    if (alphaRejection.Count() >= 3)
                    {
                        pass.AlphaRejection.Compare = ConvertCompareFunction(alphaRejection[1]);
                        if(pass.AlphaRejection.Compare == CompareFunction.NONE)
                        {
                            errorLog.Add("Unsupported compare function for alpha_rejection parameter.");
                            return null;
                        }

                        int value;
                        if (int.TryParse(alphaRejection[2], out value))
                        {
                            pass.AlphaRejection.Value = value;
                        }
                        else
                        {
                            errorLog.Add("Invalid value for alpha_rejection.");
                            return null;
                        }

                        if (alphaRejection.Count() == 4)
                            errorLog.Add("WARNING alpha_rejection only supports 2 parameters, the covert setting is always set to FALSE.");
                    }
                    else
                    {
                        errorLog.Add("Invalid number of parameters for alpha_rejection.");
                        return null;
                    }
                }
                // Check for emissive
                else if (s.StartsWith("emissive"))
                {
                    var emissive = Regex.Split(s, " ");
                    if (emissive.Count() == 5)
                    {
                        pass.Emissive = new Vector4D(float.Parse(emissive[1]), float.Parse(emissive[2]), float.Parse(emissive[3]), float.Parse(emissive[4]));
                    }
                    else if (emissive.Count() == 4)
                    {
                        pass.Emissive = new Vector4D(float.Parse(emissive[1]), float.Parse(emissive[2]), float.Parse(emissive[3]), 0.0f);
                    }
                    else
                    {
                        errorLog.Add("Invalid number of parameters for emissive.");
                        return null;
                    }
                }
                // Check for alpha_rejection
                else if (s.StartsWith("alpha_rejection"))
                {
                    errorLog.Add("alpha_rejection is unsupported.");
                    return null;
                }
                // Check for specular
                else if (s.StartsWith("texture_unit"))
                {
                    index++;
                    var passTextureUnit = new PassTextureUnit();

                    while (index < lines.Count())
                    {
                        s = lines[index].Trim().ToLower();

                        // texture <texturename> [<type>] [unlimited | numMipMaps] [alpha] [<PixelFormat>] [gamma]
                        if (s.StartsWith("texture") && nestedIndex > 1)
                        {
                            var texture = Regex.Split(s, " ");
                            if (texture.Count() == 2)
                            {
                                passTextureUnit.Texture = texture[1];
                            }
                            else if (texture.Count() == 3)
                            {
                                int numMipmap;
                                if (int.TryParse(texture[2], out numMipmap))
                                {
                                    passTextureUnit.Texture = texture[1];
                                    passTextureUnit.NumMipmaps = numMipmap;
                                }
                            }
                            else
                            {
                                errorLog.Add("Invalid number of parameters for texture.");
                                return null;
                            }
                        }
                        // filtering
                        // Format: filtering <none|bilinear|trilinear|anisotropic>
                        // Format: filtering <minification> <magnification> <mip>
                        else if (s.StartsWith("filtering") && nestedIndex > 1)
                        {
                            var filtering = Regex.Split(s, " ");
                            if (filtering.Count() == 2)
                            {
                                passTextureUnit.FilteringMin = filtering[1];
                                passTextureUnit.FilteringMax = filtering[1];
                                passTextureUnit.FilteringMip = filtering[1];
                            }
                            else if (filtering.Count() == 4)
                            {
                                passTextureUnit.FilteringMin = filtering[1];
                                passTextureUnit.FilteringMax = filtering[2];
                                passTextureUnit.FilteringMip = filtering[3];
                            }
                            else
                            {
                                errorLog.Add("Invalid number of parameters for filtering.");
                                return null;
                            }
                        }
                        else if (s.StartsWith("{"))
                        {
                            nestedIndex++;
                        }
                        else if (s.StartsWith("}"))
                        {
                            nestedIndex--;
                            index++;
                            pass.TextureUnits.Add(passTextureUnit);
                            break;
                        }
                        else
                        {
                            errorLog.Add("Unsupported texture_unit option.");
                            return null;
                        }
                        index++;
                    }

                    if (index >= lines.Count())
                    {
                        pass.TextureUnits.Add(passTextureUnit);
                    }

                }
                else if (s == "depth_func")
                {
                    errorLog.Add("depth_func is unsupported.");
                    return null;
                }
                else if (s == "pass")
                {
                }
                else if (s.StartsWith("vertex_program_ref"))
                {
                    errorLog.Add("Unsupported vertex_program_ref option.");
                    return null;
                }
                else if (s.StartsWith("fragment_program_ref"))
                {
                    errorLog.Add("Unsupported fragment_program_ref option.");
                    return null;
                }
                index++;
            }
            return pass;
        }

        private string CheckMaterialLineForAsset(string line)
        {
            var ret = string.Empty;
            if ((line.Contains("texture ") || line.Contains("source ") || line.Contains("cubic_texture ")) &&
                !(line.Contains("//") || line.Contains("src_texture")))
            {
                ret = line.Replace("\t", string.Empty).Trim();
                if (ret.Contains("cubic_texture "))
                {
                    ret = ret.Replace("cubic_texture ", string.Empty).Trim();
                    if (ret.Contains(" "))
                    {
                        ret = ret.Substring(0, ret.IndexOf(" "));
                        string extension = ret.Substring(ret.LastIndexOf("."));
                        string temp = ret.Replace(extension, "_fr") + extension;
                        //if (!externalFiles.Contains(temp))
                        //    externalFiles.Add(temp);

                        //temp = ret.Replace(extension, "_bk") + extension;
                        //if (!externalFiles.Contains(temp))
                        //    externalFiles.Add(temp);

                        //temp = ret.Replace(extension, "_up") + extension;
                        //if (!externalFiles.Contains(temp))
                        //    externalFiles.Add(temp);

                        //temp = ret.Replace(extension, "_dn") + extension;
                        //if (!externalFiles.Contains(temp))
                        //    externalFiles.Add(temp);

                        //temp = ret.Replace(extension, "_rt") + extension;
                        //if (!externalFiles.Contains(temp))
                        //    externalFiles.Add(temp);

                        //temp = ret.Replace(extension, "_lf") + extension;
                        //if (!externalFiles.Contains(temp))
                        //    externalFiles.Add(temp);

                        // Clear the ret string because we don't want to add the "base file name"
                        ret = string.Empty;
                    }
                }
                else if (ret.Contains("texture "))
                {
                    ret = ret.Replace("texture ", string.Empty).Trim();
                    if (ret.Contains(" "))
                        ret = ret.Substring(0, ret.IndexOf(" "));
                }
                else if (ret.Contains("source "))
                {
                    ret = ret.Replace("source ", string.Empty).Trim();
                }
            }
            return ret;
        }

        private CompareFunction ConvertCompareFunction(String function)
        {
            function = function.ToLower();
            if (function == "always_fail")
                return CompareFunction.CMPF_ALWAYS_FAIL;
            else if (function == "always_pass")
                return CompareFunction.CMPF_ALWAYS_PASS;
            else if (function == "less")
                return CompareFunction.CMPF_LESS;
            else if (function == "less_equal")
                return CompareFunction.CMPF_LESS_EQUAL;
            else if (function == "equal")
                return CompareFunction.CMPF_EQUAL;
            else if (function == "not_equal")
                return CompareFunction.CMPF_NOT_EQUAL;
            else if (function == "greater_equal")
                return CompareFunction.CMPF_GREATER_EQUAL;
            else if (function == "greater")
                return CompareFunction.CMPF_GREATER;
            return CompareFunction.NONE;
        }
    }
}
