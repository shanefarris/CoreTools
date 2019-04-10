using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssetManager.Test
{
    [TestClass]
    public class MaterialParserTest
    {
        [TestMethod]
        public void CubicTextureText()
        {
            // Format1 (short): cubic_texture <base_name> <combinedUVW|separateUV>
            // Format2 (long): cubic_texture <front> <back> <left> <right> <up> <down> separateUV
        }

        #region SpecularBumpmapTemplate

        private string SpecularBumpmapTemplate = 
@"material SpecularBumpmapTemplate
{
	technique
	{
		pass
		{
		  ambient 0.6 0.6 0.6 1
			diffuse 0.6 0.6 0.6 1
			specular 0 0 0 1 10

			vertex_program_ref spec_bumpmap_vs
			{
				param_named_auto view_proj_matrix worldviewproj_matrix
				param_named_auto light_position light_position_object_space 0
				param_named_auto eye_position camera_position_object_space
				param_named_auto inv_view_matrix inverse_worldview_matrix
			}

			fragment_program_ref spec_bumpmap_ps
			{
				param_named_auto specular light_specular_colour 1
				param_named Ka float 0.3
				param_named Kd float 1
				param_named Ks float 1
				param_named specular_power float 512
				param_named bumpiness float 1
				param_named_auto ambient ambient_light_colour
				param_named_auto diffuse light_diffuse_colour 0
			}
			
			texture_unit
			{
				texture_alias DiffuseMap
			}

			texture_unit
			{
				texture_alias NormalMap
			}
		}
	}
}";

        #endregion // SpecularBumpmapTemplate

        #region MotionBlur

        private string MotionBlur = 
@"material Core/Compositor/MotionBlur
{
	technique
	{
		pass
		{
			lighting off
			depth_check off

			texture_unit Sum
			{
				tex_address_mode clamp
				filtering linear linear none
				colour_op replace
                tex_coord_set 0
			}
		}
	}
}";

        #endregion // MotionBlur

        #region Combine

        private string Combine = 
@"material Core/Compositor/Combine
{
	technique
	{
		pass
		{
			depth_check off

			fragment_program_ref Core/Compositor/Combine_fp
			{
			}

			vertex_program_ref Core/Compositor/StdQuad_Cg_vp
			{
			}

			texture_unit RT
			{
				tex_address_mode clamp
				filtering linear linear none
                tex_coord_set 0
			}

			texture_unit SUM
			{
				tex_address_mode clamp
				filtering linear linear none
                tex_coord_set 0
			}
		}
	}
}";

        #endregion // Combine

        #region Combine_fp

        private string Combine_fp = 
@"fragment_program Core/Compositor/Combine_fp cg
{
	source Combine_fp.cg
	profiles ps_2_0 arbfp1
	entry_point Combine_fp

	default_params
	{
		param_named blur float 0.8
	}
}";

        #endregion // Combine_fp
    }
}
