using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models;
using APIServer.Services;
using APIServer.Models.CommonModels;

namespace APIServer.Services.CommonServices
{
    public class FetchVoteListService : Service
    {
        public Status run(
            string IMEI, int meetingID,
            out List<SignInVoteModel> voteList)
        {
            voteList = new List<SignInVoteModel>();

            //检查设备
            if (!validateDelegate(IMEI, meetingID))
            {
                return Status.PERMISSION_DENIED;
            }
            //检查会议状态:是否 正在开启
            if (!IsOpening_meeting(meetingID))
            {
                return Status.MEETING_DOES_NOT_OPEN;
            }
            //设置信息
            return setInfo(meetingID, ref voteList);
        }

        public Status setInfo(int meetingID,
            ref List<SignInVoteModel> votelist)
        {
            Dictionary<string, object> wherelist =
                new Dictionary<string, object>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();

            wherelist.Add("meetingID", meetingID);
            List<AgendaVO> agendaVolist = agendaDao.getAll<AgendaVO>(wherelist);

            if (agendaVolist == null)
            {
                return Status.SUCCESS;
            }

            VoteDAO voteDao =
               Factory.getInstance<VoteDAO>();
            VoteOptionDAO voteOptionDao =
                Factory.getInstance<VoteOptionDAO>();

            foreach (AgendaVO agendaVo in agendaVolist)
            {
                wherelist.Clear();
                wherelist.Add("agendaID", agendaVo.agendaID);
                List<VoteVO> voteVolist =
                    voteDao.getAll<VoteVO>(wherelist);
                if (voteVolist == null)
                    continue;

                foreach (VoteVO voteVo in voteVolist)
                {
                    wherelist.Clear();
                    wherelist.Add("voteID", voteVo.voteID);
                    List<VoteOptionVO> voteOptionVolist =
                        voteOptionDao.getAll<VoteOptionVO>(wherelist);
                    if (voteOptionVolist == null)
                        continue;
                    //初始化选项列表
                    List<VoteOptionModel> voteOptionlist =
                        new List<VoteOptionModel>();
                    foreach (VoteOptionVO voteOptionVo in voteOptionVolist)
                    {
                        voteOptionlist.Add(
                        new VoteOptionModel
                        {
                            optionID = voteOptionVo.voteOptionID,
                            optionItem = voteOptionVo.voteOptionName
                        });
                    }

                    votelist.Add(
                        new SignInVoteModel
                        {
                            voteID = voteVo.voteID,
                            voteTitle = voteVo.voteName,
                            voteContent = voteVo.voteDescription,
                            voteNumberNeeded = voteVo.voteType,
                            voteState = voteVo.voteStatus,
                            voteOptionsList = voteOptionlist
                        });
                }
            }
            return Status.SUCCESS;
        }
    }
}