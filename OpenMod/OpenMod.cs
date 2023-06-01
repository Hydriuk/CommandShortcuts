using CommandHotkeys.API;
using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("CommandHotkeys", DisplayName = "CommandHotkeys", Author = "Hydriuk")]

namespace CommandHotkeys.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        private readonly IServiceProvider _serviceProvider;

        public Plugin(IServiceProvider serviceProvider, IConfigurationAdapter<Configuration> configuration) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async UniTask OnLoadAsync()
        {
            _serviceProvider.GetRequiredService<IHotkeyController>();
            var configuration = _serviceProvider.GetRequiredService<IConfigurationAdapter<Configuration>>();
            
            foreach (var shortcut in configuration.Configuration.Shortcuts)
                shortcut.Validate();
        }

        protected override async UniTask OnUnloadAsync()
        {
        }
    }
}