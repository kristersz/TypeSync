using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TypeSync.Models
{
    public class Configuration
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "pathKind")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PathKind PathKind { get; set; }
    }
}
