using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models.CommonModels
{
    public class CheckUserModel
    {
        public int userID { set; get; }
        public string userName { set; get; }
        public string userDepartment { set; get; }
        public string userJob { set; get; }
        public int userRole { set; get; }
        public List<string> documentURLList { set; get; }
    }
}