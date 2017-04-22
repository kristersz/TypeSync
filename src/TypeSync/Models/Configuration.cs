using Newtonsoft.Json;

namespace TypeSync.Models
{
    public class Configuration
    {
        [JsonProperty(PropertyName = "inputPath")]
        public string InputPath { get; set; }

        [JsonProperty(PropertyName = "outputPath")]
        public string OutputPath { get; set; }
    }
}
