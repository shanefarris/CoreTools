using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AssetManager.Common.FontParser;

namespace AssetManager.Code_Generators
{
    public class FontGenerator
    {
        public void CreateSource(string fileName, string outputDir, List<FontAttributes> fontAttributesList)
        {
            using (var streamWriter = new StreamWriter(outputDir + "\\" + fileName))
            {
                streamWriter.WriteLine("#include \"OgreFont.h\"");
                streamWriter.WriteLine("#include \"OgreFontManager.h\"");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using namespace Ogre;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("void LoadFonts()");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("FontPtr font;");
                streamWriter.WriteLine("int textureAspect = 10;");
                streamWriter.WriteLine();

                foreach (var fa in fontAttributesList)
                {
                    streamWriter.WriteLine("// ============================ " + fa.Name + " ============================================");
                    streamWriter.WriteLine("font = FontManager::getSingleton().create(\"" + fa.Name + "\", \"General\");");
                    streamWriter.WriteLine("font->setSource(\"" + fa.Source + "\");");
                    streamWriter.WriteLine("font->setType(FontType::FT_IMAGE);");

                    foreach (var g in fa.Glyphs)
                    {
                        streamWriter.WriteLine("font->setGlyphTexCoords(" + g.CodePoint + ", " + g.U1 + "f, " + g.V1 + "f, " + g.U2 + "f, " + g.V2 + "f, textureAspect);");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("}");

                streamWriter.Close();
            }
        }
    }
}
