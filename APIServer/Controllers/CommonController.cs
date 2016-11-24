using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

using ApplicationServer_API.Services;
using ApplicationServer_API.Services.CommonServices;
using APIServer.Models.CommonModels;
using APIServer.Models;

namespace ApplicationServer_API.Controllers
{
    public class CommonController : ApiController
    {
        //请求会议列表
        [HttpGet]
        public Respond RequestMeetingList(string IMEI)
        {

            Respond respond = new Respond();

            List<RequestMeetingListModel> list;

            RequestMeetingListService service = new RequestMeetingListService();
            Status status = service.run(IMEI, out list);
            respond.code = (int)status;
            respond.msg = Message.msgs[respond.code];
            respond.result = list;

            return respond;
        }

        //首次检查指定设备是否属于指定会议，成功返回当前参会人员信息、会议附件下载信息
        [HttpGet]
        public Respond CheckUser(string IMEI, int meetingID)
        {
            Respond respond = new Respond();

            CheckUserModel checkUserModel;

            CheckUserService service = new CheckUserService();
            Status status = service.run(IMEI, meetingID, out checkUserModel);
            respond.code = (int)status;
            respond.msg = Message.msgs[respond.code];
            respond.result = checkUserModel;

            return respond;
        }

        //下载附件
        [HttpGet]
        public HttpResponseMessage DownloadDocument(string documentPath)
        {
            return FileDownloadService.downloadDocument(documentPath);
        }

        //签到，更新参会人员的“IsSignIn”状态，返回开启会议所需的信息：会议基本信息、参会人员信息、议程信息、附件信息、表决信息等
        [HttpGet]
        public Respond SignIn(string IMEI, int meetingID)
        {
            Respond respond = new Respond();

            SignInModel signInModel;

            SignInService service = new SignInService();
            Status status = service.run(IMEI, meetingID, out signInModel);
            respond.code = (int)status;
            respond.msg = Message.msgs[respond.code];
            respond.result = signInModel;

            return respond;
        }

        //获取表决列表
        [HttpGet]
        public Services.Respond FetchVoteList(string IMEI, int meetingID)
        {
            Respond respond = new Respond();

            List<SignInVoteModel> votelist;

            FetchVoteListService service = new FetchVoteListService();
            Status status = service.run(IMEI, meetingID, out votelist);
            respond.code = (int)status;
            respond.msg = Message.msgs[respond.code];
            respond.result = votelist;

            return respond;
        }

        [HttpPost]
        public Services.Respond UploadVoteResult(UploadVoteResultParamModle data)
        {
            Respond respond = new Respond();

            UploadVoteResultService service = new UploadVoteResultService();
            Status status = service.run(data.voteID,data.userID,data.resultMap);
            respond.code = (int)status;
            respond.msg = Message.msgs[respond.code];
            respond.result = "";

            return respond;
        }

        //获取表决结果
        [HttpGet]
        public Services.Respond FetchVoteResult(int voteID)
        {
            Respond respond = new Respond();

            List<FetchVoteResultModel> fetchVoteResultlist;

            FetchVoteResultService service = new FetchVoteResultService();
            Status status = service.run(voteID, out fetchVoteResultlist);
            respond.code = (int)status;
            respond.msg = Message.msgs[respond.code];
            respond.result = fetchVoteResultlist;

            return respond;
        }
    }
}
