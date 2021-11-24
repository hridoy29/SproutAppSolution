using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbExecutor
{
    public struct Parameters
    {
        public string paramName;
        public object paramValue;
        public DbType paramType;
        public ParameterDirection paramDirection;



        /// <summary>
        /// constructor to be used as param collection
        /// m
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <param name="direction"></param>
        public Parameters(string name, object value, DbType dbtype, ParameterDirection direction)
        {
            paramName = name;
            paramValue = value;
            paramDirection = direction;
            paramType = dbtype;
        }
    }
}
