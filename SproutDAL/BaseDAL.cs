using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SWIT.DAL
{
    public class BaseDAL
    {
        string EncryptionKey = "SWITSecpOSkEY";
        public static readonly string IsEncryptedConnection = ConfigurationManager.AppSettings["IsEncryptedConnection"].ToString();

        protected List<T> ExecuteReader<T>(Type _type, string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            List<T> lst = null;
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    CloseOpenConnection(con);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //lst = BuildList<T>(typeof(T), reader);
                        lst = GetAs<T>(reader);
                    }
                }
            }
            return lst;
        }

        protected List<T> ExecuteReader<T>(Type _type, string Query, CommandType cmdType, ref int rows, params SqlParameter[] parameters)
        {
            List<T> lst = null;
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    CloseOpenConnection(con);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //lst = BuildList<T>(typeof(T), reader);
                        lst = GetAs<T>(reader);
                        if ((reader.NextResult()) && (reader.Read()))
                        {
                            rows = reader.GetInt32(0);
                        }
                    }
                }
            }
            return lst;
        }

        protected Int32 ExecuteNonQueryToInt32(string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    SetDBNull(cmd);
                    CloseOpenConnection(con);
                    return Convert.ToInt32(cmd.ExecuteNonQuery());
                }
            }
        }

        protected Int32 ExecuteScalarToInt32(string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    SetDBNull(cmd);
                    CloseOpenConnection(con);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        protected Int64 ExecuteScalarToInt64(string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    SetDBNull(cmd);
                    CloseOpenConnection(con);
                    return Convert.ToInt64(cmd.ExecuteScalar());
                }
            }
        }

        protected Decimal ExecuteScalarToDecimal(string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    SetDBNull(cmd);
                    CloseOpenConnection(con);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        protected int DeleteByField(string Query)
        {
            return ExecuteNonQueryToInt32(Query, CommandType.Text);
        }

        protected DataTable GetDataTable(string Query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        protected DataSet GetDataSet(string Query)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        protected DataTable GetDataTable(string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        protected DataSet GetDataSet(string Query, CommandType cmdType, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(GetDbConStr()))
            {
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    cmd.CommandType = cmdType;
                    cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        private List<T> BuildList<T>(Type _type, SqlDataReader reader)
        {
            List<T> ret = new List<T>();
            T entity;

            // Get all the properties in Entity Class
            PropertyInfo[] props = _type.GetProperties();

            while (reader.Read())
            {
                // Create new instance of Entity
                entity = Activator.CreateInstance<T>();

                // Set all properties from the column names
                // NOTE: This assumes your column names are the
                //       same name as your class property names
                foreach (PropertyInfo col in props)
                {
                    try
                    {
                        if (reader[col.Name].Equals(DBNull.Value))
                            col.SetValue(entity, null, null);
                        else
                            col.SetValue(entity, reader[col.Name], null);
                    }
                    catch { }
                }

                ret.Add(entity);
            }

            return ret;
        }

        private static List<T> GetAs<T>(SqlDataReader reader)
        {
            List<T> ret = new List<T>();
            T entity;
            // Get all the properties in our Object
            PropertyInfo[] props = typeof(T).GetProperties();
            // For each property get the data from the reader to the object
            List<string> columnList = GetColumnList(reader);

            while (reader.Read())
            {
                entity = Activator.CreateInstance<T>();
                for (int i = 0; i < props.Length; i++)
                {
                    if (columnList.Contains(props[i].Name) && reader[props[i].Name] != DBNull.Value)
                        typeof(T).InvokeMember(props[i].Name, BindingFlags.SetProperty, null, entity, new Object[] { reader[props[i].Name] });
                }
                ret.Add(entity);
            }
            return ret;
        }

        public static List<string> GetColumnList(SqlDataReader reader)
        {
            List<string> columnList = new List<string>();
            DataTable readerSchema = reader.GetSchemaTable();
            for (int i = 0; i < readerSchema.Rows.Count; i++)
                columnList.Add(readerSchema.Rows[i]["ColumnName"].ToString());
            return columnList;
        }

        private void SetDBNull(SqlCommand command)
        {
            foreach (SqlParameter sp in command.Parameters)
            {
                if (sp.Direction != ParameterDirection.Output)
                {
                    if (sp.Value == null)
                    {
                        sp.Value = DBNull.Value;
                    }
                    else
                    {
                        if (sp.Value.ToString() == DateTime.MinValue.ToString())
                            sp.Value = DBNull.Value;
                    }
                }
            }
        }

        private void AddParams(SqlCommand command, string[] paramArray, object entity)
        {
            if (paramArray == null || paramArray.Count() == 0)
            {
                throw new Exception("Null parameter supplied.");
            }
            Type type = entity.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                foreach (string str in paramArray)
                {
                    if (property.Name.ToLower() == str.ToLower() || property.Name.ToLower() == str.Substring(1).ToLower() || property.Name.ToLower() == str.Substring(2).ToLower())
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            command.Parameters.Add(new SqlParameter(str, DBNull.Value));
                        }
                        else
                        {
                            command.Parameters.Add(new SqlParameter(str, property.GetValue(entity, null)));
                        }
                        break;
                    }
                }
            }
        }

        private void SetParamValue(SqlParameter[] parameters, object entity)
        {
            Type type = entity.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                foreach (SqlParameter pram in parameters)
                {
                    if (property.Name.ToLower() == pram.ParameterName.ToLower() || property.Name.ToLower() == pram.ParameterName.Substring(1).ToLower() || property.Name.ToLower() == pram.ParameterName.Substring(2).ToLower())
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            pram.Value = DBNull.Value;
                        }
                        else
                        {
                            pram.Value = property.GetValue(entity, null);
                        }
                        break;
                    }
                }
            }
        }

        protected void CloseOpenConnection(SqlConnection con)
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close();
            }
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
        }

        public string GetDbConStr()
        {
            //string encryptText = Encrypt("Data Source=ivy.arvixe.com; Initial Catalog=atraders; User Id=amin; Password=aTraDer@olDdhaKa2015;Connect Timeout=90");
            ConnectionStringsSection connectionStringsSection = GetConnectionStringsSection();
            if (IsEncryptedConnection == "1")
                return DecryptConnectionString(connectionStringsSection.ConnectionStrings[1].ConnectionString, false);
            else
                return connectionStringsSection.ConnectionStrings[1].ConnectionString;
        }

        private ConnectionStringsSection GetConnectionStringsSection()
        {
            return ConfigurationManager.GetSection("connectionStrings") as ConnectionStringsSection;
        }

        private String DecryptConnectionString(string cipherConnectionString, bool fullString)
        {
            if (fullString)
            {
                return Decrypt(cipherConnectionString);
            }
            else
            {
                string[] parts = cipherConnectionString.Split(new char[] { ';' });
                int p = -1;

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].TrimStart().ToLower().StartsWith("password="))
                    {
                        p = i;
                        break;
                    }
                }

                if (p >= 0)
                {
                    parts[p] = "Password=" + Decrypt(parts[p].TrimStart().Remove(0, 9));

                    string connectionString = "";

                    for (int i = 0; i < parts.Length; i++)
                    {
                        connectionString += parts[i] + ";";
                    }

                    return connectionString;
                }
                else
                {
                    return cipherConnectionString;
                }
            }
        }

        private string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        //http://www.aspsnippets.com/Articles/AES-Encryption-Decryption-Cryptography-Tutorial-with-example-in-ASPNet-using-C-and-VBNet.aspx
        //http://aspsnippets.com/demos/634/
        private string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
