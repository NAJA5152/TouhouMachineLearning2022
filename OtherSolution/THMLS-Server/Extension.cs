
using Newtonsoft.Json;

namespace Server
{
    public static class Extension
    {
        public static string ToJson(this object DataObject) => JsonConvert.SerializeObject(DataObject);
        public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
    }
}
