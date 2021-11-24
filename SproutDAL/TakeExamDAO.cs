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
	public class TakeExamDAO : IDisposable
	{
		private static volatile TakeExamDAO instance;
		private static readonly object lockObj = new object();
		public static TakeExamDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new TakeExamDAO();
			}
			return instance;
		}
		public static TakeExamDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new TakeExamDAO();
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

		public TakeExamDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<TakeExam> Get(Int32? id = null)
		{
			try
			{
				List<TakeExam> TakeExamLst = new List<TakeExam>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				TakeExamLst = dbExecutor.FetchData<TakeExam>(CommandType.StoredProcedure, "wsp_TakeExam_Get", colparameters);
				return TakeExamLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<TakeExam> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<TakeExam> TakeExamLst = new List<TakeExam>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				TakeExamLst = dbExecutor.FetchData<TakeExam>(CommandType.StoredProcedure, "wsp_TakeExam_GetDynamic", colparameters);
				return TakeExamLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<TakeExam> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<TakeExam> TakeExamLst = new List<TakeExam>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				TakeExamLst = dbExecutor.FetchDataRef<TakeExam>(CommandType.StoredProcedure, "wsp_TakeExam_GetPaged", colparameters, ref rows);
				return TakeExamLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(TakeExam _TakeExam, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[9]{
				new Parameters("@paramId", _TakeExam.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramExamId", _TakeExam.ExamId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramUserId", _TakeExam.UserId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramIsActive", _TakeExam.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _TakeExam.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _TakeExam.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _TakeExam.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _TakeExam.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_TakeExam_Post", colparameters, true);
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
