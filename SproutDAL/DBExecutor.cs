using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#region REQUIRED NAMESPACES
using System.Data.Common;
using System.Configuration;
using System.Data;
using System.Reflection;
#endregion

namespace DbExecutor
{
    public class DBExecutor : IDBUtility
    {
        #region INSTANCE/VARIABLE DECLARATION PRIVATE MEMEBERS
        private DbProviderFactory dbProviderFactory;
        private DbConnection dbConnection;
        private ConnectionState dbConnectionState = ConnectionState.Closed;
        public DbCommand dbCommand;
        private DbParameter dbParameter;
        private DbTransaction dbTransaction;
        private  DbDataAdapter dbDataAdapter;
        private bool blTransaction;

        public string connectionString = string.Empty;
        public string provider = string.Empty;

        string strOutputParamName = string.Empty; // use to retunr the value from stored procedure
        string strOutputParamValue =string.Empty;// use to retunr the value from stored procedure
        #endregion

        #region OPEN/CLOSE CONNECTION
        /// <summary>
        /// Description	    :This Function used to open database connection
           /// </summary>
        public void OpenConnection()
        {  
            try
            {
                dbConnection = dbProviderFactory.CreateConnection();
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.ConnectionString = connectionString;
                    dbConnection.Open();
                    dbConnectionState = ConnectionState.Open;
                }
            }
            catch (DbException objException)
            {
                throw objException;
            }
            catch (Exception objException)
            {
                throw objException;
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Close Database Connection
        /// </summary>
        public void CloseConnection()
        {

            //check for an open connection            
            try
            {
                if (dbConnection.State == ConnectionState.Open)
                {
                    dbConnection.Close();
                    dbConnectionState = ConnectionState.Closed;
                }
            }
            catch (DbException dbexception)
            {
                //catch any SQL server data provider generated error messag
                throw new Exception(dbexception.Message);
            }
            catch (System.NullReferenceException nullerror)
            {
                throw new Exception(nullerror.Message);
            }
            finally
            {
                if (null != dbConnection)
                    dbConnection.Dispose();
            }

        }
        #endregion

        #region TRANSACTION

        /// <summary>
        ///Description	    :	to handle transaction related 
        /// </summary>
        public void ManageTransaction(TransactionType transactiontype)
        {
           
            switch (transactiontype)
            {
                case TransactionType.Open:  //open a transaction
                    try
                    {
                       
                        OpenConnection();                        
                        dbTransaction = dbConnection.BeginTransaction();
                        blTransaction = true;
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw oErr;
                    }
                    break;

                case TransactionType.Commit:  //commit the transaction
                    if (null != dbTransaction.Connection)
                    {
                        try
                        {
                            dbTransaction.Commit();
                            blTransaction = false;
                            CloseConnection();
                        }
                        catch (InvalidOperationException err)
                        {
                            throw err ;
                        }
                    }
                    break;

                case TransactionType.Rollback:  //rollback the transaction
                    try
                    {
                        if (blTransaction)
                        {
                            dbTransaction.Rollback();
                        }
                        blTransaction = false;
                    }
                    catch (InvalidOperationException err)
                    {
                        throw err;
                    }
                    break;
            }
        }

        #endregion

        #region PRIVATE METHODS

        #region PARAMETER METHODS


        #region STRUCTURE BASED

        /// <summary>
        ///Description	    :	This function is used to Create Parameters for the Command For Execution        
        /// </summary>
        private void CreateDBParameters(Parameters[] parameters)
        {
            
            try
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Parameters param = (Parameters)parameters[i];

                    dbParameter = dbCommand.CreateParameter();
                    dbParameter.ParameterName = param.paramName;
                    dbParameter.Value = param.paramValue;
                    dbParameter.DbType = param.paramType;
                    dbParameter.Direction = param.paramDirection;
                    dbCommand.Parameters.Add(dbParameter);
                    if (param.paramDirection == ParameterDirection.Output || param.paramDirection == ParameterDirection.ReturnValue)
                        strOutputParamName = param.paramName;
                }
            }
            catch (TargetParameterCountException objParamException)
            {
                throw objParamException;
            }
            catch (Exception objParamException)
            {
                throw  objParamException;
            }
        }
        #endregion

