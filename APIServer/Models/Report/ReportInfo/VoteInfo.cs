using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class VoteInfo
    {
        public int voteID { set; get; }
        public string voteName { set; get; }
        public string voteDescription { set; get; }
        public string voteType { set; get; }

        public int voteStatus { set; get; }
        public int agendaID { set; get; }
        public string agendaName { set; get; }

        public List<OptionInfo> optionInfos { set; get; }
    }

    public class OptionInfo
    {
        public int voteID { set; get; }
        public string optionName { set; get; }
        public int optionResult { set; get; }
    }
}