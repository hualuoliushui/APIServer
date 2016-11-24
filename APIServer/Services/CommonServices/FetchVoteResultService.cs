using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Services;
using APIServer.Models;
using APIServer.Models.CommonModels;

namespace ApplicationServer_API.Services.CommonServices
{
    
    public class FetchVoteResultService : Service
    {
        public Status run(int voteID,out List<FetchVoteResultModel> fetchVoteResultlist)
        {
            fetchVoteResultlist = new List<FetchVoteResultModel>();

            return setInfo(voteID, ref fetchVoteResultlist);
        }

        public Status setInfo(int voteID,
            ref List<FetchVoteResultModel> fetchVoteResultlist)
        {
            VoteOptionDAO voteOptionDao = 
                Factory.getInstance<VoteOptionDAO>();
            VoteOptionPersonResultDAO voteOptionPersonResultDao = 
                Factory.getInstance<VoteOptionPersonResultDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            wherelist.Add("voteID", voteID);
            List<VoteOptionVO> voteOptionVolist =
                voteOptionDao.getAll<VoteOptionVO>(wherelist);

            if (voteOptionVolist == null)
                return Status.FAILURE;
     
            foreach (VoteOptionVO voteOptionVo in voteOptionVolist)
            {
                int resultNum = 0;
                wherelist.Clear();
                wherelist.Add("voteOptionID", voteOptionVo.voteOptionID);
                List<VoteOptionPersonResultVO> voteOptionPersonResultVolist =
                    voteOptionPersonResultDao.
                    getAll<VoteOptionPersonResultVO>(wherelist);
                if (voteOptionPersonResultVolist != null)
                {
                    resultNum = voteOptionPersonResultVolist.Count;
                }

                fetchVoteResultlist.Add(
                    new FetchVoteResultModel
                    {
                        optionItem = voteOptionVo.voteOptionName,
                        voteNum = resultNum
                    });
            }
            return Status.SUCCESS;
        }
    }
}