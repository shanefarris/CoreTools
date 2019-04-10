using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Common.FontParser
{
    public class FontAttributes
    {
        public FontType Type { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public int Resolution { get; set; }
        public List<Glyph> Glyphs { get; set; }

        public FontAttributes()
        {
            Type = FontType.Image;
            Glyphs = new List<Glyph>();
        }
    }
}
