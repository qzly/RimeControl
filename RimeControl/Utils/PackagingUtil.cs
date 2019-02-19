using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RimeControl.Utils
{
    public class PackagingUtil
    {
        /// <summary>
        /// 将文件夹及其子文件夹添加到包
        /// </summary>
        /// <param name="folderName">要添加的目录</param>
        /// <param name="compressedFileName">要创建的包路径</param>
        /// <param name="overrideExisting">覆盖已经存在的包</param>
        /// <returns></returns>
        public static int PackageFolder(string folderName, string compressedFileName, bool overrideExisting)
        {
            //去掉目录路径末尾的\
            folderName = folderName.EndsWith(@"\") ? folderName.Remove(folderName.Length - 1) : folderName;

            int intR = 0;
            if (Directory.Exists(folderName) && (overrideExisting || !File.Exists(compressedFileName)))
            {
                try
                {
                    using (Package package = Package.Open(compressedFileName, FileMode.Create))
                    {
                        //获取压缩路径下的所有子目录和子文件
                        var fileList = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);
                        foreach (string fileName in fileList)
                        {
                            //获取子目录和子文件夹的相对路径
                            string pathInPackage = Path.GetDirectoryName(fileName)?.Replace(folderName, string.Empty) + "/" + Path.GetFileName(fileName);
                            //文件/文件夹的url
                            Uri partUriDocument = PackUriHelper.CreatePartUri(new Uri(pathInPackage, UriKind.Relative));
                            //添加文件
                            PackagePart packagePartDocument = package.CreatePart(partUriDocument, "", CompressionOption.Maximum);
                            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                            {
                                if (packagePartDocument != null)
                                {
                                    fileStream.CopyTo(packagePartDocument.GetStream());
                                }
                            }
                        }
                    }
                    intR = 1;
                }
                catch (Exception e)
                {
                    intR = -1;
                    Console.WriteLine(e);
                }
            }
            return intR;
        }

        /// <summary>
        /// 单文件压缩
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="compressedFileName">压缩包路径</param>
        /// <param name="overrideExisting">是否覆盖</param>
        /// <returns></returns>
        public static int PackageFile(string fileName, string compressedFileName, bool overrideExisting)
        {
            int intR = 0;
            if (File.Exists(fileName) && (overrideExisting || !File.Exists(compressedFileName)))
            {
                try
                {
                    string filePath = Path.GetFileName(fileName);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        Uri partUriDocument = PackUriHelper.CreatePartUri(new Uri(filePath, UriKind.Relative));

                        using (Package package = Package.Open(compressedFileName, FileMode.OpenOrCreate))
                        {
                            if (package.PartExists(partUriDocument))
                            {
                                package.DeletePart(partUriDocument);
                            }

                            PackagePart packagePartDocument = package.CreatePart(partUriDocument, "", CompressionOption.Maximum);
                            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                            {
                                if (packagePartDocument != null) fileStream.CopyTo(packagePartDocument.GetStream());
                            }
                        }
                        intR = 1;
                    }
                }
                catch (Exception e)
                {
                    intR = -1;
                    Console.WriteLine(e);
                }
            }
            return intR;
        }


        /// <summary>
        /// zip解压缩 NOTE: container must be created as Open Packaging Conventions (OPC) specification
        /// </summary>
        /// <param name="folderName">要将包解压到的文件夹</param>
        /// <param name="compressedFileName">包文件</param>
        /// <param name="overrideExisting">覆盖</param>
        /// <returns></returns>
        public static int ExtractFile(string folderName, string compressedFileName, bool overrideExisting)
        {
            int intR = 0;
            try
            {
                if (File.Exists(compressedFileName))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
                    if (!directoryInfo.Exists)
                        directoryInfo.Create();

                    using (Package package = Package.Open(compressedFileName, FileMode.Open, FileAccess.Read))
                    {
                        foreach (PackagePart packagePart in package.GetParts())
                        {
                            string stringPart = folderName + HttpUtility.UrlDecode(packagePart.Uri.ToString()).Replace('\\', '/');

                            string dirPath = Path.GetDirectoryName(stringPart);
                            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }

                            if (overrideExisting || !File.Exists(stringPart))
                            {
                                using (FileStream fileStream = new FileStream(stringPart, FileMode.Create))
                                {
                                    packagePart.GetStream().CopyTo(fileStream);
                                }
                            }
                        }
                    }

                    intR = 1;
                }
            }
            catch (Exception e)
            {
                intR = -1;
                Console.WriteLine(e);
            }

            return intR;
        }
    }
}
