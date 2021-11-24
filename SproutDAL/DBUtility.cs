using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DbExecutor
{
    #region ENUMERATORS
    public enum TransactionType : uint
    {
        Open = 1,
        Commit = 2,
        Rollback = 3
    }
    #endregion
    /// <summary>
    /// Description : This Interface is to use all  data access layer
    /// Author : 
    /// Date " 10 Jan,2016
    /// </summary>
    public interface IDBUtility : IDisposable
    {
        
        void OpenConnection();
        void CloseConnection();
        int ExecuteNonQuery(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand);
        void ManageTransaction(TransactionType transactiontype);
        int ExecuteStoredProcedure(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand);
        object ExecuteNonQueryForReturnValue(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand);
        DbDataReader ExecuteReader(CommandType cmdType, string cmdText, Parameters[] cmdParms);
        DbDataReader ExecuteReader(CommandType cmdType, string cmdText);
        DataSet DataAdapter(CommandType cmdType, string cmdText, Parameters[] cmdParms);
        object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand);
        object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms);
        List<T> FetchData<T>(CommandType cmdType, string cmdText) where T : new();
        List<T> FetchData<T>(CommandType cmdType, string cmdText, Parameters[] param) where T : new();
        DataTable GetDataTable(CommandType cmdType, string cmdText, Parameters[] param, bool blDisposeCommand);
        Dictionary<string,string> FetchData(CommandType cmdType, string cmdText, Parameters[] param) ;
        Dictionary<string, string> FetchData(CommandType cmdType, string cmdText, Parameters[] param, int key_as_column_index, int value_as_column_index);
        void InitializeDBConnection(string connectionstring, string provider);
    }
}