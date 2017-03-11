using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class DelegateInfo
    {
        public string userName { set; get; }
        public string userDepartment { set; get; }
        public string userJob { set; get; }
        public string userMeetingRole { set; get; }
        public string DeviceIMEI { set; get; }
        public int DeviceIndex { set; get; }
    }
}