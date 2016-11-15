using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Models.HostModels
{
    public class CheckUpdateModel
    {
        public int isUpdate { set; get; }
        public int delegateUpdate { set; get; }
        public int agendaUpdate { set; get; }
    }
}