using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class FileInfo
    {
        public int agendaID { set; get; }
        public int fileID { set; get; }
        public string fileName { set; get; }
        public int fileSize { set; get; }
    }
}