using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetManager;

namespace AssetManager.Common.MaterialParser
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
}
