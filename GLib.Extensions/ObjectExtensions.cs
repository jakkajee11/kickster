using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;

namespace GLib.Extensions
{

    public static class ObjectExtensions
    {
        private static readonly Object locker = new Object();

        private static byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {                
                //To be thread-safe we lock the object
                lock (locker)
                {
                    formatter.Serialize(fs, objectToSerialize);
                }
                return fs.ToArray();
            }
            catch (SerializationException se)
            {                
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        private static string ComputeHash(byte[] objectAsBytes)
        {
            var hash = new SHA256CryptoServiceProvider();
            try
            {
                byte[] result = hash.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException ane)
            {
                return null;
            }
        }

        public static string HashMe<T>(this T obj)
        {
            if (obj == null) return string.Empty;
            var hashString = string.Empty;

            return ComputeHash(ObjectToByteArray(obj));
        }


        public static string JsonHash<T>(this T obj, bool casesensitive = true)
        {
            if (!casesensitive)
            {
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        var newVal = prop.GetValue(obj).ToString().ToLower();
                        prop.SetValue(obj, newVal);
                    }
                }
            }
            
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            MemoryStream fs = new MemoryStream();
            StreamWriter sw = new StreamWriter(fs);
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                try
                {
                    lock (locker)
                    {
                        serializer.Serialize(writer, obj);
                    }
                    return ComputeHash(fs.ToArray());
                }
                catch (SerializationException se)
                {
                    return null;
                }                               
            }            
        }
    }
}
