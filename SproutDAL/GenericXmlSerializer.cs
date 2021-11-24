using System;
using System.Collections.Generic;
using System.Text;

namespace Cs2Gen.BusinessObjects
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericXmlSerializer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Path"></param>
        public static void Serialize(T obj, string Path)
        {
            System.Xml.Serialization.XmlSerializer oXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamWriter oStreamWriter = new System.IO.StreamWriter(Path);
            oXmlSerializer.Serialize(oStreamWriter, obj);
            oStreamWriter.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static T Deserialize(string Path)
        {
            System.Xml.Serialization.XmlSerializer oXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamReader oStreamReader = new System.IO.StreamReader(Path);
            T obj = (T)oXmlSerializer.Deserialize(oStreamReader);
            oStreamReader.Close();
            return obj;
        }
    } 

}
