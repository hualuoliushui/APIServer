using System.Collections.Generic;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Models.CommonModels;
using APIServer.Models;
using APIServer.Services;

namespace APIServer.Services.CommonServices
{
    public class SignInService : Service
    {
        public Status run(string IMEI,int meetingID,
            out SignInModel signInModel)
        {
            signInModel = new SignInModel();

            //获取设备信息
            DeviceVO deviceVo = getDevice(IMEI);

            //检查设备
            if (!validateDelegate(deviceVo, meetingID))
            {
                return Status.PERMISSION_DENIED;
            }
            //检查会议状态:是否 正在开启
            if (!IsOpening_meeting(meetingID))
            {
                return Status.MEETING_DOES_NOT_OPEN;
            }
            //更新签到状态
            if (updateSignIn(deviceVo, meetingID) != 1)
            {
                return Status.FAILURE;
            }

            //设置信息
            return setInfo(deviceVo, meetingID,ref signInModel);
        }

        /// <summary>
        /// 参会人员签到
        /// </summary>
        /// <param name="deviceVo"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public int updateSignIn(DeviceVO deviceVo, int meetingID)
        {
            DelegateDAO delegateDao = 
                Factory.getInstance<DelegateDAO>();
            Dictionary<string, object> setlist = 
                new Dictionary<string, object>();
            Dictionary<string,object> wherelist = 
                new Dictionary<string,object>();
            setlist.Add("isSignIn", true);
            wherelist.Add("deviceID",deviceVo.deviceID);
            wherelist.Add("meetingID",meetingID);

            return delegateDao.update(setlist, wherelist);
        }

        public Status setInfo(DeviceVO deviceVo,int meetingID,
            ref SignInModel signInModel)
        {
            //数据库操作参数
            Dictionary<string, object> setlist =
                new Dictionary<string, object>();
            Dictionary<string, object> wherelist =
                new Dictionary<string, object>();

            #region 设置会议信息

            MeetingDAO meetingDao = 
                Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo =
                meetingDao.getOne<MeetingVO>(meetingID);

            if (meetingVo == null)
                return Status.FAILURE;

            signInModel.meetingModel =
                new SignInMeetingModel
                {
                    meetingName = meetingVo.meetingName,
                    meetingSummary = meetingVo.meetingSummary,
                    meetingBeginTime = meetingVo.meetingToStartTime.ToString(),
                    meetingDuration = meetingVo.meetingDuration
                };

            #endregion

            #region 设置议程信息 会议=>议程=>人员=>议程信息+主讲人信息
           
            //主讲人信息<agendaID,personID>
            Dictionary<int,int> speakers = new Dictionary<int,int>();

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            wherelist.Clear();
            wherelist.Add("meetingID",meetingID);
            List<AgendaVO> agendaVolist = 
                agendaDao.getAll<AgendaVO>(wherelist);

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
      
            //初始化议程列表 
            signInModel.agendaModelList = new List<SignInAgendaModel>();
          
            if( agendaVolist != null){
                foreach(AgendaVO agendaVo in agendaVolist){
                    PersonVO personVo = 
                        personDao.getOne<PersonVO>(agendaVo.personID);
                    if(personVo == null)
                        continue;

                    signInModel.agendaModelList.Add(
                        new SignInAgendaModel
                        {
                            agendaName = agendaVo.agendaName,
                            agendaDuration = agendaVo.agendaDuration,
                            agendaIndex = agendaVo.agendaIndex,
                            agendaSpeaker = personVo.personName
                        });
                    //记录主讲人信息
                    speakers.Add(agendaVo.agendaIndex,personVo.personID);
                }
            }

            signInModel.agendaNum = 
                signInModel.agendaModelList.Count;

            #endregion

            #region 设置参会人员信息

            DelegateDAO delegateDao =
                Factory.getInstance<DelegateDAO>();
            wherelist.Clear();
            wherelist.Add("meetingID", meetingID);
            List<DelegateVO> delegateVolist =
                delegateDao.getAll<DelegateVO>(wherelist);

            if (delegateVolist == null)
                return Status.FAILURE;

            //初始化参会人员列表
            signInModel.delegateModelList =
                new List<SignInDelegateModel>();

            
            foreach (DelegateVO delegateVo in delegateVolist)
            {
                PersonVO personVo = 
                    personDao.getOne<PersonVO>(delegateVo.personID);
               //设置主讲议程信息
                List<int> delegateAgendaIDs = new List<int>();
                foreach(var param in speakers){
                    if(param.Value == personVo.personID){
                        delegateAgendaIDs.Add(param.Key);
                    }
                }
                signInModel.delegateModelList.Add(
                    new SignInDelegateModel
                    {
                        delegateID = personVo.personID,
                        delegateName = personVo.personName,
                        delegateDepartment = personVo.personDepartment,
                        delegateJob = personVo.personJob,
                        delegateDetailInfo = personVo.personDescription,
                        delegateRole = delegateVo.personMeetingRole,
                        delegateAgendaIndexList = delegateAgendaIDs,
                        seatIndex = delegateVo.seatIndex
                    });
            }

            signInModel.delegateNum = 
                signInModel.delegateModelList.Count;

            #endregion

            #region 设置附件信息

            FileDAO fileDao = Factory.getInstance<FileDAO>();

            signInModel.documentModelList = new List<SignInDocumentModel>();
            foreach (AgendaVO agendaVo in agendaVolist)
            {
                wherelist.Clear();
                wherelist.Add("agendaID",agendaVo.agendaID);
                List<FileVO> fileVolist = fileDao.getAll<FileVO>(wherelist);
                if (fileVolist == null)
                    continue;
                foreach (FileVO fileVo in fileVolist)
                {
                    signInModel.documentModelList.Add(
                        new SignInDocumentModel
                        {
                            documentID = fileVo.fileID,
                            documentName = fileVo.fileName,
                            documentIndex = fileVo.fileIndex,
                            documentSize = fileVo.fileSize,
                            documentPath = fileVo.filePath,
                            documentAgendaIndex = agendaVo.agendaIndex
                        });
                }
            }
            
            #endregion

            #region 设置表决信息

            VoteDAO voteDao = 
                Factory.getInstance<VoteDAO>();
            VoteOptionDAO voteOptionDao = 
                Factory.getInstance<VoteOptionDAO>();
            //初始化表决列表
            signInModel.voteList = new List<SignInVoteModel>();

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

                    signInModel.voteList.Add(
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

            #endregion

            return Status.SUCCESS;
        }
    }  
}
