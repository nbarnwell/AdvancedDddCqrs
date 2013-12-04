using System;
using Newtonsoft.Json;

namespace AdvancedDddCqrs
{
    public class MementoSerialiser
    {
        public TType Deserialise<TType, TMementoType>(string json)
        {
            var dynamicObject = JsonConvert.DeserializeObject(json);
            var memento = (TMementoType)Activator.CreateInstance(typeof(TMementoType), new[] { dynamicObject });
            var result = (TType)Activator.CreateInstance(typeof(TType), new[] { memento });
            return result;
        }

        public string Serialise<TType, TMementoType>(TType item) 
            where TType : ISupportMemoisation<TMementoType>
        {
            var memento = item.GetMemento();
            var json = JsonConvert.SerializeObject(memento, Formatting.Indented);
            return json;
        }
    }

    public class Serialiser
    {
        public TType Deserialise<TType>(string json)
            where TType : IDynamicWrapper
        {
            var dynamicObject = JsonConvert.DeserializeObject(json);
            var result = (TType)Activator.CreateInstance(typeof(TType), new[] { dynamicObject });
            return result;
        }

        public string Serialise<TType>(TType item) 
        {
            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            return json;
        }
    }
}