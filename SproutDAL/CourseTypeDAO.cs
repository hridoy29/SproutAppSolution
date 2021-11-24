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
	public class CourseTypeDAO : IDisposable
	{
		private static volatile CourseTypeDAO instance;
		private static readonly object lockObj = new object();
		public static CourseTypeDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new CourseTypeDAO();
			}
			return instance;
		}
		public static CourseTypeDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new CourseTypeDAO();
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

		public CourseTypeDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<CourseType> Get(Int32? id = null)
		{
			try
			{
				List<CourseType> CourseTypeLst = new List<CourseType>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				CourseTypeLst = dbExecutor.FetchData<CourseType>(CommandType.StoredProcedure, "wsp_CourseType_Get", colparameters);
				return CourseTypeLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<CourseType> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<CourseType> CourseTypeLst = new List<CourseType>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				CourseTypeLst = dbExecutor.FetchData<CourseType>(CommandType.StoredProcedure, "wsp_CourseType_GetDynamic", colparameters);
				return CourseTypeLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<CourseType> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<CourseType> CourseTypeLst = new List<CourseType>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				CourseTypeLst = dbExecutor.FetchDataRef<CourseType>(CommandType.StoredProcedure, "wsp_CourseType_GetPaged", colparameters, ref rows);
				return CourseTypeLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(CourseType _CourseType, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[8]{
				new Parameters("@paramId", _CourseType.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramCourseTypeName", _CourseType.CourseTypeName, DbType.String, ParameterDirection.Input),
				new Parameters("@paramIsActive", _CourseType.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _CourseType.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _CourseType.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _CourseType.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _CourseType.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_CourseType_Post", colparameters, true);
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
