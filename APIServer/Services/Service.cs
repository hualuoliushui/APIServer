using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;

using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;

namespace APIServer.Services
{
    public abstract class Service
    {
        #region 判断会议状态
        /// <summary>
        /// 判断会议状态-辅助函数
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        private static int getMeetingStatus(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);

            if (meetingVo != null)
            {
                return meetingVo.meetingStatus;
            }

            return -1;
        }

        /// <summary>
        /// 判断会议是否 未开启:meetingStatus==1
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool IsNotOpen_meeting(int meetingID)
        {
            int status = getMeetingStatus(meetingID);
            if (status == -1 || status != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断会议是否 正在开启:meetingStatus==2
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool IsOpening_meeting(int meetingID)
        {
            int status = getMeetingStatus(meetingID);
            if (status == -1 || status != 2)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断会议是否 已结束:meetingStatus==16
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool IsOpend_meeting(int meetingID)
        {
            int status = getMeetingStatus(meetingID);
            if (status == -1 || status != 16)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 判断设备身份

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="IMEI"></param>
        /// <returns></returns>
        public static DeviceVO getDevice(string IMEI)
        {
            DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();

            Dictionary<string, object> parameterlist = new Dictionary<string, object>();

            parameterlist.Add("IMEI", IMEI);
            DeviceVO deviceVo = deviceDao.getOne<DeviceVO>(parameterlist);
         
            return deviceVo;
        }

        //==========================================================


        /// <summary>
        /// 判断设备身份-辅助函数-获取使用设备的参会人员的会议中角色：0：参会人员，1：主持人，2：主讲人，-1：非法设备,-2:非法参会设备
        /// </summary>
        /// <param name="IMEI"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        private static int getDelegateRole(string IMEI, int meetingID)
        {
            //查询时，参数列表
            Dictionary<string, object> parameterlist = new Dictionary<string, object>();

            //获取设备信息
            DeviceVO deviceVo = getDevice(IMEI);
            //非法设备
            if(deviceVo == null){
                return -1;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            parameterlist.Clear();
            parameterlist.Add("meetingID",meetingID);
            parameterlist.Add("deviceID", deviceVo.deviceID);
            DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(parameterlist);
            //非法参会设备
            if ( delegateVo == null )
            {
                return -2;
            }

            return delegateVo.personMeetingRole;
        }

        /// <summary>
        /// 重载getDelegateRole
        /// </summary>
        /// <param name="deviceVo"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        private static int getDelegateRole(DeviceVO deviceVo, int meetingID)
        {
            //查询时，参数列表
            Dictionary<string, object> parameterlist = new Dictionary<string, object>();

            //非法设备
            if (deviceVo == null)
            {
                return -1;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            parameterlist.Clear();
            parameterlist.Add("meetingID", meetingID);
            parameterlist.Add("deviceID", deviceVo.deviceID);
            DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(parameterlist);
            //非法参会设备
            if (delegateVo == null)
            {
                return -2;
            }

            return delegateVo.personMeetingRole;
        }

        //====================================================
        
        /// <summary>
        /// 判断是否为 主持人: meetingRole==1 
        /// </summary>
        /// <param name="IMEI"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool validateHost(String IMEI, int meetingID)
        {
            int meetingRole = getDelegateRole(IMEI, meetingID);

            if (meetingRole != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 直接使用设备信息
        /// </summary>
        /// <param name="deviceVo"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool validateHost(DeviceVO deviceVo, int meetingID)
        {
            int meetingRole = getDelegateRole(deviceVo, meetingID);

            if (meetingRole != 1)
            {
                return false;
            }
            return true;
        }

       /// <summary>
       /// 判断是否为 会议的参会人员：meetingRole == 0 or 1 or 2
       /// </summary>
       /// <param name="IMEI"></param>
       /// <param name="meetingID"></param>
       /// <returns></returns>
        public static bool validateDelegate(string IMEI,int meetingID){
            int meetingRole = getDelegateRole(IMEI, meetingID);

            if (meetingRole == 0
                || meetingRole == 1 
                || meetingRole == 2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 直接使用设备信息
        /// </summary>
        /// <param name="deviceVo"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool validateDelegate(DeviceVO deviceVo, int meetingID)
        {
            int meetingRole = getDelegateRole(deviceVo, meetingID);

            if (meetingRole == 0
                || meetingRole == 1
                || meetingRole == 2)
            {
                return true;
            }

            return false;
        }

        
        #endregion
    }
}