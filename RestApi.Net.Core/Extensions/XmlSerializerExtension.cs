using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestApi.Net.Core.Extensions
{
    /// <summary>
    /// Represent Xml Serializer Extension
    /// </summary>
    public static class XmlSerializerExtension
    {
        /// <summary>
        /// Returns XmlSerializeToString
        /// </summary>
        /// <param name="model">object to serialize</param>
        /// <returns></returns>
        public static string XmlSerializeToString<TModel>(this TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var serializer = new XmlSerializer(model.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, model);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns Deserialized string to T object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString">xml String</param>
        /// <returns></returns>
        public static T XmlDeserializeFromString<T>(this string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                throw new ArgumentNullException(nameof(xmlString));
            }

            return (T)XmlDeserializeFromString(xmlString, typeof(T));
        }

        /// <summary>
        /// Returns Deserialized string to object
        /// </summary>
        /// <param name="xmlString">xml String</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object XmlDeserializeFromString(this string xmlString, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(xmlString))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
    }
}
