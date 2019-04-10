using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialParser
{
    public enum SceneBlendFactor
    {
        SBF_ONE,
        SBF_ZERO,
        SBF_DEST_COLOUR,
        SBF_SOURCE_COLOUR,
        SBF_ONE_MINUS_DEST_COLOUR,
        SBF_ONE_MINUS_SOURCE_COLOUR,
        SBF_DEST_ALPHA,
        SBF_SOURCE_ALPHA,
        SBF_ONE_MINUS_DEST_ALPHA,
        SBF_ONE_MINUS_SOURCE_ALPHA

    };

    public enum SceneBlendType
	{
		SBT_TRANSPARENT_ALPHA,
		SBT_TRANSPARENT_COLOUR,
		SBT_ADD,
		SBT_MODULATE,
		SBT_REPLACE
	};

    public enum FogMode
	{
		FOG_NONE,
		FOG_EXP,
		FOG_EXP2,
		FOG_LINEAR
	};

    public enum ShadeOptions
	{
		SO_FLAT,
		SO_GOURAUD,
		SO_PHONG
	};

    public enum ManualCullingMode
	{
		MANUAL_CULL_NONE = 1,
		MANUAL_CULL_BACK = 2,
		MANUAL_CULL_FRONT = 3
	};

    public enum CullingMode
	{
		CULL_NONE = 1,
		CULL_CLOCKWISE = 2,
		CULL_ANTICLOCKWISE = 3
	};

    public enum CompareFunction
	{
		CMPF_ALWAYS_FAIL,
		CMPF_ALWAYS_PASS,
		CMPF_LESS,
		CMPF_LESS_EQUAL,
		CMPF_EQUAL,
		CMPF_NOT_EQUAL,
		CMPF_GREATER_EQUAL,
		CMPF_GREATER,
        NONE
	};
}
