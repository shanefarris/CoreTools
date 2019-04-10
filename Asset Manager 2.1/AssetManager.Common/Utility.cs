using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AssetManager.Common
{
    public class Utility
    {
        public static void FileSearch(string sDir, ref List<string> FileList, List<string> FileTypes)
        {
            try
            {
                if (!sDir.Contains(".svn"))
                {
                    foreach (string f in Directory.GetFiles(sDir))
                    {
                        var fi = new FileInfo(f);
                        if (FileTypes.Contains(fi.Extension.Replace(".", "")))
                            FileList.Add(f);
                    }
                }

                foreach (string d in Directory.GetDirectories(sDir))
                {

                    FileSearch(d, ref FileList, FileTypes);
                }
            }
            catch (Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        public static void DirSearch(string sDir, ref List<string> list)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    list.Add(d);
                    DirSearch(d, ref list);
                }
            }
            catch (Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        public static byte[] ConvertStringToBytes(string input)
        {
            var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(input);
                writer.Flush();
            }

            return stream.ToArray();
        }

        public static string ConvertBytesToString(byte[] bytes)
        {
            string output;
            var stream = new MemoryStream(bytes) { Position = 0 };
            using (var reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }

            return output;
        }

        public static string[] ConvertBytesToStringArray(byte[] bytes)
        {
            var output = ConvertBytesToString(bytes);
            return output.Replace("\n\r", "\n").Replace("\r\n", "\n").Split('\n');
        }

        public static bool IsImageExtension(FileInfo fi)
        {
            var extension = fi.Extension.ToLower();
            return IsImageExtension(extension);
        }

        public static bool IsImageExtension(string extension)
        {
            if (extension == ".png" ||
                extension == ".jpg" ||
                extension == ".jepg" ||
                extension == ".dds" ||
                extension == ".tga" ||
                extension == ".tif" ||
                extension == ".tiff" ||
                extension == ".bmp")
            {
                return true;
            }
            return false;
        }

        public static string GetExportImage()
        {
            return Properties.Settings.Default.ExportImage.ToLower();
        }

        public static void SetExportImage(string imageFormat)
        {
            Properties.Settings.Default.ExportImage = imageFormat;
            Properties.Settings.Default.Save();
        }

        public static string GetExportImageExtension()
        {
            return "." + Properties.Settings.Default.ExportImage.ToLower();
        }

        public static DbId GetDefaultImageId()
        {
            return Properties.Settings.Default.DefaultImageId;
        }

        public static string ReplaceExtension(string filename, string extension)
        {
            if (filename.Contains("."))
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;
                var index = filename.LastIndexOf(".");
                var ret = filename.Remove(index);
                return ret + extension;
            }
            return filename;
        }
    }
}