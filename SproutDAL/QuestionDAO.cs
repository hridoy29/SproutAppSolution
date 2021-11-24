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
	public class QuestionDAO : IDisposable
	{
		private static volatile QuestionDAO instance;
		private static readonly object lockObj = new object();
		public static QuestionDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new QuestionDAO();
			}
			return instance;
		}
		public static QuestionDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new QuestionDAO();
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

		public QuestionDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<Question> Get(Int32? id = null)
		{
			try
			{
				List<Question> QuestionLst = new List<Question>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				QuestionLst = dbExecutor.FetchData<Question>(CommandType.StoredProcedure, "wsp_Question_Get", colparameters);
				return QuestionLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<Question> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<Question> QuestionLst = new List<Question>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				QuestionLst = dbExecutor.FetchData<Question>(CommandType.StoredProcedure, "wsp_Question_GetDynamic", colparameters);
				return QuestionLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Question> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<Question> QuestionLst = new List<Question>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				QuestionLst = dbExecutor.FetchDataRef<Question>(CommandType.StoredProcedure, "wsp_Question_GetPaged", colparameters, ref rows);
				return QuestionLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(Question _Question, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[9]{
				new Parameters("@paramId", _Question.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramQuestion", _Question.QuestionName, DbType.String, ParameterDirection.Input),
				new Parameters("@paramAnswer", _Question.Answer, DbType.String, ParameterDirection.Input),
				new Parameters("@paramIsActive", _Question.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _Question.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _Question.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _Question.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _Question.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_Question_Post", colparameters, true);
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
