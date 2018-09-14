using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;
using System.Data.Common;
using System.IO;
using System.Windows.Forms;
using YAPayment.Entity;

namespace YAPayment
{
    public enum EnumConfirmFlag : int
    {
        E_SEND_AGAIN = 0,
        E_SUCC = 1,
        E_REVERSE = 2,
    }

    class TransAccessFactory : DataAccess
    {
        private BaseEntity entity = null;

        public TransAccessFactory(BaseEntity entity)
        {
            this.entity = entity;
        }

        /// <summary>
        /// 添加IC卡或磁条卡交易记录
        /// </summary>
        private string strSqlIcTransFormat = @"INSERT INTO transDetailLog" +
            "([AddTime],[TraceNo],[CardNo],[BatchNo],[RefNo],[Amount],[UserID],[PayFlowNo],[ConfirmFlag])" +
            " VALUES(#{0}#,'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8})";

        public string PayTraceNo = "";

        /// <summary>
        /// 根据ResponseData添加交易记录
        /// </summary>
        /// <param name="responseInfo"></param>
        public void InsertTransLog(ResponseData rd)
        {
            string strSql = String.Format(strSqlIcTransFormat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
               rd.TraceNo, rd.BankCardNo, rd.BatchNo, rd.RefNo, rd.Amount, rd.UserID, rd.PayFlowNo, 0);
            ExecuteCommand(strSql);
        }

        /// <summary>
        /// 根据流水号来更新销账记录
        /// </summary>
        /// <param name="confirmFlag">0重新发送，1成功，2冲正</param>
        public void UpdateTransLog(EnumConfirmFlag confirmFlag)
        {
            string strSql = String.Format(@"UPDATE transDetailLog SET ConfirmFlag={0} WHERE [TraceNo] = '{1}'", (int)confirmFlag, PayTraceNo);
            ExecuteCommand(strSql);
        }

        /// <summary>
        /// 删除交易记录
        /// </summary>
        public void DeleteTransLog()
        {
            string strSql =  @"DELETE FROM transDetailLog";
            int iRet = ExecuteCommand(strSql);
        }

        /// <summary>
        /// 提取**当天销账失败**记录
        /// </summary>
        /// <returns></returns>
        public IList<ResponseData> SelectTransLogList()
        {
            IList<ResponseData> responseList = new List<ResponseData>();
            string strSql = "SELECT * FROM transDetailLog WHERE [ConfirmFlag] = 0 ORDER BY [AddTime] DESC";
            
           
            DbDataReader dr = null;
            using (dr = ExecuteReader(strSql))
            {
                while (dr.Read())
                {
                    ResponseData responseInfo = new ResponseData();
                    responseInfo.TraceNo = (dr.IsDBNull(dr.GetOrdinal("traceNo"))) ? null : (System.String)dr["traceNo"];
                    responseInfo.BankCardNo = (dr.IsDBNull(dr.GetOrdinal("CardNo"))) ? null : (System.String)dr["CardNo"];
                    responseInfo.Amount = (dr.IsDBNull(dr.GetOrdinal("Amount"))) ? null : (System.String)dr["Amount"];
                    responseInfo.BatchNo = (dr.IsDBNull(dr.GetOrdinal("BatchNo"))) ? null : (System.String)dr["BatchNo"];
                    responseInfo.UserID = (dr.IsDBNull(dr.GetOrdinal("UserID"))) ? null : (System.String)dr["UserID"];
                    responseInfo.PayFlowNo = (dr.IsDBNull(dr.GetOrdinal("PayFlowNo"))) ? null : (System.String)dr["PayFlowNo"];
                    responseInfo.RefNo = (dr.IsDBNull(dr.GetOrdinal("RefNo"))) ? null : (System.String)dr["RefNo"];
                    DateTime temp = (dr.IsDBNull(dr.GetOrdinal("AddTime"))) ? DateTime.MinValue : (System.DateTime)dr["AddTime"];
                    TimeSpan ts = DateTime.Now - temp;
                    if (ts.Days == 0)
                        responseList.Add(responseInfo);
                }
                dr.Close();
            }
            return responseList;
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="ConfigString"></param>
        /// <returns></returns>
        public override string GetConnectionString(string ConfigString)
        {
            string dbPath = Path.Combine(Application.StartupPath, entity.AccessFile);
            string dbPsd = entity.AccessPin;
            string strConn = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password={1}", dbPath, dbPsd);
            return strConn;
        }
        /// <summary>
        /// 获取数据库驱动
        /// </summary>
        /// <param name="ConfigString"></param>
        /// <returns></returns>
        public override string GetProviderName(string ConfigString)
        {
            string ProviderName = entity.AccessProviderName;
            return ProviderName;
        }
    }
}
