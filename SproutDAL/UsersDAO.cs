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
	public class UsersDAO : IDisposable
	{
		private static volatile UsersDAO instance;
		private static readonly object lockObj = new object();
		public static UsersDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new UsersDAO();
			}
			return instance;
		}
		public static UsersDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new UsersDAO();
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

		public UsersDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<User> Get()
		{
			try
			{
				List<User> UsersLst = new List<User>();				 
				UsersLst = dbExecutor.FetchData<User>(CommandType.StoredProcedure, "wsp_Users_Get");
				return UsersLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public Users Get(int id)
        {
            try
            {
                Users User = new Users();
                // Parameters idParam =new Parameters("@_paramId",DbType.int, ParameterDirection.Input);
                //User = dbExecutor.FetchData<Users>(CommandType.StoredProcedure, "wsp_Users_Get");
                return User;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Users> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<Users> UsersLst = new List<Users>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				UsersLst = dbExecutor.FetchData<Users>(CommandType.StoredProcedure, "wsp_Users_GetDynamic", colparameters);
				return UsersLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Users> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<Users> UsersLst = new List<Users>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				UsersLst = dbExecutor.FetchDataRef<Users>(CommandType.StoredProcedure, "wsp_Users_GetPaged", colparameters, ref rows);
				return UsersLst;
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
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_Users_Post", colparameters, true);
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
