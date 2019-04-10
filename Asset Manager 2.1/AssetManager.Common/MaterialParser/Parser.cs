using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AssetManager.Common.MaterialParser
{
    public class Parser
    {
        // Are we parsing this for an export operation.
        private bool _isForExport = true;

        public Parser(bool isForExportOperation)
        {
            _isForExport = isForExportOperation;
        }

        /// <summary>
        /// Takes a material file as a string, and parses the individual materials in the file.
        /// </summary>
        public List<MaterialAttributes> ParseFile(string[] content, ref List<string> errorLog)
        {
            var materialList = new List<MaterialAttributes>();
            int nestedIndex = 0;
            int i = 0;
            string materialBlock = string.Empty;

            while (i < content.Count())
            {
                var str = content[i];
                materialBlock += str + "\r\n";

                // Check for {
                if (str.Contains("{"))
                {
                    nestedIndex++;
                }
                // Check for }
                else if (str.Contains("}"))
                {
                    nestedIndex--;

                    // Check for complete material block
                    if (nestedIndex == 0)
                    {
                        var materialAttributes = Parse(materialBlock, ref errorLog);
                        if (materialAttributes != null)
                        {
                            materialList.Add(materialAttributes);
                        }

                        // Clear our material block of text
                        materialBlock = string.Empty;
                    }
                }

                i++;
            }
            return materialList;
        }

        /// <summary>
        /// Similar to the ParseFile method, but it will take the file data and parse it as if it was one material in the file.
        /// The ParseFile method uses this method to parse an individual material once its found in the string.
        /// </summary>
        public MaterialAttributes Parse(string material, ref List<string> errorLog)
        {
            var materialAttributes = new MaterialAttributes();
            materialAttributes.Data = material;

            bool isFoundName = false;
            int nestedIndex = 0;

            var lines = Regex.Split(material, "\n");
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
                // Check for vertex name
                else if (s.StartsWith("vertex_program ") && !isFoundName)
                {
                    materialAttributes.Program = ParseProgram(lines, ref i, ref errorLog);
                    isFoundName = true;
                }
                // Check for fragment name
                else if (s.StartsWith("fragment_program ") && !isFoundName)
                {
                    materialAttributes.Program = ParseProgram(lines, ref i, ref errorLog);
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
                i++;
            }
            return materialAttributes;
        }

        #region Private

        private MaterialAttributes.Technique ParseTechnique(string[] lines, ref int index, ref List<string> errorLog)
        {
            var technique = new MaterialAttributes.Technique();
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

        private MaterialAttributes.Technique.Pass ParsePass(string[] lines, ref int index, ref List<string> errorLog)
        {
            var pass = new MaterialAttributes.Technique.Pass();
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
                    if (!ParseSceneBlend(lines, ref index, ref nestedIndex, ref pass, ref errorLog))
                    {
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
                    if (!ParseAlphaRejection(lines, ref index, ref nestedIndex, ref pass, ref errorLog))
                    {
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
                // Check for texture_unit
                else if (s.StartsWith("texture_unit"))
                {
                    if (!ParseTextureUnit(lines, ref index, ref nestedIndex, ref pass, ref errorLog))
                    {
                        return null;
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
                if (s.StartsWith("cubic_texture "))
                {
                    if (!ParseCubicTexture(lines, ref index, ref nestedIndex, ref pass, ref errorLog))
                    {
                        return null;
                    }
                }
                if (s.StartsWith("vertex_program_ref ") || s.StartsWith("fragment_program_ref "))
                {
                    if (ParseReferenceProgram(lines, ref index, ref nestedIndex, ref pass, ref errorLog))
                    {
                    }
                    else
                    {
                        return null;
                    }
                }
                index++;
            }
            return pass;
        }

        private bool ParseAlphaRejection(string[] lines, ref int index, ref int nestedIndex, ref MaterialAttributes.Technique.Pass pass, ref List<string> errorLog)
        {
            pass.AlphaRej = new MaterialAttributes.Technique.Pass.AlphaRejection();
            var alphaRejection = Regex.Split(lines[index], " ");
            if (alphaRejection.Count() >= 3)
            {
                pass.AlphaRej.Compare = ConvertCompareFunction(alphaRejection[1]);
                if (pass.AlphaRej.Compare == CompareFunction.NONE)
                {
                    errorLog.Add("Unsupported compare function for alpha_rejection parameter.");
                    return false;
                }

                int value;
                if (int.TryParse(alphaRejection[2], out value))
                {
                    pass.AlphaRej.Value = value;
                }
                else
                {
                    errorLog.Add("Invalid value for alpha_rejection.");
                    return false;
                }

                if (alphaRejection.Count() == 4)
                    errorLog.Add("WARNING alpha_rejection only supports 2 parameters, the covert setting is always set to FALSE.");
            }
            else
            {
                errorLog.Add("Invalid number of parameters for alpha_rejection.");
                return false;
            }

            return true;
        }

        private bool ParseSceneBlend(string[] lines, ref int index, ref int nestedIndex, ref MaterialAttributes.Technique.Pass pass, ref List<string> errorLog)
        {
            var scene_blend = Regex.Split(lines[index], " ");
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
                    return false;
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
                    return false;
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
                    return false;
                }
            }
            else
            {
                errorLog.Add("Unsupported scene_blend parameter.");
                return false;
            }

            return true;
        }

        private bool ParseCubicTexture(string[] lines, ref int index, ref int nestedIndex, ref MaterialAttributes.Technique.Pass pass, ref List<string> errorLog)
        {
            // Format1 (short): cubic_texture <base_name> <combinedUVW|separateUV>
            // Format2 (long): cubic_texture <front> <back> <left> <right> <up> <down> separateUV
            var cubicTexture = new MaterialAttributes.Technique.Pass.CubicTexture();
            var options = Regex.Split(lines[index], " ");
            if (options.Length == 3)
            {
                var fileName = Path.GetFileNameWithoutExtension(options[1]);
                var extension = Path.GetExtension(options[1]);
                cubicTexture.Front = fileName + "_fr" + extension;
                cubicTexture.Back = fileName + "_bk" + extension;
                cubicTexture.Right = fileName + "_rt" + extension;
                cubicTexture.Left = fileName + "_lt" + extension;
                cubicTexture.Top = fileName + "_up" + extension;
                cubicTexture.Bottom = fileName + "_dn" + extension;
                cubicTexture.UvOption = options[2];
            }
            else if (options.Length == 8)
            {
                cubicTexture.Front = options[1];
                cubicTexture.Back = options[2];
                cubicTexture.Right = options[3];
                cubicTexture.Left = options[4];
                cubicTexture.Top = options[5];
                cubicTexture.Bottom = options[6];
                cubicTexture.UvOption = options[7];
            }
            else
            {
                errorLog.Add("Unsupported cubic_texture option.");
                return false;
            }
            pass.Cube = cubicTexture;
            index++;

            return true;
        }

        private bool ParseTextureUnit(string[] lines, ref int index, ref int nestedIndex, ref MaterialAttributes.Technique.Pass pass, ref List<string> errorLog)
        {
            index++;
            var passTextureUnit = new MaterialAttributes.Technique.Pass.PassTextureUnit();

            while (index < lines.Count())
            {
                var s = lines[index].Trim().ToLower();

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
                        return false;
                    }

                    // Fix the extension if this is for exporting
                    if (passTextureUnit.Texture != null && passTextureUnit.Texture.Length > 0)
                    {
                        if (_isForExport)
                        {
                            if (AssetManager.Common.Utility.GetExportImageExtension().ToLower() != ".none")
                            {
                                passTextureUnit.Texture = AssetManager.Common.Utility.ReplaceExtension(passTextureUnit.Texture, AssetManager.Common.Utility.GetExportImageExtension());
                            }
                        }
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
                        return false;
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
                    return false;
                }
                index++;
            }

            if (index >= lines.Count())
            {
                pass.TextureUnits.Add(passTextureUnit);
            }

            return true;
        }

        private bool ParseReferenceProgram(string[] lines, ref int index, ref int nestedIndex, ref MaterialAttributes.Technique.Pass pass, ref List<string> errorLog)
        {
            var programReference = new MaterialAttributes.Technique.Pass.ProgramReference();

            while (index < lines.Count())
            {
                var s = lines[index].Trim().ToLower();

                // vertex_program_ref spec_bumpmap_vs
                if (s.StartsWith("vertex_program_ref") || s.StartsWith("fragment_program_ref"))
                {
                    var name = Regex.Split(s, " ");
                    if (name.Count() == 2)
                    {
                        programReference.ProgramName = name[1];
                    }
                    else
                    {
                        errorLog.Add("Invalid number of parameters for ParseReferenceProgram name.");
                        return false;
                    }
                }
                // param_named_auto view_proj_matrix worldviewproj_matrix
                else if (s.StartsWith("param_indexed") ||
                    s.StartsWith("param_indexed_auto") ||
                    s.StartsWith("param_named") ||
                    s.StartsWith("param_named_auto") ||
                    s.StartsWith("shared_params_ref"))
                {
                    var options = Regex.Split(s, " ");
                    programReference.Parameters.Add(options);
                }
                else if (s.StartsWith("{"))
                {
                    nestedIndex++;
                }
                else if (s.StartsWith("}"))
                {
                    nestedIndex--;
                    index++;
                    pass.ProgramRefs.Add(programReference);
                    break;
                }
                else
                {
                    errorLog.Add("Unsupported ParseReferenceProgram option.");
                    return false;
                }
                index++;
            }

            return true;
        }

        private MaterialAttributes.ProgramAttributes ParseProgram(string[] lines, ref int index, ref List<string> errorLog)
        {
            var program = new MaterialAttributes.ProgramAttributes();
            int nestedIndex = 0;
            for (; index < lines.Count(); index++)
            {
                string s = lines[index].Trim().ToLower();

                // Check for name
                if (s.StartsWith("vertex_program "))
                {
                    var options = Regex.Split(s, " ");
                    if (options.Length >= 2)
                    {
                        program.Name = options[1];
                    }
                    else
                    {
                        errorLog.Add("Error parsing options/name for program, line: " + index);
                    }

                    program.Type = MaterialAttributes.ProgramAttributes.ProgramType.Vertex;
                }
                // Check for fragment name
                else if (s.StartsWith("fragment_program "))
                {
                    var options = Regex.Split(s, " ");
                    if (options.Length >= 2)
                    {
                        program.Name = options[1];
                    }
                    else
                    {
                        errorLog.Add("Error parsing options/name for program, line: " + index);
                    }

                    program.Type = MaterialAttributes.ProgramAttributes.ProgramType.Fragment;
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
                    if (nestedIndex == 0)
                        break;
                }
                // Check for source
                else if (s.StartsWith("source"))
                {
                    var source = Regex.Split(s, " ");
                    if (source.Length == 2)
                    {
                        program.Source = source[1];
                    }
                    else
                    {
                        errorLog.Add("Error parsing source for program, line: " + index);
                    }
                }
                // Check for entry point
                else if (s.StartsWith("entry_point"))
                {
                    var entryPoint = Regex.Split(s, " ");
                    if (entryPoint.Length == 2)
                    {
                        program.EntryPoint = entryPoint[1];
                    }
                    else
                    {
                        errorLog.Add("Error parsing entry point for program, line: " + index);
                    }
                }
                // Check for the profiles
                else if (s.StartsWith("profiles"))
                {
                    var profiles = Regex.Split(s, " ");
                    foreach (var profile in profiles)
                    {
                        program.Profiles.Add(profile);
                    }
                }
                // default_params
                else if (s.StartsWith("default_params"))
                {
                    throw new Exception("default_params not supported yet.");
                }
                // Unsupported option
                else if (s.Length > 0)
                {
                    errorLog.Add("Error parsing entry point for program, unsupported (" + s + "), line: " + index);
                    return null;
                }
            }
            return program;
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

        #endregion // Private
    }
}
