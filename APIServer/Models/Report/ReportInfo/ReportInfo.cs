using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class ReportInfo
    {
        public MeetingInfo meeting { set; get; }
        public List<DelegateInfo> delegates { set; get; }
        public List<AgendaInfo> agendas { set; get; }
    }
}