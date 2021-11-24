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
	public class ExamDetailsDAO : IDisposable
	{
		private static volatile ExamDetailsDAO instance;
		private static readonly object lockObj = new object();
		public static ExamDetailsDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new ExamDetailsDAO();
			}
			return instance;
		}
		public static ExamDetailsDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new ExamDetailsDAO();
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

		public ExamDetailsDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<ExamDetails> Get(Int32? id = null)
		{
			try
			{
				List<ExamDetails> ExamDetailsLst = new List<ExamDetails>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				ExamDetailsLst = dbExecutor.FetchData<ExamDetails>(CommandType.StoredProcedure, "wsp_ExamDetails_Get", colparameters);
				return ExamDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<ExamDetails> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<ExamDetails> ExamDetailsLst = new List<ExamDetails>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				ExamDetailsLst = dbExecutor.FetchData<ExamDetails>(CommandType.StoredProcedure, "wsp_ExamDetails_GetDynamic", colparameters);
				return ExamDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<ExamDetails> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<ExamDetails> ExamDetailsLst = new List<ExamDetails>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				ExamDetailsLst = dbExecutor.FetchDataRef<ExamDetails>(CommandType.StoredProcedure, "wsp_ExamDetails_GetPaged", colparameters, ref rows);
				return ExamDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(ExamDetails _ExamDetails, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[4]{
				new Parameters("@paramId", _ExamDetails.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramExamId", _ExamDetails.ExamId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramQuestionId", _ExamDetails.QuestionId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_ExamDetails_Post", colparameters, true);
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
