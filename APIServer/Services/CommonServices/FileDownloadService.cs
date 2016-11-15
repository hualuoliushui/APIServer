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

namespace ApplicationServer_API.Services.CommonServices
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
                //服务器文件路径
                string root = @"\upfiles\PDF\" + @documentPath;
                var FilePath = System.Web.Hosting.HostingEnvironment.MapPath(root);
                //文件不存在时，返回http状态码 404 给客户端进行异常处理
                if (!System.IO.File.Exists(FilePath))
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                //以流的方式下载文件
                var stream = new FileStream(FilePath, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                ////通知浏览器下载文件而不是打开文件
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //解决文件名存在中文而出现乱码的问题
                documentPath = System.Web.HttpUtility.UrlEncode(documentPath, System.Text.Encoding.UTF8);
                //以附件的形式下载，可在这里更改下载下来的文件的名字
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = documentPath };
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

        }
        #endregion
        #endregion
    }
}