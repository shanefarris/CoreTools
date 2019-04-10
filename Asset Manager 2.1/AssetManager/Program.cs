using System;
using System.IO;
using System.Windows.Forms;

namespace AssetManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Directory.Exists(Application.StartupPath + "/sandbox"))
            {
                Directory.CreateDirectory(Application.StartupPath + "/sandbox");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
            Clean();
        }

        static void Clean()
        {
            var dir = new DirectoryInfo(Application.StartupPath);
            foreach (var fi in dir.GetFiles())
            {
                if (fi.Extension == ".mesh")
                    File.Delete(fi.FullName);
            }

            if (Directory.Exists(Application.StartupPath + "/sandbox"))
                Directory.Delete(Application.StartupPath + "/sandbox", true);
        }
    }
}
