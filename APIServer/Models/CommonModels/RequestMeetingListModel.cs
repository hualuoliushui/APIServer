using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models.CommonModels
{
    public class RequestMeetingListModel
    {
        public int meetingID { set; get; }
        public string meetingName { set; get; }
        public string meetingBeginTime { set; get; }
        public int meetingState { set; get; }
    }
}