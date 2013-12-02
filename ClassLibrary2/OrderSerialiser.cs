using System;
using Newtonsoft.Json;

namespace ClassLibrary2
{
    public class OrderSerialiser
    {
        public T Deserialise<T>(string json)
        {
            var order = JsonConvert.DeserializeObject(json);
            var result = (T)Activator.CreateInstance(typeof(T), new[] { order });
            return result;
        }

        public string Serialise<T>(T document) where T : IDynamicWrapper
        {
            dynamic content = document.Content;
            var result = JsonConvert.SerializeObject(content, Formatting.Indented);
            return result;
        }
    }
}