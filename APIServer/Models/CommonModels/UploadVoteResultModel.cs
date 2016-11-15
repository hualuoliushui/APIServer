using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models.CommonModels
{
    public class UploadVoteResultParamModle
    {
        public int voteID { set; get; }
        public int userID { set; get; }
        public string IMEI { set; get; }
        //选择的选项标识列表
        public List<int> resultMap { set; get; }
    }
}