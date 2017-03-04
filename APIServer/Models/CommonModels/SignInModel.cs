using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models.CommonModels
{
    public class SignInModel
    {
        public SignInMeetingModel meetingModel { set; get; }
        public int delegateNum { set; get; }
        public List<SignInDelegateModel> delegateModelList { set; get; }
        public int agendaNum { set; get; }
        public List<SignInAgendaModel> agendaModelList { set; get; }
        public List<SignInDocumentModel> documentModelList { set; get; }
        public List<SignInVoteModel> voteList { set; get; }
    }

    public class SignInMeetingModel
    {
        public string meetingSummary { set; get; }
        public string meetingName { set; get; }
        public string meetingBeginTime { set; get; }
        public int meetingDuration { set; get; }
    }

    public class SignInDelegateModel
    {
        public int delegateID { set; get; }
        public string delegateName { set; get; }
        public string delegateDepartment { set; get; }
        public string delegateJob { set; get; }
        public int delegateRole { set; get; }
        public string delegateDetailInfo { set; get; }
        public List<int> delegateAgendaIndexList { set; get; }
        public int seatIndex { set; get; }
    }

    public class SignInAgendaModel
    {
        public int agendaIndex { set; get; }
        public int agendaDuration { set; get; }
        public string agendaName { set; get; }
        public string agendaSpeaker { set; get; }
    }

    public class SignInDocumentModel
    {
        public int documentID { set; get; }
        public int documentAgendaIndex { set; get; }
        public int documentIndex { set; get; }
        public string documentName { set; get; }
        public int documentSize { set; get; }
        public string documentPath { set; get; }
    }

    public class VoteOptionModel
    {
        public int optionID { set; get; }
        public string optionItem { set; get; }
    }
    public class SignInVoteModel
    {
        public int voteID { set; get; }
        //表决名称
        public string voteTitle { set; get; }
        //表决内容，描述当前表决的信息
        public string voteContent { set; get; }
        //当前表决的选项列表
        public List<VoteOptionModel> voteOptionsList { set; get; }
        //表决选项，单选或多选，0：不限；N:最多选择N个选项
        public int voteNumberNeeded { set; get; }
        //表决状态，0：未开启，1：结束
        public int voteState { set; get; }
    }
}