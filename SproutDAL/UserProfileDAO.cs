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
	public class UserProfileDAO : IDisposable
	{
		private static volatile UserProfileDAO instance;
		private static readonly object lockObj = new object();
		public static UserProfileDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new UserProfileDAO();
			}
			return instance;
		}
		public static UserProfileDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new UserProfileDAO();
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

		public UserProfileDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<UserProfile> Get(Int32? id = null)
		{
			try
			{
				List<UserProfile> UserProfileLst = new List<UserProfile>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)};
				UserProfileLst = dbExecutor.FetchData<UserProfile>(CommandType.StoredProcedure, "wsp_UserProfile_Get", colparameters);
				return UserProfileLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<UserProfile> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<UserProfile> UserProfileLst = new List<UserProfile>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				UserProfileLst = dbExecutor.FetchData<UserProfile>(CommandType.StoredProcedure, "wsp_UserProfile_GetDynamic", colparameters);
				return UserProfileLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<UserProfile> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<UserProfile> UserProfileLst = new List<UserProfile>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				UserProfileLst = dbExecutor.FetchDataRef<UserProfile>(CommandType.StoredProcedure, "wsp_UserProfile_GetPaged", colparameters, ref rows);
				return UserProfileLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(UserProfile _UserProfile, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[6]{
				new Parameters("@paramId", _UserProfile.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramUserId", _UserProfile.UserId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramName", _UserProfile.Name, DbType.String, ParameterDirection.Input),
				new Parameters("@paramAddress1", _UserProfile.Address1, DbType.String, ParameterDirection.Input),
				new Parameters("@paramAddress2", _UserProfile.Address2, DbType.String, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_UserProfile_Post", colparameters, true);
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
