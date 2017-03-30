using System.IO;
using Newtonsoft.Json;
using TypeSync.Models;

namespace TypeSync.Providers
{
    public class JsonConfigurationProvider : IConfigurationProvider
    {
        public Configuration GetConfiguration()
        {
            var pathToConfig = @"..\..\config.json";
            var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(pathToConfig));

            return config;
        }
    }
}
