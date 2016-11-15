using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ApplicationServer_API.Services;
using ApplicationServer_API.Services.HostModels;
using APIServer.Models;
using APIServer.Models.HostModels;

namespace ApplicationServer_API.Controllers
{
    public class HostController : ApiController
    {
        //开启会议
        [HttpGet]
        public Services.Respond StartMeeting(string IMEI, int meetingID)
        {
            Respond respond = new Respond();

            StartMeetingService service = new StartMeetingService();
            Status status = service.run(IMEI, meetingID);
            respond.code = (int)status;
            respond.msg = status.ToString();
            respond.result = "";

            return respond;
        }

        [HttpGet]
        public Services.Respond EndMeeting(string IMEI, int meetingID)
        {
            Respond respond = new Respond();

            EndMeetingService service = new EndMeetingService();
            Status status = service.run(IMEI, meetingID);
            respond.code = (int)status;
            respond.msg = status.ToString();
            respond.result = "";

            return respond;
        }

        //检查会议信息是否有更新
        [HttpGet]
        public Services.Respond CheckUpdate( string IMEI,int meetingID)
        {
            Respond respond = new Respond();

            CheckUpdateService service = new CheckUpdateService();

            CheckUpdateModel checkUpdateModel;
            Status status = service.run(IMEI, meetingID,out checkUpdateModel);
            respond.code = (int)status;
            respond.msg = status.ToString();
            respond.result = checkUpdateModel;

            return respond;
        }

        //开启或结束投票
        [HttpGet]
        public Services.Respond StartOrEndVote(string IMEI,int meetingID,int voteID,int startOrEnd)
        {
            Respond respond = new Respond();

            StartOrEndVoteService service = new StartOrEndVoteService();
            Status status = service.run(IMEI, meetingID,voteID,startOrEnd);
            respond.code = (int)status;
            respond.msg = status.ToString();
            respond.result = "";

            return respond;
        }     
    }
}
