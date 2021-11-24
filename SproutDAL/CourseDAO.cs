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
	public class CourseDAO : IDisposable
	{
		private static volatile CourseDAO instance;
		private static readonly object lockObj = new object();
		public static CourseDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new CourseDAO();
			}
			return instance;
		}
		public static CourseDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new CourseDAO();
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

		public CourseDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<Course> Get(Int32? id = null)
		{
			try
			{
				List<Course> CourseLst = new List<Course>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				CourseLst = dbExecutor.FetchData<Course>(CommandType.StoredProcedure, "wsp_Course_Get", colparameters);
				return CourseLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<Course> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<Course> CourseLst = new List<Course>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				CourseLst = dbExecutor.FetchData<Course>(CommandType.StoredProcedure, "wsp_Course_GetDynamic", colparameters);
				return CourseLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Course> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<Course> CourseLst = new List<Course>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				CourseLst = dbExecutor.FetchDataRef<Course>(CommandType.StoredProcedure, "wsp_Course_GetPaged", colparameters, ref rows);
				return CourseLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(Course _Course, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[9]{
				new Parameters("@paramId", _Course.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramCourseTypeId", _Course.CourseTypeId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramCourseName", _Course.CourseName, DbType.String, ParameterDirection.Input),
				new Parameters("@paramIsActive", _Course.IsActive, DbType.Boolean, ParameterDirection.Input),
				new Parameters("@paramCreator", _Course.Creator, DbType.String, ParameterDirection.Input),
				new Parameters("@paramCreationDate", _Course.CreationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramModifier", _Course.Modifier, DbType.String, ParameterDirection.Input),
				new Parameters("@paramModificationDate", _Course.ModificationDate, DbType.Date, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_Course_Post", colparameters, true);
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
