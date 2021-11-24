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
	public class ExamDAO : IDisposable
	{
		private static volatile ExamDAO instance;
		private static readonly object lockObj = new object();
		public static ExamDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new ExamDAO();
			}
			return instance;
		}
		public static ExamDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new ExamDAO();
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

		public ExamDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<Exam> Get(Int32? id = null)
		{
			try
			{
				List<Exam> ExamLst = new List<Exam>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				ExamLst = dbExecutor.FetchData<Exam>(CommandType.StoredProcedure, "wsp_Exam_Get", colparameters);
				return ExamLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<Exam> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<Exam> ExamLst = new List<Exam>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				ExamLst = dbExecutor.FetchData<Exam>(CommandType.StoredProcedure, "wsp_Exam_GetDynamic", colparameters);
				return ExamLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Exam> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<Exam> ExamLst = new List<Exam>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				ExamLst = dbExecutor.FetchDataRef<Exam>(CommandType.StoredProcedure, "wsp_Exam_GetPaged", colparameters, ref rows);
				return ExamLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(Exam _Exam, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[11]{
				new Parameters("@paramId", _Exam.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramExamTypeId", _Exam.ExamTypeId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramCourseId", _Exam.CourseId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramExamName", _Exam.ExamName, DbType.String, ParameterDirection.Input),
				new Parameters("@paramTimeDuration", _Exam.TimeDuration, DbType.Time, ParameterDirection.Input),
				new Parameters("@paramIsActive", _Exam.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _Exam.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _Exam.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _Exam.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _Exam.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_Exam_Post", colparameters, true);
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
