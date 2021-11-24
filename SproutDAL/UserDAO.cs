using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Common;
using DbExecutor;
using SproutEntity;

namespace SproutDAL
{
	public class UserDAO : IDisposable
	{
		private static volatile UserDAO instance;
		private static readonly object lockObj = new object();
		public static UserDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new UserDAO();
			}
			return instance;
		}
		public static UserDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new UserDAO();
						}
					}
				}
				return instance;
			}
		}

		public void Dispose()
		{
			((IDisposable)GetInstanceThreadSafe).Dispose();
		}

		DBExecutor dbExecutor;

		public UserDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<User> Get(Int32? id = null)
		{
			try
			{
				List<User> UserLst = new List<User>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				UserLst = dbExecutor.FetchData<User>(CommandType.StoredProcedure, "wsp_User_Get", colparameters);
				return UserLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<User> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<User> UserLst = new List<User>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				UserLst = dbExecutor.FetchData<User>(CommandType.StoredProcedure, "wsp_User_GetDynamic", colparameters);
				return UserLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<User> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<User> UserLst = new List<User>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				UserLst = dbExecutor.FetchDataRef<User>(CommandType.StoredProcedure, "wsp_User_GetPaged", colparameters, ref rows);
				return UserLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(User _User, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[9]{
				new Parameters("@paramId", _User.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramMobileNo", _User.MobileNo, DbType.String, ParameterDirection.Input),
				new Parameters("@paramPassword", _User.Password, DbType.String, ParameterDirection.Input),
				new Parameters("@paramIsActive", _User.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _User.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _User.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _User.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _User.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_User_Post", colparameters, true);
				dbExecutor.ManageTransaction(TransactionType.Commit);
			}
			catch (DBConcurrencyException except)
			{
				dbExecutor.ManageTransaction(TransactionType.Rollback);
				throw except;
			}
			catch (Exception ex)
			{
				dbExecutor.ManageTransaction(TransactionType.Rollback);
				throw ex;
			}
			return ret;
		}
	}
}
