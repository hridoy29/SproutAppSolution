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
	public class CourseDetailsDAO : IDisposable
	{
		private static volatile CourseDetailsDAO instance;
		private static readonly object lockObj = new object();
		public static CourseDetailsDAO GetInstance()
		{
			if (instance == null)
			{
				instance = new CourseDetailsDAO();
			}
			return instance;
		}
		public static CourseDetailsDAO GetInstanceThreadSafe
		{
			get
			{
				if (instance == null)
				{
					lock (lockObj)
					{
						if (instance == null)
						{
							instance = new CourseDetailsDAO();
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

		public CourseDetailsDAO()
		{
			//dbExecutor = DBExecutor.GetInstanceThreadSafe;
			dbExecutor = new DBExecutor();
		}

		public List<CourseDetails> Get(Int32? id = null)
		{
			try
			{
				List<CourseDetails> CourseDetailsLst = new List<CourseDetails>();
				Parameters[] colparameters = new Parameters[1]{
				new Parameters("@paramId", id, DbType.Int32, ParameterDirection.Input)
				};
				CourseDetailsLst = dbExecutor.FetchData<CourseDetails>(CommandType.StoredProcedure, "wsp_CourseDetails_Get", colparameters);
				return CourseDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<CourseDetails> GetDynamic(string whereCondition,string orderByExpression)
		{
			try
			{
				List<CourseDetails> CourseDetailsLst = new List<CourseDetails>();
				Parameters[] colparameters = new Parameters[2]{
				new Parameters("@paramWhereCondition", whereCondition, DbType.String, ParameterDirection.Input),
				new Parameters("@paramOrderByExpression", orderByExpression, DbType.String, ParameterDirection.Input),
				};
				CourseDetailsLst = dbExecutor.FetchData<CourseDetails>(CommandType.StoredProcedure, "wsp_CourseDetails_GetDynamic", colparameters);
				return CourseDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<CourseDetails> GetPaged(int startRecordNo, int rowPerPage, string whereClause, string sortColumn, string sortOrder, ref int rows)
		{
			try
			{
				List<CourseDetails> CourseDetailsLst = new List<CourseDetails>();
				Parameters[] colparameters = new Parameters[5]{
				new Parameters("@StartRecordNo", startRecordNo, DbType.Int32, ParameterDirection.Input),
				new Parameters("@RowPerPage", rowPerPage, DbType.Int32, ParameterDirection.Input),
				new Parameters("@WhereClause", whereClause, DbType.String, ParameterDirection.Input),
				new Parameters("@SortColumn", sortColumn, DbType.String, ParameterDirection.Input),
				new Parameters("@SortOrder", sortOrder, DbType.String, ParameterDirection.Input),
				};
				CourseDetailsLst = dbExecutor.FetchDataRef<CourseDetails>(CommandType.StoredProcedure, "wsp_CourseDetails_GetPaged", colparameters, ref rows);
				return CourseDetailsLst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string Post(CourseDetails _CourseDetails, string transactionType)
		{
			string ret = string.Empty;
			try
			{
				Parameters[] colparameters = new Parameters[6]{
				new Parameters("@paramId", _CourseDetails.Id, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramCourseId", _CourseDetails.CourseId, DbType.Int32, ParameterDirection.Input),
				new Parameters("@paramVideoContent", _CourseDetails.VideoContent, DbType.String, ParameterDirection.Input),
				new Parameters("@paramAudioContent", _CourseDetails.AudioContent, DbType.String, ParameterDirection.Input),
				new Parameters("@paramTextContent", _CourseDetails.TextContent, DbType.String, ParameterDirection.Input),
				new Parameters("@paramTransactionType", transactionType, DbType.String, ParameterDirection.Input)
				};
				dbExecutor.ManageTransaction(TransactionType.Open);
				ret = dbExecutor.ExecuteScalarString(true, CommandType.StoredProcedure, "wsp_CourseDetails_Post", colparameters, true);
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
