using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            string tempPath = Path.GetTempPath() + "FaceAI\\";
            System.IO.Directory.CreateDirectory(tempPath);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HomePage(tempPath));
            Application.Exit();

            DirectoryInfo d = new DirectoryInfo(tempPath);
            try
            {
                foreach (var file in d.GetFiles("*.*"))
                {
                    File.Delete($"{tempPath}{file.FullName}");
                }

                Directory.Delete(tempPath, true);
            }
            catch (Exception) { }
        }
    }
}
