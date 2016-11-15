using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace APIServer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //强制将返回值改为json格式
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            // 在出现未处理的错误时运行的代码
            string errorLog = Server.MapPath("~/App_Data/Error.log");
            System.IO.FileStream fs = new System.IO.FileStream(errorLog, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            Exception ex = Server.GetLastError();
            string IP = "";//客户端IP
            HttpRequest request = HttpContext.Current.Request;
            string result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                IP = request.ServerVariables["REMOTE_ADDR"];
            }
            else if (request.ServerVariables["HTTP_VIA"] != null)
            {
                IP = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            }
            else
            {
                IP = request.UserHostAddress;
            }
            if (ex.InnerException != null)
            {
                sw.WriteLine("IP:{0}\n时间:{1}\n错误消息:{2}\n位置:{3}\n地址：{4}\n\n", IP, DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss ffff"), ex.ToString(), ex.InnerException, Request.Url.AbsolutePath);
            }
            else if (ex != null)
            {
                sw.WriteLine("IP:{0}\n时间:{1}\n错误消息:{2}\n地址：{3}\n\n", IP, DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss ffff"), ex.ToString(), Request.Url.AbsolutePath);
            }
            else
            {
                sw.WriteLine("未知错误");
            }
            Server.ClearError();
            sw.Close();
        }
    }
}