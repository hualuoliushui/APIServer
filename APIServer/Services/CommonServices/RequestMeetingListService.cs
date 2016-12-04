using System.Collections.Generic;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models.CommonModels;
using APIServer.Models;
using APIServer.Services;

namespace APIServer.Services.CommonServices
{
    public class RequestMeetingListService : Service
    {
        
        public Status run(string IMEI,
            out List<RequestMeetingListModel> meetinglist)
        {
            meetinglist = new List<RequestMeetingListModel>();

            //获取设备信息
            DeviceVO deviceVo = getDevice(IMEI);

            //检查是否为合法设备
            if(deviceVo == null){
                return Status.PERMISSION_DENIED;
            }

            //设置meetinglist
            setMeetingList(deviceVo, ref meetinglist);

            return Status.SUCCESS;
        }

        public void setMeetingList(DeviceVO deviceVo , ref List<RequestMeetingListModel> meetinglist)
        {
            Dictionary<string, object> parameterlist = new Dictionary<string, object>();

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            parameterlist.Add("deviceID", deviceVo.deviceID);
            List<DelegateVO> delegateVolist = delegateDao.getAll<DelegateVO>(parameterlist);
            //没有参与会议
            if (delegateVolist == null || delegateVolist.Count == 0)
            {
                return;
            }

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            foreach (DelegateVO delegateVo in delegateVolist)
            {
                MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(delegateVo.meetingID);
             
                if (meetingVo == null)
                    continue; //除非数据库出错

                meetinglist.Add(
                    new RequestMeetingListModel
                    {
                        meetingID = meetingVo.meetingID,
                        meetingName = meetingVo.meetingName,
                        meetingBeginTime = meetingVo.meetingToStartTime.ToString(),
                        meetingState = meetingVo.meetingStatus
                    });
            }
        }
    }
}
