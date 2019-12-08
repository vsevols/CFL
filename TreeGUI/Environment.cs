using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeGUI
{
    public static class Environment
    {
        public static string PrepareDataPath(string relative)
        {
            string path;
            path = Application.ExecutablePath + "_data\\";
            if(TreeController.isDbg&&path== "D:\\Git\\Vsevols\\MyLifeResource\\TreeGUI\\bin\\Debug\\TreeGUI.exe_data\\")
                path = "d:\\Git\\Vsevols\\MyLifeResource\\TreeGUI\\_data\\";
            Directory.CreateDirectory(path);
            return path + relative;
        }
    }
}
