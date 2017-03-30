using TypeSync.Models;

namespace TypeSync.Providers
{
    public interface IConfigurationProvider
    {
        Configuration GetConfiguration();
    }
}
