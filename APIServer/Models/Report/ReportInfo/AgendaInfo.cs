using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class AgendaInfo
    {
        public int agendaID { set; get; }
        public string agendaName { set; get; }
        public int agendaIndex { set; get; }
        public string agendaSpeakerName { set; get; }
        public int agendaDuration { set; get; }

        public List<Report.ReportInfo.FileInfo> fileInfos { set; get; }

        public List<Report.ReportInfo.VoteInfo> voteInfos { set; get; }
    }
}