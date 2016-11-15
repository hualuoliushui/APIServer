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
}