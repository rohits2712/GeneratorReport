
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

    }

}
