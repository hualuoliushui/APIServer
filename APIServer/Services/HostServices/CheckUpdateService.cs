using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models;
using APIServer.Services;
using System.Collections.Generic;
using APIServer.Models.HostModels;

namespace ApplicationServer_API.Services.HostModels
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

        /// <summary>
        /// 判断会议是否内容是否有更新
        /// </summary>
        /// <param name="meetingVo"></param>
        /// <returns></returns>
        private bool IsUpdate(MeetingVO meetingVo)
        {
            return meetingVo.meetingUpdateStatus == 1;
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

            //会议无更新
            if (!IsUpdate(meetingVo))
            {
                checkUpdateModel.isUpdate = 0;
                checkUpdateModel.agendaUpdate = 0;
                checkUpdateModel.delegateUpdate = 0;
                return Status.SUCCESS;
            }

            //会议有更新
            checkUpdateModel.isUpdate = 1;
            checkUpdateModel.agendaUpdate = 1;
            checkUpdateModel.delegateUpdate = 1;

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