using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialParser
{
    public class Pass
	{
		public string Name { get; set; }
		public Vector4D Ambient { get; set; }
		public Vector4D Diffuse { get; set; }
        public Vector4D Emissive { get; set; }
        public Vector5D Specular { get; set; }
		public Vector4D SelfIllumination { get; set; }
        public bool? IsDepthCheckEnabled { get; set; }              // TODO not supported yet
		public bool? IsDepthWriteEnabled { get; set; }
        public bool? IsColourWriteEnabled { get; set; }             // TODO not supported yet
        public bool? IsLightingEnabled { get; set; }                // TODO not supported yet
        public CullingMode? CullingMode { get; set; }               // TODO not supported yet
        public ManualCullingMode? ManualCullingMode { get; set; }   // TODO not supported yet
        public ShadeOptions? ShadingMode { get; set; }              // TODO not supported yet
        public PassFog Fog { get; set; }                            // TODO not supported yet
        public AlphaRejection AlphaRejection { get; set; }
		public SceneBlendType? SceneBlending { get; set; }
        public List<PassTextureUnit> TextureUnits { get; set; }
        public CompareFunction? DepthFunction { get; set; }          // TODO not supported yet
        public SceneBlendFactor? SceneBlendSource { get; set; }
        public SceneBlendFactor? SceneBlendDest { get; set; }

		private float? _constantBias = null;
		private float? _slopeScaleBias;

        public Pass()
        {
            Name = null;
            Ambient = null;
            Diffuse = null;
            Emissive = null;
            Specular = null;
            SelfIllumination = null;
            IsDepthCheckEnabled = null;
            IsDepthWriteEnabled = null;
            IsColourWriteEnabled = null;
            IsLightingEnabled = null;
            CullingMode = null;
            ManualCullingMode = null;
            ShadingMode = null;
            Fog = null;
            AlphaRejection = null;
            SceneBlending = null;
            DepthFunction = null;
            TextureUnits = new List<PassTextureUnit>();
        }

		public void SetFog(bool isOverrideScene, Vector4D colour, FogMode mode = FogMode.FOG_NONE, float expDensity = 0.001f, float linearStart = 0.0f, float linearEnd = 1.0f)
		{
			Fog = new PassFog();
			Fog.IsOverrideScene = isOverrideScene;
			Fog.Color = colour;
			Fog.Mode = mode;
			Fog.ExpDensity = expDensity;
			Fog.LinearStart = linearStart;
			Fog.LinearEnd = linearEnd;
		}

		public void setDepthBias(float constantBias, float slopeScaleBias)
		{
			_constantBias = constantBias;
			_slopeScaleBias = slopeScaleBias;
		}
	}

    public class Technique
	{
		public string Scheme { get; set; }
		public int? LoDLevel { get; set; }
		public bool? IsLightingEnabled { get; set; }
		public List<Pass> Passes = new List<Pass>();

        public Technique()
        {
            Scheme = "Default";
            LoDLevel = null;
            IsLightingEnabled = null;
        }
	}

    public class MaterialAttributes
	{
        public string Name { get; set; }
		public bool IsReceiveShadows { get; set; }
		public bool TransparencyCastsShadows { get; set; }
		public List<Technique> Techniques { get; set; }

		public MaterialAttributes()
		{
			IsReceiveShadows = true;
			TransparencyCastsShadows = false;
			Techniques = new List<Technique>();
		}
	}
}

// MISSING:
// TextureUnitState::setTextureFiltering
// TextureUnitState::setTextureAnisotropy
// Pass::setSeparateSceneBlending
// Pass::setVertexColourTracking
// Pass::setPointSize
// Pass::setPointSpritesEnabled
// Pass::setPointAttenuation
// Pass::setPointMinSize
// Pass::setPointMaxSize
// Pass::addTextureUnitState
// Pass::
// Material::setLodLevels
// Material::setLodStrategy