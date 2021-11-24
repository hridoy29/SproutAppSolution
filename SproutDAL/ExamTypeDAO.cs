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
	public class ExamTypeDAO : IDisposable
	{
		private static volatile ExamTypeDAO instance;
		private static readonly object lockObj = new object();
		public static ExamTypeDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new ExamTypeDAO();
			}
			return instance;
		}
		public static ExamTypeDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new ExamTypeDAO();
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

		public ExamTypeDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<ExamType> Get(Int32? id = null)
		{
			try
			{
				List<ExamType> ExamTypeLst = new List<ExamType>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				ExamTypeLst = dbExecutor.FetchData<ExamType>(CommandType.StoredProcedure, "wsp_ExamType_Get", colparameters);
				return ExamTypeLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<ExamType> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<ExamType> ExamTypeLst = new List<ExamType>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				ExamTypeLst = dbExecutor.FetchData<ExamType>(CommandType.StoredProcedure, "wsp_ExamType_GetDynamic", colparameters);
				return ExamTypeLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<ExamType> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<ExamType> ExamTypeLst = new List<ExamType>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				ExamTypeLst = dbExecutor.FetchDataRef<ExamType>(CommandType.StoredProcedure, "wsp_ExamType_GetPaged", colparameters, ref rows);
				return ExamTypeLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(ExamType _ExamType, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[8]{
				new Parameters("@paramId", _ExamType.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramExamTypeName", _ExamType.ExamTypeName, DbType.String, ParameterDirection.Input),
				new Parameters("@paramIsActive", _ExamType.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _ExamType.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _ExamType.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _ExamType.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _ExamType.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_ExamType_Post", colparameters, true);
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
