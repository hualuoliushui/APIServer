using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models.CommonModels
{
    public class FetchVoteResultModel
    {
        public string optionItem { set; get; }
        public int voteNum { set; get; }
    }
}