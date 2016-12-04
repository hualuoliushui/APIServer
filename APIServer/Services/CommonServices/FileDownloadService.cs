using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

namespace APIServer.Services.CommonServices
{
    public class FileDownloadService
    {
        //负责人：李杭澍
        #region 接口:下载:DownloadDocument:http get
        #region 处理请求：string=>documentPath # HttpResponseMessage
        [HttpGet]
        //读取服务器的指定路径文件流，将其做为返回的HttpResponseMessage的Content
        public static HttpResponseMessage downloadDocument(string documentPath)
        {
            try
            {
                //获取服务器根目录
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
                string targetRelativeDirectory = @"\upfiles\html\" + System.IO.Path.GetDirectoryName(documentPath) + "\\" + targetFileName;
                //构造目标文件相对路径
                string targetPath = targetRelativeDirectory + "\\" + targetFileName + ".html";

                //获取目标文件绝对路径
                string targetAbsolutePath = rootPath + targetPath;

                //检查目标文件是否存在
                FileInfo fileInfoTgt = new FileInfo(targetAbsolutePath);
                if (!fileInfoTgt.Exists)//目标文件不存在，进行源文件转换
                {
                    //检查目标目录是否存在
                    //绝对路径
                    string targetAbsoluteDirectory = rootPath + targetRelativeDirectory;
                    if (!Directory.Exists(targetAbsoluteDirectory))//如果不存在，就创建
                    {
                        Directory.CreateDirectory(targetAbsoluteDirectory);
                    }
                    //获取源文件路径
                    string sourcePath = @"\upfiles\origin\" + documentPath;
                    string sourceAbsolutePath = rootPath + sourcePath;
                    FileInfo fileInfoSrc = new FileInfo(sourceAbsolutePath);
                    if (!fileInfoSrc.Exists)//如果源文件不存在，返回http状态码 404 给客户端进行异常处理
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }
                    //源文件存在，进行文件转换
                    FileConvertBase method = new AposeMethod();
                    switch (fileExtensionWithoutPoint)
                    {

                        //转换excel
                        case "xlsx":
                        case "xls":
                            if (!method.ExcelToHTML(sourceAbsolutePath, targetAbsolutePath, targetRelativeDirectory))
                            {
                                //文件转换失败
                                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                            }
                            break;
                        //转换word
                        case "doc":
                        case "docx":
                            if (!method.WordToHTML(sourceAbsolutePath, targetAbsolutePath, targetRelativeDirectory))
                            {
                                //文件转换失败
                                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                            }   
                            break;
                        case "ppt":
                            if (!method.PPToHTML(sourceAbsolutePath, targetAbsolutePath, targetAbsoluteDirectory))
                            {
                                //文件转换失败
                                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                            }
                            break;
                        default://不支持其他文件的转换
                            return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
                    }
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                MemoryStream ms = new MemoryStream();
                //以流的方式下载目标文件
                FileStream fs = File.OpenRead(targetAbsolutePath);

                fs.CopyTo(ms);
                fs.Close();

                response.Content = new ByteArrayContent(ms.ToArray());
                ////通知浏览器下载文件而不是打开文件
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //解决文件名存在中文而出现乱码的问题
                documentPath = System.Web.HttpUtility.UrlEncode(documentPath, System.Text.Encoding.UTF8);
                //以附件的形式下载，可在这里更改下载下来的文件的名字
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("Inline") { FileName = documentPath };

                return response;
            }
            catch(Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

        }
        #endregion
        #endregion
    }
}