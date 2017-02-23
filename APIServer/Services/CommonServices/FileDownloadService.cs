using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Caching;
using System.Web;
using APIServer.Models;
using System.Configuration;
namespace APIServer.Services.CommonServices
{
    public class FileDownloadService
    {
        private static Cache filesCache = HttpRuntime.Cache;

        private static string separator = Path.DirectorySeparatorChar.ToString();

        private string GetHtmlDir()
        {
            string htmlDir = ConfigurationManager.AppSettings["HtmlDir"];
            if (string.IsNullOrEmpty(htmlDir))
            {
                htmlDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download");
            }

            if (!Directory.Exists(htmlDir))
            {
                Directory.CreateDirectory(htmlDir);
            }
            return htmlDir;
        }

        private string GetHtmlRelativeDir()
        {
            string htmlRelativeDir = ConfigurationManager.AppSettings["htmlRelativeDir"];

            if (string.IsNullOrEmpty(htmlRelativeDir))
            {
                htmlRelativeDir = separator + "Download" + separator;
            }

            return htmlRelativeDir;
        }

        //返回字符串中，最后一个/之前包括/的字符串
        private string GetDirectoryName(string path)
        {
            int lastPosition = path.LastIndexOf(Path.DirectorySeparatorChar);
            return path.Substring(0, lastPosition +1 );
        }

        //读取服务器的指定路径文件流，将其做为返回的HttpResponseMessage的Content
        public Status downloadDocument(string documentPath, out HttpResponseMessage response)
        {
            response = new HttpResponseMessage();
            //Log.DebugInfo("开始处理下载文件请求");

            int fileDownloadCachingTime_Second = Int32.Parse(ConfigurationManager.AppSettings["fileDownloadCachingTime_Second"]);
           
            try
            {
                #region 获取文件路径、目录、文件名等
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(documentPath);

                string fileName = fileNameWithoutExtension + ".html";

                string targetDir = GetHtmlDir() + fileNameWithoutExtension + separator;

                string targetPath = targetDir + fileName;

                #endregion

                #region 检查目标文件信息
                FileInfo fileInfoTgt = new FileInfo(targetPath);

                if (!fileInfoTgt.Exists)//目标文件不存在，直接返回
                {
                    return Status.NOTFOUND;
                }
                #endregion

                response.StatusCode = HttpStatusCode.OK;

                MemoryStream ms = null;

                #region 检查是否存在此文件的缓存
                if (filesCache[targetPath] == null)
                {
                    //以流的方式下载目标文件
                    FileStream fs = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    MemoryStream temp = new MemoryStream();
                    fs.CopyTo(temp);
                    fs.Close();
                    filesCache.Add(targetPath, temp, null, DateTime.Now.AddSeconds(fileDownloadCachingTime_Second), TimeSpan.Zero, CacheItemPriority.Default, null);
                }
                #endregion

                ms = (MemoryStream)filesCache.Get(targetPath);

                #region 设置返回信息
                response.Content = new ByteArrayContent(ms.ToArray());
                ////通知浏览器下载文件而不是打开文件
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //解决文件名存在中文而出现乱码的问题
                documentPath = System.Web.HttpUtility.UrlEncode(documentPath, System.Text.Encoding.UTF8);
                //以附件的形式下载，可在这里更改下载下来的文件的名字
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("Inline") { FileName = documentPath };
                #endregion

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.LogInfo("文件浏览", e);
                return Status.FAILURE;
            }
        }
    }
}