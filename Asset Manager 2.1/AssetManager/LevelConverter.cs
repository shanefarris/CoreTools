using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AssetManager
{
    class LevelConverter
    {
        public enum FileType { Ogitor, DotScene, Artifex  }

        public static List<string> Convert(string fileName, FileType fileType)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show(@"Error finding file.");
                return null;
            }

            if (fileType == FileType.Ogitor)
            {
                var reader = new XmlTextReader(fileName);
                var ogitorLevelConverter = new OgitorLevelConverter();
                return ogitorLevelConverter.Convert(reader);
            }

            MessageBox.Show(@"No other file format is supported yet.");
            return null;
        }

        private class OgitorLevelConverter
        {
            private struct PagedTerrainValues
            {
                public int MinX;
                public int MinY;
                public int MaxX;
                public int MaxY;
            }

            private PagedTerrainValues _pagedTerrainValues = new PagedTerrainValues();

            public List<string> Convert(XmlReader reader)
            {
                var pagedTerrain = string.Empty;
                var returnValue = new List<string>();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "OBJECT" && reader.GetAttribute("typename") != null)
                    {
                        if (reader.GetAttribute("typename") == "OctreeSceneManager")
                        {
                            returnValue.Add(ParseSceneManager(reader));
                        }
                        else if (reader.GetAttribute("typename") == "Viewport Object")
                        {
                            returnValue.Add(ParseViewport(reader));;
                        }
                        else if (reader.GetAttribute("typename") == "Entity Object")
                        {
                            returnValue.Add(ParseEntity(reader));
                        }
                        else if (reader.GetAttribute("typename") == "Light Object")
                        {
                            returnValue.Add(ParseLight(reader));
                        }
                        else if (reader.GetAttribute("typename") == "Camera Object")
                        {
                            returnValue.Add(ParseCamera(reader));
                        }
                        else if (reader.GetAttribute("typename") == "Terrain Group Object")
                        {
                            pagedTerrain = ParseTerrain(reader);
                        }
                        else if (reader.GetAttribute("typename") == "Caelum Object")
                        {
                            returnValue.Add(ParseCaelum(reader));
                        }
                        else if (reader.GetAttribute("typename") == "Hydrax Object")
                        {
                            returnValue.Add(ParseHydrax(reader));
                        }
                        else if (reader.GetAttribute("typename") == "Terrain Page Object")
                        {
                            ParseTerrainName(reader);
                        }
                    }
                }

                CombineTerrainData(ref pagedTerrain);
                if (pagedTerrain.Length > 0)
                    returnValue.Add(pagedTerrain);

                return returnValue;
            }

            private string ParseSceneManager(XmlReader reader)
            {
                /*  Not collecting: rendering distance, locked, layer, position, orientation, scale,
	             *  shadows enabled, shadows::renderingdistance, shadows::resolutionfar, shadows::resolutionmiddle,
	             *  shadows::resolutionnear, shadows::technique, 
	             */
                string returnValue = "<SceneManager name=\"MySceneManager\" ";

	            reader.Read();
                while (reader.Read() && reader.Name != "OBJECT")
	            {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
		            {
                        if (reader.GetAttribute("id") == "ambient")
                            returnValue += " ambient=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "fog::colour")
                            returnValue += " fogcolour=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "fog::density")
                            returnValue += " fogdensity=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "fog::end")
                            returnValue += " fogend=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "fog::start")
                            returnValue += " fogstart=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "skybox::active")
                            returnValue += " skyboxactive=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "skybox::distance")
                            returnValue += " skyboxdistance=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "skybox::material")
                            returnValue += " skyboxmaterial=\"" + reader.GetAttribute("value") + "\" ";
                        else if (reader.GetAttribute("id") == "scenemanagertype")
                            returnValue += " scenemanagertype=\"" + reader.GetAttribute("value") + "\" ";

                        else if (reader.GetAttribute("id") == "fog::mode")
                        {
                            if (reader.GetAttribute("value") == "0")
                                returnValue += " fogmode=\"FOG_NONE\"";
                            else if (reader.GetAttribute("value") == "1")
                                returnValue += " fogmode=\"FOG_EXP\"";
                            else if (reader.GetAttribute("value") == "2")
                                returnValue += " fogmode=\"FOG_EXP2\"";
                            else if (reader.GetAttribute("value") == "3")
                                returnValue += " fogmode=\"FOG_LINEAR\"";
                        }
		            }
	            }

                return returnValue + " ></SceneManager>";
            }

            private string ParseViewport(XmlReader reader)
            {
                string returnValue = "<Viewport ";
	            
                if (reader.GetAttribute("typename") == "Viewport Object")
	            {
		            returnValue += "name=\"" + reader.GetAttribute("name") + "\" ";
	            }

                while (reader.Read() && reader.Name != "OBJECT")
                {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
		            {
			            if(reader.GetAttribute("id") == "colour")
				            returnValue += "colour=\"" + reader.GetAttribute("value") + "\" ";
		            }
                }

                return returnValue + "></Viewport>";
            }

            private string ParseEntity(XmlReader reader)
            {
                /*  Not collecting: autotracktarget, renderingdistance, subentity
                 */

                string returnValue = "<GameObject ";
                if ("Entity Object" == reader.GetAttribute("typename"))
                {
                    returnValue += "name=\"" + reader.GetAttribute("name") + "\" ";
                }

                reader.Read();
                while("OBJECT" != reader.Name)
                {
                    if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
                    {
                        if(reader.GetAttribute("id") == "castshadows")
                            returnValue += "shadows=\"" + reader.GetAttribute("value") + "\" ";
                        else if(reader.GetAttribute("id") == "meshfile")
                            returnValue += "meshfile=\"" + reader.GetAttribute("value") + "\" ";
                        else if(reader.GetAttribute("id") == "orientation")
                            returnValue += "orientation=\"" + reader.GetAttribute("value") + "\" ";
                        else if(reader.GetAttribute("id") == "position")
                            returnValue += "position=\"" + reader.GetAttribute("value") + "\" ";
                        else if(reader.GetAttribute("id") == "scale")
                            returnValue += "scale=\"" + reader.GetAttribute("value") + "\" ";
                    }
                    reader.Read();
                }

                // Create a sphere obstacle by default
                returnValue += "obstacletype=\"sphere\" ";

                return returnValue + "></GameObject>";
            }

            private string ParseLight(XmlReader reader)
            {
                /*  Not collecting: parent, orientation
	             */

	            string returnValue = "<Light ";
	            if ("Light Object" == reader.GetAttribute("typename"))
	            {
		            returnValue += "name=\"" + reader.GetAttribute("name") + "\" ";
	            }

	            reader.Read();
	            while("OBJECT" != reader.Name)
	            {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
		            {
			            if(reader.GetAttribute("id") == "attenuation")
				            returnValue += "attenuation=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "castshadows")
				            returnValue += "shadows=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "diffuse")
				            returnValue += "diffuse=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "direction")
				            returnValue += "direction=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lightrange")
				            returnValue += "range=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "position")
				            returnValue += "position=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "power")
				            returnValue += "power=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "specular")
				            returnValue += "specular=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lighttype")
			            {
				            if(reader.GetAttribute("value") == "0")
					            returnValue += "lighttype=\"LT_POINT\" ";
				            else if(reader.GetAttribute("value") == "1")
					            returnValue += "lighttype=\"LT_DIRECTIONAL\" ";
				            else
					            returnValue += "lighttype=\"LT_SPOT\" ";
			            }
		            }
		            reader.Read();
	            }

	            return returnValue + "></Light>";
            }

            private string ParseCamera(XmlReader reader)
            {
                /*  Not collecting: autoaspectratio, autotracktarget
	             */

	            string returnValue = "<Camera ";

	            // This needs to be set manually in the xml
	            returnValue += "type=\"ECM_FREE\" ";

	            if ("Camera Object" == reader.GetAttribute("typename"))
	            {
		            returnValue += "name=\"" + reader.GetAttribute("name") + "\" ";
	            }

	            reader.Read();
	            while("OBJECT" != reader.Name)
	            {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
		            {
			            if(reader.GetAttribute("id") == "clipdistance")
                            returnValue += "clipdistance=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "fov")
                            returnValue += "fov=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "orientation")
                            returnValue += "orientation=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "position")
                            returnValue += "position=\"" + reader.GetAttribute("value") + "\" ";
		            }
		            reader.Read();
	            }

                returnValue += "lookat=\"0 0 0\" ";		        // This information isn't saved in ogScene
                returnValue += "defualt=\"true\" ";							// All are "default" for now
	            return returnValue + "></Camera>";
            }

            private string ParseTerrain(XmlReader reader)
            {
                /*  Not collecting: tuning::compositemapdistance
	             */

                string returnValue = "<PagedTerrain ";

                // These values are default and need to be set manually for now
	            returnValue += "name=\"PagedTerrain\" ";
                returnValue += "maxx=\"0\" ";
                returnValue += "minx=\"0\" ";
                returnValue += "maxy=\"0\" ";
                returnValue += "miny=\"0\" ";
                returnValue += "position=\"0 0 0\" ";
		        returnValue += "resourcegroup=\"DEFAULT_RESOURCE_GROUP_NAME\" ";
                returnValue += "terrainfile=\"Page\" ";

	            reader.Read();
	            while("OBJECT" != reader.Name)
	            {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
		            {
			            if(reader.GetAttribute("id") == "blendmap::texturesize")
				            returnValue += "colourmap_texturesize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "colourmap::enabled")
				            returnValue += "colourmap_enabled=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "colourmap::texturesize")
				            returnValue += "colormaptexturesize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lightmap::texturesize")
				            returnValue += "lightmaptexturesize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pagemapsize")
				            returnValue += "mapsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pageworldsize")
				            returnValue += "worldsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pg::densitymapsize")
				            returnValue += "pagedesitymapsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pg::detaildistance")
				            returnValue += "detaildistance=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pg::pagesize")
				            returnValue += "pagesize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "tuning::maxbatchsize")
				            returnValue += "maxbatchsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "tuning::maxpixelerror")
				            returnValue += "maxpixelerror=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "tuning::minbatchsize")
                            returnValue += "minbatchsize=\"" + reader.GetAttribute("value") + "\" ";
		            }
		            reader.Read();
	            }
                return returnValue + "></PagedTerrain>";
            }

            private string ParseCaelum(XmlReader reader)
            {
                /*  Not collecting: cloud data becaues there are multiple layers, and it is not a high priority at the moment
	             */

	            string returnValue = "<Caelum name=\"CaelumSky\" ";

	            reader.Read();
	            while("OBJECT" != reader.Name)
	            {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndElement)
		            {
			            if(reader.GetAttribute("id") == "clock::day")
				            returnValue += "timeday=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clock::hour")
				            returnValue += "timehour=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clock::minute")
				            returnValue += "timeminute=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clock::month")
				            returnValue += "timemonth=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clock::second")
				            returnValue += "timesec=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clock::speed")
				            returnValue += "timespeed=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clock::year")
				            returnValue += "timeyear=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clouds::enable")
				            returnValue += "isclouds=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "clouds::layer_count")
				            returnValue += "layerofclouds=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "fog::density_multiplier")
				            returnValue += "fogdenmultiplier=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "fog::manage")
				            returnValue += "ismanagefog=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "layer")
				            returnValue += "layer=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lighting::ensure_single_lightsource")
				            returnValue += "issinglelightsource=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lighting::ensure_single_shadowsource")
				            returnValue += "issingleshadowsource=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lighting::manage_ambient_light")
				            returnValue += "ismanageambientlight=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "lighting::minimum_ambient_light")
				            returnValue += "minambientlight=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::ambient_multiplier")
				            returnValue += "moonambientmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::attenuation::constant_multiplier")
				            returnValue += "moonattmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::attenuation::distance")
				            returnValue += "moonattdistance=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::attenuation::linear_multiplier")
				            returnValue += "moonattlinearmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::attenuation::quadric_multiplier")
				            returnValue += "moonquadmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::auto_disable")
				            returnValue += "ismoonautodisable=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::cast_shadow")
				            returnValue += "ismooncastshadow=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::diffuse_multiplier")
				            returnValue += "moondiffusemultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::enable")
				            returnValue += "ismoonenabled=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "moon::specular_multiplier")
				            returnValue += "moonspecularmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "observer::latitude")
				            returnValue += "observerlatitude=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "observer::longitude")
				            returnValue += "observerlongitude=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "stars::enable")
				            returnValue += "isstarsenabled=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "stars::mag0_pixel_size")
				            returnValue += "starsmag0pixelsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "stars::magnitude_scale")
				            returnValue += "starsmagscale=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "stars::max_pixel_size")
				            returnValue += "starsmaxpixelsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "stars::min_pixel_size")
				            returnValue += "starsminpixelsize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::ambient_multiplier")
				            returnValue += "sunambientmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::attenuation::constant_multiplier")
				            returnValue += "sunattmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::attenuation::distance")
				            returnValue += "sundistance=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::attenuation::linear_multiplier")
				            returnValue += "sunattlinearmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::attenuation::quadric_multiplier")
				            returnValue += "sunquadmultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::auto_disable")
				            returnValue += "issunautodisable=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::cast_shadow")
				            returnValue += "issuncastshadow=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::colour")
				            returnValue += "suncolor=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::diffuse_multiplier")
				            returnValue += "sundiffusemultipler=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::enable")
				            returnValue += "issunenabled=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::lightcolour")
				            returnValue += "sunlightcolor=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::position")
				            returnValue += "sunposition=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::specular_multiplier")
                            returnValue += "sundiffusemultipler=\"" + reader.GetAttribute("value") + "\" ";
		            }
		            reader.Read();
	            }
                return returnValue + "></Caelum>";
            }

            private string ParseHydrax(XmlReader reader)
            {
                string returnValue = "<Hydrax name=\"HydraxWater\" ";

	            reader.Read();
	            while("OBJECT" != reader.Name)
	            {
		            if(reader.Name == "PROPERTY" && reader.NodeType != XmlNodeType.EndEntity)
		            {
			            if(reader.GetAttribute("id") == "caelumintegration")
				            returnValue += "iscaelumitegrated=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "caustics::end")
				            returnValue += "causticsend=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "caustics::power")
				            returnValue += "causticspower=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "caustics::scale")
				            returnValue += "causticsscale=\"" + reader.GetAttribute("value") + "\" ";

			            // Componets
			            else if(reader.GetAttribute("id") == "components::caustics")
				            returnValue += "iscomponentscaustics=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::depth")
				           returnValue += "iscomponentsdepth=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::foam")
				            returnValue += "iscomponentsfoam=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::smooth")
				            returnValue += "iscomponentssmooth=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::sun")
				            returnValue += "iscomponentssun=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::underwater")
				            returnValue += "iscomponentsunderwater=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::underwater_godrays")
				            returnValue += "iscomponentsgodrays=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "components::underwater_reflections")
				            returnValue += "iscomponentsreflections=\"" + reader.GetAttribute("value") + "\" ";

			            else if(reader.GetAttribute("id") == "configfile")
				            returnValue += "configfile=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "depth::limit")
				            returnValue += "depthlimit=\"" + reader.GetAttribute("value") + "\" ";

			            // Foam
			            else if(reader.GetAttribute("id") == "foam::max_distance")
				            returnValue += "foammaxdistance=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "foam::scale")
				            returnValue += "foamscale=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "foam::start")
				            returnValue += "foamstart=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "foam::transparency")
				            returnValue += "foamtransparency=\"" + reader.GetAttribute("value") + "\" ";

			            else if(reader.GetAttribute("id") == "full_reflection_distance")
				            returnValue += "fullreflectiondistance=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "global_transparency")
				            returnValue += "globaltransparency=\"" + reader.GetAttribute("value") + "\" ";

			            // Godrays
			            else if(reader.GetAttribute("id") == "godrays::exposure")
				            returnValue += "godraysexposure=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "godrays::intensity")
				            returnValue += "godraysintensity=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "godrays::intersections")
				            returnValue += "godraysintersections=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "godrays::number_of_rays")
				            returnValue += "godraysnumrays=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "godrays::rays_size")
				            returnValue += "godrayssize=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "godrays::speed")
				            returnValue += "godraysspeed=\"" + reader.GetAttribute("value") + "\" ";

			            else if(reader.GetAttribute("id") == "layer")
				            returnValue += "layers=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "module_name")
				            returnValue += "modulename=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "noise_module_name")
				            returnValue += "noisemodulename=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "normal_distortion")
				            returnValue += "normaldistortion=\"" + reader.GetAttribute("value") + "\" ";

			            // Perlin noise
			            else if(reader.GetAttribute("id") == "perlin::anim_speed")
				            returnValue += "perlinanimspeed=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "perlin::falloff")
				            returnValue += "perlinfalloff=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "perlin::gpu_lod")
				            returnValue += "perlingpulod=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "perlin::gpu_strength")
				            returnValue += "perlingpustrength=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "perlin::octaves")
				            returnValue += "perlinoctaves=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "perlin::scale")
				            returnValue += "perlinscale=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "perlin::time_multi")
				            returnValue += "perlintimemulti=\"" + reader.GetAttribute("value") + "\" ";

			            // PgModule
			            else if(reader.GetAttribute("id") == "pgmodule::choppy_strength")
				            returnValue += "pgmodulechoppystrength=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pgmodule::choppy_waves")
				            returnValue += "ispgmodulechoppywaves=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pgmodule::complexity")
				            returnValue += "pgmodulecomplexity=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pgmodule::elevation")
				            returnValue += "pgmoduleelevation=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pgmodule::force_recalculate_geometry")
				            returnValue += "ispgmoduleFforcerecalculategeometry=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pgmodule::smooth")
				            returnValue += "ispgmodulesmooth=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "pgmodule::strength")
				            returnValue += "pgmodulestrength=\"" + reader.GetAttribute("value") + "\" ";

			            else if(reader.GetAttribute("id") == "planes_error")
				            returnValue += "planeserror=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "position")
				            returnValue += "position=\"" + reader.GetAttribute("value") + "\" ";

			            // RTT
			            else if(reader.GetAttribute("id") == "rtt_quality::depth")
				            returnValue += "rttqualitydepth=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "rtt_quality::depth_aip")
				            returnValue += "rttqualitydepthaip=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "rtt_quality::depth_reflection")
				            returnValue += "rttqualitydepthreflection=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "rtt_quality::gpu_normal_map")
				            returnValue += "rttqualitygpunormalmap=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "rtt_quality::reflection")
				            returnValue += "rttqualityrefraction=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "rtt_quality::refraction")
				            returnValue += "rttqualityrefraction=\"" + reader.GetAttribute("value") + "\" ";

			            // TODO: shader mode

			            else if(reader.GetAttribute("id") == "smooth::power")
				            returnValue += "smoothpower=\"" + reader.GetAttribute("value") + "\" ";

			            // Sun
			            else if(reader.GetAttribute("id") == "sun::area")
				            returnValue += "sunarea=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::colour")
				            returnValue += "suncolour=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::position")
				            returnValue += "sunposition=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "sun::strength")
				            returnValue += "sunstrength=\"" + reader.GetAttribute("value") + "\" ";

			            else if(reader.GetAttribute("id") == "technique_add")
				            returnValue += "techniqueadd=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "technique_remove")
				            returnValue += "techniqueremove=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "updatescript")
				            returnValue += "updatescript=\"" + reader.GetAttribute("value") + "\" ";
			            else if(reader.GetAttribute("id") == "watercolour")
                            returnValue += "watercolour=\"" + reader.GetAttribute("value") + "\" ";
		            }
		            reader.Read();
	            }
                return returnValue + "></Hydrax>";
            }

            private void ParseTerrainName(XmlReader reader)
            {
                // Get the page name first
	            // Parse the page terrain name to get the X/Y min/max

	            var name = reader.GetAttribute("name");
                if(string.IsNullOrEmpty(name))
                    return;

                name = name.Replace("Page", string.Empty);
	            var values = name.Split('x');
	            if(values.Length == 2)
	            {
		            var tempY = int.Parse(values[0]);
		            var tempX = int.Parse(values[1]);

                    if (tempX > _pagedTerrainValues.MaxX)
                        _pagedTerrainValues.MaxX = tempX;
                    else if (tempX < _pagedTerrainValues.MinX)
                        _pagedTerrainValues.MinX = tempX;

                    if (tempY > _pagedTerrainValues.MaxY)
                        _pagedTerrainValues.MaxY = tempY;
                    else if (tempY < _pagedTerrainValues.MinY)
                        _pagedTerrainValues.MinY = tempY;
	            }

	            reader.Read();
            }

            private void CombineTerrainData(ref string pagedTerrain)
            {
                pagedTerrain = pagedTerrain.Replace("maxx=\"0\"", "maxx=\"" + _pagedTerrainValues.MaxX + "\"");
                pagedTerrain = pagedTerrain.Replace("maxy=\"0\"", "maxy=\"" + _pagedTerrainValues.MaxY + "\"");
                pagedTerrain = pagedTerrain.Replace("minx=\"0\"", "minx=\"" + _pagedTerrainValues.MinX + "\"");
                pagedTerrain = pagedTerrain.Replace("miny=\"0\"", "miny=\"" + _pagedTerrainValues.MinY + "\"");
            }
        }



    }
}
