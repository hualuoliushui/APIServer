using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models
{
    public enum Status
    {
        FAILURE=-1,
        SUCCESS=0,
        PERMISSION_DENIED=1,
        MEETING_DOES_NOT_OPEN=2,
        MEETING_HAS_BEEN_OPENED=3
    }

    public class Message
    {
        public static Dictionary<int, string> msgs;
        static Message()
        {
            msgs = new Dictionary<int, string>();
            msgs.Add((int)Status.FAILURE, "失败");
            msgs.Add((int)Status.SUCCESS, "成功");
            msgs.Add((int)Status.PERMISSION_DENIED, "无权限");
            msgs.Add((int)Status.MEETING_DOES_NOT_OPEN, "会议未开启");
            msgs.Add((int)Status.MEETING_HAS_BEEN_OPENED, "会议已结束");
        }
    }
}