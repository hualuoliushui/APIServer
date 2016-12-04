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
    public class EndMeetingService : Service
    {
        public Status run(String IMEI, int meetingID)
        {
            if (!validateHost(IMEI, meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            return endMeeting(meetingID);

        }
        public Status endMeeting(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            //检查会议是否为 开启状态
            if (IsOpening_meeting(meetingID))
            {
                Dictionary<string, object> parameterlist = new Dictionary<string, object>();
                //设置为结束会议状态
                parameterlist.Add("meetingStatus", 16);

                int changedNum = meetingDao.update(parameterlist, meetingID);

                if (changedNum == 1)
                {
                    return Status.SUCCESS;
                }
                return Status.FAILURE;
            }else
                return Status.MEETING_HAS_BEEN_OPENED;
        }
    }
}