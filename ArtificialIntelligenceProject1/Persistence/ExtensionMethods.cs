using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtificialIntelligenceProject1.Persistence
{
    public static class ExtensionMethods
    {
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}