using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using APIServer.Services;
using APIServer.Services;
using System.Configuration;
using APIServer.Services.FileService;
using System.Threading;
using System.Web.Caching;
using System.Web;
using APIServer.Models;
namespace APIServer.Services.CommonServices
{
    public class FileDownloadService
    {
        private static Cache filesCache = HttpRuntime.Cache;

        //读取服务器的指定路径文件流，将其做为返回的HttpResponseMessage的Content
        public Status downloadDocument(string documentPath,out HttpResponseMessage response)
        {
            response = new HttpResponseMessage();
            //Log.DebugInfo("开始处理下载文件请求");
            Mutex mutex = null;
            try
            {
                #region 获取文件路径、目录、文件名等
                //Log.DebugInfo("获取服务器根目录");
                string rootPath = System.Web.Hosting.HostingEnvironment.MapPath("\\");
                //获取文件扩展名
                string fileExtension = System.IO.Path.GetExtension(documentPath);

                //获取文件名,无扩展名
                string fileName = System.IO.Path.GetFileNameWithoutExtension(documentPath);

                //去除文件扩展名的"."
                string fileExtensionWithoutPoint = fileExtension.Substring(1, fileExtension.Length - 1);

                //构造目标文件名
                string targetFileName = fileName + "_" + fileExtensionWithoutPoint;
                //构造目标文件相对目录,以目标文件名创建 目录
                string targetRelativeDirectory = @"upfiles\html" + System.IO.Path.GetDirectoryName(documentPath) + "\\" + targetFileName;
                //构造目标文件相对路径
                string targetPath = targetRelativeDirectory + "\\" + targetFileName + ".html";

                //获取目标文件绝对路径
                string targetAbsolutePath = rootPath + targetPath; 
                #endregion

                #region 检查目标文件信息
                FileInfo fileInfoTgt = new FileInfo(targetAbsolutePath);

                try
                {
                    while (!fileInfoTgt.Exists)//目标文件不存在，进行源文件转换
                    {
                        //检查目标目录是否存在
                        //绝对路径
                        string targetAbsoluteDirectory = rootPath + targetRelativeDirectory;

                        if (!Directory.Exists(targetAbsoluteDirectory))//如果不存在，就创建
                        {
                            Directory.CreateDirectory(targetAbsoluteDirectory);
                        }

                        mutex = new Mutex();
                        mutex.WaitOne();//等待其他线程转换文件
                        if (filesCache[targetAbsolutePath] != null)
                        {
                            //如果文件已转换，退出转换
                            break;
                        }

                        #region 进行文件转换

                        //资源路径
                        targetRelativeDirectory = "\\" + targetRelativeDirectory;

                        //获取源文件路径
                        string sourcePath = @"upfiles\origin" + documentPath;
                        string sourceAbsolutePath = rootPath + sourcePath;
                        FileInfo fileInfoSrc = new FileInfo(sourceAbsolutePath);
                        if (!fileInfoSrc.Exists)//如果源文件不存在，返回http状态码 404 给客户端进行异常处理
                        {
                            response.StatusCode = HttpStatusCode.NotFound;
                            return Status.NOTFOUND;
                        }
                        Log.DebugInfo("源文件存在，进行文件转换");
                        FileConvertBase method = new OfficeMethod();
                        switch (fileExtensionWithoutPoint)
                        {

                            //转换excel
                            case "xlsx":
                            case "xls":
                                if (!method.ExcelToHTML(sourceAbsolutePath, targetAbsolutePath, targetRelativeDirectory))
                                {
                                    //文件转换失败
                                    return Status.FILE_CONVERT_FAIL;
                                }
                                break;
                            //转换word
                            case "doc":
                            case "docx":
                                Log.DebugInfo(method.ToString());
                                Log.DebugInfo("准备转化word");
                                if (!method.WordToHTML(sourceAbsolutePath, targetAbsolutePath, targetRelativeDirectory))
                                {
                                    Log.DebugInfo("文件转换失败");
                                    return Status.FILE_CONVERT_FAIL;
                                }
                                Log.DebugInfo("文件转化成功");
                                break;
                            case "ppt":
                            case "pptx":
                                if (!method.PPToHTML(sourceAbsolutePath, targetAbsolutePath, targetRelativeDirectory))
                                {
                                    //文件转换失败
                                    return Status.FILE_CONVERT_FAIL;
                                }
                                break;
                            default://不支持其他文件的转换
                                return Status.FILE_CONVERT_FAIL;
                        }
                        fileInfoTgt = new FileInfo(targetAbsolutePath);
                        #endregion
                    }
                }
                catch (Exception e)
                {
                    Log.LogInfo("文件转换", e);
                    return Status.FILE_CONVERT_EXCEPTION;
                }
                finally
                {
                    
                } 
                #endregion

                response.StatusCode = HttpStatusCode.OK;

                MemoryStream ms = null;

                #region 检查是否存在此文件的缓存
                if (filesCache[targetAbsolutePath] == null)
                {
                    //以流的方式下载目标文件
                    FileStream fs = new FileStream(targetAbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    MemoryStream temp = new MemoryStream();
                    fs.CopyTo(temp);
                    fs.Close();
                    filesCache.Add(targetAbsolutePath, temp, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero, CacheItemPriority.Default,null);
                } 
                #endregion

                ms = (MemoryStream)filesCache.Get(targetAbsolutePath);
           
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
            catch(Exception e)
            {
                Log.LogInfo("文件转化", e);
                return Status.FILE_CONVERT_EXCEPTION;
            }
            finally
            {
                Log.DebugInfo("退出前，释放锁");
                while (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex = null;
                }
            }

        }
    }
}