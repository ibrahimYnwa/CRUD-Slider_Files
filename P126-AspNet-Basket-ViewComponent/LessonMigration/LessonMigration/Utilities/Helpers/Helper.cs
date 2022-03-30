using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LessonMigration.Utilities.Helpers
{
    public class Helper           //helper common du her yerde el catan olsun
    {
        public static string GetFilePath(string root, string folder, string fileName)
        {
            return Path.Combine(root, "img", fileName);
        }  

        public static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        
    }
}
