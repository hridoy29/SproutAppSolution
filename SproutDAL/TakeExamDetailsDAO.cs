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
	public class TakeExamDetailsDAO : IDisposable
	{
		private static volatile TakeExamDetailsDAO instance;
		private static readonly object lockObj = new object();
		public static TakeExamDetailsDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new TakeExamDetailsDAO();
			}
			return instance;
		}
		public static TakeExamDetailsDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new TakeExamDetailsDAO();
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

		public TakeExamDetailsDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<TakeExamDetails> Get(Int32? id = null)
		{
			try
			{
				List<TakeExamDetails> TakeExamDetailsLst = new List<TakeExamDetails>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				TakeExamDetailsLst = dbExecutor.FetchData<TakeExamDetails>(CommandType.StoredProcedure, "wsp_TakeExamDetails_Get", colparameters);
				return TakeExamDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<TakeExamDetails> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<TakeExamDetails> TakeExamDetailsLst = new List<TakeExamDetails>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				TakeExamDetailsLst = dbExecutor.FetchData<TakeExamDetails>(CommandType.StoredProcedure, "wsp_TakeExamDetails_GetDynamic", colparameters);
				return TakeExamDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<TakeExamDetails> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<TakeExamDetails> TakeExamDetailsLst = new List<TakeExamDetails>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				TakeExamDetailsLst = dbExecutor.FetchDataRef<TakeExamDetails>(CommandType.StoredProcedure, "wsp_TakeExamDetails_GetPaged", colparameters, ref rows);
				return TakeExamDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(TakeExamDetails _TakeExamDetails, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[6]{
				new Parameters("@paramId", _TakeExamDetails.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramQuestionId", _TakeExamDetails.QuestionId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramUserAnswer", _TakeExamDetails.UserAnswer, DbType.String, ParameterDirection.Input),
				new Parameters("@paramExamId", _TakeExamDetails.ExamId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramTakeExamId", _TakeExamDetails.TakeExamId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_TakeExamDetails_Post", colparameters, true);
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
