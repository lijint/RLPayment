using System;
using System.Collections.Generic;
using System.Text;

namespace PetroChina.Package
{
    /// <summary>
    /// IC卡交易
    /// </summary>
    public enum PbocTransType : int
    {
        //IC卡交易
        /// <summary>
        /// 电子现金查询
        /// </summary>
        EC_INQUERY = 80,//电子现金查询
        /// <summary>
        /// IC卡日志查询
        /// </summary>
        EC_LOGINQ = 83,//IC卡日志查询
        /// <summary>
        /// 脱机消费
        /// </summary>
        EC_PURCHASE = 81,//脱机消费
        /// <summary>
        /// 指定帐户圈存
        /// </summary>
        EC_LOAD = 82,//指定帐户圈存
        /// <summary>
        /// 非指定帐户圈存
        /// </summary>
        EC_LOAD_U = 84,//非指定帐户圈存
        /// <summary>
        /// 消费
        /// </summary>
        PURCHASE = 1,//消费
        /// <summary>
        /// 余额查询
        /// </summary>
        INQUERY = 2,//余额查询
    }

    /// <summary>
    /// 冲正接口
    /// </summary>
    interface IReverse
    {
        /// <summary>
        /// 创建冲正文件
        /// </summary>
        void CreateReverseFile();
        /// <summary>
        /// 清除冲正文件
        /// </summary>
        void ClearReverseFile();
        /// <summary>
        /// 发送冲正文件
        /// </summary>
        void DoReverseFile();
    }
}
