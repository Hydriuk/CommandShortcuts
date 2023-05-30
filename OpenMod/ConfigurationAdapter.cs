using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;

namespace CommandHotkeys.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ConfigurationAdapter : Configuration, IConfigurationAdapter<Configuration>
    {
        public Configuration Configuration => this;

        public ConfigurationAdapter(IConfiguration configuration)
        {
            configuration.Bind(this);
        }
    }
}