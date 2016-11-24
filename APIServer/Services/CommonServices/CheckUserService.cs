using System.Collections.Generic;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using APIServer.Services;
using APIServer.Models;
using APIServer.Models.CommonModels;

namespace ApplicationServer_API.Services.CommonServices
{
    class CheckUserService : Service
    {
        public Status run(string IMEI,int meetingID,
            out CheckUserModel checkUserModel)
        {
            checkUserModel = new CheckUserModel();

            //获取设备信息
            DeviceVO deviceVo = getDevice(IMEI);

            //检查设备
            if (!validateDelegate(deviceVo, meetingID))
            {
                return Status.PERMISSION_DENIED;
            }
            //检查会议状态:是否 正在开启
            if (IsOpening_meeting(meetingID))
            {
                //设置信息
                return setInfo(deviceVo, meetingID, ref checkUserModel);              
            }
            return Status.MEETING_DOES_NOT_OPEN;
        }

        public Status setInfo(DeviceVO deviceVo, int meetingID,
            ref CheckUserModel checkUserModel)
        {
            #region 获取参会人员信息 设备+会议=参会人员 =>参会人员信息
            Dictionary<string,object> wherelist = new Dictionary<string,object>();
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            wherelist.Add("deviceID",deviceVo.deviceID);
            wherelist.Add("meetingID",meetingID);

            DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(wherelist);

            if (delegateVo == null)
                return Status.FAILURE;

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            PersonVO personVo = personDao.getOne<PersonVO>(delegateVo.personID);

            if (personVo == null)
                return Status.FAILURE;

            checkUserModel.userID = personVo.personID;
            checkUserModel.userName = personVo.personName;
            checkUserModel.userDepartment = personVo.personDepartment;
            checkUserModel.userJob = personVo.personJob;
            checkUserModel.userRole = delegateVo.personMeetingRole;

            #endregion

            #region 获取文件路径 会议 => 议程 => 附件 => 附件路径
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            wherelist.Clear();
            wherelist.Add("meetingID",meetingID);
            List<AgendaVO> agendaVolist = agendaDao.getAll<AgendaVO>(wherelist);
            //即使无议程，也是成功
            if (agendaVolist == null)
                return Status.SUCCESS;

            List<string> filePathlist = new List<string>();
            FileDAO fileDao = Factory.getInstance<FileDAO>();
            foreach (AgendaVO agendaVo in agendaVolist)
            {
                wherelist.Clear();
                wherelist.Add("agendaID",agendaVo.agendaID);
                List<FileVO> fileVolist = fileDao.getAll<FileVO>(wherelist);

                //某个议程无附件
                if (fileVolist == null)
                    continue;

                foreach (FileVO fileVo in fileVolist)
                {
                    filePathlist.Add(fileVo.filePath);
                }
            }
            checkUserModel.documentURLList = filePathlist;

            #endregion

            return Status.SUCCESS;
        }
    }
}
