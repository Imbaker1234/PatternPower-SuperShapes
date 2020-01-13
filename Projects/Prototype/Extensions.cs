using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Prototype
{
    public static class Extensions
    {
        /// <summary>
        /// Creates a deep copy of the object. Undergoing
        /// a serialization and deserialization process to
        /// ensure that not only is a perfect replica made
        /// but that no references to any values held by the
        /// original are made.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T BinaryProtoType<T>(this T original)
        {
            using (var ms = new MemoryStream())
            {
                var s = new BinaryFormatter();
                s.Serialize(ms, original);
                ms.Position = 0;
                return (T) s.Deserialize(ms);
            }
        }

        public static T XmlProtoType<T>(this T original)
        {
            using (var ms = new MemoryStream())
            {
                var s = new XmlSerializer(typeof(T));
                s.Serialize(ms, original);
                ms.Position = 0;
                return (T)s.Deserialize(ms);
            }
        }
    }
}