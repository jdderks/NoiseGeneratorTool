using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace MVVM.General
{
    class SerializeDeSerialize<T>
    {
        public SerializeDeSerialize()
        {
            //Default constructor
        }

        public static void ToFile(string path, T o)
        {
            TextWriter textWriter;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (textWriter = new StreamWriter(path))
                {
                    serializer.Serialize(textWriter, o);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static MemoryStream ToMemory(T o)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(memoryStream, o);
                return memoryStream;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string ToStr(T o)
        {
            string returnVal = string.Empty;

            using (MemoryStream memoryStream = ToMemory(o))
            {
                memoryStream.Position = 0;
                using (StreamReader sr = new StreamReader(memoryStream))
                {
                    returnVal = sr.ReadToEnd();
                }
            }
            return returnVal;
        }

        public static T FromFile(string path)
        {
            T o;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (TextReader textReader = new StreamReader(path))
                {
                    o = (T)serializer.Deserialize(textReader);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return o;
        }

        public static T FromStr(string str)
        {
            T o;
            try
            {
                StringReader stringReader = new StringReader(str);
                XmlTextReader textReader = new XmlTextReader(stringReader);
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                o = (T)serializer.Deserialize(textReader);
                stringReader.Close();
                textReader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return o;
        }
    }
}
