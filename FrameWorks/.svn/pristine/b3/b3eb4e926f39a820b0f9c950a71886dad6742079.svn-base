using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;

/*
 * ==========================================================
 *      开发人员:柯永林
 *      当前版本：1.0
 *      创建时间：2006-11-06
 *      修改时间：
 *      功能说明：
 * ==========================================================
 * */
namespace Landi.FrameWorks
{
    public interface DataAccessInterface
    {
        string ConfigString{get;set;}
        string GetProviderName(string ConfigString);
        string GetProviderName();
        //=====================================================获得连接字符串====================================
        /// <summary>
        /// 获得连接字符串
        /// </summary>
        /// <returns></returns>
        string GetConnectionString(string ConfigString);
        string GetConnectionString();
        //==============================================GetDbproviderFactory=========================
        DbProviderFactory GetDbProviderFactory();
        DbProviderFactory GetDbProviderFactory(string providername);
        //==============================================CreateConnection=============================
        DbConnection CreateConnection();
        DbConnection CreateConnection(string provdername);
        //==============================================CreateCommand================================
        DbCommand CreateCommand(string sql, CommandType cmdType, DbParameter[] parameters);
        DbCommand CreateCommand(string sql);
        DbCommand CreateCommand(string sql, CommandType cmdtype);
        DbCommand CreateCommand(string sql, DbParameter[] parameters);
        //=========================================CreateAdapter()==============================================
        DbDataAdapter CreateAdapter(string sql);
        DbDataAdapter CreateAdapter(string sql, CommandType cmdtype);
        DbDataAdapter CreateAdapter(string sql, CommandType cmdtype, DbParameter[] parameters);
        DbParameter CreateParameter(string field, string dbtype, string value);
        //======================================================ExecuteCommand()============================================
        int ExecuteCommand(string sql);
        int ExecuteCommand(string sql, CommandType cmdtype);
        int ExecuteCommand(string sql, DbParameter[] parameters);
        bool ExecuteCommand(ArrayList SqlList);
        int ExecuteCommand(string sql, CommandType cmdtype, DbParameter[] parameters);
        //=====================================================ExecuteScalar()=============================================
        object ExecuteScalar(string sql);
        object ExecuteScalar(string sql, CommandType cmdtype);
        object ExecuteScalar(string sql, DbParameter[] parameters);
        object ExecuteScalar(string sql, CommandType cmdtype, DbParameter[] parameters);
        //=====================================================ExecuteReader()=============================================
        DbDataReader ExecuteReader(string sql);
        DbDataReader ExecuteReader(string sql, CommandType cmdtype);
        DbDataReader ExecuteReader(string sql, DbParameter[] parameters);
        DbDataReader ExecuteReader(string sql, CommandType cmdtype, DbParameter[] parameters);
        DataSet GetDataSet(string sql);
        DataSet GetDataSet(string sql, CommandType cmdtype);
        DataSet GetDataSet(string sql, CommandType cmdtype, DbParameter[] parameters);
        DataSet GetDataSet(string sql, int StartIndex, int RecordCount);
        DataView GetDataView(string sql);
        DataView GetDataView(string sql, CommandType cmdtype);
        DataView GetDataView(string sql, DbParameter[] parameters);
        DataView GetDataView(string sql, CommandType cmdtype, DbParameter[] parameters);
        DataView GetDataView(string sql, int StartIndex, int RecordCount);
        DataTable GetDataTable(string sql);
        DataTable GetDataTable(string sql, CommandType cmdtype);
        DataTable GetDataTable(string sql, DbParameter[] parameters);
        DataTable GetDataTable(string sql, CommandType cmdtype, DbParameter[] parameters);
        DataTable GetDataTable(string sql, int StartIndex, int RecordCount);
        DataTable GetDataTable(string sql, int SizeCount);
    }
}
