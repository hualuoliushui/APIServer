using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models;
using APIServer.Services;
using System.Collections.Generic;
using APIServer.Models.HostModels;

namespace APIServer.Services.HostModels
{
    public class CheckUpdateService :Service
    {

        public Status run(string IMEI, int meetingID,
            out CheckUpdateModel checkUpdateModel)
        {
            checkUpdateModel = new CheckUpdateModel();

            //判断是否为主持人
            if (!validateHost(IMEI, meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            return checkUpdate(meetingID,ref checkUpdateModel);
        }

        /// <summary>
        /// 获取会议信息
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        private MeetingVO getMeeting(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            return meetingDao.getOne<MeetingVO>(meetingID);
        }


        public Status checkUpdate(int meetingID ,
            ref CheckUpdateModel checkUpdateModel)
        {
            MeetingVO meetingVo = getMeeting(meetingID);

            if (meetingVo == null)
                return Status.FAILURE;
            
            //判断会议是否为 开启状态
            if (!IsOpening_meeting(meetingID))
            {
                return Status.MEETING_HAS_BEEN_OPENED;
            }

            //设置状态
            checkUpdateModel.isUpdate = (meetingVo.delegateUpdateStatus == 1)
                || (meetingVo.agendaUpdateStatus==1)
                || (meetingVo.fileUpdateStatus==1)
                || (meetingVo.voteUpdateStatus==1)
                ? 1 : 0;
            checkUpdateModel.agendaUpdate = meetingVo.agendaUpdateStatus;
            checkUpdateModel.delegateUpdate = meetingVo.delegateUpdateStatus;

            //更新会议更新状态
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> parameterlist = new Dictionary<string, object>();
            //设置为无更新状态
            parameterlist.Add("meetingUpdateStatus", 0);

            int changedNum = meetingDao.update(parameterlist, meetingID);

            if (changedNum == 1)
            {
                return Status.SUCCESS;
            }
            return Status.FAILURE;
        }
    }
}