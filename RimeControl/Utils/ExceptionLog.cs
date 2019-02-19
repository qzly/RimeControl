using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimeControl.Utils
{
    public class ExceptionLog
    {
        public static void WriteLog(Exception ex)
        {
            string strLogPath = AppDomain.CurrentDomain.BaseDirectory+"Log";
            if (!Directory.Exists(strLogPath))
            {
                Directory.CreateDirectory(strLogPath);
            }

            string logPath = strLogPath + "\\" + "erroLog_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") +".log";
            //保存错误信息到磁盘
            File.WriteAllText(logPath, ex.ToString());
            //在文件文件中管理器中定位文件
            System.Diagnostics.Process.Start("Explorer", "/select," + logPath);
        }
    }
}
