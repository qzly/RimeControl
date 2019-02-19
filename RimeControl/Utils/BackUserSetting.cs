using RimeControl.Entitys;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace RimeControl.Utils
{
    /// <summary>
    /// 以前的压缩方法，依赖7z，备份
    /// </summary>
    class BackUserSetting
    {
        string strPrejectRootPath = "";
        string strBackupsFolder = "";
        string str7zExePath = "";

        public BackUserSetting()
        {
            strPrejectRootPath = AppDomain.CurrentDomain.BaseDirectory;
            str7zExePath = strPrejectRootPath + "7-Zip\\7z.exe";
            strBackupsFolder = strPrejectRootPath + "Backups\\";
        }

        /// <summary>
        /// 检查7z是否存在
        /// </summary>
        /// <returns></returns>
        public bool Check7ZExits()
        {
            string str7ZPath = strPrejectRootPath + "7-Zip";
            return (File.Exists(str7ZPath + "\\7z.exe") && File.Exists(str7ZPath + "\\7z.dll"));
        }

        /// <summary>
        /// 载入备份信息
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<BackAndRestoreItems> LoadBackups()
        {
            //检查备份目录
            CheckBackupsFolder();
            ObservableCollection<BackAndRestoreItems> dgBackAndRestoreItem = new ObservableCollection<BackAndRestoreItems>();
            //获取备份目录下所有的文件
            string[] strFiles = Directory.GetFiles(strBackupsFolder);
            //遍历文件信息
            foreach (string file in strFiles)
            {
                string filesNmae = Regex.Match(file, "(20\\d+\\.7z)").Value;
                string fileTimeYmd = Regex.Match(filesNmae, "20\\d{2}[01]\\d[0123]\\d").Value;
                fileTimeYmd = Regex.Replace(fileTimeYmd, "(.{4})(.{2})(.{2})", "$1-$2-$3");
                string fileTimeHms = Regex.Match(filesNmae, "(?<=(20\\d{2}[01]\\d[0123]\\d))[012]\\d[012345]\\d[012345]\\d").Value;
                fileTimeHms = Regex.Replace(fileTimeHms, "(.{2})(.{2})(.{2})", "$1:$2:$3");
                fileTimeYmd += " " + fileTimeHms;
                FileInfo myFileInfo = new FileInfo(file);
                float fileSize = Convert.ToSingle(myFileInfo.Length / 1024.0 / 1024.0);

                dgBackAndRestoreItem.Add(new BackAndRestoreItems
                {
                    ItemFileName = filesNmae,
                    ItemFileTime = fileTimeYmd,
                    ItemFileSize = fileSize
                });
            }
            return dgBackAndRestoreItem;
        }

        /// <summary>
        /// 备份用户设置
        /// </summary>
        /// <param name="strUserFolderPath"></param>
        /// <returns></returns>
        public bool BackUserCustomFile(string strUserFolderPath)
        {
            //Process closeWeaselServer=new Process
            //{
            //    StartInfo =
            //    {
            //        FileName = strPrejectRootPath+"RimeColse.exe"
            //    }
            //};
            //closeWeaselServer.Start();
            //closeWeaselServer.WaitForExit();
            //检查备份目录
            CheckBackupsFolder();
            //构建备份文件的 路径+文件名
            string strDataTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string strArchiveFileName = strBackupsFolder + strDataTime + ".7z";
            //需要备份的文件路径
            string strBackFiles = strUserFolderPath + "\\installation.yaml " +
                                  strUserFolderPath + "\\user.yaml " +
                                  strUserFolderPath + "\\*.custom.yaml " +
                                  strUserFolderPath + "\\*.userdb.kct " +
                                  strUserFolderPath + "\\*.userdb.txt " +
                                  strUserFolderPath + "\\*.userdb.kct.snapshot";



            //调用7z进行备份
            Process myProcess = new Process
            {
                StartInfo =
                {
                    FileName = str7zExePath,
                    Arguments = $"a -t7z {strArchiveFileName} {strUserFolderPath}"
                }
            };
            myProcess.Start();
            myProcess.WaitForExit();

            return true;
        }

        /// <summary>
        /// 还原用户设置
        /// </summary>
        /// <param name="strUserFolderPath"></param>
        /// <param name="strFileName"></param>
        public void RestoreUserCustomFile(string strUserFolderPath, string strFileName)
        {
            //检查备份目录
            CheckBackupsFolder();
            strFileName = strBackupsFolder + strFileName;//拼接还原文件
            //处理用户目录，解压到用户名上层目录
            //F:\Users\小狼毫配置 ---> F:\Users
            string strDir = strUserFolderPath.Substring(0, strUserFolderPath.LastIndexOf("\\"));

            Process myProcess = new Process
            {
                StartInfo =
                {
                    FileName = str7zExePath,
                    Arguments = $"x {strFileName} -o{strDir} -y"
                }
            };
            myProcess.Start();
            myProcess.WaitForExit();
        }
        /// <summary>
        /// 删除备份
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public bool DeleteBackupFile(string strFileName)
        {
            strFileName = strBackupsFolder+ strFileName;
            try
            {
                File.Delete(strFileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检查备份目录
        /// </summary>
        private void CheckBackupsFolder()
        {
            if (!Directory.Exists(strBackupsFolder))
            {
                Directory.CreateDirectory(strBackupsFolder);
            }
        }
    }
}
