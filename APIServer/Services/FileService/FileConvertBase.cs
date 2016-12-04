using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace APIServer.Services.FileService
{
    public abstract class FileConvertBase
    {
        /// <summary>
        /// 将word文档转换为html，同时将html文件中的src元素的路径加上targetRelativeDirectory
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="targetRelativeDirectory">目标文件相对目录路径（相对网站根目录）</param>
        /// <returns></returns>
        abstract public bool WordToHTML(string sourcePath, string targetPath, string targetRelativeDirectory);

        abstract public bool ExcelToHTML(string sourcePath, string targetPath, string targetRelativeDirectory);

        abstract public bool PPToHTML(string sourcePath, string targetPath, string targetRelativeDirectory);

        protected static bool changePathInHtml(string htmlFilePath, string relativeDirectory)
        {
            try
            {
                string content = File.ReadAllText(htmlFilePath);
                string pattern = @"src=""([^""]*)""";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                string resultContent = regex.Replace(content, "src=\"" + relativeDirectory + "\\$1\"");
                File.WriteAllText(htmlFilePath, resultContent);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}