using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Services;
using APIServer.Models;

namespace ApplicationServer_API.Services.HostModels
{

    public class StartOrEndVoteService : Service
    {
        /// <summary>
        /// 获取表决信息
        /// </summary>
        /// <param name="voteID"></param>
        /// <returns></returns>
        private VoteVO getVote(int voteID)
        {
            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            return voteDao.getOne<VoteVO>(voteID);
        }

        /// <summary>
        /// 检验当前表决中，操作状态改变是否合理
        /// </summary>
        /// <param name="voteVo"></param>
        /// <param name="voteStatus"></param>
        /// <returns></returns>
        private bool validateStatus(VoteVO voteVo,int voteStatus)
        {
            if (voteVo == null)
                return false;
            //如果当前表决未启动，且操作为开始表决，合理
            if (voteVo.voteStatus == 1 && voteStatus == 0)
                return true;
            //如果当前表决为开启状态，且操作为结束表决，合理
            if (voteVo.voteStatus == 2 && voteStatus == -1)
                return true;
            //其余情况，不合理
            return false;
        }

        public Status run(string IMEI, int meetingID, 
            int voteID, int startOrEnd)
        {
            //检查是否为主持人
            if (!validateHost(IMEI, meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            //检查会议是否为 开启状态
            if (IsOpening_meeting(meetingID))
            {
                return startOrEndVote(voteID, startOrEnd); 
            }else
                return Status.MEETING_HAS_BEEN_OPENED;

            
        }
        public Status startOrEndVote(int voteID, int startOrEnd)
        {
            VoteVO voteVo = getVote(voteID);
            if (!validateStatus(voteVo, startOrEnd))
            {
                return Status.FAILURE;
            }
            VoteDAO voteDao = Factory.getInstance<VoteDAO>();

            Dictionary<string, object> parameterlist = new Dictionary<string, object>();
            int voteStatus = startOrEnd == 0 ? 2 : 16;
            //设置表决为voteStatus状态
            parameterlist.Add("voteStatus", voteStatus);

            int changedNum = voteDao.update(parameterlist, voteID);

            if (changedNum == 1)
            {
                return Status.SUCCESS;
            }
            return Status.FAILURE;
        }
    }
}