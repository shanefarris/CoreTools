using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssetManager.Common.FontParser
{
    public class Parser
    {
        public FontAttributes Parse(string fontdef, ref List<string> errorLog)
        {
            var fontAttributes = new FontAttributes();

            var name = string.Empty;
            var source = string.Empty;

            var lines = Regex.Split(fontdef, "\r\n");
            int i = 0;
            while (i < lines.Count())
            {
                string str = lines[i];

                if (str.Trim().ToLower().Contains("source"))
                {
                    string temp = str.Trim();
                    fontAttributes.Source = temp.Substring(str.IndexOf("source") + 6).Trim();
                }
                else if (str.Trim().ToLower().Contains("glyph"))
                {
                    var glyph = ParseGlyph(lines[i]);
                    if (glyph == null)
                    {
                        errorLog.Add("Unable to add glyph to font: " + fontdef);
                        return null;
                    }

                    fontAttributes.Glyphs.Add(glyph);
                }
                else if (str.Trim().Length > 0 && fontAttributes.Name == null)
                {
                    fontAttributes.Name = str.Trim();
                }
                i++;
            }

            return fontAttributes;
        }

        private Glyph ParseGlyph(string line)
        {
            int codePoint = 0;
            float u1 = 0.0f;
            float v1 = 0.0f;
            float u2 = 0.0f;
            float v2 = 0.0f;

            // glyph ! 0.0859375 0.125 0.101563 0.1875
            line = line.Replace("\t", " ");
            line = line.Trim();
            var elements = line.Split(' ');
            if (elements.Length < 6)
                return null;

            if (elements[0] != "glyph")
                return null;

            if (elements[1].Length > 1)
            {
                return null;
            }
            else
            {
                codePoint = (int)elements[1].ToCharArray()[0];
            }

            if (!float.TryParse(elements[2], out u1))
                return null;
            if (!float.TryParse(elements[3], out v1))
                return null;
            if (!float.TryParse(elements[4], out u2))
                return null;
            if (!float.TryParse(elements[5], out v2))
                return null;

            return new Glyph(codePoint, u1, v1, u2, v2);
        }
    }
}
