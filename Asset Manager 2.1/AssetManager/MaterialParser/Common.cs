using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager;

namespace MaterialParser
{
    public class Vector4D
	{
		public float x, y, z, w;
		public Vector4D(float _x, float _y, float _z, float _w)
		{
			x = _x;
			y = _y;
			z = _z;
			w = _w;
		}
	}

    public class Vector5D
    {
        public float x, y, z, w, a;
        public Vector5D(float _x, float _y, float _z, float _w, float _a)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
            a = _a;
        }
    }

    public class PassFog
	{
		public bool IsOverrideScene { get; set; }
		public Vector4D Color { get; set; }
		public FogMode Mode { get; set; }
		public float ExpDensity { get; set; }
		public float LinearStart {get; set; }
		public float LinearEnd { get; set; }
	}

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

        private string _texture;
        public string Texture 
        {
            get
            {
                return _texture;
            }

            set
            {
                if (AssetManager.Common.Utility.GetExportImageExtension() != "None")
                {
                    _texture = AssetManager.Common.Utility.ReplaceExtension(value, AssetManager.Common.Utility.GetExportImageExtension());
                }
            }
        }
    }
}
