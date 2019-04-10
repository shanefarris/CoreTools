using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetManager.Common.MaterialParser
{
    

    

    #region MaterialAttributes

    public class MaterialAttributes
    {
        #region ProgramAttributes

        public class ProgramAttributes
        {
            public enum ProgramType
            {
                Fragment,
                Vertex,
                Pixel,
                Geometry,
            };

            public string Name { get; set; }
            public ProgramType Type { get; set; }
            public string EntryPoint { get; set; }
            public string Source { get; set; }
            public List<string> Profiles { get; set; }

            public ProgramAttributes()
            {
                Profiles = new List<string>();
            }
        }

        #endregion // ProgramAttributes

        #region Technique

        public class Technique
        {
            #region Pass

            public class Pass
            {
                #region ProgramReference

                public class ProgramReference
                {
                    public string ProgramName { get; set; }
                    public List<string[]> Parameters { get; set; }
                }

                #endregion // ProgramReference

                #region CubicTexture

                public class CubicTexture
                {
                    public string Front { get; set; }
                    public string Back { get; set; }
                    public string Top { get; set; }
                    public string Bottom { get; set; }
                    public string Right { get; set; }
                    public string Left { get; set; }
                    public string UvOption { get; set; }
                }

                #endregion // CubicTexture

                #region PassFog

                public class PassFog
                {
                    public bool IsOverrideScene { get; set; }
                    public Vector4D Color { get; set; }
                    public FogMode Mode { get; set; }
                    public float ExpDensity { get; set; }
                    public float LinearStart { get; set; }
                    public float LinearEnd { get; set; }
                }

                #endregion // PassFog

                #region AlphaRejection

                public class AlphaRejection
                {
                    public CompareFunction Compare { get; set; }
                    public int Value { get; set; }
                    public bool AlphaToCoverage { get; set; }

                    public AlphaRejection()
                    {
                        AlphaToCoverage = false;
                    }
                }

                #endregion // AlphaRejection

                #region PassTextureUnit

                public class PassTextureUnit
                {
                    public PassTextureUnit()
                    {
                        Texture = string.Empty;
                        NumMipmaps = null;
                    }

                    public PassTextureUnit(string texture)
                    {
                        Texture = texture;
                        NumMipmaps = null;
                    }

                    public int? NumMipmaps { get; set; }
                    public string FilteringMin { get; set; }
                    public string FilteringMax { get; set; }
                    public string FilteringMip { get; set; }
                    public string Texture { get; set; }
                }

                #endregion // PassTextureUnit

                public string Name { get; set; }
                public Vector4D Ambient { get; set; }
                public Vector4D Diffuse { get; set; }
                public Vector4D Emissive { get; set; }
                public Vector5D Specular { get; set; }
                public Vector4D SelfIllumination { get; set; }
                public bool? IsDepthCheckEnabled { get; set; }                  // TODO not supported yet
                public bool? IsDepthWriteEnabled { get; set; }
                public bool? IsColourWriteEnabled { get; set; }                 // TODO not supported yet
                public bool? IsLightingEnabled { get; set; }                    // TODO not supported yet
                public CullingMode? CullingMode { get; set; }                   // TODO not supported yet
                public ManualCullingMode? ManualCullingMode { get; set; }       // TODO not supported yet
                public ShadeOptions? ShadingMode { get; set; }                  // TODO not supported yet
                public PassFog Fog { get; set; }                                // TODO not supported yet
                public AlphaRejection AlphaRej { get; set; }
                public SceneBlendType? SceneBlending { get; set; }
                public List<PassTextureUnit> TextureUnits { get; set; }
                public CompareFunction? DepthFunction { get; set; }             // TODO not supported yet
                public SceneBlendFactor? SceneBlendSource { get; set; }
                public SceneBlendFactor? SceneBlendDest { get; set; }
                public CubicTexture Cube { get; set; }                          // Not in C++ yet
                public List<ProgramReference> ProgramRefs { get; set; }         // Not in C++ yet

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
                    AlphaRej = null;
                    SceneBlending = null;
                    DepthFunction = null;
                    TextureUnits = new List<PassTextureUnit>();
                    ProgramRefs = new List<ProgramReference>();
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

            #endregion // Pass

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

        #endregion // Technique

        public string Name { get; set; }
        public bool IsReceiveShadows { get; set; }
        public bool TransparencyCastsShadows { get; set; }
        public List<Technique> Techniques { get; set; }
        public ProgramAttributes Program { get; set; }
        public string Data { get; set; }

        public MaterialAttributes()
        {
            IsReceiveShadows = true;
            TransparencyCastsShadows = false;
            Techniques = new List<Technique>();
            Program = null;
        }

        public static bool operator ==(MaterialAttributes m1, MaterialAttributes m2)
        {
            if (((object)m1 == null && (object)m2 != null) ||
                ((object)m2 == null && (object)m1 != null))
            {
                return false;
            }

            // No: name, data,
            if (m1.IsReceiveShadows != m2.IsReceiveShadows ||
                m1.TransparencyCastsShadows != m2.TransparencyCastsShadows ||
                m1.Techniques.Count != m2.Techniques.Count)
            {
                return false;
            }

            // Compare technique
            int i = 0;
            foreach (var t in m1.Techniques)
            {
                if (t.Scheme == m2.Techniques[i].Scheme &&
                    t.IsLightingEnabled == m2.Techniques[i].IsLightingEnabled &&
                    t.LoDLevel == m2.Techniques[i].LoDLevel &&
                    t.Passes.Count == m2.Techniques[i].Passes.Count)
                {
                    // Compare pass
                    int j = 0;
                    foreach (var p in t.Passes)
                    {
                        if (!(p.AlphaRej == m2.Techniques[i].Passes[j].AlphaRej &&
                            p.Ambient == m2.Techniques[i].Passes[j].Ambient &&
                            p.CullingMode == m2.Techniques[i].Passes[j].CullingMode &&
                            p.DepthFunction == m2.Techniques[i].Passes[j].DepthFunction &&
                            p.Diffuse == m2.Techniques[i].Passes[j].Diffuse &&
                            p.Emissive == m2.Techniques[i].Passes[j].Emissive &&
                            p.Fog == m2.Techniques[i].Passes[j].Fog &&
                            p.IsColourWriteEnabled == m2.Techniques[i].Passes[j].IsColourWriteEnabled &&
                            p.IsDepthCheckEnabled == m2.Techniques[i].Passes[j].IsDepthCheckEnabled &&
                            p.IsDepthWriteEnabled == m2.Techniques[i].Passes[j].IsDepthWriteEnabled &&
                            p.IsLightingEnabled == m2.Techniques[i].Passes[j].IsLightingEnabled &&
                            p.ManualCullingMode == m2.Techniques[i].Passes[j].ManualCullingMode &&
                            p.SceneBlendDest == m2.Techniques[i].Passes[j].SceneBlendDest &&
                            p.SceneBlending == m2.Techniques[i].Passes[j].SceneBlending &&
                            p.SceneBlendSource == m2.Techniques[i].Passes[j].SceneBlendSource &&
                            p.SelfIllumination == m2.Techniques[i].Passes[j].SelfIllumination &&
                            p.ShadingMode == m2.Techniques[i].Passes[j].ShadingMode &&
                            p.Specular == m2.Techniques[i].Passes[j].Specular &&
                            p.TextureUnits == m2.Techniques[i].Passes[j].TextureUnits))
                        {
                            return false;
                        }
                    }
                    j++;
                }
                else
                {
                    return false;
                }

                i++;
            }



            return true;
        }

        public static bool operator !=(MaterialAttributes m1, MaterialAttributes m2)
        {
            return !(m1 == m2);
        }

        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (MaterialAttributes)o);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    #endregion // MaterialAttributes
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