using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models;
using APIServer.Services;

namespace ApplicationServer_API.Services.CommonServices
{


    public class UploadVoteResultService : Service
    {
        public Status run(int voteID, int personID,
            List<int> selectIDs)
        {
            //上传个人表决结果
            return uploadVoteResult(voteID, personID, selectIDs);
        }

        /// <summary>
        /// 获取表决信息
        /// </summary>
        /// <param name="voteID"></param>
        /// <returns></returns>
        private VoteVO getVote(int voteID){
            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            return voteDao.getOne<VoteVO>(voteID);
        }

        /// <summary>
        /// 判断指定表决是否处于开启状态：voteStatus==2
        /// </summary>
        /// <param name="voteVo"></param>
        /// <returns></returns>
        private bool IsOpening_vote(VoteVO voteVo){
            return voteVo != null && voteVo.voteStatus == 2;
        }

        public Status uploadVoteResult(int voteID, int personID,
            List<int> selectIDs)
        {
            VoteVO voteVo = getVote(voteID);
            //判断表决是否为开启状态 voteStatus==2
            if (!IsOpening_vote(voteVo))
            {
                return Status.FAILURE;
            }

            if (selectIDs == null)
                return Status.FAILURE;

            VoteOptionPersonResultDAO voteOptionPersonResultDao = 
                Factory.getInstance<VoteOptionPersonResultDAO>();

            int id = 0;
            int changedNum =0;
            foreach (int voteOptionID in selectIDs)
            {
                id = VoteOptionPersonResultDAO.getID();
                changedNum = voteOptionPersonResultDao.
                    insert<VoteOptionPersonResultVO>(
                    new VoteOptionPersonResultVO
                    {
                        voteOptionPersonResultID = id,
                        voteOptionID = voteOptionID,
                        personID = personID
                    });
                //未成功插入
                if (changedNum != 1)
                {
                    return Status.FAILURE;
                }
            }
            return Status.SUCCESS;
        }      
    }
}