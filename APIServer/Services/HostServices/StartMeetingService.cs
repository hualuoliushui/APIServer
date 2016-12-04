using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models;
using APIServer.Services;

namespace APIServer.Services.HostModels
{
    public class StartMeetingService : Service
    {
        public Status run(String IMEI, int meetingID)
        {
            if (!validateHost(IMEI,meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            return startMeeting(meetingID);
        }
        public Status startMeeting(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            if (IsNotOpen_meeting(meetingID))
            {
                Dictionary<string, object> parameterlist = new Dictionary<string, object>();
                //设置为开会状态
                parameterlist.Add("meetingStatus", 2);

                int changedNum = meetingDao.update(parameterlist, meetingID);

                if (changedNum == 1)
                {
                    return Status.SUCCESS;
                }
                return Status.FAILURE;
            }
            else
            {
                return Status.MEETING_HAS_BEEN_OPENED;
            }

            
        }
    }
}