using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Common.FontParser
{
    public class Glyph
    {
        public int CodePoint { get; set; }
        public float U1 { get; set; }
        public float V1 { get; set; }
        public float U2 { get; set; }
        public float V2 { get; set; }

        public Glyph(int codePoint, float u1, float v1, float u2, float v2)
        {
            CodePoint = codePoint;
            U1 = u1;
            V1 = v1;
            U2 = u2;
            V2 = v2;
        }
    }
}