        #endregion
        private void SetupCommand(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {   
            try
            {

                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.ConnectionString = connectionString;
                    dbConnection.Open();
                    dbConnectionState = ConnectionState.Open;
                }

                dbCommand = dbProviderFactory.CreateCommand();
                dbCommand.Connection = dbConnection;
                dbCommand.CommandText = cmdText;
                dbCommand.CommandType = cmdType;

                if (blTransaction)
                    dbCommand.Transaction = dbTransaction;

                if (null != cmdParms)
                    CreateDBParameters(cmdParms);
            }
            catch (DbException objException)
            {
                throw objException;
            }
            catch (Exception objException)
            {
                throw objException;
            }

        }
        private void SetupCommand(bool blTransaction, CommandType cmdType, string cmdText)
        {
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.ConnectionString = connectionString;
                    dbConnection.Open();
                    dbConnectionState = ConnectionState.Open;
                }

                if (null == dbCommand)
                    dbCommand = dbProviderFactory.CreateCommand();

                dbCommand.Connection = dbConnection;
                dbCommand.CommandText = cmdText;
                dbCommand.CommandType = cmdType;

                if (blTransaction)
                    dbCommand.Transaction = dbTransaction;
            }
            catch (DbException objException)
            {
                throw objException;
            }
            catch (Exception objException)
            {
                throw objException;
            }
        }
        #endregion

        #region PUBLIC MEMBERS
        #region STRUCTURE BASED PARAMETER ARRAY
        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Reader	        
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {

            try
            {
                OpenConnection();
                SetupCommand(false, cmdType, cmdText, cmdParms);
                return dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();
            }
        }
        #region PARAMETERLESS METHODS

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Reader	        
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {

                OpenConnection();
                SetupCommand(false, cmdType, cmdText);
                DbDataReader dr = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                dbCommand.Parameters.Clear();
                return dr;

            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();
            }
        }

        #endregion
        #endregion

        #region ADAPTER METHODS
        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Adapter	        
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Data Set
        ///Comments			:	
        /// </summary>
        public DataSet DataAdapter(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            DbDataAdapter dda = null;
            try
            {
                OpenConnection();
                dda = dbProviderFactory.CreateDataAdapter();
                SetupCommand(false, cmdType, cmdText, cmdParms);

                dda.SelectCommand = dbCommand;
                DataSet ds = new DataSet();
                dda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();
                CloseConnection();
            }
        }

        #endregion
        #endregion

        #region SCALAR METHODS

        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	

        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {
                OpenConnection();
                SetupCommand(false, cmdType, cmdText, cmdParms);
                return dbCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                CloseConnection();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	Overloaded Method. 
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {
            return ExecuteScalar(cmdType, cmdText, cmdParms, true);
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {

                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                return dbCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {
            return ExecuteScalar(blTransaction, cmdType, cmdText, cmdParms, true);
        }

        #endregion

        public int ExecuteScalar32(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            int result = 0;
            try
            {
                //OpenConnection();
                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                result = Convert.ToInt32(dbCommand.ExecuteScalar());
            }
            catch (DbException except)
            {
                throw except;
            }
            catch (Exception excpt)
            {
                throw excpt;
            }

            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                if (!blTransaction)
                    CloseConnection();
            }
            return result;
        }

        public Int64 ExecuteScalar64(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            Int64 result = 0;
            try
            {
                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                result = Convert.ToInt32(dbCommand.ExecuteScalar());
            }
            catch (DbException except)
            {
                throw except;
            }
            catch (Exception excpt)
            {
                throw excpt;
            }

            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                if (!blTransaction)
                    CloseConnection();
            }
            return result;
        }

        public Int64 ExecuteScalar64Ref(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand, ref string text)
        {
            Int64 result = 0;
            int result2 = 0;
            decimal result3 = 0;
            try
            {
                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                //result = Convert.ToInt32(dbCommand.ExecuteScalar());

                DbDataReader dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (dbDataReader.Read())
                {
                    result3 = dbDataReader.GetDecimal(0);
                }
                if ((dbDataReader.NextResult()) && (dbDataReader.Read()))
                {
                    text = dbDataReader.GetString(0);
                }
                if (null != dbDataReader)
                    dbDataReader.Dispose();
            }
            catch (DbException except)
            {
                throw except;
            }
            catch (Exception excpt)
            {
                throw excpt;
            }

            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                if (!blTransaction)
                    CloseConnection();
            }
            return result=Convert.ToInt64(result3);
        }

        public string ExecuteScalarString(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            string result = string.Empty;
            try
            {
                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                result = Convert.ToString(dbCommand.ExecuteScalar());
            }
            catch (DbException except)
            {
                throw except;
            }
            catch (Exception excpt)
            {
                throw excpt;
            }

            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                if (!blTransaction)
                    CloseConnection();
            }
            return result;
        }

        public int ExecuteStoredProcedure(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            int result = 0;
            try
            {
                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                result = dbCommand.ExecuteNonQuery();
            }
            catch (DbException except)
            {
                throw except;
            }
            catch (Exception excpt)
            {

                throw excpt;
            }

            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                if (!blTransaction)
                    CloseConnection();
            }
            return result;
        }
        
        public object ExecuteNonQueryForReturnValue(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            object result = null;
            try
            {
                SetupCommand(blTransaction, cmdType, cmdText, cmdParms);
                result =dbCommand.ExecuteScalar();
                strOutputParamName = string.Empty;
            }
            catch (DBConcurrencyException objException)
            {
                throw objException;
            }
            catch (DbException objException)
            {
                throw objException;
            }
            catch (Exception objException)
            {
                throw objException;
            }
            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
            }
            return result;
        }
        #endregion

        #region FETCH DATA
        public List<T> FetchData<T>(CommandType cmdType, string cmdText) where T : new()
        {
            
            List<T> list = new List<T>();
            try
            {

                OpenConnection();
                SetupCommand(false, cmdType, cmdText);
                dbCommand.Parameters.Clear();
                DbDataReader dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);


                T obj = new T();

                while (dbDataReader.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        int ordinal = -1;
                        try
                        {
                            ordinal = dbDataReader.GetOrdinal(prop.Name);
                        }
                        catch
                        {
                            ordinal = -1;
                        }
                        if (ordinal > -1 && !object.Equals(dbDataReader[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dbDataReader[prop.Name], null);
                        }

                    }
                    list.Add(obj);

                }
                if (null != dbDataReader)
                    dbDataReader.Dispose();

            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();
            }
            return list;
        }
        public List<T> FetchData<T>(CommandType cmdType, string cmdText, Parameters[] param) where T : new()
        {
            
            List<T> list = new List<T>();
            try
            {

                OpenConnection();
                SetupCommand(false, cmdType, cmdText, param);

                DbDataReader dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                dbCommand.Parameters.Clear();

                List<string> columns = Enumerable.Range(0, dbDataReader.FieldCount).Select(dbDataReader.GetName).ToList();

                T obj = new T();

                while (dbDataReader.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        string columnName = string.Empty;
                        try
                        {
                            columnName = columns.FirstOrDefault(c => c == prop.Name);
                        }
                        catch (Exception)
                        {
                            
                        }
                        if (!string.IsNullOrEmpty(columnName) && (!object.Equals(dbDataReader[prop.Name], DBNull.Value)))
                        {
                            prop.SetValue(obj, dbDataReader[prop.Name], null);
                        }

                    }
                    list.Add(obj);

                }
                if (null != dbDataReader)
                    dbDataReader.Dispose();

            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();

            }
            return list;
        }
        public List<T> FetchDataRef<T>(CommandType cmdType, string cmdText, Parameters[] param, ref int rows) where T : new()
        {
            List<T> list = new List<T>();
            try
            {

                OpenConnection();
                SetupCommand(false, cmdType, cmdText, param);

                DbDataReader dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                dbCommand.Parameters.Clear();

                List<string> columns = Enumerable.Range(0, dbDataReader.FieldCount).Select(dbDataReader.GetName).ToList();

                T obj = new T();

                while (dbDataReader.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        string columnName = string.Empty;
                        try
                        {
                            columnName = columns.FirstOrDefault(c => c == prop.Name);
                        }
                        catch (Exception)
                        {

                        }
                        if (!string.IsNullOrEmpty(columnName) && (!object.Equals(dbDataReader[prop.Name], DBNull.Value)))
                        {
                            prop.SetValue(obj, dbDataReader[prop.Name], null);
                        }

                    }
                    list.Add(obj);

                }
                if ((dbDataReader.NextResult()) && (dbDataReader.Read()))
                {
                    rows = dbDataReader.GetInt32(0);
                }
                if (null != dbDataReader)
                    dbDataReader.Dispose();

            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();

            }
            return list;
        }
        public DataTable GetDataTable(CommandType cmdType, string cmdText, bool blDisposeCommand)
        {

            DataTable dtResult = new DataTable();
            try
            {
                OpenConnection();
                SetupCommand(false, cmdType, cmdText);
                dbDataAdapter = dbProviderFactory.CreateDataAdapter();
                dbDataAdapter.SelectCommand = dbCommand;
                dbDataAdapter.Fill(dtResult);
                dbCommand.Parameters.Clear();
            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();

                if (dbDataAdapter != null)
                    dbDataAdapter.Dispose();
            }
            return dtResult;
        }
        public DataTable GetDataTable(CommandType cmdType, string cmdText, Parameters[] param, bool blDisposeCommand)
        {
            
            DataTable dtResult = new DataTable();
            try
            {
                OpenConnection();
                SetupCommand(false, cmdType, cmdText, param);
                dbDataAdapter = dbProviderFactory.CreateDataAdapter();
                dbDataAdapter.SelectCommand = dbCommand;
                dbDataAdapter.Fill(dtResult);
                dbCommand.Parameters.Clear();
            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();

                if (dbDataAdapter != null)
                    dbDataAdapter.Dispose();
            }
            return dtResult;
        }
        #endregion
        public int ExecuteNonQuery(System.Data.CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {
                OpenConnection();
                SetupCommand(false, cmdType, cmdText, cmdParms);
                return dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != dbCommand)
                    dbCommand.Dispose();
                CloseConnection();
            }
        }
        #endregion

        #region CONSTRUCTOR SINGLE TONE PATTERN
        private static volatile DBExecutor instance;
        private static readonly object lockObj = new object();
        /// <summary>
        /// Returns instance of SEPHelper
        /// </summary>
        /// <returns></returns>
        public static DBExecutor GetInstance()
        {
            if (instance == null)
            {
                instance = new DBExecutor();
            }
            return instance;
        }

        public static DBExecutor GetInstanceThreadSafe
        {
            get
            {
                if (instance == null) // First check
                {
                    lock (lockObj)
                    {
                        if (instance == null) // Second check
                        {
                            instance = new DBExecutor();
                        }
                    }
                }
                return instance;
            }
        }
        public DBExecutor()
        {
            
            try
            {
                //this.provider = ConfigurationManager.AppSettings["PROVIDER"];
                //dbProviderFactory = DbProviderFactories.GetFactory(provider);
                //ConnectionStringsSection connectionStringsSection = GetConnectionStringsSection();
                //dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                //connectionString = "Data Source=192.168.0.7; Initial Catalog=Retail; User Id=sa; Password=dbserver420#; Connect Timeout=90;";
                ConnectionStringsSection connectionStringsSection = GetConnectionStringsSection();
                dbProviderFactory = DbProviderFactories.GetFactory(connectionStringsSection.ConnectionStrings["dbCon"].ProviderName);
                connectionString = connectionStringsSection.ConnectionStrings["dbCon"].ConnectionString;
            }
            catch (DbException objException)
            {
                throw objException;
            }
            catch (Exception objException)
            {
                throw objException;
            }
        }



        #endregion

        #region IDisposable Members
        // Track whether Dispose has been called.
        private bool disposed = false;
        // Pointer to an external unmanaged resource.
        private IntPtr handle;

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Call the appropriate methods to clean up unmanaged resources here.
                // If disposing is false, only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use interop to call the method necessary to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# destructor syntax for finalization code. This destructor will run only if the Dispose method does not get called.
        // It gives your base class the opportunity to finalize. Do not provide destructors in types derived from this class.
        ~DBExecutor()
        {
            // Do not re-create Dispose clean-up code here. // Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }
        #endregion

        public Dictionary<string, string> FetchData(CommandType cmdType, string cmdText, Parameters[] param)
        {
            Dictionary<string, string> fetchrecord = new Dictionary<string, string>();
            try
            {

                OpenConnection();
                SetupCommand(false, cmdType, cmdText);
                dbCommand.Parameters.Clear();
                DbDataReader dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (dbDataReader.Read())
                {
                    fetchrecord.Add(dbDataReader.GetValue(0).ToString(), dbDataReader.GetValue(1).ToString());

                }
                if (null != dbDataReader)
                    dbDataReader.Dispose();

            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();
            }
            return fetchrecord;
        }
        public Dictionary<string, string> FetchData(CommandType cmdType, string cmdText, Parameters[] param,int key_as_column_index,int value_as_column_index)
        {
            Dictionary<string, string> fetchrecord = new Dictionary<string, string>();
            try
            {

                OpenConnection();
                SetupCommand(false, cmdType, cmdText);
                dbCommand.Parameters.Clear();
                DbDataReader dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (dbDataReader.Read())
                {
                    fetchrecord.Add(dbDataReader.GetValue(key_as_column_index).ToString(), dbDataReader.GetValue(value_as_column_index).ToString());

                }
                if (null != dbDataReader)
                    dbDataReader.Dispose();

            }
            catch (DbException excpt)
            {
                CloseConnection();
                throw excpt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                if (null != dbCommand)
                    dbCommand.Dispose();
            }
            return fetchrecord;
        }

        public void InitializeDBConnection(string connectionstring, string provider)
        {
            this.connectionString = connectionstring;
            this.provider = provider;
        }

        private ConnectionStringsSection GetConnectionStringsSection()
        {
            return ConfigurationManager.GetSection("connectionStrings") as ConnectionStringsSection;
        }

      /* public Dictionary<string, string> FetchData(CommandType cmdType, string cmdText, Parameters[] param, int key_as_column_index, int value_as_column_index)
        {
            Dictionary<string, string> fetchrecord = new Dictionary<string, string>();
            try
            {
                try
                {
                    this.OpenConnection();
                    this.SetupCommand(false, cmdType, cmdText);
                    this.dbCommand.Parameters.Clear();
                    DbDataReader dbDataReader = this.dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dbDataReader.Read())
                    {
                        fetchrecord.Add(dbDataReader.GetValue(key_as_column_index).ToString(), dbDataReader.GetValue(value_as_column_index).ToString());
                    }
                    if (null != dbDataReader)
                    {
                        dbDataReader.Dispose();
                    }
                }
                catch (DbException dbException)
                {
                    DbException excpt = dbException;
                    this.CloseConnection();
                    throw excpt;
                }
                catch (Exception exception)
                {
                    Exception ex = exception;
                    this.CloseConnection();
                    throw ex;
                }
            }
            finally
            {
                if (null != this.dbCommand)
                {
                    this.dbCommand.Dispose();
                }
            }
            return fetchrecord;
        }*/
    }
}
