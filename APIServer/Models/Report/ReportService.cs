using APIServer;
using DAL.DAO;
using DAL.DAOFactory;
using DAL.DAOVO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebServer.Models.Report
{
    public class ReportService
    {
        public ReportService(int meetingID)
        {
            try
            {
                this.meetingID = meetingID;
               setMeetingInfo();
               setDelegateInfos();
               setAgendaInfos();
            }
            catch (Exception e)
            {
                Log.LogInfo("报表数据初始化失败：", e);
                throw;
            }
        }

        private Dictionary<string, object> wherelist = new Dictionary<string, object>();

        private int meetingID;
        private ReportInfo.MeetingInfo meetingInfo;
        private List<ReportInfo.DelegateInfo> delegateInfos;
        private List<ReportInfo.AgendaInfo> agendaInfos;

        public ReportInfo.ReportInfo run()
        {
            return new ReportInfo.ReportInfo
            {
                meeting = this.meetingInfo,
                delegates = this.delegateInfos,
                agendas = this.agendaInfos
            };
        }

        private void setMeetingInfo()
        {
            this.meetingInfo = new ReportInfo.MeetingInfo();

            var meetingDao = Factory.getInstance<MeetingDAO>();
            var meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            var meetingVo = meetingDao.getOne<MeetingVO>(meetingID);

            do
            {
                if (meetingVo == null)
                    break;

                meetingInfo.meetingID = meetingVo.meetingID;
                meetingInfo.meetingName = meetingVo.meetingName;
                meetingInfo.meetingSummary = meetingVo.meetingSummary;
                meetingInfo.meetingToStartTime = meetingVo.meetingToStartTime;
                meetingInfo.meetingStatus = meetingVo.meetingStatus;
                var meetingPlaceVo = meetingPlaceDao.getOne<MeetingPlaceVO>(meetingVo.meetingPlaceID);
                if (meetingPlaceVo == null)
                    break;
                meetingInfo.meetingPlaceName = meetingPlaceVo.meetingPlaceName;

            } while (false);
        }

        private void setDelegateInfos()
        {
            this.delegateInfos = new List<ReportInfo.DelegateInfo>();

            var delegateDao = Factory.getInstance<DelegateDAO>();
            var personDao = Factory.getInstance<PersonDAO>();
            var deviceDao = Factory.getInstance<DeviceDAO>();

            wherelist.Clear();
            wherelist.Add("meetingID", meetingID);
            var delegateVos = delegateDao.getAll<DelegateVO>(wherelist);

            do
            {
                if (delegateVos == null)
                    break;
                foreach (var vo in delegateVos)
                {
                    var delegateInfo = new ReportInfo.DelegateInfo();

                    var personVo = personDao.getOne<PersonVO>(vo.personID);

                    var deviceVo = deviceDao.getOne<DeviceVO>(vo.deviceID);

                    delegateInfo.userName = personVo.personName;
                    delegateInfo.userDepartment = personVo.personDepartment;
                    delegateInfo.userJob = personVo.personJob;
                    delegateInfo.userMeetingRole = (vo.personMeetingRole == 1 ? "主持人" : (vo.personMeetingRole == 2 ? "主讲人" : "参会人员"));
                    delegateInfo.DeviceIMEI = deviceVo.IMEI;
                    delegateInfo.DeviceIndex = deviceVo.deviceIndex;

                    delegateInfos.Add(delegateInfo);
                }
            } while (false);
        }

        private void setAgendaInfos()
        {
            var ds = new DataSet();

            //添加议程信息
            this.agendaInfos = new List<ReportInfo.AgendaInfo>();

            var agendaDao = Factory.getInstance<AgendaDAO>();
            var personDao = Factory.getInstance<PersonDAO>();
            var fileDao = Factory.getInstance<FileDAO>();

            wherelist.Clear();
            wherelist.Add("meetingID", meetingID);
            var agendaVos = agendaDao.getAll<AgendaVO>(wherelist);

            do
            {
                if (agendaVos == null)
                    break;

                agendaVos.Sort((x, y) =>
                {
                    return x.agendaIndex < y.agendaIndex ? -1 : (x.agendaIndex > y.agendaIndex ? 1 : 0);
                });
                foreach (var vo in agendaVos)
                {
                    var personVo = personDao.getOne<PersonVO>(vo.personID);

                    var agendaInfo = new ReportInfo.AgendaInfo();

                    agendaInfo.agendaID = vo.agendaID;
                    agendaInfo.agendaName = vo.agendaName;
                    agendaInfo.agendaDuration = vo.agendaDuration;
                    agendaInfo.agendaIndex = vo.agendaIndex;
                    agendaInfo.agendaSpeakerName = personVo.personName;



                    wherelist.Clear();
                    wherelist.Add("agendaID", vo.agendaID);
                    var fileVos = fileDao.getAll<FileVO>(wherelist);

                    //添加文件信息
                    var fileInfos = new List<ReportInfo.FileInfo>();
                    if (fileVos != null)
                    {
                        fileVos.Sort((x, y) =>
                        { return x.fileIndex < y.fileIndex ? -1 : (x.fileIndex > y.fileIndex ? 1 : 0); });
                        foreach (var fileVo in fileVos)
                        {
                            fileInfos.Add(new ReportInfo.FileInfo
                            {
                                agendaID = vo.agendaID,
                                fileName = fileVo.fileName,
                                fileSize = fileVo.fileSize,
                                fileID = fileVo.fileID
                            });
                        }
                    }
                    agendaInfo.fileInfos = fileInfos;

                    var voteDao = Factory.getInstance<VoteDAO>();
                    var voteOptionDao = Factory.getInstance<VoteOptionDAO>();
                    var voteOptionPersonResultDao = Factory.getInstance<VoteOptionPersonResultDAO>();

                    var voteVos = voteDao.getAll<VoteVO>(wherelist);
                    //添加表决信息
                    var voteInfos = new List<ReportInfo.VoteInfo>();
                    if (voteVos != null)
                    {
                        voteVos.Sort((x, y) =>
                            {
                                return x.voteIndex < y.voteIndex ? -1 : (x.voteIndex > y.voteIndex ? 1 : 0);
                            });
                        foreach (var voteVo in voteVos)
                        {
                            var voteInfo = new ReportInfo.VoteInfo();
                            voteInfo.agendaID = vo.agendaID;
                            voteInfo.agendaName = vo.agendaName;
                            voteInfo.voteID = voteVo.voteID;
                            voteInfo.voteName = voteVo.voteName;
                            voteInfo.voteDescription = voteVo.voteDescription;
                            voteInfo.voteStatus = voteVo.voteStatus;

                            wherelist.Clear();
                            wherelist.Add("voteID", voteVo.voteID);
                            var voteOptionVos = voteOptionDao.getAll<VoteOptionVO>(wherelist);

                            //添加选项
                            var optionInfos = new List<ReportInfo.OptionInfo>();
                            if (voteOptionVos != null)
                            {
                                voteOptionVos.Sort((x, y) =>
                                {
                                    return x.voteOptionIndex < y.voteOptionIndex ? -1 : (x.voteOptionIndex > y.voteOptionIndex ? 1 : 0);
                                });
                                foreach (var voteOptionVo in voteOptionVos)
                                {
                                    var optionInfo = new ReportInfo.OptionInfo();

                                    optionInfo.voteID = voteVo.voteID;
                                    optionInfo.optionName = voteOptionVo.voteOptionName;

                                    //获取选项结果
                                    wherelist.Clear();
                                    wherelist.Add("voteOptionID", voteOptionVo.voteOptionID);
                                    var optionResultVos = voteOptionPersonResultDao.getAll<VoteOptionPersonResultVO>(wherelist);
                                    optionInfo.optionResult = optionResultVos == null ? 0 : optionResultVos.Count;

                                    optionInfos.Add(optionInfo);
                                }
                            }
                            voteInfo.voteType = optionInfos.Count + "选" + voteVo.voteType;

                            voteInfo.optionInfos = optionInfos;

                            voteInfos.Add(voteInfo);
                        }
                    }
                    agendaInfo.voteInfos = voteInfos;

                    this.agendaInfos.Add(agendaInfo);
                }
            } while (false);
        }

        /// <summary> 
        /// 将泛型集合类转换成DataTable 
        /// </summary> 
        /// <typeparam name="T">集合项类型</typeparam> 
        /// <param name="list">集合</param> 
        /// <param name="propertyName">需要返回的列的列名</param> 
        /// <returns>数据集(表)</returns> 
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        } 
    }
}