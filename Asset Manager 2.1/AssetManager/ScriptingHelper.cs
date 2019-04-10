using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AssetManager
{
    public class ScriptingHelper
    {
        public enum Language
        {
            CS,
            Lua,
            LuaPP
        };

        public void PopulateListControl(CheckedListBox clb, string dir)
        {
            try
            {
                var di = new DirectoryInfo(dir);
                if (!di.Exists)
                {
                    MessageBox.Show(@"Invalid directory.");
                    return;
                }

                foreach (string d in Directory.GetDirectories(dir))
                {
                    if (d.Contains(".svn"))
                    {
                        // Don't add svn
                    }
                    else if (d.Contains("Bullet") ||
                             d.Contains("Ode") ||
                             d.Contains("Nature") ||
                             d.Contains("Network") ||
                             d.Contains("NxOgre") ||
                             d.Contains("Plugins") ||
                             d.Contains("RuntimeEditor") ||
                             d.Contains("VisualDebugger") ||
                             d.Contains("FileDatabase") ||
                             d.Contains("AgentSmith"))
                    {
                        clb.Items.Add(d, false);
                        PopulateListControl(clb, d);
                    }
                    else
                    {
                        clb.Items.Add(d, true);
                        PopulateListControl(clb, d);
                    }
                }
            }
            catch (Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        public void GeneratePackages(Language language, CheckedListBox clb, string outputDir, ListBox ingnoreList, TextBox example)
        {
            var di = new DirectoryInfo(outputDir);
            if (di.Exists)
                di.Delete(true);

            di.Create();
            example.Text = string.Empty;

            if (language == Language.CS)
            {
                GeneratePackagesCS(clb, outputDir, ingnoreList, example);
            }
            else if (language == Language.Lua)
            {
                MessageBox.Show("Not supported yet");
            }
            else if(language == Language.LuaPP)
            {
                GeneratePackagesLuaPP(clb, outputDir, ingnoreList, example);
            }
        }

        private void GeneratePackagesCS(CheckedListBox clb, string outputDir, ListBox ingnoreList, TextBox example)
        {
            var headersUsed = new List<string>();

            example.Text += "swig -c++ -csharp - module FpsFramework.dll ./CS_Exports.h \r\n";
            using (var writer = new StreamWriter("./CS_Exports.h"))
            {
                // Go through each directory
                foreach (string dir in clb.CheckedItems)
                {
                    var itemsDir = new DirectoryInfo(dir);

                    // Go through each file in the directory
                    foreach (FileInfo file in itemsDir.GetFiles())
                    {
                        if ((file.Extension == ".h" || file.Extension == ".hpp") &&
                            !ingnoreList.Items.Contains(file.Name))
                        {
                            headersUsed.Add(file.Name);

                            using (var reader = new StreamReader(file.FullName))
                            {
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();

                                    // Filters
                                    if (line.Contains(">>") && line.Contains("typedef"))
                                    {
                                        line = line.Replace(">>", "> >");
                                    }
                                    else if (line.Contains("CORE_EXPORT"))
                                    {
                                        line = line.Replace("CORE_EXPORT", string.Empty);
                                    }

                                    writer.WriteLine(line);
                                }
                            }
                        }
                    } // foreach

                } // foreach
            }

            foreach (var name in headersUsed)
            {
                example.Text += "#include \"" + name + "\"\r\n";
            }
        }

        private void GeneratePackagesLuaPP(CheckedListBox clb, string outputDir, ListBox ingnoreList, TextBox example)
        {
            var pkgsCreated = new List<string>();

            foreach (string dir in clb.CheckedItems)
            {
                var itemsDir = new DirectoryInfo(dir);
                string tempFileName = outputDir + "\\" + itemsDir.Name + ".temp";
                pkgsCreated.Add(tempFileName);
                using (var writer = new StreamWriter(tempFileName))
                {
                    example.Text += "tolua++ -n " +
                                    itemsDir.Name +
                                    " -o " + itemsDir.Name +
                                    "_lua.cpp -H " + itemsDir.Name + "_lua.h ./" + itemsDir.Name + " \r\n";

                    //  Keep track of all the header files that are being accessed
                    var headerFiles = new List<string>();

                    foreach (FileInfo file in itemsDir.GetFiles())
                    {
                        if (file.Name != "pkg" && !ingnoreList.Items.Contains(file.Name) &&
                            (file.Extension == ".h" || file.Extension == ".hpp"))
                        {
                            // Add to our total header files
                            headerFiles.Add(file.Name);

                            using (var reader = new StreamReader(file.FullName))
                            {
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();

                                    // **************** Validate the line *************************

                                    // Many issues with this
                                    Debug.Assert(line != null, "GeneratePackages(): line != null");
                                    if (line.Trim().StartsWith("#"))
                                        line = string.Empty;
                                    // Not needed
                                    if (line.Trim().StartsWith("typedef") && !line.Contains("typedef struct"))
                                        line = string.Empty;
                                    // Variable definition
                                    if (line.Contains("NUM_TEAMS"))
                                        line = string.Empty;
                                    // Doesn't like friends
                                    if (line.Trim().StartsWith("friend"))
                                        line = string.Empty;
                                    // Don't need this
                                    if (line.Trim().StartsWith("using namespace"))
                                        line = string.Empty;
                                    // Has a problem with dereferencing
                                    if (line.Contains("** ") && !line.Contains("/*"))
                                        line = string.Empty;

                                    // Core specfic
                                    line = line.Replace("CORE_EXPORT", "");
                                    line = line.Replace("NATURE_EXPORT", "");
                                    line = line.Replace("__declspec(dllexport)", "");
                                    line = line.Replace("virtual", "");
                                    line = line.Replace("f32", "float");
                                    line = line.Replace("s32", "int");
                                    line = line.Replace("u32", "unsigned int");
                                    line = line.Replace("c8", "char");
                                    line = line.Replace("s8", "signed char");
                                    line = line.Replace("u8", "unsigned char");

                                    // AI specific
                                    line = line.Replace("template <class Super>", "");
                                    line = line.Replace("template<>", "");
                                    line = line.Replace("template <class ContentType>", "");
                                    line =
                                        line.Replace(
                                            "template< class PathAlike, class Mapping, class BaseDataExtractionPolicy = PointToPathAlikeBaseDataExtractionPolicy< PathAlike > >",
                                            "");
                                    line = line.Replace("template< class PathAlike, class Mapping >", "");
                                    line =
                                        line.Replace(
                                            "template< class PathAlike, class Mapping, class BaseDataExtractionPolicy = DistanceToPathAlikeBaseDataExtractionPolicy< PathAlike > > ",
                                            "");
                                    line = line.Replace("template< class PathAlike >", "");

                                    if (line.Length > 0)
                                    {
                                        writer.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }

                    // Print out a list of headers to paste into the generated cpp files
                    if (headerFiles.Count > 0)
                    {
                        example.Text += "********************************\r\n";
                        example.Text += itemsDir.Name + "\r\n";
                        foreach (var header in headerFiles)
                            example.Text += "#include \"" + header + "\"\r\n";
                        example.Text += "********************************\r\n";
                    }
                }
            }
            FilterFiles(pkgsCreated);
        }

        private void FilterFiles(IEnumerable<string> fileList)
        {
            foreach (string str in fileList)
            {
                string fileName = str.Replace(".temp", "");
                using (var writer = new StreamWriter(fileName))
                {
                    using (var reader = new StreamReader(str))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line != null && (line.Trim() == "private:" ||
                                                 line.Trim() == "protected:"))
                            {
                                writer.WriteLine(FilterPrivate(reader));
                            }
                            else
                            {
                                writer.WriteLine(line);
                            }
                        }
                    }
                }

                // Delete temp file
                var fi = new FileInfo(str);
                if (fi.Exists)
                    fi.Delete();
            }
        }

        private string FilterPrivate(StreamReader reader)
        {
            int count = 0;
            // Read through the stream until the private or protected members are skipped.
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                Debug.Assert(line != null, "FilterPrivate(): line != null");
                if (line.Trim() == "public:")
                {
                    return "public:";
                }
				else if ((line.Contains("enum ") || line.Contains("struct ") || line.Contains("typedef struct")) && !line.Contains(";"))
				{
					FilterStructures(reader);
				}
                else if (line.Contains("{") && !line.Contains("}"))
                {
                    count++;
                }
				else if ((line.Contains("}") || line.Contains("};")) && !line.Contains("{"))
                {
                    count--;
					if (count <= 0)
						return "};";
                }
            }
            return string.Empty;
        }

		private string FilterStructures(StreamReader reader)
		{
			int count = 0;
			// Read through the stream until the enums, or structs are skipped.  This is to filter out private enums or structs.
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				Debug.Assert(line != null, "FilterStructures(): line != null");
				if (line.Trim() == "public:")
				{
					return "public:";
				}
				else if (line.Contains("{") && !line.Contains("}"))
				{
					count++;
				}
				else if (line.Contains("};") || line.Contains("}"))
				{
					count--;
					if(count == 0)
						return string.Empty;
				}
			}
			return string.Empty;
		}
    }
}