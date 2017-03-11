using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models.Report;


//完全照搬webserver中的报告控制器、model、view
namespace APIServer.Controllers
{
    public class ReportController : Controller
    {
        [OutputCache(Duration=10)]
        [HttpGet]
        public ActionResult Index(int meetingID)
        {
            var service = new ReportService(meetingID);

            Session["meetingID"] = meetingID;

            var model = service.run();
            return View(model);
        }
    }
}
