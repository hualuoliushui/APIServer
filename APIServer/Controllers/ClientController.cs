using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;

namespace APIServer.Controllers
{
    //客户端
    public class ClientController : ApiController
    {
        [HttpGet]
        public Services.Respond reset(int meetingID)
        {
            Services.Respond respond = new Services.Respond();
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> list = new Dictionary<string, object>();
            list.Add("meetingStatus", 1);
            if (meetingDao.update(list,meetingID)!=1)
            {
                respond.code = -1;
                respond.msg = "failure";
                respond.result = "";
                return respond;
            }
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            list.Clear();
            list.Add("meetingID",meetingID);
            List<AgendaVO> agendas = agendaDao.getAll<AgendaVO>(list);
            if (agendas != null)
            {
                foreach (AgendaVO agenda in agendas)
                {
                    VoteDAO voteDao = Factory.getInstance<VoteDAO>();
                    VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
                    VoteOptionPersonResultDAO voteOptionPersonResultDao = Factory.getInstance<VoteOptionPersonResultDAO>();

                    list.Clear();
                    list.Add("agendaID", agenda.agendaID);
                    List<VoteVO> votes = voteDao.getAll<VoteVO>(list);

                    if (votes != null)
                    {
                        foreach (VoteVO vote in votes)
                        {
                            list.Clear();
                            list.Add("voteStatus", 0);
                            voteDao.update(list, vote.voteID);

                            list.Clear();
                            list.Add("voteID", vote.voteID);
                            List<VoteOptionVO> voteOptionVolist = voteOptionDao.getAll<VoteOptionVO>(list);
                            if (voteOptionVolist != null)
                            {
                                foreach (VoteOptionVO voteOption in voteOptionVolist)
                                {
                                    list.Clear();
                                    list.Add("voteOptionID", voteOption.voteOptionID);
                                    voteOptionPersonResultDao.delete(list);
                                }
                            }
                        }
                    }
                }
            }

            respond.code = 0;
            respond.msg = "";
            respond.result = "";
            return respond;
        }
    }
}
