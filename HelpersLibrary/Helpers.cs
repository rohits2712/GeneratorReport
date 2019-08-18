
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace HelpersLibrary
{
    public static class Helpers
    {
        public static T Deserialize<T>(this XElement value)
        {
            var xmlDeserializer = new XmlSerializer(typeof(T));
            if (value != null) return (T) xmlDeserializer.Deserialize(value.CreateReader());
            return default(T);
        }

        public static XElement Serialize<T>(this object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

                }

            }
        }

    }

}
